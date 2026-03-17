public class NumNode : ExprNode
{
    public NumNode(Token tok) : base(tok) { }

    public override void setType()
    {
        type = new IntType();
    }
}