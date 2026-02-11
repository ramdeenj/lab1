//Program.cs

using System;
using System.IO;

public class StopIteration : Exception
{
}

public class Program
{
    static void walk(TreeNode n, Action<TreeNode> callback)
    {
        try
        {
            walkHelper(n, callback);
        }
        catch (StopIteration)
        {
        }
    }

    static void walkHelper(TreeNode n, Action<TreeNode> callback)
    {
        callback(n);
        foreach (TreeNode c in n.getChildNodes())
        {
            walkHelper(c, callback);
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

        ProgramNode? p = null;

        try
        {
            p = ProgramNode.parse(T);
        }
        catch (Exception e)
        {
            // show the REAL error (very important for debugging)
            Console.Error.WriteLine(e.Message);
            Environment.Exit(1);
            return;
        }

        if (p == null)
            return;

        Treedump.textTree(p, Console.Out);

        walk(p, (TreeNode n) =>
        {
            ExprNode e = n as ExprNode;
            if (e == null)
                return;

            using (var w = new StreamWriter("tree.dot"))
            {
                w.WriteLine("graph foo {");

                // write nodes
                walk(e, (TreeNode c) =>
                {
                    ExprNode ee = (ExprNode)c;
                    w.WriteLine($"{ee.unique} [label=\"{ee.token.lexeme}\"];");
                });

                // write edges
                walk(e, (TreeNode c) =>
                {
                    ExprNode ee = (ExprNode)c;
                    foreach (TreeNode x in ee.getChildNodes())
                    {
                        ExprNode xx = (ExprNode)x;
                        w.WriteLine($"{ee.unique} -- {xx.unique};");
                    }
                });

                w.WriteLine("}");
            }

            // stop after first ExprNode
            throw new StopIteration();
        });

        Environment.Exit(0);
    }
}
