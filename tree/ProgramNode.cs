using System;
using System.Collections.Generic;

public class ProgramNode : TreeNode
{
    public List<FuncdefNode> functions;

    public ProgramNode(List<FuncdefNode> functions)
    {
        this.functions = functions;
    }

    public static ProgramNode parse(Tokenizer T)
    {
        T.expect("FUNC");
        Token name = T.expect("ID");
        T.expect("LPAREN");
        T.expect("RPAREN");

        // Optional return type annotation: ": int" etc.
        if (T.peek() == ":")
        {
            T.next(); // consume ":"
            T.next(); // consume the type (int, float, etc.)
        }

        T.expect("LBRACE");
        StmtsNode body = parseStmts(T);
        T.expect("RBRACE");

        FuncdefNode f = new FuncdefNode(name.lexeme, body);
        return new ProgramNode(new List<FuncdefNode> { f });
    }

    static StmtsNode parseStmts(Tokenizer T)
    {
        var stmts = new List<StmtNode>();
        while (T.peek() != "}" && T.peek() != "")
        {
            stmts.Add(parseStmt(T));
        }
        return new StmtsNode(stmts);
    }

    static StmtNode parseStmt(Tokenizer T)
    {
        if (T.peek() == "return")
        {
            T.next();
            ExprNode e = ExprNode.parse(T);
            return new ReturnNode(e);
        }
        else if (T.peek() == "if")
        {
            T.next();
            ExprNode cond = ExprNode.parse(T);
            T.expect("LBRACE");
            StmtsNode body = parseStmts(T);
            T.expect("RBRACE");
            return new CondNode(cond, body);
        }
        else if (T.peek() == "while")
        {
            T.next();
            ExprNode cond = ExprNode.parse(T);
            T.expect("LBRACE");
            StmtsNode body = parseStmts(T);
            T.expect("RBRACE");
            return new LoopNode(cond, body);
        }
        else
        {
            // Standalone expression statement
            ExprNode e = ExprNode.parse(T);
            return new ReturnNode(e); // wrap as return-like for tree purposes
        }
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode>(functions);
    }
}
