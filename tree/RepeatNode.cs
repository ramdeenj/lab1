using System.Collections.Generic;

public class RepeatNode : StmtNode
{
    public StmtsNode body;
    public ExprNode condition;

    public ASM.Label testLabel = new ASM.Label();
    public ASM.Label exitLabel = new ASM.Label();

    public CFGNode testNode;

    public RepeatNode(StmtsNode body, ExprNode condition)
    {
        this.body = body;
        this.condition = condition;
        this.testNode = new CFGNode("repeat-test", this);
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

    public override void setupCFG()
    {
        entry.addNext(body);
        body.exit.addNext(testNode);
        testNode.addNext(condition);
        condition.exit.addNext(body);
        condition.exit.addNext(exit);
    }

    public override void genCode()
    {
        var topLabel = new ASM.Label();

        ASM.Asm.emit(topLabel);

        body.genCode();

        ASM.Asm.emit(testLabel);

        condition.genCode();
        condition.temporary.moveToRegister(ASM.Register.rax);
        ASM.Asm.emit(new ASM.RawOp("    testq %rax, %rax"));
        ASM.Asm.emit(new ASM.RawOp($"    je {topLabel.lbl}"));

        ASM.Asm.emit(exitLabel);
    }
}