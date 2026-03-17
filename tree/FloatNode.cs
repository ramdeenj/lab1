public class FloatNode : ExprNode
{
    public FloatNode(Token tok) : base(tok) { }

    public override void setType()
    {
        type = new FloatType();
    }
}