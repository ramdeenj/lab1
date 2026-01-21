// StmtNode.cs
public abstract class StmtNode : TreeNode
{
    public static StmtNode parse(Tokenizer T)
    {
        if (ReturnNode.canParse(T))
            return ReturnNode.parse(T);

        // CondNode / LoopNode not enabled yet

        Utils.error("Expecting statement but did not get one at line");
        throw new Exception();
    }
}
