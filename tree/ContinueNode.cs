using System.Collections.Generic;

public class ContinueNode : StmtNode
{
    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode>();
    }

    public override void typeCheck()
    {
        TreeNode cur = this.parent;
        while (cur != null)
        {
            if (cur is LoopNode || cur is RepeatNode) return;
            if (cur is FuncdefNode) break;
            cur = cur.parent;
        }
        Utils.error("'continue' used outside of a loop");
    }

    public override void genCode()
    {
        TreeNode cur = this.parent;
        while (cur != null)
        {
            if (cur is LoopNode ln)
            {
                ASM.Asm.emit(new ASM.RawOp($"    jmp {ln.testLabel.lbl}"));
                return;
            }
            if (cur is RepeatNode rn)
            {
                ASM.Asm.emit(new ASM.RawOp($"    jmp {rn.testLabel.lbl}"));
                return;
            }
            cur = cur.parent;
        }
    }
}