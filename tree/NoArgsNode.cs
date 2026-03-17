using System.Collections.Generic;

public class NoArgsNode : ExprNode
{
    public NoArgsNode() : base(new Token("NO_ARGS", 0, "no-args")) { }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode>();
    }

    public override void setType() { }
}