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
        {
            type = ft.returnType ?? new VoidType();
            return;
        }
        if (function is DotNode dn && dn.member.type is FuncType mft)
        {
            type = mft.returnType ?? new VoidType();
            return;
        }
    }

    public override void typeCheck()
    {
        if (function is VarNode vn)
        {
            if (vn.info == null) return;
            if (!(vn.info.type is FuncType ft))
            {
                Utils.error($"'{vn.token.lexeme}' is not a function");
                return;
            }
            checkArgs(ft, vn.token.lexeme);
            return;
        }

        if (function is DotNode dn)
        {
            if (dn.member.type == null) return;
            if (!(dn.member.type is FuncType mft))
            {
                Utils.error($"'{dn.member.token.lexeme}' is not a function");
                return;
            }
            checkArgs(mft, dn.member.token.lexeme);
            return;
        }
    }

    private void checkArgs(FuncType ft, string funcName)
    {
        var actual = new List<VarType>();
        collectArgs(args, actual);
        var expected = ft.parameters;

        if (actual.Count != expected.Count)
        {
            Utils.error($"Function {funcName} called with {actual.Count} argument(s) but expects {expected.Count}");
            return;
        }

        for (int i = 0; i < expected.Count; i++)
        {
            VarType expType = expected[i].type;
            VarType actType = actual[i];
            if (actType == null) continue;
            if (expType.GetType() != actType.GetType())
                Utils.error($"Argument {i + 1} of {funcName}: expected {expType.typeName()} but got {actType.typeName()}");
        }
    }

    private void collectArgs(ExprNode node, List<VarType> result)
    {
        if (node is NoArgsNode) return;
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