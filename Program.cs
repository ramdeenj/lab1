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

        // Postorder walk: set types bottom-up
        walkPostorder(p, (TreeNode n) =>
        {
            (n as ExprNode)?.setType();
        });

        // Preorder walk: type check statements
        walkPreorder(p, (TreeNode n) =>
        {
            n.typeCheck();
        });

        Environment.Exit(0);
    }
}