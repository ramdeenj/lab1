using System.Collections.Generic;

public class FuncdefNode : TreeNode
{
    public string name;
    public Token nameToken;
    public List<(Token idToken, VarType vtype)> parameters;
    public StmtsNode body;
    public VarType returnType;

    public int maxTemporaries = 0;
    public int localVarBytes = 0;

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

    public override void setupCFG()
    {
        entry.addNext(body);
        body.exit.addNext(exit);
    }

    public override void genCode()
    {
        int counter = 0;
        maxTemporaries = 0;
        assignTemporaries(this, ref counter);

        int localBytes = localVarBytes;

        patchParamOffsets(this);

        ASM.Asm.emit(new ASM.Label(name));
        ASM.Asm.emit(new ASM.OpPushReg(ASM.Register.rbp));
        ASM.Asm.emit(new ASM.OpMovRegReg(ASM.Register.rsp, ASM.Register.rbp));

        int totalBytes = maxTemporaries * 16 + localBytes;
        totalBytes = (totalBytes + 15) & ~15;

        if (totalBytes > 0)
            ASM.Asm.emit(new ASM.OpSubRegConstant(totalBytes, ASM.Register.rsp));

        if (name == "main")
        {
            ASM.Asm.emit(new ASM.RawOp("    movq $0x8007, %rcx"));
            ASM.Asm.emit(new ASM.RawOp("    callq SetErrorMode"));
            ASM.Asm.emit(new ASM.RawOp("    subq $32, %rsp"));
            ASM.Asm.emit(new ASM.RawOp("    callq _rtinit"));
            ASM.Asm.emit(new ASM.RawOp("    addq $32, %rsp"));
        }

        body.genCode();

        ASM.Asm.emit(new ASM.OpMovRegReg(ASM.Register.rbp, ASM.Register.rsp));
        ASM.Asm.emit(new ASM.OpPopReg(ASM.Register.rbp));
        ASM.Asm.emit(new ASM.Ret());
    }

    private void patchParamOffsets(TreeNode node)
    {
        foreach (var child in node.getChildNodes())
            patchParamOffsets(child);

        if (node is VarNode vn && vn.info != null && vn.info.location is ParameterLocation pl)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].idToken.lexeme == vn.token.lexeme)
                {
                    pl.offset = 16 + i * 16;
                    break;
                }
            }
        }
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