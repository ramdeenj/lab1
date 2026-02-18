//Terminals.cs

using System.Text.RegularExpressions;
using System.Collections.Generic;

public static class Terminals
{
    public static string terminalspec = @"

    WHITESPACE :: [ \t\r\n]+

    FUNC :: func
    RETURN :: return

    NUM :: [0-9]+
    ID :: [a-zA-Z_][a-zA-Z0-9_]*

    POWER :: \*\*
    SHIFTLEFT :: <<
    SHIFTRIGHT :: >>

    EQ :: ==
    NEQ :: !=
    LE :: <=
    GE :: >=

    LT :: <
    GT :: >

    ANDKW :: and
    ORKW :: or

    AND :: &
    OR :: \|

    PLUS :: \+
    MINUS :: -
    STAR :: \*
    SLASH :: /

    ASSIGN :: =
    DOT :: \.

    LPAREN :: \(
    RPAREN :: \)
    LBRACE :: \{
    RBRACE :: \}

";

    public class Terminal
    {
        public string sym;
        public Regex rex;

        public Terminal(string sym, Regex rex)
        {
            this.sym = sym;
            this.rex = rex;
        }
    }

    public static List<Terminal> terminals = new();

    public static void init()
    {
        terminals.Clear(); 

        foreach (var line_ in terminalspec.Split('\n'))
        {
            var line = line_.Trim();
            if (line.Length == 0)
                continue;

            var tmp = line.Split("::");
            string sym = tmp[0].Trim();
            string regex = tmp[1].Trim();

            terminals.Add(
                new Terminal(sym, new Regex("\\G(" + regex + ")"))
            );
        }
    }
}