using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownSolutionUsingTokenizer
{
    public class HtmlGenerator
    {
        private readonly Dictionary<TokenType, string> _tokenToHtmlTagMapping = new Dictionary<TokenType, string>()
        {
            {TokenType.Strong, "strong"},
            {TokenType.Italic, "em"},
            {TokenType.List, "li"},
            {TokenType.UList, "ul"},
            {TokenType.Header1, "h1"},
            {TokenType.Header2, "h2"},
            {TokenType.Header3, "h3"},
            {TokenType.Header4, "h4"},
            {TokenType.Header5, "h5"},
            {TokenType.Header6, "h6"},
            {TokenType.Paragraph, "p"},
            {TokenType.Text, ""}
        };

        public string GenerateHtml(IEnumerable<Token> tokens)
        {
            var html = "";
            foreach (var token in tokens)
            {
                html += GenerateTags(token);
            }

            return html;
        }

        private string GenerateTags(Token token)
        {
            var text = new StringBuilder();

            if (token.Tokens.Any())
            {
                foreach (var innerToken in token.Tokens)
                {
                    text.Append(GenerateTags(innerToken));
                }
            }
            else
            {
                text.Append(token.Text);
            }

            return ApplyTagToText(token.Type, text.ToString());
        }

        private string ApplyTagToText(TokenType type, string text)
        {
            var tag = _tokenToHtmlTagMapping[type];
            return type == TokenType.Text
                ? text
                : $"<{tag}>{text}</{tag}>";
        }
    }
}