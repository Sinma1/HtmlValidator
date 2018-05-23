using System;
using System.Collections.Generic;
using System.Linq;
using HtmlValidator.Lexer;

namespace HtmlValidator
{
    public class HtmlValidator
    {
        public string ErrorMessage { get; private set; } = "";

        private List<Tag> _tags;

        private bool ErrorOccurred => ErrorMessage != string.Empty;

        private readonly Lexer.Lexer _lexer;

        public HtmlValidator()
        {
            _lexer = new Lexer.Lexer(
                new TokenDefinition("NewLine", @"(\n|\r\n?)"),
                new TokenDefinition("Space", @"\s+"),
                new TokenDefinition("SelfClosingTag", @"<[^<>/]+/>"),
                new TokenDefinition("ClosingTag", @"</[^<>/]+>"),
                new TokenDefinition("OpeningTag", @"<[^<>/]+>"),
                new TokenDefinition("Word", @"[^<>\s]+")
            ) {MatchEverything = true};
        }

        public HtmlValidator(string html) : this()
        {
            Parse(html);
        }

        public void Parse(string html)
        {
            ErrorMessage = "";
            _tags = FindTags(html);
        }

        public void PrintTokens(string html)
        {
            _lexer.Tokanize(html).ForEach(Console.WriteLine);
        }

        public void PrintTags(string html)
        {
            var tags = FindTags(html);

            foreach (var tag in tags)
            {
                Console.WriteLine(tag);
            }
        }

        public bool IsValid()
        {
            if (ErrorOccurred)
            {
                return false;
            }
            
            var tagStack = new Stack<Tag>();

            foreach (var tag in _tags)
            {
                if (!IsTagCorrect(tag, tagStack))
                {
                    return false;
                }
            }

            if (tagStack.Count == 0) 
                return true;


            ErrorMessage = $"Tag {tagStack.Peek().Name} has not been closed\n" +
                           $"at position ({tagStack.Peek().Line}, {tagStack.Peek().Line}).";
            return false;
        }

        private bool IsTagCorrect(Tag tag, Stack<Tag> tagStack)
        {
            if (!AreAttributesCorrect(tag))
            {
                return false;
            }

            if (tag.Type == TagType.Opening)
            {
                tagStack.Push(tag);
            }
            else if (tag.Type == TagType.Closing)
            {
                if (tagStack.Count == 0)
                {
                    ErrorMessage = "Closing tags that where never opened\n" +
                                   $"tag {tag.Name} at position ({tag.Line}, {tag.Line}).";
                    return false;
                }

                var expectedTag = tagStack.Pop();
                if (expectedTag.Name != tag.Name)
                {
                    ErrorMessage = $"Closing wrong tag, expected </{expectedTag.Name}>\n" +
                                   $"instead of {tag.Name} at position ({tag.Line}, {tag.Line}).";
                    return false;
                }
            }

            return true;
        }

        private List<Tag> FindTags(string html)
        {
            var foundTags = new List<Tag>();

            var tokens = _lexer.Tokanize(html);

            foreach (var token in tokens)
            {
                var tag = CreateTag(token);

                if (ErrorOccurred)
                    break;

                if (tag != null)
                    foundTags.Add(tag);
            }

            return foundTags;
        }

        private Tag CreateTag(Token token)
        {
            var tag = new Tag()
            {
                Line = token.Line,
                Column = token.Column
            };

            var tokenValue = token.Value;

            switch (token.Type)
                {
                    case "closingtag":
                        tag.Type = TagType.Closing;
                        tokenValue = tokenValue.Substring(2, tokenValue.Length - 3);
                        break;
                    case "selfclosingtag":
                        tag.Type = TagType.SelfClosing;
                        tokenValue = tokenValue.Substring(1, tokenValue.Length - 3);
                        break;
                    case "openingtag":
                        tag.Type = TagType.Opening;
                        tokenValue = tokenValue.Substring(1, tokenValue.Length - 2);
                        break;
                    case "word":
                        return null;
                    default:
                        ErrorMessage = $"Unknown token: {token.Type}\n" +
                                       $"at position ({token.Line}, {token.Line}).";
                        return null;
                }

            var splitedTagParts = tokenValue.Trim().Split(' ').ToList();

            if (splitedTagParts.Count < 1)
            {
                ErrorMessage = "Tag name has not been found\n" +
                               $"at position ({tag.Line}, {tag.Line}).";
                return null;
            }

            tag.Name = splitedTagParts[0];
            splitedTagParts.RemoveAt(0);

            if (tag.Type == TagType.Closing && splitedTagParts.Count > 0)
            {
                ErrorMessage = "Closing tags can't have attributes\n" +
                               $"Tag {tag.Name} at position ({token.Line}, {token.Line}).";
                return null;
            }

            splitedTagParts.ForEach(tag.AttributesList.Add);

            return tag;
        }

        private bool AreAttributesCorrect(Tag tag)
        {
            var attribLexer = new Lexer.Lexer(
                new TokenDefinition("equal", @"="),
                new TokenDefinition("string", @"""[^""']+"""),
                new TokenDefinition("string", @"'[^""']+'"),
                new TokenDefinition("float", @"[-+]?\d*\.\d+([eE][-+]?\d+)?"),
                new TokenDefinition("number", @"\d+"),
                new TokenDefinition("word", @"[^<>\s=""]+")
            ) {MatchEverything = true};

            foreach (var attrib in tag.AttributesList)
            {
                if (!IsAttributeCorrect(attribLexer.Tokanize(attrib).Select(s => s.Type).ToArray()))
                {
                    ErrorMessage = "Incorrect attribute\n" +
                                   $"in tag {tag.Name} at position ({tag.Line}, {tag.Line}).";
                    return false;
                }
            }

            return true;
        }

        private bool IsAttributeCorrect(string[] tokenTypes)
        {
            if (tokenTypes.Length != 3)
                return false;

            if (tokenTypes[0] != "word")
                return false;

            if (tokenTypes[1] != "equal")
                return false;

            var allowedTypes = new[] {"word", "float",
                                      "number", "string"};

            return allowedTypes.Contains(tokenTypes[2]);
        }
    }
}