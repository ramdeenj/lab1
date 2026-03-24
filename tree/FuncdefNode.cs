using System.Collections.Generic;

public class FuncdefNode : TreeNode
{
    public string name;
    public Token nameToken;
    public List<(Token idToken, VarType vtype)> parameters;
    public StmtsNode body;
    public VarType returnType;

    public FuncdefNode(string name, Token nameToken, List<(Token, VarType)> parameters, StmtsNode body, VarType returnType)
    {
        this.name = name;
        this.nameToken = nameToken;
        this.parameters = parameters;
        this.body = body;
        this.returnType = returnType;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { body };
    }

    public override void genCode()
    {
        ASM.Asm.emit(new ASM.Label(name));
        base.genCode();
    }
}