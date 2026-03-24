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
        if (function is VarNode vn && vn.info != null && vn.info.type is FuncType ft)
            type = ft.returnType ?? new VoidType();
    }

    public override void typeCheck()
    {
        if (!(function is VarNode vn) || vn.info == null)
            return;

        if (!(vn.info.type is FuncType ft))
        {
            Utils.error($"'{vn.token.lexeme}' is not a function");
            return;
        }

        var actualTypes = new List<VarType>();
        collectArgs(args, actualTypes);

        var expected = ft.parameters;

        if (actualTypes.Count != expected.Count)
        {
            Utils.error($"Function {vn.token.lexeme} called with {actualTypes.Count} argument(s) but expects {expected.Count}");
            return;
        }

        for (int i = 0; i < expected.Count; i++)
        {
            VarType expType = expected[i].type;
            VarType actType = actualTypes[i];
            if (actType == null) continue;
            if (expType.GetType() != actType.GetType())
                Utils.error($"Argument {i + 1} of {vn.token.lexeme}: expected {expType.GetType().Name} but got {actType.GetType().Name}");
        }
    }

    private void collectArgs(ExprNode node, List<VarType> result)
    {
        if (node is NoArgsNode)
            return;
        if (node is BinOpNode bin && bin.token.lexeme == ",")
        {
            collectArgs(bin.left, result);
            collectArgs(bin.right, result);
        }
        else
        {
            result.Add(node.type);
        }
    }
}