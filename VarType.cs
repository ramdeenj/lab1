using System.Collections.Generic;

public class VarType
{
    public static VarType fromToken(Token t)
    {
        switch (t.lexeme)
        {
            case "int": return new IntType();
            case "float": return new FloatType();
            case "string": return new StringType();
            case "bool": return new BoolType();
            default:
                return ProgramNode.getClassType(t);
        }
    }

    public string typeName()
    {
        if (this is IntType) return "int";
        if (this is FloatType) return "float";
        if (this is StringType) return "string";
        if (this is BoolType) return "bool";
        if (this is FuncType) return "function";
        if (this is ClassType ct) return ct.name;
        if (this is VoidType) return "void";
        return "unknown";
    }
}

public class IntType : VarType { }
public class FloatType : VarType { }
public class StringType : VarType { }
public class BoolType : VarType { }
public class VoidType : VarType { }

public class FuncType : VarType
{
    public VarType returnType;
    public List<(string name, VarType type)> parameters;
    public bool isBuiltin;

    public FuncType(VarType returnType, List<(string, VarType)> parameters, bool isBuiltin = false)
    {
        this.returnType = returnType;
        this.parameters = parameters;
        this.isBuiltin = isBuiltin;
    }
}

public class ClassType : VarType
{
    public string name;
    public ClassDeclNode declarer;

    public ClassType(string name)
    {
        this.name = name;
        this.declarer = null;
    }
}