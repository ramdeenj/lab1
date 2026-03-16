using System;
using System.Collections.Generic;

public class Tokenizer
{
    private string input;
    private int index;
    private int line;
    private Stack<int> rewindStack = new Stack<int>();

    public Tokenizer()
    {
        Terminals.init();
        line = 1;
    }

    public void setInput(string input)
    {
        this.input = input;
        this.index = 0;
        this.line = 1;
    }

    public Token next()
    {
        if (index >= input.Length)
            return new Token("$$", line, "");

        Token best = null;
        int bestLen = 0;

        foreach (var t in Terminals.terminals)
        {
            var m = t.rex.Match(input, index);
            if (m.Success && m.Length > bestLen)
            {
                bestLen = m.Length;
                best = new Token(t.sym, line, m.Value);
            }
        }

        if (best == null)
            throw new Exception($"Tokenizer error at line {line}");

        rewindStack.Push(index);
        index += bestLen;
        line += best.lexeme.Split('\n').Length - 1;

        if (best.sym == "WHITESPACE")
            return next();

        return best;
    }

    public string peek()
    {
        int save = index;
        int saveLine = line;
        Token t = next();
        index = save;
        line = saveLine;
        rewindStack.Pop();
        return t.sym == "$$" ? "" : t.lexeme;
    }

    public Token expect(string sym)
    {
        Token t = next();
        if (t.sym != sym)
            throw new Exception($"Expected {sym}, got {t.sym}");
        return t;
    }

    public void rewind()
    {
        if (rewindStack.Count > 0)
            index = rewindStack.Pop();
    }
}
