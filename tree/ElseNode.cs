using System.Collections.Generic;

public class ElseNode : StmtNode
{
    public StmtsNode body;

    public ElseNode(StmtsNode body)
    {
        this.body = body;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { body };
    }

    public override void setupCFG()
    {
        entry.addNext(body);
        body.exit.addNext(exit);
    }

    public override void genCode()
    {
        body.genCode();
    }
}