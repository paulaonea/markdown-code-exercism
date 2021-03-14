using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarkdownSolutionSimpleRefactoring
{
public static class Markdown
{
    public static string Parse(string markdown)
    {
        var lines = markdown.Split('\n').ToList();
        var result = "";

        while (lines.Any())
        {
            result += lines[0][0] switch
            {
                '#' => ParseHeader(lines),
                '*' => ParseList(lines),
                _ => ParseParagraph(lines)
            };
        }

        return result;
    }
    
    private static string WrapTextWithTag(string text, string tag) => $"<{tag}>{text}</{tag}>";

    private static string ReplaceDelimiterWithTag(string markdown, string delimiter, string tag)
    {
        var pattern = $"{delimiter}(.+){delimiter}";
        var replacement = $"<{tag}>$1</{tag}>";
        return Regex.Replace(markdown, pattern, replacement);
    }

    private static string ApplyTagStrong(string markdown) => ReplaceDelimiterWithTag(markdown, "__", "strong");
    private static string ApplyTagItalic(string markdown) => ReplaceDelimiterWithTag(markdown, "_", "em");
    private static bool IsListItem(string line) => line.StartsWith("*");

    private static string ParseText(string markdown) => ApplyTagItalic(ApplyTagStrong((markdown)));

    private static string ParseHeader(IList<string> lines)
    {
        var currentLine = lines[0];
        var parsedLine = ParseText(currentLine);
        parsedLine = GetHeaderHtml(parsedLine);
        lines.Remove(currentLine);
        
        return parsedLine;

    }

    private static string ParseList(List<string> lines)
    {
        if (lines.Any())
        { 
            var list = GenerateList(lines);
            list = GenerateListTag(list);
            return list;
        }
        return "";
    }

    private static string GenerateListTag(string list) =>
        list.Length > 0
            ? WrapTextWithTag(list, "ul")
            : "";

    private static string GenerateList(List<string> lines)
    {
        var list = "";
        while (lines.Count > 0)
        {
            if (IsListItem(lines[0]))
            {
                list += ParseListItem(lines[0]);
                lines.Remove(lines[0]);
            }
            else break;
        }

        return list;
    }

    private static string ParseListItem(string listItem)
    {
        var line = ParseText(listItem);
        return WrapTextWithTag(line.Substring(2), "li");
    }

    private static string ParseParagraph(List<string> lines)
    {
       var line = ParseText(lines[0]);
        lines.Remove(lines[0]);
        return WrapTextWithTag(line, "p");
    }

    private static int GetHeaderCount(string markdown) => markdown.TakeWhile(c => c == '#').Count();

    public static string GetHeaderHtml(string markdown)
    {
        var headerCount = GetHeaderCount(markdown);
        var headerTag = $"h{headerCount.ToString()}";
        return WrapTextWithTag(markdown.Substring(headerCount + 1), headerTag);
    }
}
}