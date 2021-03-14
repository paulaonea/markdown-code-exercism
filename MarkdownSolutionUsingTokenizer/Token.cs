using System.Collections.Generic;

namespace MarkdownSolutionUsingTokenizer
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Text { get; set; }
        public string RawText { get; set; }
        public List<Token> Tokens { get; set; }
    }
}