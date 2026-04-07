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

    public override void setupCFG()
    {
        if (!isRealReturn)
        {
            if (expr != null)
            {
                entry.addNext(expr);
                expr.exit.addNext(exit);
            }
            else
            {
                entry.addNext(exit);
            }
            return;
        }

        FuncdefNode enclosing = null;
        TreeNode cur = this.parent;
        while (cur != null)
        {
            if (cur is FuncdefNode fd) { enclosing = fd; break; }
            cur = cur.parent;
        }

        if (expr != null)
        {
            var realExit = new CFGNode("return-exit", this);
            entry.addNext(expr);
            expr.exit.addNext(realExit);
            if (enclosing != null)
                realExit.addNext(enclosing.exit);
        }
        else
        {
            if (enclosing != null)
                entry.addNext(enclosing.exit);
        }
    }

    public override void genCode()
    {
        if (!isRealReturn)
        {
            if (expr != null)
                base.genCode();
            return;
        }

        if (expr != null)
        {
            base.genCode();

            if (expr.type is FloatType)
            {
                expr.temporary.moveToXmmRegister(ASM.Register.xmm0);
                ASM.Asm.emit(new ASM.OpMovqXmmReg(ASM.Register.xmm0, ASM.Register.rax));
            }
            else
            {
                expr.temporary.moveToRegister(ASM.Register.rax);
            }
            ASM.Asm.emit(new ASM.OpMovConstReg((long)ASM.StorageClass.STATIC, ASM.Register.rbx));
        }

        ASM.Asm.emit(new ASM.OpMovRegReg(ASM.Register.rbp, ASM.Register.rsp));
        ASM.Asm.emit(new ASM.OpPopReg(ASM.Register.rbp));
        ASM.Asm.emit(new ASM.Ret());
    }
}