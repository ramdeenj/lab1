using System.Collections.Generic;

public class UnaryNode : ExprNode
{
    public ExprNode operand;

    public UnaryNode(Token tok, ExprNode operand) : base(tok)
    {
        this.operand = operand;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { operand };
    }

    public override void setType()
    {
        string op = token.lexeme;
        VarType T = operand.type;

        if (T == null) return;

        switch (op)
        {
            case "-":
                if (T is IntType) { type = new IntType(); return; }
                if (T is FloatType) { type = new FloatType(); return; }
                Utils.error($"Type error: cannot negate {T.GetType().Name}");
                break;

            case "~":
                if (T is IntType) { type = new IntType(); return; }
                Utils.error($"Type error: cannot apply '~' to {T.GetType().Name}");
                break;

            case "not":
                if (T is BoolType) { type = new BoolType(); return; }
                Utils.error($"Type error: cannot apply 'not' to {T.GetType().Name}");
                break;
        }
    }

    public override void genCode()
    {
        operand.genCode();

        string op = token.lexeme;

        if (type is IntType)
        {
            operand.temporary.moveToRegister(ASM.Register.rax);

            switch (op)
            {
                case "-":
                    ASM.Asm.emit(new ASM.Comment("integer negate"));
                    ASM.Asm.emit(new ASM.OpNeg(ASM.Register.rax));
                    break;

                case "~":
                    ASM.Asm.emit(new ASM.Comment("bitwise NOT"));
                    ASM.Asm.emit(new ASM.OpNot(ASM.Register.rax));
                    break;

                default:
                    throw new System.NotImplementedException(
                        $"UnaryNode.genCode: unhandled int op '{op}'");
            }

            temporary.moveFromRegister(ASM.Register.rax, ASM.StorageClass.STATIC);
        }
        else if (type is FloatType)
        {
            // Float negate: XOR the sign bit using integer ops
            operand.temporary.moveToRegister(ASM.Register.rax);
            long signBit = unchecked((long)0x8000000000000000L);
            ASM.Asm.emit(new ASM.Comment("float negate: flip sign bit"));
            ASM.Asm.emit(new ASM.OpMovConstReg(signBit, ASM.Register.rbx));
            ASM.Asm.emit(new ASM.OpXor(ASM.Register.rax, ASM.Register.rbx));
            ASM.Asm.emit(new ASM.OpMovqRegXmm(ASM.Register.rax, ASM.Register.xmm0));
            temporary.moveFromXmmRegister(ASM.Register.xmm0, ASM.StorageClass.STATIC);
        }
        else
        {
            throw new System.NotImplementedException(
                $"UnaryNode.genCode: unhandled type for op '{op}'");
        }
    }
}