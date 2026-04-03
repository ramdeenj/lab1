using System.Collections.Generic;

public class FuncdefNode : TreeNode
{
    public string name;
    public Token nameToken;
    public List<(Token idToken, VarType vtype)> parameters;
    public StmtsNode body;
    public VarType returnType;

    public int maxTemporaries = 0;
    public int localVarBytes = 0;  // bytes needed for local variables

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
        int counter = 0;
        maxTemporaries = 0;
        assignTemporaries(this, ref counter);

        // How many bytes of locals were allocated during parse?
        // We stored this per-function at parse time.
        int localBytes = localVarBytes;

        // Prologue
        ASM.Asm.emit(new ASM.Comment($"********** {name} **********"));
        ASM.Asm.emit(new ASM.Label(name));
        ASM.Asm.emit(new ASM.OpPushReg(ASM.Register.rbp));
        ASM.Asm.emit(new ASM.OpMovRegReg(ASM.Register.rsp, ASM.Register.rbp));

        // Allocate space for temporaries AND local variables.
        // Temporaries start at rbp-8, -16, ... (each 16 bytes for value+class)
        // Local variables are placed BELOW the temporaries region.
        // We put locals at: rbp - (maxTemporaries*16) - 8, -16, ...
        // But simpler: locals use rbp-relative offsets that are tracked in LocalLocation.
        // We just need to allocate enough stack space.
        int totalBytes = maxTemporaries * 16 + localBytes;
        // Round up to 16-byte alignment
        totalBytes = (totalBytes + 15) & ~15;

        ASM.Asm.emit(new ASM.Comment($"Allocate {maxTemporaries} temporaries + {localBytes} bytes for locals"));
        if (totalBytes > 0)
            ASM.Asm.emit(new ASM.OpSubRegConstant(totalBytes, ASM.Register.rsp));

        // Suppress Windows crash dialogs so crashes are detectable by exit code
        if (name == "main")
        {
            ASM.Asm.emit(new ASM.Comment("Suppress Windows crash dialogs"));
            ASM.Asm.emit(new ASM.RawOp("    movq $0x8007, %rcx"));
            ASM.Asm.emit(new ASM.RawOp("    callq SetErrorMode"));
        }

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