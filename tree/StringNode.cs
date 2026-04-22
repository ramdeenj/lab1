public class StringNode : ExprNode
{
    public StringNode(Token tok) : base(tok)
    {
        // Validate escape sequences at parse time so invalid ones cause compile errors.
        StringPool.processEscapes(tok.lexeme);
    }
 
    public override void setType()
    {
        type = new StringType();
    }
 
    public override void genCode()
    {
        string label = StringPool.getLabel(token.lexeme);
        // Load address of the string constant (pointer to the pool entry) into rax.
        ASM.Asm.emit(new ASM.RawOp($"    leaq {label}(%rip), %rax"));
        temporary.moveFromRegister(ASM.Register.rax, ASM.StorageClass.STATIC);
    }
}