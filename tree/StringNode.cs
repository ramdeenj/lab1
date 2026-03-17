public class StringNode : ExprNode
{
    public StringNode(Token tok) : base(tok) { }

    public override void setType()
    {
        type = new StringType();
    }
}