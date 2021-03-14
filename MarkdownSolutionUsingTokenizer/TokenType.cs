using System;

namespace MarkdownSolutionUsingTokenizer
{
    [Flags]
    public enum TokenType    
    {
        Header1 = 1,
        Header2 = 2,
        Header3 = 3,
        Header4 = 4,
        Header5 = 5,
        Header6 = 6,
        List = 7,
        UList = 8,
        Italic = 9,
        Strong = 10,
        Paragraph = 11,
        Text = 12
    }
}