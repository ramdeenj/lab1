using System.Collections.Generic;

public class DotNode : ExprNode
{
    public ExprNode left;
    public MemberNode member;

    public DotNode(Token tok, ExprNode left, MemberNode member) : base(tok)
    {
        this.left = left;
        this.member = member;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { left, member };
    }

    public override void setType()
    {
        ClassType ct = left.type as ClassType;
        if (ct == null || ct.declarer == null) return;

        string memberName = member.token.lexeme;
        VarType memberType = ct.declarer.getMemberType(memberName);
        if (memberType == null) return;

        member.declaringClass = ct;
        member.type = memberType;
        type = memberType;
    }

    public override void typeCheck()
    {
        if (left.type == null) return;

        ClassType ct = left.type as ClassType;
        if (ct == null)
        {
            Utils.error($"Cannot access member of non-class type");
            return;
        }

        if (ct.declarer == null)
        {
            Utils.error($"Class {ct.name} is not defined");
            return;
        }

        // 'this' is only valid inside a class method body
        if (left is VarNode vn && vn.token.sym == "THIS")
        {
            bool insideClassMethod = false;
            TreeNode cur = this.parent;
            while (cur != null)
            {
                if (cur is ClassDeclNode) { insideClassMethod = true; break; }
                cur = cur.parent;
            }
            if (!insideClassMethod)
            {
                Utils.error("'this' used outside of a class method");
                return;
            }
        }

        string memberName = member.token.lexeme;
        VarType memberType = ct.declarer.getMemberType(memberName);
        if (memberType == null)
        {
            Utils.error($"Class {ct.name} has no member '{memberName}'");
            return;
        }
    }
}