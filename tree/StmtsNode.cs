//StmtsNode.cs
public class StmtsNode : TreeNode
{
    public List<StmtNode> stmts = new();
    public static StmtsNode parse(Tokenizer T)
    {
        T.expect("LBRACE");

        var s = new StmtsNode();

        while(true){
            if( T.peek() == "RBRACE" )
                break;
            s.stmts.Add(StmtNode.parse(T));
        }

        T.expect("RBRACE");

        return s;
    }
}