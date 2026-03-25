using System.Collections.Generic;

public class FuncdefNode : TreeNode
{
    public string name;
    public Token nameToken;
    public List<(Token idToken, VarType vtype)> parameters;
    public StmtsNode body;
    public VarType returnType;

    public int maxTemporaries = 0;

    public FuncdefNode(string name, Token nameToken, List<(Token, VarType)> parameters, StmtsNode body, VarType returnType)
    {
        this.name = name;
        this.nameToken = nameToken;
        this.parameters = parameters;
        this.body = body;
        this.returnType = returnType;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { body };
    }

    public override void genCode()
    {
        // Walk tree post-order, assign a Temporary to every ExprNode
        int counter = 0;
        maxTemporaries = 0;
        assignTemporaries(this, ref counter);

        // Prologue
        ASM.Asm.emit(new ASM.Comment($"********** {name} **********"));
        ASM.Asm.emit(new ASM.Label(name));
        ASM.Asm.emit(new ASM.OpPushReg(ASM.Register.rbp));
        ASM.Asm.emit(new ASM.OpMovRegReg(ASM.Register.rsp, ASM.Register.rbp));
        ASM.Asm.emit(new ASM.Comment($"Allocate space for {maxTemporaries} temporaries"));
        if (maxTemporaries > 0)
            ASM.Asm.emit(new ASM.OpSubRegConstant(maxTemporaries * 16, ASM.Register.rsp));

        body.genCode();

        ASM.Asm.emit(new ASM.Comment($"********** End of {name} **********"));
    }

    private void assignTemporaries(TreeNode node, ref int counter)
    {
        foreach (var child in node.getChildNodes())
            assignTemporaries(child, ref counter);

        if (node is ExprNode expr)
        {
            expr.temporary = new Temporary(counter);
            counter++;
            if (counter > maxTemporaries)
                maxTemporaries = counter;
        }
    }
}