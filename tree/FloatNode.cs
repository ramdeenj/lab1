public class FloatNode : ExprNode
{
    public FloatNode(Token tok) : base(tok) { }

    public override void setType()
    {
        type = new FloatType();
    }

    public override void genCode()
    {
        double f = double.Parse(token.lexeme,
            System.Globalization.CultureInfo.InvariantCulture);
        long bits = System.BitConverter.DoubleToInt64Bits(f);
        ASM.Asm.emit(new ASM.Comment($"Float constant {token}"));
        ASM.Asm.emit(new ASM.OpMovConstReg(bits, ASM.Register.rax));
        ASM.Asm.emit(new ASM.OpMovqRegXmm(ASM.Register.rax, ASM.Register.xmm0));
        temporary.moveFromXmmRegister(ASM.Register.xmm0, ASM.StorageClass.STATIC);
    }
}