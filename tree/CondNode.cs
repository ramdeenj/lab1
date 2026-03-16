using System.Collections.Generic;

public class CondNode : StmtNode
{
    public ExprNode condition;
    public StmtsNode thenBranch;

    public CondNode(ExprNode condition, StmtsNode thenBranch)
    {
        this.condition = condition;
        this.thenBranch = thenBranch;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { condition, thenBranch };
    }
}
