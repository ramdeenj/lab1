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

    public override void typeCheck()
    {
        if (condition.type != null && !(condition.type is BoolType))
            Utils.error($"Type error: 'while' condition must be bool, got {condition.type.GetType().Name}");
    }
}