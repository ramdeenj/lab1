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

    public static ExprNode parse(Tokenizer T)
    {
        return parseAssign(T);
    }

    // = (right associative, lowest precedence)
    static ExprNode parseAssign(Tokenizer T)
    {
        ExprNode left = parseOr(T);

        if (T.peek() == "=")
        {
            Token op = T.next();
            ExprNode right = parseAssign(T);
            return new BinOpNode(op, left, right);
        }

        return left;
    }

    // or
    static ExprNode parseOr(Tokenizer T)
    {
        ExprNode left = parseAnd(T);

        while (T.peek() == "or")
        {
            Token op = T.next();
            ExprNode right = parseAnd(T);
            left = new BinOpNode(op, left, right);
        }

        return left;
    }

    // and
    static ExprNode parseAnd(Tokenizer T)
    {
        ExprNode left = parseBitOr(T);

        while (T.peek() == "and")
        {
            Token op = T.next();
            ExprNode right = parseBitOr(T);
            left = new BinOpNode(op, left, right);
        }

        return left;
    }

    // |
    static ExprNode parseBitOr(Tokenizer T)
    {
        ExprNode left = parseBitAnd(T);

        while (T.peek() == "|")
        {
            Token op = T.next();
            ExprNode right = parseBitAnd(T);
            left = new BinOpNode(op, left, right);
        }

        return left;
    }

    // &
    static ExprNode parseBitAnd(Tokenizer T)
    {
        ExprNode left = parseEquality(T);

        while (T.peek() == "&")
        {
            Token op = T.next();
            ExprNode right = parseEquality(T);
            left = new BinOpNode(op, left, right);
        }

        return left;
    }

    // == !=  (NOT chainable)
    static ExprNode parseEquality(Tokenizer T)
    {
        ExprNode left = parseRelational(T);

        if (T.peek() == "==" || T.peek() == "!=")
        {
            Token op = T.next();
            ExprNode right = parseRelational(T);

            if (T.peek() == "==" || T.peek() == "!=")
                throw new System.Exception("invalid syntax");

            return new BinOpNode(op, left, right);
        }

        return left;
    }

    // < > <= >=  (NOT chainable)
    static ExprNode parseRelational(Tokenizer T)
    {
        ExprNode left = parseShift(T);

        if (T.peek() == "<" || T.peek() == ">" ||
            T.peek() == "<=" || T.peek() == ">=")
        {
            Token op = T.next();
            ExprNode right = parseShift(T);

            if (T.peek() == "<" || T.peek() == ">" ||
                T.peek() == "<=" || T.peek() == ">=")
                throw new System.Exception("invalid syntax");

            return new BinOpNode(op, left, right);
        }

        return left;
    }

    // << >>
    static ExprNode parseShift(Tokenizer T)
    {
        ExprNode left = parseAddSub(T);

        while (T.peek() == "<<" || T.peek() == ">>")
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
        ExprNode left = parsePower(T);

        while (T.peek() == "*" || T.peek() == "/")
        {
            Token op = T.next();
            ExprNode right = parsePower(T);
            left = new BinOpNode(op, left, right);
        }

        return left;
    }

    // ** (right associative)
    static ExprNode parsePower(Tokenizer T)
    {
        ExprNode left = parsePostfix(T);

        if (T.peek() == "**")
        {
            Token op = T.next();
            ExprNode right = parsePower(T);
            return new BinOpNode(op, left, right);
        }

        return left;
    }

    // . and function calls (highest precedence)
static ExprNode parsePostfix(Tokenizer T)
{
    ExprNode left = parsePrimary(T);

    while (true)
    {
        // member access
        if (T.peek() == ".")
        {
            Token op = T.next();
            Token next = T.next();

            if (next.sym != "ID")
                throw new System.Exception("invalid member access");

            ExprNode right = new VarNode(next);
            left = new BinOpNode(op, left, right);
        }

        // array access
        else if (T.peek() == "[")
        {
            T.next(); // consume [

            ExprNode index = parse(T);   // FULL expression

            T.expect("RBRACKET");

            left = new ArrayAccessNode(left, index);
        }

        // function call
        else if (T.peek() == "(")
        {
            T.next(); // consume '('

            if (T.peek() == ")")
            {
                T.next(); // consume ')'
                left = new CallNode(left, null);  // no-args
            }
            else
            {
                // FIRST ARGUMENT
                ExprNode arg = parse(T);  // IMPORTANT: parse(), not parseAssign

                // ADDITIONAL ARGUMENTS
                while (T.peek() == ",")
                {
                    Token comma = T.next();

                    if (T.peek() == ")" || T.peek() == ",")
                        throw new System.Exception("invalid syntax");

                    ExprNode rightArg = parse(T);  // IMPORTANT: parse()

                    arg = new BinOpNode(comma, arg, rightArg);
                }

                T.expect("RPAREN");

                left = new CallNode(left, arg);
            }
        }
        else
        {
            break;
        }
    }

    return left;
}

    static ExprNode parsePrimary(Tokenizer T)
    {
        Token tok = T.next();

        if (tok.sym == "NUM")
            return new NumNode(tok);

        if (tok.sym == "ID")
            return new VarNode(tok);

        if (tok.sym == "LPAREN")
        {
            ExprNode e = parse(T);
            T.expect("RPAREN");
            return e;
        }

        throw new System.Exception("invalid expression");
    }
}