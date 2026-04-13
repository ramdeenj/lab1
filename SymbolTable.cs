using System.Collections.Generic;

public abstract class VarLocation { }
public class GlobalLocation : VarLocation
{
    public string label;
    public GlobalLocation(string label = "") { this.label = label; }
}
public class LocalLocation : VarLocation
{
    public int offset; // rbp-relative byte offset (negative)
    public LocalLocation(int offset) { this.offset = offset; }
}
public class ParameterLocation : VarLocation
{
    public int offset;
    public ParameterLocation(int offset = 0) { this.offset = offset; }
}

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

    private static int nextLocalSlot = 0;

    public static int allocLocal()
    {
        return nextLocalSlot++;
    }

    public static int getLocalSlotCount() => nextLocalSlot;

    public static void resetLocals()
    {
        nextLocalSlot = 0;
    }

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

    // Track all global variables for .bss section emission
    private static List<(string name, VarType type)> globalVars = new();

    public static IReadOnlyList<(string name, VarType type)> GlobalVars => globalVars;

    public static void declareInGlobal(Token id, VarType type)
    {
        SymbolTable? scope = current;
        while (scope!.prev != null)
            scope = scope.prev;
        string label = "g_" + id.lexeme;
        scope.decls[id.lexeme] = new VarInfo(id, type, new GlobalLocation(label));
        if (!(type is FuncType))
            globalVars.Add((label, type));
    }

    public static void resetGlobals()
    {
        globalVars.Clear();
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

    private static void declareBuiltin(string name, VarType rtype, List<VarType> argTypes)
    {
        var t = new Token("ID", -1, name);
        var parameters = new List<(string, VarType)>();
        for (int i = 0; i < argTypes.Count; i++)
            parameters.Add(($"arg{i}", argTypes[i]));
        var F = new FuncType(rtype, parameters, isBuiltin: true);
        declare(t, F, new GlobalLocation("_" + name));
    }

    public static void populateBuiltins()
    {
        declareBuiltin("putc",    new BoolType(), new List<VarType> { new IntType() });
        declareBuiltin("newline", new VoidType(), new List<VarType>());
        declareBuiltin("putv",    new BoolType(), new List<VarType> { new IntType(), new IntType() });
        declareBuiltin("getc",    new IntType(),  new List<VarType>());
    }

    public static void reset()
    {
        current = new SymbolTable(null);
        nextLocalSlot = 0;
        globalVars.Clear();
        populateBuiltins();
    }
}