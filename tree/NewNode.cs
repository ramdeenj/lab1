using System.Collections.Generic;

public class NewNode : ExprNode
{
    public ClassType classType;
    public ExprNode args;

    public NewNode(Token tok, ClassType classType, ExprNode args) : base(tok)
    {
        this.classType = classType;
        this.args = args ?? new NoArgsNode();
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { args };
    }

    public override void setType()
    {
        type = classType;
    }

    public override void typeCheck()
    {
        if (classType.declarer == null)
        {
            Utils.error($"Class {classType.name} is not defined");
            return;
        }

        var initMethod = classType.declarer.methods.Find(m => m.name == "__init__");
        var actual = new List<VarType>();
        collectArgs(args, actual);

        if (initMethod == null)
        {
            if (actual.Count != 0)
                Utils.error($"Class {classType.name} has no constructor but was called with {actual.Count} argument(s)");
            return;
        }

        var expected = initMethod.parameters;
        if (actual.Count != expected.Count)
        {
            Utils.error($"Constructor of {classType.name} expects {expected.Count} argument(s) but got {actual.Count}");
            return;
        }

        for (int i = 0; i < expected.Count; i++)
        {
            VarType expType = expected[i].vtype;
            VarType actType = actual[i];
            if (actType == null) continue;
            if (expType.GetType() != actType.GetType())
                Utils.error($"Constructor argument {i + 1}: expected {expType.typeName()} but got {actType.typeName()}");
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