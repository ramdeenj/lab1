//Program.cs

using System;
using System.IO;
using System.Collections.Generic;

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

    //JSON WRITER
    static void WriteJson(ExprNode node, TextWriter w)
    {
        w.Write("{");
        w.Write("\"token\":\"" + node.token.lexeme + "\"");
        w.Write(",\"children\":[");

        List<TreeNode> children = node.getChildNodes();

        for (int i = 0; i < children.Count; i++)
        {
            WriteJson((ExprNode)children[i], w);
            if (i < children.Count - 1)
                w.Write(",");
        }

        w.Write("]}");
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
            Console.Error.WriteLine(e.Message);
            Environment.Exit(1);
            return;
        }

        if (p == null)
            return;

        // Walk tree and stop at first expression
        walk(p, (TreeNode n) =>
        {
            ExprNode e = n as ExprNode;
            if (e == null)
                return;

            //WRITE DOT FILE
            using (var w = new StreamWriter("tree.dot"))
            {
                w.WriteLine("graph foo {");

                walk(e, (TreeNode c) =>
                {
                    ExprNode ee = (ExprNode)c;
                    w.WriteLine($"{ee.unique} [label=\"{ee.token.lexeme}\"];");
                });

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

            //WRITE JSON FILE
            using (var w = new StreamWriter("tree.json"))
            {
                WriteJson(e, w);
            }

            // stop after first expression
            throw new StopIteration();
        });

        Environment.Exit(0);
    }
}
