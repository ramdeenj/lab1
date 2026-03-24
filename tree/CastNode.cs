using System.Collections.Generic;

public class CastNode : ExprNode
{
    public ExprNode operand;
    public VarType targetType;

    public CastNode(Token tok, ExprNode operand, VarType targetType) : base(tok)
    {
        this.operand = operand;
        this.targetType = targetType;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { operand };
    }

    public override void setType()
    {
        type = targetType;
    }

    public override void typeCheck()
    {
        VarType from = operand.type;
        VarType to = targetType;

        if (from == null || to == null)
            return;

        // int, float, string can cast to int, float, string
        // bool can only cast to bool
        bool ok = false;

        if (from is IntType)
            ok = (to is IntType || to is FloatType || to is StringType);
        else if (from is FloatType)
            ok = (to is IntType || to is FloatType || to is StringType);
        else if (from is StringType)
            ok = (to is IntType || to is FloatType || to is StringType);
        else if (from is BoolType)
            ok = (to is BoolType);

        if (!ok)
            Utils.error($"Type error: cannot cast {from.GetType().Name} to {to.GetType().Name}");
    }
}