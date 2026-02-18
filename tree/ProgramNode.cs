//ProgramNode.cs

using System.Collections.Generic;

public class ProgramNode : TreeNode
{
    public List<FuncdefNode> functions;

    public ProgramNode(List<FuncdefNode> functions)
    {
        this.functions = functions;
    }

public static ProgramNode parse(Tokenizer T)
{
    T.expect("FUNC");
    Token name = T.expect("ID");
    T.expect("LPAREN");
    T.expect("RPAREN");
    T.expect("LBRACE");
    T.expect("RETURN");

    ExprNode e = ExprNode.parse(T);

    // STRICT: expression must end here
    Token next = T.next();
    if (next.sym != "RBRACE")
        throw new Exception("invalid syntax");

    ReturnNode r = new ReturnNode(e);
    StmtsNode s = new StmtsNode(new List<StmtNode> { r });
    FuncdefNode f = new FuncdefNode(name.lexeme, s);

    return new ProgramNode(new List<FuncdefNode> { f });
}

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode>(functions);
    }
}