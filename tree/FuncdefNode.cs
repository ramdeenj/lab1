using System.Collections.Generic;

public class FuncdefNode : TreeNode
{
    public string name;
    public StmtsNode body;

    public FuncdefNode(string name, StmtsNode body)
    {
        this.name = name;
        this.body = body;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { body };
    }
}
