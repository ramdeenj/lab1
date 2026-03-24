using System.Collections.Generic;

public class ClassDeclNode : TreeNode
{
    public Token nameToken;
    public ClassType classType;
    public List<VarDeclNode> fields = new List<VarDeclNode>();
    public List<FuncdefNode> methods = new List<FuncdefNode>();

    public ClassDeclNode(Token nameToken, ClassType classType)
    {
        this.nameToken = nameToken;
        this.classType = classType;
        this.classType.declarer = this;
    }

    public override List<TreeNode> getChildNodes()
    {
        var children = new List<TreeNode>();
        children.AddRange(fields);
        children.AddRange(methods);
        return children;
    }

    public VarType getMemberType(string name)
    {
        foreach (var f in fields)
            if (f.idToken.lexeme == name)
                return f.varType;
        foreach (var m in methods)
            if (m.name == name)
            {
                var paramList = new List<(string, VarType)>();
                foreach (var (tok, vtype) in m.parameters)
                    paramList.Add((tok.lexeme, vtype));
                return new FuncType(m.returnType, paramList);
            }
        return null;
    }
}