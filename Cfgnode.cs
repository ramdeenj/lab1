using System.Collections.Generic;

public class CFGNode
{
    public List<CFGNode> prev = new();
    public List<CFGNode> next = new();
    public TreeNode owner;
    public string name;

    public CFGNode(string name, TreeNode owner, params CFGNode[] next_)
    {
        this.name = name;
        this.owner = owner;
        foreach (var n in next_)
            addNext(n);
    }

    public void addNext(CFGNode n)
    {
        this.next.Add(n);
        n.prev.Add(this);
    }

    public void addNext(TreeNode n)
    {
        this.next.Add(n.entry);
        n.entry.prev.Add(this);
    }
}