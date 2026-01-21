using System.Text.RegularExpressions;

public static class Terminals
{
    public static string terminalspec = @"

        WHITESPACE :: [ \t\r\n]+

        FUNC    :: func
        RETURN  :: return
        IF      :: if

        LBRACE  :: \{
        RBRACE  :: \}
        LPAREN  :: \(
        RPAREN  :: \)
        COLON   :: :

        NUM     :: [0-9]+
        ID      :: [a-zA-Z_][a-zA-Z0-9_]*

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
            if (line.Length == 0) continue;

            var parts = line.Split("::");
            string sym = parts[0].Trim();
            string regex = parts[1].Trim();

            terminals.Add(
                new Terminal(sym, new Regex(@"\G(" + regex + ")"))
            );
        }
    }
}
