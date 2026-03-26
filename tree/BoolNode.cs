public class BoolNode : ExprNode
{
    public BoolNode(Token tok) : base(tok) { }

    public override void setType()
    {
        type = new BoolType();
    }

    public override void genCode()
    {
        long v = token.lexeme == "true" ? 1L : 0L;
        ASM.Asm.emit(new ASM.Comment($"Bool constant {token.lexeme}"));
        ASM.Asm.emit(new ASM.OpMovConstReg(v, ASM.Register.rax));
        temporary.moveFromRegister(ASM.Register.rax, ASM.StorageClass.STATIC);
    }
}