using System.Collections.Generic;

public class ReturnNode : StmtNode
{
    public ExprNode expr;
    public bool isRealReturn;

    public ReturnNode(ExprNode expr, bool isRealReturn = true)
    {
        this.expr = expr;
        this.isRealReturn = isRealReturn;
    }

    public override List<TreeNode> getChildNodes()
    {
        if (expr == null)
            return new List<TreeNode>();
        return new List<TreeNode> { expr };
    }

    public override void typeCheck()
    {
        if (!isRealReturn)
            return;

        FuncdefNode enclosing = null;
        TreeNode cur = this.parent;
        while (cur != null)
        {
            if (cur is FuncdefNode fd) { enclosing = fd; break; }
            cur = cur.parent;
        }

        if (enclosing == null)
            return;

        VarType declared = enclosing.returnType;

        if (expr == null)
        {
            if (declared != null)
                Utils.error($"Function {enclosing.name} must return {declared.GetType().Name} but has bare return");
            return;
        }

        VarType actual = expr.type;
        if (actual == null) return;

        if (declared == null)
        {
            if (!(actual is VoidType))
                Utils.error($"Function {enclosing.name} is void but returns a value");
            return;
        }

        if (declared.GetType() != actual.GetType())
            Utils.error($"Function {enclosing.name} declared return type {declared.GetType().Name} but returns {actual.GetType().Name}");
    }
}