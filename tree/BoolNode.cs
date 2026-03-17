public class BoolNode : ExprNode
{
    public BoolNode(Token tok) : base(tok) { }

    public override void setType()
    {
        type = new BoolType();
    }
}