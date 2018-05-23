namespace HtmlValidator.Lexer
{
    public class Token
    {
        private string _type;

        public string Type
        {
            get => _type.ToLower();
            set => _type = value.ToLower();
        }

        public string Value { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        public override string ToString()
        {
            return $"<{Type}>: ({Value}) - Pos({Line}, {Column})";
        }
    }
}