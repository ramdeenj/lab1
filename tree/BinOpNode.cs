using System.Collections.Generic;

public class BinOpNode : ExprNode
{
    public ExprNode left;
    public ExprNode right;

    public BinOpNode(Token tok, ExprNode left, ExprNode right) : base(tok)
    {
        this.left = left;
        this.right = right;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { left, right };
    }

    public override void setType()
    {
        string op = token.lexeme;

        if (op == "=")
        {
            type = right.type;
            return;
        }

        VarType L = left.type;
        VarType R = right.type;

        if (L == null || R == null)
            return;

        switch (op)
        {
            case "+":
                if (L is IntType && R is IntType) { type = new IntType(); return; }
                if (L is FloatType && R is FloatType) { type = new FloatType(); return; }
                if (L is StringType && R is StringType) { type = new StringType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.typeName()} and {R.typeName()}");
                break;

            case "-":
            case "*":
            case "/":
            case "%":
            case "**":
                if (L is IntType && R is IntType) { type = new IntType(); return; }
                if (L is FloatType && R is FloatType) { type = new FloatType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.typeName()} and {R.typeName()}");
                break;

            case "<<":
            case ">>":
            case ">>>":
            case "&":
            case "|":
            case "^":
                if (L is IntType && R is IntType) { type = new IntType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.typeName()} and {R.typeName()}");
                break;

            case "==":
            case "!=":
                if (L is ClassType lct && R is ClassType rct && lct.name == rct.name) { type = new BoolType(); return; }
                if (!(L is ClassType) && !(R is ClassType) && L.GetType() == R.GetType()) { type = new BoolType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.typeName()} and {R.typeName()}");
                break;

            case ">":
            case ">=":
            case "<":
            case "<=":
                if (L is IntType && R is IntType) { type = new BoolType(); return; }
                if (L is FloatType && R is FloatType) { type = new BoolType(); return; }
                if (L is StringType && R is StringType) { type = new BoolType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.typeName()} and {R.typeName()}");
                break;

            case "and":
            case "or":
                if (L is BoolType && R is BoolType) { type = new BoolType(); return; }
                Utils.error($"Type error: cannot apply '{op}' to {L.typeName()} and {R.typeName()}");
                break;

            default:
                break;
        }
    }

    public override void typeCheck()
    {
        if (token.lexeme == "=")
        {
            VarType L = left.type;
            VarType R = right.type;
            if (L == null || R == null) return;

            if (L is ClassType lc && R is ClassType rc)
            {
                if (lc.name != rc.name)
                    Utils.error($"Type error: cannot assign {R.typeName()} to {L.typeName()}");
                return;
            }

            if (L.GetType() != R.GetType())
                Utils.error($"Type error: cannot assign {R.typeName()} to {L.typeName()}");
        }
    }

    public override void genCode()
    {
        string op = token.lexeme;

        left.genCode();
        right.genCode();

        if (type is IntType)
            genCodeInt(op);
        else if (type is FloatType)
            genCodeFloat(op);
        else if (type is BoolType)
            genCodeBool(op);
        else
            throw new System.NotImplementedException(
                $"BinOpNode.genCode: no codegen for op '{op}' on type {type}");
    }

    private void genCodeInt(string op)
    {
        left.temporary.moveToRegister(ASM.Register.rax);
        right.temporary.moveToRegister(ASM.Register.rbx);

        switch (op)
        {
            case "+":
                ASM.Asm.emit(new ASM.OpAdd(ASM.Register.rax, ASM.Register.rbx));
                break;

            case "-":
                ASM.Asm.emit(new ASM.OpSub(ASM.Register.rax, ASM.Register.rbx));
                break;

            case "*":
                ASM.Asm.emit(new ASM.OpMul(ASM.Register.rax, ASM.Register.rbx));
                break;

            case "/":
                ASM.Asm.emit(new ASM.OpCqo());
                ASM.Asm.emit(new ASM.OpIDiv(ASM.Register.rbx));
                break;

            case "%":
                ASM.Asm.emit(new ASM.OpCqo());
                ASM.Asm.emit(new ASM.OpIDiv(ASM.Register.rbx));
                ASM.Asm.emit(new ASM.OpMovRegReg(ASM.Register.rdx, ASM.Register.rax));
                break;

            case "&":
                ASM.Asm.emit(new ASM.OpAnd(ASM.Register.rax, ASM.Register.rbx));
                break;

            case "|":
                ASM.Asm.emit(new ASM.OpOr(ASM.Register.rax, ASM.Register.rbx));
                break;

            case "^":
                ASM.Asm.emit(new ASM.OpXor(ASM.Register.rax, ASM.Register.rbx));
                break;

            case "<<":
                ASM.Asm.emit(new ASM.OpMovRegReg(ASM.Register.rbx, ASM.Register.rcx));
                ASM.Asm.emit(new ASM.OpShl(ASM.Register.rax));
                ASM.Asm.emit(new ASM.Comment("if shift count >= 64, zero the result"));
                ASM.Asm.emit(new ASM.OpMovConstReg(0, ASM.Register.rdx));
                ASM.Asm.emit(new ASM.RawOp("    cmpq $64, %rbx"));
                ASM.Asm.emit(new ASM.RawOp("    cmovgeq %rdx, %rax"));
                break;

            case ">>":
                ASM.Asm.emit(new ASM.OpMovRegReg(ASM.Register.rbx, ASM.Register.rcx));
                ASM.Asm.emit(new ASM.RawOp("    cmpq $63, %rcx"));
                ASM.Asm.emit(new ASM.RawOp("    movq $63, %rdx"));
                ASM.Asm.emit(new ASM.RawOp("    cmovgq %rdx, %rcx"));
                ASM.Asm.emit(new ASM.OpSar(ASM.Register.rax));
                break;

            case ">>>":
                ASM.Asm.emit(new ASM.OpMovRegReg(ASM.Register.rbx, ASM.Register.rcx));
                ASM.Asm.emit(new ASM.OpShr(ASM.Register.rax));
                ASM.Asm.emit(new ASM.Comment("if shift count >= 64, zero the result"));
                ASM.Asm.emit(new ASM.OpMovConstReg(0, ASM.Register.rdx));
                ASM.Asm.emit(new ASM.RawOp("    cmpq $64, %rbx"));
                ASM.Asm.emit(new ASM.RawOp("    cmovgeq %rdx, %rax"));
                break;

            default:
                throw new System.NotImplementedException(
                    $"BinOpNode.genCodeInt: unhandled op '{op}'");
        }

        temporary.moveFromRegister(ASM.Register.rax, ASM.StorageClass.STATIC);
    }

