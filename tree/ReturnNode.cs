// ReturnNode.cs
public abstract class ReturnNode : StmtNode
{
    public Token RETURN;    // for error reporting

    protected ReturnNode(Token t)
    {
        this.RETURN = t;
    }

    public new static ReturnNode parse(Tokenizer T)
    {
        // consume the 'return' keyword
        var ret = T.expect("RETURN");

        // If the next token closes the block, this is a void return
        if (T.peek() == "RBRACE")
        {
            return new ReturnVoidNode(ret);
        }
        else
        {
            // otherwise, we expect an expression (currently just NUM)
            var expr = ExprNode.parse(T);
            return new ReturnExprNode(ret, expr);
        }
    }

    public static bool canParse(Tokenizer T)
    {
        return T.peek() == "RETURN";
    }
}

public class ReturnExprNode : ReturnNode
{
    public ExprNode expr;

    public ReturnExprNode(Token ret, ExprNode e) : base(ret)
    {
        this.expr = e;
    }
}

public class ReturnVoidNode : ReturnNode
{
    public ReturnVoidNode(Token ret) : base(ret) { }
}
