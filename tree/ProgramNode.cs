using System;
using System.Collections.Generic;

public class ProgramNode : TreeNode
{
    public List<TreeNode> topLevel;

    private static Dictionary<string, ClassType> classRegistry = new Dictionary<string, ClassType>();

    public static ClassType getClassType(Token t)
    {
        string name = t.lexeme;
        if (!classRegistry.ContainsKey(name))
            classRegistry[name] = new ClassType(name);
        return classRegistry[name];
    }

    public static void resetClassRegistry()
    {
        classRegistry.Clear();
    }

    public ProgramNode(List<TreeNode> topLevel)
    {
        this.topLevel = topLevel;
    }

    public static ProgramNode parse(Tokenizer T)
    {
        resetClassRegistry();
        var topLevel = new List<TreeNode>();

        while (T.peek() != "")
        {
            if (T.peek() == "func")
                topLevel.Add(parseFuncdef(T, null));
            else if (T.peek() == "var")
                topLevel.Add(VarDeclNode.parse(T, new GlobalLocation()));
            else if (T.peek() == "class")
                topLevel.Add(parseClassDecl(T));
            else
                Utils.error($"Unexpected token at top level: {T.peek()}");
        }

        return new ProgramNode(topLevel);
    }

    static ClassDeclNode parseClassDecl(Tokenizer T)
    {
        T.expect("CLASS");
        Token nameToken = T.expect("ID");

        ClassType ct = getClassType(nameToken);
        if (ct.declarer != null)
            Utils.error($"Class {nameToken.lexeme} already declared");

        var node = new ClassDeclNode(nameToken, ct);

        T.expect("LBRACE");

        var memberNames = new HashSet<string>();

        while (T.peek() != "}" && T.peek() != "")
        {
            if (T.peek() == "var")
            {
                var field = VarDeclNode.parseClassMember(T);
                if (!memberNames.Add(field.idToken.lexeme))
                    Utils.error($"Duplicate member '{field.idToken.lexeme}' in class {nameToken.lexeme}");
                node.fields.Add(field);
            }
            else if (T.peek() == "func")
            {
                var method = parseFuncdef(T, ct);
                if (!memberNames.Add(method.name))
                    Utils.error($"Duplicate member '{method.name}' in class {nameToken.lexeme}");
                node.methods.Add(method);
            }
            else
            {
                Utils.error($"Unexpected token in class body: {T.peek()}");
            }
        }

        T.expect("RBRACE");
        return node;
    }

    static FuncdefNode parseFuncdef(Tokenizer T, ClassType classContext)
    {
        T.expect("FUNC");
        Token nameToken = T.expect("ID");
        string funcName = nameToken.lexeme;

        T.expect("LPAREN");
        var parameters = new List<(Token idToken, VarType vtype)>();

        SymbolTable.addScope();

        if (classContext != null)
        {
            Token thisToken = new Token("THIS", nameToken.line, "this");
            SymbolTable.declare(thisToken, classContext, new ParameterLocation());
        }

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
            returnType = VarType.fromToken(typeToken);
        }

        if (classContext == null)
        {
            var paramList = new List<(string, VarType)>();
            foreach (var (idTok, vtype) in parameters)
                paramList.Add((idTok.lexeme, vtype));
            SymbolTable.declareInGlobal(nameToken, new FuncType(returnType, paramList));
        }

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