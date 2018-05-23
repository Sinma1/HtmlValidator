using System;
using HtmlValidator.Lexer;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var htmlPage =
                "<html>\r\n<head>\r\n<title k>\r\nA Simple HTML Document\r\n</title>\r\n</head>\r\n<body>\r\n<p>This is a very simple HTML document</p>\r\n<p>It only has two paragraphs</p>\r\nSend me mail at <a href=\"mailto:support@yourcompany.com\">here</a>\r\n<br />\r\n</body>\r\n</html>";

            var validator = new HtmlValidator.HtmlValidator(htmlPage);

            if (validator.IsValid())
            {
                Console.WriteLine("HTML is correct");
            }
            else
            {
                Console.WriteLine("ERROR: " + validator.ErrorMessage);
            }

            validator.PrintTokens(htmlPage);

            Console.ReadKey();
        }
    }
}
