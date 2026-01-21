//VarType.cs
public class VarType{
    public static VarType fromToken(Token t){
        switch(t.lexeme){
            case "int": return new IntType();
            case "float": return new FloatType();
            case "string": return new StringType();
            default:
                Utils.error($"Expected variable type, but got {t}");
                throw new Exception();  //dummy
        }
    }
}

public class IntType: VarType{
}
public class FloatType: VarType{
}
public class StringType: VarType{
}
public class VoidType: VarType{
}