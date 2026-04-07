public class VarNode : ExprNode
{
    public VarInfo? info;

    public VarNode(Token tok) : base(tok)
    {
        info = SymbolTable.lookupIfExists(tok.lexeme);
    }

    public override void setType()
    {
        if (info != null)
            type = info.type;
    }

    public override void typeCheck()
    {
        if (info != null && info.type is FuncType)
        {
            TreeNode cur = this.parent;
            if (cur is CallNode cn && cn.function == this)
                return;
            Utils.error($"'{token.lexeme}' is a function and cannot be used as a value");
        }
    }

    private int localOffset(int slotIndex)
    {
        TreeNode cur = this.parent;
        while (cur != null && !(cur is FuncdefNode))
            cur = cur.parent;
        int maxTemp = (cur is FuncdefNode fn) ? fn.maxTemporaries : 0;
        return -(maxTemp * 16 + slotIndex * 8 + 8);
    }

    public override void genCode()
    {
        if (info == null) return;

        if (info.location is GlobalLocation gl)
        {
            ASM.Asm.emit(new ASM.RawOp($"    movq {gl.label}(%rip), %rax"));
            temporary.moveFromRegister(ASM.Register.rax, ASM.StorageClass.STATIC);
        }
        else if (info.location is LocalLocation ll)
        {
            int off = localOffset(ll.offset);
            ASM.Asm.emit(new ASM.OpMovRegIndReg(off, ASM.Register.rbp, ASM.Register.rax));
            temporary.moveFromRegister(ASM.Register.rax, ASM.StorageClass.STATIC);
        }
        else if (info.location is ParameterLocation pl)
        {
            ASM.Asm.emit(new ASM.OpMovRegIndReg(pl.offset, ASM.Register.rbp, ASM.Register.rax));
            temporary.moveFromRegister(ASM.Register.rax, ASM.StorageClass.STATIC);
        }
    }

    public void storeFromRegister()
    {
        if (info == null) return;

        if (info.location is GlobalLocation gl)
        {
            ASM.Asm.emit(new ASM.RawOp($"    movq %rax, {gl.label}(%rip)"));
        }
        else if (info.location is LocalLocation ll)
        {
            int off = localOffset(ll.offset);
            ASM.Asm.emit(new ASM.OpMovRegRegInd(ASM.Register.rax, off, ASM.Register.rbp));
        }
        else if (info.location is ParameterLocation pl)
        {
            ASM.Asm.emit(new ASM.OpMovRegRegInd(ASM.Register.rax, pl.offset, ASM.Register.rbp));
        }
    }
}