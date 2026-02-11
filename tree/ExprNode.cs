//ExprNode.cs

using System.Collections.Generic;

public abstract class ExprNode : TreeNode
{
    private static int nextId = 0;

    public int unique;
    public Token token;

    protected ExprNode(Token token)
    {
        this.token = token;
        this.unique = nextId++;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode>();
    }

    // ===== ENTRY POINT =====
    public static ExprNode parse(Tokenizer T)
    {
        // IMPORTANT:
        // Do NOT check for extra tokens here.
        // Expressions are parsed inside a larger grammar (function body).
        return parseOr(T);
    }

    // |
    static ExprNode parseOr(Tokenizer T)
    {
        ExprNode left = parseAnd(T);

        while (T.peek() == "|")
        {
            Token op = T.next();
            ExprNode right = parseAnd(T);
            left = new BinOpNode(op, left, right);
        }

        return left;
    }

    // &
    static ExprNode parseAnd(Tokenizer T)
    {
        ExprNode left = parseShift(T);

        while (T.peek() == "&")
        {
            Token op = T.next();
            ExprNode right = parseShift(T);
            left = new BinOpNode(op, left, right);
        }

        return left;
    }

    // <<
    static ExprNode parseShift(Tokenizer T)
    {
        ExprNode left = parseAddSub(T);

        while (T.peek() == "<<")
        {
            Token op = T.next();
            ExprNode right = parseAddSub(T);
            left = new BinOpNode(op, left, right);
        }

        return left;
    }

    // + -
    static ExprNode parseAddSub(Tokenizer T)
    {
        ExprNode left = parseMulDiv(T);

        while (T.peek() == "+" || T.peek() == "-")
        {
            Token op = T.next();
            ExprNode right = parseMulDiv(T);
            left = new BinOpNode(op, left, right);
        }

        return left;
    }

    // * /
    static ExprNode parseMulDiv(Tokenizer T)
    {
        ExprNode left = parsePrimary(T);

        while (T.peek() == "*" || T.peek() == "/")
        {
            Token op = T.next();
            ExprNode right = parsePrimary(T);
            left = new BinOpNode(op, left, right);
        }

        return left;
    }

    // NUM | ID
    static ExprNode parsePrimary(Tokenizer T)
    {
        Token tok = T.next();

        if (tok.sym == "NUM")
            return new NumNode(tok);

        if (tok.sym == "ID")
            return new VarNode(tok);

        throw new System.Exception("invalid expression");
    }
}

