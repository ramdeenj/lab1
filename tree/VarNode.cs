public class VarNode : ExprNode
{
    public VarInfo? info;

    public VarNode(Token tok) : base(tok)
    {
        // Try to look up now; null means it's a hoisted global, fixed in Phase 2
        info = SymbolTable.lookupIfExists(tok.lexeme);
    }

    public override void setType()
    {
        if (info != null)
            type = info.type;
    }
}