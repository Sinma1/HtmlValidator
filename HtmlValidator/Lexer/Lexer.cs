using System.Collections.Generic;
using System.Linq;

namespace HtmlValidator.Lexer
{
    public class Lexer
    {
        public bool MatchEverything { get; set; }

        //public HtmlLexer()
        //{
        //    _tokenDefinitions.Add(new TokenDefinition(TokenType.LeftAngleSlash, "</"));
        //    _tokenDefinitions.Add(new TokenDefinition(TokenType.RightAngleSlash, "/>"));
        //    _tokenDefinitions.Add(new TokenDefinition(TokenType.RightAngle, ">"));
        //    _tokenDefinitions.Add(new TokenDefinition(TokenType.LeftAngle, "<"));
        //    //_tokenDefinitions.Add(new TokenDefinition(TokenType.Attribute, @"[^<>\s=""]+="".*"""));
        //    _tokenDefinitions.Add(new TokenDefinition(TokenType.Equal, @"="));
        //    _tokenDefinitions.Add(new TokenDefinition(TokenType.String, @""".*"""));
        //    _tokenDefinitions.Add(new TokenDefinition(TokenType.FloatNumber, @"[-+]?\d*\.\d+([eE][-+]?\d+)?"));
        //    _tokenDefinitions.Add(new TokenDefinition(TokenType.Number, @"\d+"));
        //    _tokenDefinitions.Add(new TokenDefinition(TokenType.Word, @"[^<>\s=""]+"));
        //}

        public Lexer(params TokenDefinition[] tokenDefinitions)
        {
            foreach (var tokenDefinition in tokenDefinitions)
            {
                _tokenDefinitions.Add(tokenDefinition);
            }
        }

        private readonly List<TokenDefinition> _tokenDefinitions = new List<TokenDefinition>();

        public List<Token> Tokanize(string html)
        {
            var tokens = new List<Token>();
            int line = 1;
            int column = 1;

            while (html.Length > 0)
            {
                var match = FindTokenMatch(html);

                html = match.TextLeft;

                if (match.IsMatch)
                {
                    match.Token.Line = line;
                    match.Token.Column = column;

                    if (match.Token.Type == "newline")
                    {
                        match.Token.Value = "";
                        line++;
                        column = 1;
                        continue;
                    }

                    column += match.Token.Value.Length;

                    if (match.Token.Type == "space")
                    {
                        continue;
                    }

                    tokens.Add(match.Token);
                }
            }

            return tokens;
        }

        private TokenMatch FindTokenMatch(string html)
        {
            var token = new Token();

            var possibleTokenMatches = new List<TokenMatch>();

            foreach (var tokenDefinition in _tokenDefinitions)
            {
                var match = tokenDefinition.CheckMatch(html);

                if (!match.IsMatch) 
                    continue;


                if (match.AtIndex == 0)
                {
                    return match;
                }

                possibleTokenMatches.Add(match);
            }

            possibleTokenMatches = possibleTokenMatches.OrderBy(match => match.AtIndex).ToList();

            if (possibleTokenMatches.Count == 0)
            {
                return new TokenMatch()
                {
                    Token = new Token()
                    {
                        Type = "unknown",
                        Value = html
                    },
                    IsMatch = MatchEverything,
                    TextLeft = ""
                };
            }

            if (possibleTokenMatches.First().AtIndex != 0)
            {
                return new TokenMatch()
                {
                    Token = new Token()
                    {
                        Type = "unknown",
                        Value = html.Substring(0, possibleTokenMatches.First().AtIndex + 1)
                    },
                    IsMatch = MatchEverything,
                    TextLeft = html.Substring(html.Substring(0, possibleTokenMatches.First().AtIndex + 1).Length)
                };
            }

            return possibleTokenMatches.First();
        }
    }
}