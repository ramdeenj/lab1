using System.Collections.Generic;

public class UnaryNode : ExprNode
{
    public ExprNode operand;

    public UnaryNode(Token tok, ExprNode operand) : base(tok)
    {
        this.operand = operand;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { operand };
    }
}