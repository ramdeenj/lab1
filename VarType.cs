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
                Utils.error($"Expected variable type, but got {t}");
                throw new System.Exception();
        }
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

    public FuncType(VarType returnType, List<(string, VarType)> parameters)
    {
        this.returnType = returnType;
        this.parameters = parameters;
    }
}