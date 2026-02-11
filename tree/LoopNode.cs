//LoopNode.cs

using System.Collections.Generic;

public class LoopNode : StmtNode
{
    public ExprNode condition;
    public StmtsNode body;

    public LoopNode(ExprNode condition, StmtsNode body)
    {
        this.condition = condition;
        this.body = body;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { condition, body };
    }
}
