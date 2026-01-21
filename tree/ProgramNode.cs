//ProgramNode.cs
public class ProgramNode : TreeNode
{
    public List<FuncdefNode> funcs = new();   
    public static ProgramNode parse(Tokenizer T)
    {
        ProgramNode p = new();
        while (true)
        {
            if( T.peek() == "FUNC" )
                p.funcs.Add(FuncdefNode.parse(T));
            else if( T.peek() == "$" )
                break;
            else
                Utils.error("Unexpected thing");
        }
        return p;
    }
}