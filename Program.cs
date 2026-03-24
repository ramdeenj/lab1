using System;
using System.IO;
using System.Collections.Generic;

public class Program
{
    static void walkPostorder(TreeNode n, Action<TreeNode> callback)
    {
        foreach (TreeNode c in n.getChildNodes())
            walkPostorder(c, callback);
        callback(n);
    }

    static void walkPreorder(TreeNode n, Action<TreeNode> callback)
    {
        callback(n);
        foreach (TreeNode c in n.getChildNodes())
            walkPreorder(c, callback);
    }

    static void setParents(TreeNode n)
    {
        foreach (TreeNode c in n.getChildNodes())
        {
            c.parent = n;
            setParents(c);
        }
    }

    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Missing input file");
            Environment.Exit(1);
        }

        var T = new Tokenizer();
        using (var r = new StreamReader(args[0]))
        {
            T.setInput(r.ReadToEnd());
        }

        ProgramNode p = null;
        try
        {
            p = ProgramNode.parse(T);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            Environment.Exit(1);
            return;
        }

        setParents(p);

        try
        {
            walkPreorder(p, (TreeNode n) =>
            {
                if (n is VarNode vn && vn.info == null)
                {
                    var info = SymbolTable.lookupInGlobal(vn.token.lexeme);
                    if (info == null)
                        Utils.error($"Variable {vn.token.lexeme} on line {vn.token.line} is not declared");
                    vn.info = info;
                }
            });
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            Environment.Exit(1);
            return;
        }

        walkPostorder(p, (TreeNode n) =>
        {
            (n as ExprNode)?.setType();
        });

        try
        {
            walkPreorder(p, (TreeNode n) =>
            {
                n.typeCheck();
            });
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            Environment.Exit(1);
            return;
        }

        Environment.Exit(0);
    }
}