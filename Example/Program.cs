using System;
using HtmlValidator.Lexer;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var htmlPage = 
@"<html>
<head>
<title>
A Simple HTML Document
</title>
</head>
<body>
<p>This is a very simple HTML document</p>
<p>It only has two paragraphs</p>
Send me mail at <a href=""mailto:support@yourcompany.com"">here</a>
<br />
</body>
</html>";

            var validator = new HtmlValidator.HtmlValidator();
            validator.Parse(htmlPage);

            Console.WriteLine("Printing all tokens found on page.");
            validator.PrintTokens(htmlPage);
            Console.WriteLine();

            if (validator.IsValid())
            {
                Console.WriteLine("HTML is correct");
            }
            else
            {
                Console.WriteLine("ERROR: " + validator.ErrorMessage);
            }

            Console.ReadKey();
        }
    }
}
