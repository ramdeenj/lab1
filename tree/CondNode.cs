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

    public override void typeCheck()
    {
        if (condition.type != null && !(condition.type is BoolType))
            Utils.error($"Type error: 'if' condition must be bool, got {condition.type.GetType().Name}");
    }
}