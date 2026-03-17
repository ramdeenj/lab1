public class VarNode : ExprNode
{
    public VarNode(Token tok) : base(tok) { }

    public override void setType()
    {
        // Variables: type unknown 
    }
}