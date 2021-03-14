using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownSolutionUsingTokenizer
{
public class Tokenizer
{
    private string RawText { get; set; }
    private string RemainingText { get; set; }
    public IEnumerable<Token> Tokenize(string text)
    {
        RawText = text;
        RemainingText = text;

        while (!string.IsNullOrEmpty(RemainingText))
        {
            var token = RemainingText[0] switch
            {
                '#' => TokenizeHeader(),
                '*' => TokenizeList(),
                _ => TokenizeParagraph()
            };
            yield return token;
        }

    }

    private Token TokenizeHeader()
    {
        var currentLine = ExtractFirstLineFromString();
        var headerCount = Math.Min(GetHeaderCount(currentLine), 6);

        return new Token
        {
            Type = (TokenType) Enum.Parse(typeof(TokenType), headerCount.ToString(), true),
            Text = currentLine.Substring(headerCount + 1),
            RawText = currentLine,
            Tokens = GenerateInnerTokens(currentLine.Substring(headerCount + 1))
        };

    }

    private Token TokenizeList()
    {
        var token = new Token
        {
            Type = TokenType.UList,
            Text = "",
            RawText = "",
            Tokens = new List<Token>(),
        };

        while (!string.IsNullOrEmpty(RemainingText))
        {
            if (RemainingText[0] != '*')
            {
                break;
            }

            var currentLine = ExtractFirstLineFromString();
            token.Tokens.Add(TokenizeListItem(currentLine));
            token.Text += currentLine;
            token.RawText += currentLine;

        }

        return token;
    }

    private Token TokenizeParagraph()
    {
        var currentLine = ExtractFirstLineFromString();
        var token = new Token
        {
            Type = TokenType.Paragraph,
            Text = currentLine,
            RawText = currentLine,
            Tokens = GenerateInnerTokens(currentLine)
        };
        return token;
    }

    private Token TokenizeListItem(string text)
    {
        var token = new Token
        {
            Type = TokenType.List,
            Text = text.Substring(1).Trim(),
            RawText = text,
            Tokens = GenerateInnerTokens(text.Substring(1).Trim())
        };

        return token;
    }

    private static int GetHeaderCount(string text) => text.TakeWhile(c => c == '#').Count();

    private string ExtractFirstLineFromString()
    {
        // var index = RemainingText.IndexOf(Environment.NewLine, StringComparison.CurrentCulture);
        var index = RemainingText.IndexOf('\n', StringComparison.CurrentCulture);
        var firstLine = index == -1
            ? RemainingText
            : RemainingText.Substring(0, index);
        RemainingText = RemainingText.Substring(firstLine.Length).TrimStart('\n');
        return firstLine;
    }

    public string InnerString { get; set; }
    
    private List<Token> GenerateInnerTokens(string text)
    {
        InnerString = text;

        var tokens = new List<Token>();
        while (!string.IsNullOrEmpty(InnerString))
        {
            var token = (InnerString[0] == '_')
                ? TokenizeStrongOrItalic()
                : TokeniseText();
            tokens.Add(token);
        }

        return tokens;
    }

    private Token TokenizeStrongOrItalic()
    {
        var token = (InnerString.Length >=2 && InnerString[1] == '_')
            ? GenerateStrongToken()
            : GenerateItalicToken();

        return token;
    }

    private Token GenerateItalicToken()
    {
        var index = InnerString.Substring(1).IndexOf("_", StringComparison.CurrentCulture);
        var currentTextItalicToken = index == -1
            ? ""
            : InnerString.Substring(1, index);
        InnerString = InnerString.Substring(currentTextItalicToken.Length + 2);
        
        return new Token
        {
            Type = TokenType.Italic,
            Text = currentTextItalicToken,
            RawText = $"_{currentTextItalicToken}_",
            Tokens = new List<Token>()
        };
    }

    private Token GenerateStrongToken()
    {
        var index = InnerString.Substring(2).IndexOf("__", StringComparison.CurrentCulture);
        var currentTextStrongToken = index == -1
            ? ""
            : InnerString.Substring(2, index);
        InnerString = InnerString.Substring(currentTextStrongToken.Length + 4);
        
        return new Token
        {
            Type = TokenType.Strong,
            Text = currentTextStrongToken,
            RawText = $"__{currentTextStrongToken}__",
            Tokens = new List<Token>()
        };
        
    }

    private Token TokeniseText()
    {
        var text = new StringBuilder();
        while (!string.IsNullOrEmpty(InnerString) && InnerString[0] != '_')
        {
            text.Append(InnerString[0]);
            InnerString = InnerString.Substring(1);
        }
        return new Token
        {
            Type = TokenType.Text,
            Text = text.ToString(),
            RawText = text.ToString(),
            Tokens = new List<Token>()
        };
        
    }
    
    // TODO:
    // this is not the best solution as it introduces a bug / does not deal correctly with the case the markdown
    // includes a strong inside an italic  (_example __strong__ test_). 
    
    // Also, extracting inner tokens should be recursive, to extract all nested tokens.  Should iterate
    // until there is no inner text left to tokenize.
    
    // should use Regex.
}
}