using System;
using System.Collections.Generic;

public class ProgramNode : TreeNode
{
    public List<TreeNode> topLevel;

    public ProgramNode(List<TreeNode> topLevel)
    {
        this.topLevel = topLevel;
    }

    public static ProgramNode parse(Tokenizer T)
    {
        var topLevel = new List<TreeNode>();

        while (T.peek() != "")
        {
            if (T.peek() == "func")
                topLevel.Add(parseFuncdef(T));
            else if (T.peek() == "var")
                topLevel.Add(VarDeclNode.parse(T, new GlobalLocation()));
            else
                Utils.error($"Unexpected token at top level: {T.peek()}");
        }

        return new ProgramNode(topLevel);
    }

    static FuncdefNode parseFuncdef(Tokenizer T)
    {
        T.expect("FUNC");
        Token nameToken = T.expect("ID");
        string funcName = nameToken.lexeme;

        T.expect("LPAREN");
        var parameters = new List<(Token idToken, VarType vtype)>();

        SymbolTable.addScope();

        if (T.peek() != ")")
        {
            parseParam(T, parameters, funcName);
            while (T.peek() == ",")
            {
                T.next();
                if (T.peek() == "," || T.peek() == ")")
                    Utils.error($"Invalid parameter list in function {funcName}");
                parseParam(T, parameters, funcName);
            }
        }

        T.expect("RPAREN");

        VarType returnType = null;
        if (T.peek() == ":")
        {
            T.next();
            Token typeToken = T.next();
            if (typeToken.lexeme == "int" || typeToken.lexeme == "float" ||
                typeToken.lexeme == "string" || typeToken.lexeme == "bool")
            {
                returnType = VarType.fromToken(typeToken);
            }
            else
            {
                Utils.error($"Expected return type after ':', got '{typeToken.lexeme}' in function {funcName}");
            }
        }

        var paramList = new List<(string, VarType)>();
        foreach (var (idTok, vtype) in parameters)
            paramList.Add((idTok.lexeme, vtype));
        SymbolTable.declareInGlobal(nameToken, new FuncType(returnType, paramList));

        T.expect("LBRACE");
        StmtsNode body = parseStmts(T);
        T.expect("RBRACE");

        SymbolTable.removeScope();

        return new FuncdefNode(funcName, nameToken, parameters, body, returnType);
    }

    static void parseParam(Tokenizer T, List<(Token, VarType)> parameters, string funcName)
    {
        Token id = T.expect("ID");
        T.expect("COLON");
        Token typeToken = T.next();
        VarType vtype = VarType.fromToken(typeToken);
        parameters.Add((id, vtype));
        SymbolTable.declare(id, vtype, new ParameterLocation());
    }

    static StmtsNode parseStmts(Tokenizer T)
    {
        var stmts = new List<StmtNode>();
        while (T.peek() != "}" && T.peek() != "")
            stmts.Add(parseStmt(T));
        return new StmtsNode(stmts);
    }

    static StmtNode parseStmt(Tokenizer T)
    {
        if (T.peek() == "return")
        {
            T.next();
            if (T.peek() == "}" || T.peek() == "")
                return new ReturnNode(null, isRealReturn: true);
            return new ReturnNode(ExprNode.parse(T), isRealReturn: true);
        }
        else if (T.peek() == "if")
        {
            T.next();
            ExprNode cond = ExprNode.parse(T);
            T.expect("LBRACE");
            SymbolTable.addScope();
            StmtsNode body = parseStmts(T);
            T.expect("RBRACE");
            SymbolTable.removeScope();
            return new CondNode(cond, body);
        }
        else if (T.peek() == "while")
        {
            T.next();
            ExprNode cond = ExprNode.parse(T);
            T.expect("LBRACE");
            SymbolTable.addScope();
            StmtsNode body = parseStmts(T);
            T.expect("RBRACE");
            SymbolTable.removeScope();
            return new LoopNode(cond, body);
        }
        else if (T.peek() == "var")
        {
            return VarDeclNode.parse(T, new LocalLocation());
        }
        else
        {
            ExprNode e = ExprNode.parse(T);
            return new ReturnNode(e, isRealReturn: false);
        }
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode>(topLevel);
    }
}