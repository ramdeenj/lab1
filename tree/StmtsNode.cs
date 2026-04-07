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

    public override void setupCFG()
    {
        if (statements.Count == 0)
        {
            entry.addNext(exit);
            return;
        }

        entry.addNext(statements[0]);
        for (int i = 0; i < statements.Count - 1; i++)
            statements[i].exit.addNext(statements[i + 1]);
        statements[statements.Count - 1].exit.addNext(exit);
    }
}