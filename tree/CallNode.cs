using System.Collections.Generic;

public class CallNode : ExprNode
{
    public ExprNode function;
    public ExprNode args;

    public CallNode(ExprNode function, ExprNode args)
        : base(new Token("FUNC_CALL", 0, "func-call"))
    {
        this.function = function;
        this.args = args ?? new NoArgsNode();
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { function, args };
    }

    public override void setType()
    {
        if (function is VarNode vn && vn.info != null && vn.info.type is FuncType ft)
        {
            type = ft.returnType ?? new VoidType();
            return;
        }
        if (function is DotNode dn && dn.member.type is FuncType mft)
        {
            type = mft.returnType ?? new VoidType();
            return;
        }
    }

    public override void typeCheck()
    {
        if (function is VarNode vn)
        {
            if (vn.info == null) return;
            if (!(vn.info.type is FuncType ft))
            {
                Utils.error($"'{vn.token.lexeme}' is not a function");
                return;
            }
            checkArgs(ft, vn.token.lexeme);
            return;
        }

        if (function is DotNode dn)
        {
            if (dn.member.type == null) return;
            if (!(dn.member.type is FuncType mft))
            {
                Utils.error($"'{dn.member.token.lexeme}' is not a function");
                return;
            }
            checkArgs(mft, dn.member.token.lexeme);
            return;
        }
    }

    private void checkArgs(FuncType ft, string funcName)
    {
        var actual = new List<VarType>();
        collectArgs(args, actual);
        var expected = ft.parameters;

        if (actual.Count != expected.Count)
        {
            Utils.error($"Function {funcName} called with {actual.Count} argument(s) but expects {expected.Count}");
            return;
        }

        for (int i = 0; i < expected.Count; i++)
        {
            VarType expType = expected[i].type;
            VarType actType = actual[i];
            if (actType == null) continue;
            if (expType.GetType() != actType.GetType())
                Utils.error($"Argument {i + 1} of {funcName}: expected {expType.typeName()} but got {actType.typeName()}");
        }
    }

    private void collectArgs(ExprNode node, List<VarType> result)
    {
        if (node is NoArgsNode) return;
        if (node is BinOpNode bin && bin.token.lexeme == ",")
        {
            collectArgs(bin.left, result);
            collectArgs(bin.right, result);
        }
        else
        {
            result.Add(node.type);
        }
    }

    private void collectArgExprs(ExprNode node, List<ExprNode> result)
    {
        if (node is NoArgsNode) return;
        if (node is BinOpNode bin && bin.token.lexeme == ",")
        {
            collectArgExprs(bin.left, result);
            collectArgExprs(bin.right, result);
        }
        else
        {
            result.Add(node);
        }
    }

    private void genCodeArgs(ExprNode node)
    {
        if (node is NoArgsNode) return;
        if (node is BinOpNode bin && bin.token.lexeme == ",")
        {
            genCodeArgs(bin.left);
            genCodeArgs(bin.right);
        }
        else
        {
            node.genCode();
        }
    }

    public override void genCode()
    {
        genCodeArgs(args);

        var argExprs = new List<ExprNode>();
        collectArgExprs(args, argExprs);
        int argCount = argExprs.Count;

        // Check if this is a builtin (C foreign function) call
        bool isBuiltin = false;
        if (function is VarNode vn3 && vn3.info?.type is FuncType ft3)
            isBuiltin = ft3.isBuiltin;

        if (isBuiltin)
        {
            // Push args right-to-left onto the stack (value + storage class, 16 bytes each)
            for (int i = argCount - 1; i >= 0; i--)
            {
                ExprNode arg = argExprs[i];
                ASM.Asm.emit(new ASM.OpMovConstReg((long)ASM.StorageClass.STATIC, ASM.Register.rax));
                ASM.Asm.emit(new ASM.OpPushReg(ASM.Register.rax));

                if (arg.type is FloatType)
                {
                    arg.temporary.moveToXmmRegister(ASM.Register.xmm0);
                    ASM.Asm.emit(new ASM.OpMovqXmmReg(ASM.Register.xmm0, ASM.Register.rax));
                }
                else
                {
                    arg.temporary.moveToRegister(ASM.Register.rax);
                }
                ASM.Asm.emit(new ASM.OpPushReg(ASM.Register.rax));
            }

            // Pass stack pointer (pointing at the args) in rcx — the C ABI first parameter
            ASM.Asm.emit(new ASM.RawOp("    movq %rsp, %rcx"));

            // Add 32 bytes of shadow space required by Windows x64 ABI
            ASM.Asm.emit(new ASM.RawOp("    subq $32, %rsp"));

            string funcName = ((VarNode)function).token.lexeme;
            ASM.Asm.emit(new ASM.RawOp($"    callq _{funcName}"));

            // Discard shadow space
            ASM.Asm.emit(new ASM.RawOp("    addq $32, %rsp"));

            // Discard arguments from stack
            if (argCount > 0)
                ASM.Asm.emit(new ASM.RawOp($"    addq ${argCount * 16}, %rsp"));

            // Return value comes back in rax
            temporary.moveFromRegister(ASM.Register.rax, ASM.StorageClass.STATIC);
        }
        else
        {
            // Normal (non-builtin) function call
            for (int i = argCount - 1; i >= 0; i--)
            {
                ExprNode arg = argExprs[i];
                ASM.Asm.emit(new ASM.OpMovConstReg((long)ASM.StorageClass.STATIC, ASM.Register.rax));
                ASM.Asm.emit(new ASM.OpPushReg(ASM.Register.rax));

                if (arg.type is FloatType)
                {
                    arg.temporary.moveToXmmRegister(ASM.Register.xmm0);
                    ASM.Asm.emit(new ASM.OpMovqXmmReg(ASM.Register.xmm0, ASM.Register.rax));
                }
                else
                {
                    arg.temporary.moveToRegister(ASM.Register.rax);
                }
                ASM.Asm.emit(new ASM.OpPushReg(ASM.Register.rax));
            }

            if (function is VarNode vn2)
            {
                ASM.Asm.emit(new ASM.RawOp($"    callq {vn2.token.lexeme}"));
            }
            else
            {
                throw new System.NotImplementedException("CallNode.genCode: non-VarNode function not supported");
            }

            if (argCount > 0)
                ASM.Asm.emit(new ASM.RawOp($"    addq ${argCount * 16}, %rsp"));

            temporary.moveFromRegister(ASM.Register.rax, ASM.StorageClass.STATIC);
        }
    }
}