using System.Collections.Generic;

public class CondNode : StmtNode
{
    public ExprNode condition;
    public StmtsNode thenBranch;
    public StmtNode elseBranch;

    public CondNode(ExprNode condition, StmtsNode thenBranch, StmtNode elseBranch = null)
    {
        this.condition = condition;
        this.thenBranch = thenBranch;
        this.elseBranch = elseBranch;
    }

    public override List<TreeNode> getChildNodes()
    {
        var list = new List<TreeNode> { condition, thenBranch };
        if (elseBranch != null) list.Add(elseBranch);
        return list;
    }

    public override void typeCheck()
    {
        if (condition.type != null && !(condition.type is BoolType))
            Utils.error($"Type error: 'if' condition must be bool, got {condition.type.typeName()}");
    }

    public override void setupCFG()
    {
        entry.addNext(condition);
        condition.exit.addNext(thenBranch);
        thenBranch.exit.addNext(exit);

        if (elseBranch != null)
        {
            condition.exit.addNext(elseBranch);
            elseBranch.exit.addNext(exit);
        }
        else
        {
            condition.exit.addNext(exit);
        }
    }

    public override void genCode()
    {
        if (elseBranch == null)
        {
            var endLabel = new ASM.Label();

            condition.genCode();
            condition.temporary.moveToRegister(ASM.Register.rax);
            ASM.Asm.emit(new ASM.RawOp("    testq %rax, %rax"));
            ASM.Asm.emit(new ASM.RawOp($"    je {endLabel.lbl}"));

            thenBranch.genCode();

            ASM.Asm.emit(endLabel);
        }
        else
        {
            var elseLabel = new ASM.Label();
            var endLabel  = new ASM.Label();

            condition.genCode();
            condition.temporary.moveToRegister(ASM.Register.rax);
            ASM.Asm.emit(new ASM.RawOp("    testq %rax, %rax"));
            ASM.Asm.emit(new ASM.RawOp($"    je {elseLabel.lbl}"));

            thenBranch.genCode();
            ASM.Asm.emit(new ASM.RawOp($"    jmp {endLabel.lbl}"));

            ASM.Asm.emit(elseLabel);
            elseBranch.genCode();

            ASM.Asm.emit(endLabel);
        }
    }
}