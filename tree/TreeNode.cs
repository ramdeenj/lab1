using System.Collections.Generic;

public abstract class TreeNode
{
    public abstract List<TreeNode> getChildNodes();

    public virtual void typeCheck() { }
}