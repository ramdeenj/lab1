using System.Collections.Generic;

public class BinOpNode : ExprNode
{
    public ExprNode left;
    public ExprNode right;

    public BinOpNode(Token tok, ExprNode left, ExprNode right) : base(tok)
    {
        this.left = left;
        this.right = right;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { left, right };
    }
}
