// terminals.cs
using System.Text.RegularExpressions;

public static class Terminals
{
    public static string terminalspec = @"

WHITESPACE :: [ \t\r\n]+
COMMENT :: //[^\r\n]*(\r?\n)?

VAR :: var
FUNC :: func
WHILE :: while
IF :: if
RETURN :: return
TYPE :: int

BOOLCONST :: true|false
STRINGCONST :: ""(\\[nt\""\\]|[^""\\])*""

ID :: [a-zA-Z_][a-zA-Z0-9_]*

FNUM :: ([0-9]+\.[0-9]*|\.[0-9]+|[0-9]+)([eE][+-]?[0-9]+)
FNUM :: [0-9]+\.[0-9]+
FNUM :: \.[0-9]+
NUM  :: [0-9]+

LPAREN :: \(
RPAREN :: \)
LBRACE :: \{
RBRACE :: \}
COMMA  :: ,
COLON  :: :

SHIFTOP :: <<|>>
RELOP   :: <=|>=|==
RELOP   :: <
EQ      :: =
ADDOP   :: [+\-]
MULOP   :: [/*%]
BITOP   :: [&|^]
BITNOT  :: ~

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
