using System.Collections.Generic;

public abstract class ExprNode : TreeNode
{
    private static int nextId = 0;
    public int unique;
    public Token token;
    public VarType type = null;

    protected ExprNode(Token token)
    {
        this.token = token;
        this.unique = nextId++;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode>();
    }

    public abstract void setType();

    public static ExprNode parse(Tokenizer T)
    {
        return parseAssign(T);
    }

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

    static ExprNode parseAnd(Tokenizer T)
    {
        ExprNode left = parseNot(T);
        while (T.peek() == "and")
        {
            Token op = T.next();
            ExprNode right = parseNot(T);
            left = new BinOpNode(op, left, right);
        }
        return left;
    }

    static ExprNode parseNot(Tokenizer T)
    {
        if (T.peek() == "not")
        {
            Token op = T.next();
            ExprNode operand = parseNot(T);
            return new UnaryNode(op, operand);
        }
        return parseBitOr(T);
    }

    static ExprNode parseBitOr(Tokenizer T)
    {
        ExprNode left = parseBitXor(T);
        while (T.peek() == "|")
        {
            Token op = T.next();
            ExprNode right = parseBitXor(T);
            left = new BinOpNode(op, left, right);
        }
        return left;
    }

    static ExprNode parseBitXor(Tokenizer T)
    {
        ExprNode left = parseBitAnd(T);
        while (T.peek() == "^")
        {
            Token op = T.next();
            ExprNode right = parseBitAnd(T);
            left = new BinOpNode(op, left, right);
        }
        return left;
    }

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

    static ExprNode parseRelational(Tokenizer T)
    {
        ExprNode left = parseShift(T);
        if (T.peek() == "<" || T.peek() == ">" || T.peek() == "<=" || T.peek() == ">=")
        {
            Token op = T.next();
            ExprNode right = parseShift(T);
            if (T.peek() == "<" || T.peek() == ">" || T.peek() == "<=" || T.peek() == ">=")
                throw new System.Exception("invalid syntax");
            return new BinOpNode(op, left, right);
        }
        return left;
    }

    static ExprNode parseShift(Tokenizer T)
    {
        ExprNode left = parseAddSub(T);
        while (T.peek() == "<<" || T.peek() == ">>" || T.peek() == ">>>")
        {
            Token op = T.next();
            ExprNode right = parseAddSub(T);
            left = new BinOpNode(op, left, right);
        }
        return left;
    }

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

    static ExprNode parseMulDiv(Tokenizer T)
    {
        ExprNode left = parseUnary(T);
        while (T.peek() == "*" || T.peek() == "/")
        {
            Token op = T.next();
            ExprNode right = parseUnary(T);
            left = new BinOpNode(op, left, right);
        }
        return left;
    }

    static ExprNode parseUnary(Tokenizer T)
    {
        if (T.peek() == "~")
        {
            Token op = T.next();
            ExprNode operand = parseUnary(T);
            return new UnaryNode(op, operand);
        }
        if (T.peek() == "-")
        {
            Token op = T.next();
            ExprNode operand = parseUnary(T);
            return new UnaryNode(op, operand);
        }
        return parsePower(T);
    }

    static ExprNode parsePower(Tokenizer T)
    {
        ExprNode left = parseCast(T);
        if (T.peek() == "**")
        {
            Token op = T.next();
            ExprNode right = parseUnary(T);
            return new BinOpNode(op, left, right);
        }
        return left;
    }

    static ExprNode parseCast(Tokenizer T)
    {
        ExprNode left = parsePostfix(T);
        while (T.peek() == "as")
        {
            Token asTok = T.next();
            Token typeTok = T.next();
            VarType targetType = VarType.fromToken(typeTok);
            left = new CastNode(asTok, left, targetType);
        }
        return left;
    }

    static ExprNode parsePostfix(Tokenizer T)
    {
        ExprNode left = parseNew(T);
        while (true)
        {
            if (T.peek() == ".")
            {
                Token dotTok = T.next();
                Token memberTok = T.next();
                if (memberTok.sym != "ID")
                    throw new System.Exception("Expected member name after '.'");
                var memberNode = new MemberNode(memberTok);
                left = new DotNode(dotTok, left, memberNode);
            }
            else if (T.peek() == "[")
            {
                T.next();
                ExprNode index = parse(T);
                T.expect("RBRACKET");
                left = new ArrayAccessNode(left, index);
            }
            else if (T.peek() == "(")
            {
                T.next();
                if (T.peek() == ")")
                {
                    T.next();
                    left = new CallNode(left, null);
                }
                else
                {
                    ExprNode arg = parse(T);
                    while (T.peek() == ",")
                    {
                        Token comma = T.next();
                        if (T.peek() == ")" || T.peek() == ",")
                            throw new System.Exception("invalid syntax");
                        ExprNode rightArg = parse(T);
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

    static ExprNode parseNew(Tokenizer T)
    {
        if (T.peek() == "new")
        {
            Token newTok = T.next();
            Token classNameTok = T.next();
            if (classNameTok.sym != "ID")
                throw new System.Exception("Expected class name after 'new'");
            ClassType ct = ProgramNode.getClassType(classNameTok);
            T.expect("LPAREN");
            ExprNode args = null;
            if (T.peek() == ")")
            {
                T.next();
            }
            else
            {
                ExprNode arg = parse(T);
                while (T.peek() == ",")
                {
                    Token comma = T.next();
                    if (T.peek() == ")" || T.peek() == ",")
                        throw new System.Exception("invalid syntax");
                    ExprNode rightArg = parse(T);
                    arg = new BinOpNode(comma, arg, rightArg);
                }
                T.expect("RPAREN");
                args = arg;
            }
            return new NewNode(newTok, ct, args);
        }
        return parsePrimary(T);
    }

    static ExprNode parsePrimary(Tokenizer T)
    {
        Token tok = T.next();
        if (tok.sym == "NUM") return new NumNode(tok);
        if (tok.sym == "FLOAT") return new FloatNode(tok);
        if (tok.sym == "STRING") return new StringNode(tok);
        if (tok.sym == "TRUE" || tok.sym == "FALSE") return new BoolNode(tok);
        if (tok.sym == "THIS") return new VarNode(tok);
        if (tok.sym == "ID") return new VarNode(tok);
        if (tok.sym == "LPAREN")
        {
            ExprNode e = parse(T);
            T.expect("RPAREN");
            return e;
        }
        throw new System.Exception($"invalid expression: got '{tok.lexeme}'");
    }
}