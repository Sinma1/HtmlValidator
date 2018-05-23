using System.Collections.Generic;

namespace HtmlValidator
{
    public class Tag
    {
        public string Name { get; set; }

        public TagType Type { get; set; }

        public List<string> AttributesList { get; set; } = new List<string>();

        public int Line { get; set; }

        public int Column { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}, Type: {Type}, Number of attributes: {AttributesList.Count}";
        }
    }
}