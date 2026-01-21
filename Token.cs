//Token.cs
public class Token {
    public string sym;
    public string lexeme;
    public int line;
    public Token(string sym, int line, string lexeme)
    {
        this.sym = sym;
        this.line= line;
        this.lexeme = lexeme;
    }

    public override string ToString()
    {
        return $"[{this.sym} {this.line} {this.lexeme}]";
    }
}