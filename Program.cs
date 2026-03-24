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

        // Phase 2: Resolve hoisted globals (VarNodes with null info)
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

        // Phase 3: Validate all ClassTypes have been defined
        try
        {
            walkPreorder(p, (TreeNode n) =>
            {
                if (n is VarNode vn2 && vn2.info != null && vn2.info.type is ClassType ct && ct.declarer == null)
                    Utils.error($"Class {ct.name} is not defined");
                if (n is VarDeclNode vd && vd.varType is ClassType ct2 && ct2.declarer == null)
                    Utils.error($"Class {ct2.name} is not defined");
            });
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            Environment.Exit(1);
            return;
        }

        // Phase 4: Set types bottom-up
        walkPostorder(p, (TreeNode n) =>
        {
            (n as ExprNode)?.setType();
        });

        // Phase 5: Type checks
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

        // Output: print variable and member references
        walkPreorder(p, (TreeNode n) =>
        {
            if (n is VarNode vn && vn.info != null)
            {
                Console.WriteLine($"Variable {vn.token.lexeme} on line {vn.token.line} is of type {vn.info.type.typeName()}");
            }
            else if (n is MemberNode mn && mn.declaringClass != null)
            {
                Console.WriteLine($"Variable {mn.token.lexeme} on line {mn.token.line} is a member of class {mn.declaringClass.name} and is of type {mn.type.typeName()}");
            }
        });

        Environment.Exit(0);
    }
}