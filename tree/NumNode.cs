public class NumNode : ExprNode
{
    public NumNode(Token tok) : base(tok) { }

    public override void setType()
    {
        type = new IntType();
    }

    public override void genCode()
    {
        long v = long.Parse(token.lexeme);
        ASM.Asm.emit(new ASM.Comment($"Constant {token}"));
        ASM.Asm.emit(new ASM.OpMovConstReg(v, ASM.Register.rax));
        temporary.moveFromRegister(ASM.Register.rax, ASM.StorageClass.STATIC);
    }
}