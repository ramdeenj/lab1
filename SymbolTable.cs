using System.Collections.Generic;

public abstract class VarLocation { }
public class GlobalLocation : VarLocation { }
public class LocalLocation : VarLocation { }
public class ParameterLocation : VarLocation { }

public class VarInfo
{
    public Token token;
    public VarType type;
    public VarLocation location;

    public VarInfo(Token token, VarType type, VarLocation location)
    {
        this.token = token;
        this.type = type;
        this.location = location;
    }
}

public class SymbolTable
{
    private static SymbolTable current = new SymbolTable(null);
    private SymbolTable? prev;
    private Dictionary<string, VarInfo> decls = new();

    private SymbolTable(SymbolTable? prev)
    {
        this.prev = prev;
    }

    public static void addScope()
    {
        current = new SymbolTable(current);
    }

    public static void removeScope()
    {
        current = current.prev!;
    }

    public static void declare(Token id, VarType type, VarLocation location)
    {
        if (current.decls.ContainsKey(id.lexeme))
            Utils.error($"Variable {id.lexeme} redeclared on line {id.line}");
        current.decls[id.lexeme] = new VarInfo(id, type, location);
    }

    public static VarInfo lookup(string name, int line)
    {
        var result = lookupIfExists(name);
        if (result == null)
            Utils.error($"Variable {name} used on line {line} but never declared");
        return result!;
    }

    public static VarInfo? lookupIfExists(string name)
    {
        SymbolTable? scope = current;
        while (scope != null)
        {
            if (scope.decls.TryGetValue(name, out var info))
                return info;
            scope = scope.prev;
        }
        return null;
    }

    public static void declareInGlobal(Token id, VarType type)
    {
        SymbolTable? scope = current;
        while (scope!.prev != null)
            scope = scope.prev;
        scope.decls[id.lexeme] = new VarInfo(id, type, new GlobalLocation());
    }

    public static VarInfo? lookupInGlobal(string name)
    {
        SymbolTable? scope = current;
        while (scope!.prev != null)
            scope = scope.prev;
        if (scope.decls.TryGetValue(name, out var info))
            return info;
        return null;
    }

    public static void reset()
    {
        current = new SymbolTable(null);
    }
}