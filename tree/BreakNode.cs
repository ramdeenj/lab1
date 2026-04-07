using System.Collections.Generic;

public class BreakNode : StmtNode
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
        Utils.error("'break' used outside of a loop");
    }

    public override void setupCFG()
    {
        TreeNode cur = this.parent;
        while (cur != null)
        {
            if (cur is LoopNode ln) { entry.addNext(ln.exit); return; }
            if (cur is RepeatNode rn) { entry.addNext(rn.exit); return; }
            cur = cur.parent;
        }
    }

    public override void genCode()
    {
        TreeNode cur = this.parent;
        while (cur != null)
        {
            if (cur is LoopNode ln)
            {
                ASM.Asm.emit(new ASM.RawOp($"    jmp {ln.exitLabel.lbl}"));
                return;
            }
            if (cur is RepeatNode rn)
            {
                ASM.Asm.emit(new ASM.RawOp($"    jmp {rn.exitLabel.lbl}"));
                return;
            }
            cur = cur.parent;
        }
    }
}