    private void genCodeFloat(string op)
    {
        left.temporary.moveToXmmRegister(ASM.Register.xmm0);
        right.temporary.moveToXmmRegister(ASM.Register.xmm1);

        switch (op)
        {
            case "+":
                ASM.Asm.emit(new ASM.OpAddsd(ASM.Register.xmm0, ASM.Register.xmm1));
                break;
            case "-":
                ASM.Asm.emit(new ASM.OpSubsd(ASM.Register.xmm0, ASM.Register.xmm1));
                break;
            case "*":
                ASM.Asm.emit(new ASM.OpMulsd(ASM.Register.xmm0, ASM.Register.xmm1));
                break;
            case "/":
                ASM.Asm.emit(new ASM.OpDivsd(ASM.Register.xmm0, ASM.Register.xmm1));
                break;
            default:
                throw new System.NotImplementedException(
                    $"BinOpNode.genCodeFloat: unhandled op '{op}'");
        }

        temporary.moveFromXmmRegister(ASM.Register.xmm0, ASM.StorageClass.STATIC);
    }

    private void genCodeBool(string op)
    {
        // and / or: bools are 0 or 1, so bitwise AND/OR works perfectly
        if (op == "and" || op == "or")
        {
            left.temporary.moveToRegister(ASM.Register.rax);
            right.temporary.moveToRegister(ASM.Register.rbx);
            if (op == "and")
                ASM.Asm.emit(new ASM.OpAnd(ASM.Register.rax, ASM.Register.rbx));
            else
                ASM.Asm.emit(new ASM.OpOr(ASM.Register.rax, ASM.Register.rbx));
            temporary.moveFromRegister(ASM.Register.rax, ASM.StorageClass.STATIC);
            return;
        }

        VarType operandType = left.type;

        if (operandType is FloatType)
        {
            left.temporary.moveToXmmRegister(ASM.Register.xmm0);
            right.temporary.moveToXmmRegister(ASM.Register.xmm1);
            // compare, then immediately setcc with no instructions in between
            ASM.Asm.emit(new ASM.RawOp("    comisd %xmm1, %xmm0"));
        }
        else
        {
            left.temporary.moveToRegister(ASM.Register.rax);
            right.temporary.moveToRegister(ASM.Register.rbx);
            // compare, then immediately setcc with no instructions in between
            ASM.Asm.emit(new ASM.RawOp("    cmpq %rbx, %rax"));
        }

        string setcc = op switch
        {
            "==" => "sete",
            "!=" => "setne",
            "<"  => operandType is FloatType ? "setb"  : "setl",
            "<=" => operandType is FloatType ? "setbe" : "setle",
            ">"  => operandType is FloatType ? "seta"  : "setg",
            ">=" => operandType is FloatType ? "setae" : "setge",
            _    => throw new System.NotImplementedException(
                        $"BinOpNode.genCodeBool: unhandled op '{op}'")
        };

        // setcc writes only the low byte — use movzbl to zero-extend into eax
        // (writing eax automatically zeros the upper 32 bits of rax on x86-64)
        ASM.Asm.emit(new ASM.RawOp($"    {setcc} %al"));
        ASM.Asm.emit(new ASM.RawOp("    movzbl %al, %eax"));
        temporary.moveFromRegister(ASM.Register.rax, ASM.StorageClass.STATIC);
    }
}