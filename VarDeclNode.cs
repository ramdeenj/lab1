using System.Collections.Generic;

public class VarDeclNode : StmtNode
{
    public Token idToken;
    public VarType varType;
    public VarInfo info;

    public VarDeclNode(Token idToken, VarType varType)
    {
        this.idToken = idToken;
        this.varType = varType;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode>();
    }

    public override void genCode()
    {
        // Only initialize local variables (not globals or class members)
        if (info == null || !(info.location is LocalLocation ll))
            return;

        // Walk up to find enclosing FuncdefNode to get maxTemporaries
        int maxTemp = 0;
        TreeNode cur = this.parent;
        while (cur != null)
        {
            if (cur is FuncdefNode fn) { maxTemp = fn.maxTemporaries; break; }
            cur = cur.parent;
        }

        int off = -(maxTemp * 16 + ll.offset * 8 + 8);

        if (varType is StringType)
            ASM.Asm.emit(new ASM.RawOp("    leaq emptyString(%rip), %rax"));
        else
            ASM.Asm.emit(new ASM.RawOp("    movq $0, %rax"));

        ASM.Asm.emit(new ASM.OpMovRegRegInd(ASM.Register.rax, off, ASM.Register.rbp));
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

        if (location is GlobalLocation)
        {
            // Declare in global scope with a proper label for .bss
            SymbolTable.declareInGlobal(id, vtype);
        }
        else if (location is LocalLocation)
        {
            location = new LocalLocation(SymbolTable.allocLocal());
            SymbolTable.declare(id, vtype, location);
        }
        else
        {
            SymbolTable.declare(id, vtype, location);
        }

        var node = new VarDeclNode(id, vtype);
        node.info = SymbolTable.lookupIfExists(id.lexeme);
        return node;
    }

    // Used inside class body: parse but do NOT add to symbol table
    public static VarDeclNode parseClassMember(Tokenizer T)
    {
        T.expect("VAR");
        Token id = T.expect("ID");
        T.expect("COLON");
        Token typeToken = T.next();
        VarType vtype = VarType.fromToken(typeToken);

        var node = new VarDeclNode(id, vtype);
        node.info = new VarInfo(id, vtype, new GlobalLocation());
        return node;
    }
}