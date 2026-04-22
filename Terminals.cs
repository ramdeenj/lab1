using System.Text.RegularExpressions;
using System.Collections.Generic;

public static class Terminals
{
    public static string terminalspec = @"
WHITESPACE :: [ \t\r\n]+
COMMENT :: //[^\n]*
CLASS :: class
NEW :: new
THIS :: this
FUNC :: func
RETURN :: return
IF :: if
ELSE :: else
WHILE :: while
REPEAT :: repeat
UNTIL :: until
BREAK :: break
CONTINUE :: continue
VAR :: var
TRUE :: true
FALSE :: false
AS :: as
FLOAT :: [0-9]+\.[0-9]+
NUM :: [0-9]+
STRING :: ""([^""\\]|\\.)*""
ID :: [a-zA-Z_][a-zA-Z0-9_]*
POWER :: \*\*
SHIFTLEFT :: <<
SHIFTRIGHT3 :: >>>
SHIFTRIGHT :: >>
EQ :: ==
NEQ :: !=
LE :: <=
GE :: >=
LT :: <
GT :: >
ANDKW :: and
ORKW :: or
NOTKW :: not
AND :: &
OR :: \|
XOR :: \^
BITNOT :: ~
PLUS :: \+
MINUS :: -
STAR :: \*
SLASH :: /
PERCENT :: %
ASSIGN :: =
DOT :: \.
COMMA :: ,
LPAREN :: \(
RPAREN :: \)
LBRACE :: \{
RBRACE :: \}
LBRACKET :: \[
RBRACKET :: \]
COLON :: :
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

    public static List<Terminal> terminals = new List<Terminal>();

    public static void init()
    {
        terminals.Clear();
        foreach (var line_ in terminalspec.Split('\n'))
        {
            var line = line_.Trim();
            if (line.Length == 0) continue;
            int sep = line.IndexOf("::");
            string sym = line.Substring(0, sep).Trim();
            string regex = line.Substring(sep + 2).Trim();
            terminals.Add(
                new Terminal(sym, new Regex("\\G(" + regex + ")"))
            );
        }
    }
}