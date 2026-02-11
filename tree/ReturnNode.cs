//ReturnNode.cs

using System.Collections.Generic;

public class ReturnNode : StmtNode
{
    public ExprNode expr;

    public ReturnNode(ExprNode expr)
    {
        this.expr = expr;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { expr };
    }
}
