using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
        var index = RemainingText.IndexOf('\n', StringComparison.CurrentCulture);
        var firstLine = index == -1
            ? RemainingText
            : RemainingText.Substring(0, index);
        RemainingText = RemainingText.Substring(firstLine.Length).TrimStart('\n');
        return firstLine;
    }

    private string InnerString { get; set; }
    
    private List<Token> GenerateInnerTokens(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }
        InnerString = text;

        var tokens = new List<Token>();
        while (!string.IsNullOrEmpty(InnerString))
        {
            var token = TokenizeStrongToken();
            if (token != null)
            {
                tokens.Add(token);
                continue;
            }

            token = TokenizeItalicToken();
            if (token != null)
            {
                tokens.Add(token);
                continue;
            }
            
            token = TokeniseText();
            if (token != null)
            {
                tokens.Add(token);
            }

        }

        foreach (var token in tokens.Where(token => token.Type != TokenType.Text))
        {
            token.Tokens = GenerateInnerTokens(token.Text);
        }

        return tokens;
    }
    

    private Token TokenizeItalicToken()
    {
        // search for text like "_text_" at the beginning of the text
        var match = Regex.Match(InnerString, "^((?<!_)_(?!_)).*((?<!_)_(?!_))");
        if (match.Success)
        {
            InnerString = InnerString.Substring(match.Length);
            return new Token 
            {
                Type = TokenType.Italic,
                Text = match.Value.Substring(1,match.Length - 2),
                RawText = match.Value,
                Tokens = new List<Token>()
            };
        }

        return null;

    }

    private Token TokenizeStrongToken()
    {
        // search for text like "__text__" at the beginning of the text
        var match = Regex.Match(InnerString, "^(__).*(__)");
        if (match.Success)
        {
            InnerString = InnerString.Substring(match.Length);
            return new Token
            {
                Type = TokenType.Strong,
                Text = match.Value.Substring(2, match.Length - 4),
                RawText = match.Value,
                Tokens = new List<Token>()
            };
        }
        return null;
        
    }

    private Token TokeniseText()
    {
        // search for text until the first "_"
        var match = Regex.Match(InnerString, "[^_]*");
        if (match.Success)
        {
            InnerString = InnerString.Substring(match.Length);
            return new Token
            {
                Type = TokenType.Text,
                Text = match.Value,
                RawText = match.Value,
                Tokens = new List<Token>()
            };
            
        }

        return null;

    }
}
}