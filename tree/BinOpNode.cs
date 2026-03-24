using System.Collections.Generic;

public class BinOpNode : ExprNode
{
    public ExprNode left;
    public ExprNode right;

    public BinOpNode(Token tok, ExprNode left, ExprNode right) : base(tok)
    {
        this.left = left;
        this.right = right;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { left, right };
    }

    public override void setType()
    {
        string op = token.lexeme;

        if (op == "=")
        {
            type = right.type;
            return;
        }

        VarType L = left.type;
        VarType R = right.type;

        if (L == null || R == null)
            return;

        switch (op)
        {
            case "+":
                if (L is IntType && R is IntType) { type = new IntType(); return; }
                if (L is FloatType && R is FloatType) { type = new FloatType(); return; }
                if (L is StringType && R is StringType) { type = new StringType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.typeName()} and {R.typeName()}");
                break;

            case "-":
            case "*":
            case "/":
            case "%":
            case "**":
                if (L is IntType && R is IntType) { type = new IntType(); return; }
                if (L is FloatType && R is FloatType) { type = new FloatType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.typeName()} and {R.typeName()}");
                break;

            case "<<":
            case ">>":
            case ">>>":
            case "&":
            case "|":
            case "^":
                if (L is IntType && R is IntType) { type = new IntType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.typeName()} and {R.typeName()}");
                break;

            case "==":
            case "!=":
                if (L is ClassType lct && R is ClassType rct && lct.name == rct.name) { type = new BoolType(); return; }
                if (!(L is ClassType) && !(R is ClassType) && L.GetType() == R.GetType()) { type = new BoolType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.typeName()} and {R.typeName()}");
                break;

            case ">":
            case ">=":
            case "<":
            case "<=":
                if (L is IntType && R is IntType) { type = new BoolType(); return; }
                if (L is FloatType && R is FloatType) { type = new BoolType(); return; }
                if (L is StringType && R is StringType) { type = new BoolType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.typeName()} and {R.typeName()}");
                break;

            case "and":
            case "or":
                if (L is BoolType && R is BoolType) { type = new BoolType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.typeName()} and {R.typeName()}");
                break;

            default:
                break;
        }
    }

    public override void typeCheck()
    {
        if (token.lexeme == "=")
        {
            VarType L = left.type;
            VarType R = right.type;
            if (L == null || R == null) return;

            if (L is ClassType lc && R is ClassType rc)
            {
                if (lc.name != rc.name)
                    Utils.error($"Type error: cannot assign {R.typeName()} to {L.typeName()}");
                return;
            }

            if (L.GetType() != R.GetType())
                Utils.error($"Type error: cannot assign {R.typeName()} to {L.typeName()}");
        }
    }
}