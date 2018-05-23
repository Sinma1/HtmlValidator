using System.Text.RegularExpressions;

namespace HtmlValidator.Lexer
{
    public class TokenDefinition
    {
        private readonly Regex _regex;

        private readonly string _tokenToReturn;

        public TokenDefinition(string tokenToReturn, string pattern)
        {
            _regex = new Regex(pattern, RegexOptions.IgnoreCase);

            _tokenToReturn = tokenToReturn;
        }

        public TokenMatch CheckMatch(string input)
        {
            var match = _regex.Match(input);

            if (!match.Success)
            {
                return new TokenMatch()
                {
                    IsMatch = false
                };
            }

            var textLeft = string.Empty;

            if (match.Length < input.Length)
            {
                textLeft = input.Substring(match.Length);
            }

            return new TokenMatch()
            {
                IsMatch = true,
                TextLeft = textLeft,
                AtIndex = match.Index,
                Token = new Token()
                {
                    Type = _tokenToReturn,
                    Value = match.Value
                }
            };

        }
    }
}