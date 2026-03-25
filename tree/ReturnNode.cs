using System.Collections.Generic;

public class ReturnNode : StmtNode
{
    public ExprNode expr;
    public bool isRealReturn;

    public ReturnNode(ExprNode expr, bool isRealReturn = true)
    {
        this.expr = expr;
        this.isRealReturn = isRealReturn;
    }

    public override List<TreeNode> getChildNodes()
    {
        if (expr == null)
            return new List<TreeNode>();
        return new List<TreeNode> { expr };
    }

    public override void typeCheck()
    {
        if (!isRealReturn)
            return;

        FuncdefNode enclosing = null;
        TreeNode cur = this.parent;
        while (cur != null)
        {
            if (cur is FuncdefNode fd) { enclosing = fd; break; }
            cur = cur.parent;
        }

        if (enclosing == null)
            return;

        VarType declared = enclosing.returnType;

        if (expr == null)
        {
            if (declared != null)
                Utils.error($"Function {enclosing.name} must return {declared.typeName()} but has bare return");
            return;
        }

        VarType actual = expr.type;
        if (actual == null) return;

        if (declared == null)
        {
            if (!(actual is VoidType))
                Utils.error($"Function {enclosing.name} is void but returns a value");
            return;
        }

        if (declared is ClassType dc && actual is ClassType ac)
        {
            if (dc.name != ac.name)
                Utils.error($"Function {enclosing.name} declared return type {dc.name} but returns {ac.name}");
            return;
        }

        if (declared.GetType() != actual.GetType())
            Utils.error($"Function {enclosing.name} declared return type {declared.typeName()} but returns {actual.typeName()}");
    }

    public override void genCode()
    {
        if (!isRealReturn)
            return;

        if (expr != null)
        {
            ASM.Asm.emit(new ASM.Comment("return <expr>"));
            base.genCode(); // generates expr's code

            if (expr.type is FloatType)
            {
                // Move float bits into rax for the test harness to read
                expr.temporary.moveToXmmRegister(ASM.Register.xmm0);
                ASM.Asm.emit(new ASM.OpMovqXmmReg(ASM.Register.xmm0, ASM.Register.rax));
            }
            else
            {
                expr.temporary.moveToRegister(ASM.Register.rax);
            }
        }

        // Epilogue
        ASM.Asm.emit(new ASM.Comment("Epilogue"));
        ASM.Asm.emit(new ASM.OpMovRegReg(ASM.Register.rbp, ASM.Register.rsp));
        ASM.Asm.emit(new ASM.OpPopReg(ASM.Register.rbp));
        ASM.Asm.emit(new ASM.Ret());
    }
}