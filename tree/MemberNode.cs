using System.Collections.Generic;

public class MemberNode : ExprNode
{
    public ClassType declaringClass;

    public MemberNode(Token tok) : base(tok) { }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode>();
    }

    public override void setType() { }
}