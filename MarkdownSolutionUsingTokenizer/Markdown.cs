using System.Linq;

namespace MarkdownSolutionUsingTokenizer
{
    public static class Markdown
    {

        public static string Parse(string markdown)
        {

            var tokeniser = new Tokenizer();
            var tokens = tokeniser.Tokenize(markdown).ToList();

            var htmlGenerator = new HtmlGenerator();
            var html = htmlGenerator.GenerateHtml(tokens);

            return html;
        }
    
    }
}