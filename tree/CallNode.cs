using System.Collections.Generic;

public class CallNode : ExprNode
{
    public ExprNode function;
    public ExprNode args;

    public CallNode(ExprNode function, ExprNode args)
        : base(new Token("FUNC_CALL", 0, "func-call"))
    {
        this.function = function;
        this.args = args ?? new NoArgsNode();
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { function, args };
    }

    public override void setType()
    {
        // Return type unknown without function signatures
    }
}