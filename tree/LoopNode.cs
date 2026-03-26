using System.Collections.Generic;

public class LoopNode : StmtNode
{
    public ExprNode condition;
    public StmtsNode body;

    public LoopNode(ExprNode condition, StmtsNode body)
    {
        this.condition = condition;
        this.body = body;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { condition, body };
    }

    public override void typeCheck()
    {
        if (condition.type != null && !(condition.type is BoolType))
            Utils.error($"Type error: 'while' condition must be bool, got {condition.type.GetType().Name}");
    }

    public override void genCode()
    {
        var topLabel = new ASM.Label();
        var endLabel = new ASM.Label();

        ASM.Asm.emit(topLabel);

        condition.genCode();
        condition.temporary.moveToRegister(ASM.Register.rax);

        ASM.Asm.emit(new ASM.RawOp("    testq %rax, %rax"));
        ASM.Asm.emit(new ASM.RawOp($"    je {endLabel.lbl}"));

        body.genCode();

        ASM.Asm.emit(new ASM.RawOp($"    jmp {topLabel.lbl}"));

        ASM.Asm.emit(endLabel);
    }
}