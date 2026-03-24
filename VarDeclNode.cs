using System.Collections.Generic;

// AST node for:  var ID : TYPE
public class VarDeclNode : StmtNode
{
    public Token idToken;
    public VarType varType;
    public VarInfo? info;

    public VarDeclNode(Token idToken, VarType varType)
    {
        this.idToken = idToken;
        this.varType = varType;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode>();
    }

    public static bool canParse(Tokenizer T)
    {
        return T.peek() == "var";
    }

    public static VarDeclNode parse(Tokenizer T, VarLocation location)
    {
        T.expect("VAR");
        Token id = T.expect("ID");
        T.expect("COLON");
        Token typeToken = T.next();
        VarType vtype = VarType.fromToken(typeToken);

        var node = new VarDeclNode(id, vtype);

        SymbolTable.declare(id, vtype, location);
        node.info = SymbolTable.lookupIfExists(id.lexeme);

        return node;
    }
}