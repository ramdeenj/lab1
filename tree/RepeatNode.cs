using System.Collections.Generic;

public class RepeatNode : StmtNode
{
    public StmtsNode body;
    public ExprNode condition;

    public ASM.Label testLabel = new ASM.Label();
    public ASM.Label exitLabel = new ASM.Label();

    public RepeatNode(StmtsNode body, ExprNode condition)
    {
        this.body = body;
        this.condition = condition;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { body, condition };
    }

    public override void typeCheck()
    {
        if (condition.type != null && !(condition.type is BoolType))
            Utils.error($"Type error: 'repeat/until' condition must be bool, got {condition.type.typeName()}");
    }

    public override void genCode()
    {
        var topLabel = new ASM.Label();

        ASM.Asm.emit(topLabel);   // top of loop

        body.genCode();

        ASM.Asm.emit(testLabel);  // continue jumps here

        condition.genCode();
        condition.temporary.moveToRegister(ASM.Register.rax);
        ASM.Asm.emit(new ASM.RawOp("    testq %rax, %rax"));
        ASM.Asm.emit(new ASM.RawOp($"    je {topLabel.lbl}")); // false = repeat

        ASM.Asm.emit(exitLabel);  // break jumps here
    }
}