using System.Collections.Generic;

public abstract class TreeNode
{
    public TreeNode parent = null;

    public abstract List<TreeNode> getChildNodes();

    public virtual void typeCheck() { }

    public virtual void genCode()
    {
        foreach (var n in getChildNodes())
            n.genCode();
    }
}