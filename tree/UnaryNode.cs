using System.Collections.Generic;

public class UnaryNode : ExprNode
{
    public ExprNode operand;

    public UnaryNode(Token tok, ExprNode operand) : base(tok)
    {
        this.operand = operand;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { operand };
    }

    public override void setType()
    {
        string op = token.lexeme;
        VarType T = operand.type;

        if (T == null) return;

        switch (op)
        {
            case "-":
                if (T is IntType) { type = new IntType(); return; }
                if (T is FloatType) { type = new FloatType(); return; }
                Utils.error($"Type error: cannot negate {T.GetType().Name}");
                break;

            case "~":
                if (T is IntType) { type = new IntType(); return; }
                Utils.error($"Type error: cannot apply '~' to {T.GetType().Name}");
                break;

            case "not":
                if (T is BoolType) { type = new BoolType(); return; }
                Utils.error($"Type error: cannot apply 'not' to {T.GetType().Name}");
                break;
        }
    }
}