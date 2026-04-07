using System.Collections.Generic;

public class LoopNode : StmtNode
{
    public ExprNode condition;
    public StmtsNode body;

    public ASM.Label testLabel = new ASM.Label();
    public ASM.Label exitLabel = new ASM.Label();

    public CFGNode testNode;

    public LoopNode(ExprNode condition, StmtsNode body)
    {
        this.condition = condition;
        this.body = body;
        this.testNode = new CFGNode("while-test", this);
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { condition, body };
    }

    public override void typeCheck()
    {
        if (condition.type != null && !(condition.type is BoolType))
            Utils.error($"Type error: 'while' condition must be bool, got {condition.type.typeName()}");
    }

    public override void setupCFG()
    {
        entry.addNext(testNode);
        testNode.addNext(condition);
        condition.exit.addNext(body);
        condition.exit.addNext(exit);
        body.exit.addNext(testNode);
    }

    public override void genCode()
    {
        ASM.Asm.emit(testLabel);

        condition.genCode();
        condition.temporary.moveToRegister(ASM.Register.rax);
        ASM.Asm.emit(new ASM.RawOp("    testq %rax, %rax"));
        ASM.Asm.emit(new ASM.RawOp($"    je {exitLabel.lbl}"));

        body.genCode();

        ASM.Asm.emit(new ASM.RawOp($"    jmp {testLabel.lbl}"));
        ASM.Asm.emit(exitLabel);
    }
}