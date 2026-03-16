using System.Collections.Generic;

public class StmtsNode : TreeNode
{
    public List<StmtNode> statements;

    public StmtsNode(List<StmtNode> statements)
    {
        this.statements = statements;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode>(statements);
    }
}
