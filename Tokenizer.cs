// tokenizer.cs
using System.Text.RegularExpressions;

public class Tokenizer
{
    private string input = "";
    private int index = 0;
    private int line = 1;
    private int column = 0;

    public void setInput(string input)
    {
        this.input = input;
        index = 0;
        line = 1;
        column = 0;
    }

    public Token next()
    {
        if (index >= input.Length)
            return new Token("$", line, column, "");

        Match? bestMatch = null;
        Terminals.Terminal? bestTerminal = null;

        foreach (var t in Terminals.terminals)
        {
            var m = t.rex.Match(input, index);
            if (m.Success)
            {
                if (bestMatch == null || m.Length > bestMatch.Length)
                {
                    bestMatch = m;
                    bestTerminal = t;
                }
            }
        }

        if (bestMatch == null)
        {
            throw new Exception($"Tokenizer error at line {line}, column {column}");
        }

        string lexeme = bestMatch.Value;
        int startLine = line;
        int startColumn = column;

        // advance index, line, column
        foreach (char c in lexeme)
        {
            index++;
            if (c == '\n')
            {
                line++;
                column = 1;
            }
            else
            {
                column++;
            }
        }

        // skip whitespace tokens
        if (bestTerminal!.sym == "WHITESPACE" || bestTerminal!.sym == "COMMENT")
        return next();

        return new Token(
            bestTerminal.sym,
            startLine,
            startColumn,
            lexeme
        );
    }
}