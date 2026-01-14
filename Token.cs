// token.cs
public class Token
{
    public string sym;
    public string lexeme;
    public int line;
    public int column;

    public Token(string sym, int line, int column, string lexeme)
    {
        this.sym = sym;
        this.line = line;
        this.column = column;
        this.lexeme = lexeme;
    }

    public override string ToString()
    {
        return $"[{sym} {line}:{column} {lexeme}]";
    }
}
