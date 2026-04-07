using System.Collections.Generic;

public abstract class TreeNode
{
    public TreeNode parent = null;

    public CFGNode entry;
    public CFGNode exit;

    protected TreeNode()
    {
        this.entry = new CFGNode("entry", this);
        this.exit = new CFGNode("exit", this);
    }

    public abstract List<TreeNode> getChildNodes();

    public virtual void typeCheck() { }

    public virtual void setupCFG()
    {
        entry.addNext(exit);
    }

    public virtual void genCode()
    {
        foreach (var n in getChildNodes())
            n.genCode();
    }
}