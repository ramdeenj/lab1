//FuncdefNode.cs
public class FuncdefNode: TreeNode{

    public VarType returnType;
    public Token name;
    public StmtsNode stmts;

    private FuncdefNode(VarType returnType, Token name, StmtsNode stmts)
    {
        this.returnType=returnType;
        this.name=name;
        this.stmts=stmts;
    }

    public static FuncdefNode parse(Tokenizer T)
    {
        T.expect("FUNC");
        var name = T.expect("ID");
        T.expect("LPAREN");
        T.expect("RPAREN");
        VarType returnType;
        if( T.peek() == "COLON")
        {
            T.expect("COLON");
            returnType = VarType.fromToken(T.next());
        } else
        {
            returnType = new VoidType();
        }
        StmtsNode stmts = StmtsNode.parse(T);
        return new FuncdefNode(returnType, name, stmts );
    }

}