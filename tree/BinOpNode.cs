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

        // Assignment: skip type checking per spec
        if (op == "=")
        {
            type = right.type;
            return;
        }

        VarType L = left.type;
        VarType R = right.type;

        // If either side is unknown (e.g. variable), skip
        if (L == null || R == null)
            return;

        switch (op)
        {
            case "+":
                if (L is IntType && R is IntType) { type = new IntType(); return; }
                if (L is FloatType && R is FloatType) { type = new FloatType(); return; }
                if (L is StringType && R is StringType) { type = new StringType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.GetType().Name} and {R.GetType().Name}");
                break;

            case "-":
            case "*":
            case "/":
            case "%":
            case "**":
                if (L is IntType && R is IntType) { type = new IntType(); return; }
                if (L is FloatType && R is FloatType) { type = new FloatType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.GetType().Name} and {R.GetType().Name}");
                break;

            case "<<":
            case ">>":
            case ">>>":
            case "&":
            case "|":
            case "^":
                if (L is IntType && R is IntType) { type = new IntType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.GetType().Name} and {R.GetType().Name}");
                break;

            case "==":
            case "!=":
                if (L.GetType() == R.GetType()) { type = new BoolType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.GetType().Name} and {R.GetType().Name}");
                break;

            case ">":
            case ">=":
            case "<":
            case "<=":
                if (L is IntType && R is IntType) { type = new BoolType(); return; }
                if (L is FloatType && R is FloatType) { type = new BoolType(); return; }
                if (L is StringType && R is StringType) { type = new BoolType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.GetType().Name} and {R.GetType().Name}");
                break;

            case "and":
            case "or":
                if (L is BoolType && R is BoolType) { type = new BoolType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.GetType().Name} and {R.GetType().Name}");
                break;

            default:
                // Unknown op (comma for args, dot for member): leave type null
                break;
        }
    }
}