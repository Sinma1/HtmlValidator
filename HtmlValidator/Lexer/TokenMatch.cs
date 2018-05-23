namespace HtmlValidator.Lexer
{
    public class TokenMatch
    {
        public bool IsMatch { get; set; }

        public int AtIndex { get; set; }

        public Token Token { get; set; }

        public string TextLeft { get; set; }
    }
}