// tokenizer.cs
using System.Text.RegularExpressions;
using System.Text;

public class Tokenizer
{
    private string input = "";
    private int index = 0;
    private int line = 1;
    private int column = 0;   // ✅ 0-based columns

    public void setInput(string input)
    {
        this.input = input;
        index = 0;
        line = 1;
        column = 0;           // ✅ reset to 0
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
        int startColumn = column;   // ✅ correct

        // advance index, line, column
        foreach (char c in lexeme)
        {
            index++;
            if (c == '\n')
            {
                line++;
                column = 0;        // ✅ reset to 0 on newline
            }
            else
            {
                column++;
            }
        }

        // skip whitespace and comments
        if (bestTerminal!.sym == "WHITESPACE" || bestTerminal.sym == "COMMENT")
            return next();

        // STRINGCONST handling
        if (bestTerminal.sym == "STRINGCONST")
        {
            string raw = lexeme.Substring(1, lexeme.Length - 2);
            var sb = new StringBuilder();

            for (int i = 0; i < raw.Length; i++)
            {
                if (raw[i] == '\\' && i + 1 < raw.Length)
                {
                    char c = raw[++i];
                    sb.Append(c switch
                    {
                        'n'  => '\n',
                        't'  => '\t',
                        '"'  => '"',
                        '\\' => '\\',
                        _    => c
                    });
                }
                else
                {
                    sb.Append(raw[i]);
                }
            }

            lexeme = sb.ToString();
        }

        return new Token(
            bestTerminal.sym,
            startLine,
            startColumn,
            lexeme
        );
    }
}
