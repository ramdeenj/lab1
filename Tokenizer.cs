public class Tokenizer
{
    private string input = "";
    private int index = 0;
    private int line = 1;

    private Token? peeked = null;

    public Tokenizer()
    {
        Terminals.init();
    }

    public void setInput(string input)
    {
        this.input = input;
        index = 0;
        line = 1;
        peeked = null;
    }

    public Token next()
    {
        if (peeked != null)
        {
            Token t = peeked;
            peeked = null;
            return t;
        }

        if (index >= input.Length)
            return new Token("$", line, "");

        Terminals.Terminal? best = null;
        int bestLen = 0;

        foreach (var t in Terminals.terminals)
        {
            var m = t.rex.Match(input, index);
            if (m.Success && m.Length > bestLen)
            {
                best = t;
                bestLen = m.Length;
            }
        }

        if (best == null)
        {
            Utils.error($"Tokenizer error at line {line}");
            throw new Exception();
        }

        string lexeme = input.Substring(index, bestLen);
        index += bestLen;

        foreach (char c in lexeme)
            if (c == '\n') line++;

        if (best.sym == "WHITESPACE")
            return next();

        return new Token(best.sym, line, lexeme);
    }

    public Token expect(string sym)
    {
        Token t = next();
        if (t.sym != sym)
        {
            Utils.error($"Expected {sym} but got {t}");
            throw new Exception();
        }
        return t;
    }

    public string peek()
    {
        if (peeked == null)
            peeked = next();
        return peeked.sym;
    }
}
