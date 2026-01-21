//Program.cs
using System.Text.Encodings.Web;
using System.Text.Unicode;

public class Program {

    public static void Main(string[] args)
    {
        var T = new Tokenizer();
        using( var r = new StreamReader(args[0]))
        {
            T.setInput(r.ReadToEnd());
        }

        var p = ProgramNode.parse(T);

        Treedump.textTree(p, Console.Out);
        // List<Token> tokens = new();
        // while (true)
        // {
        //     Token tok = T.next();
        //     if( tok.sym == "$")
        //         break;
        //     tokens.Add(tok);
        // }

        // var opts = new System.Text.Json.JsonSerializerOptions();
        // opts.IncludeFields=true;
        // opts.WriteIndented=true;
        // opts.MaxDepth=1000000;
        // //next line is optional; minimizes output escapes
        // opts.Encoder=JavaScriptEncoder.Create(UnicodeRanges.All);
        // string J = System.Text.Json.JsonSerializer.Serialize(tokens,opts);
        // Console.WriteLine(J);

        Environment.Exit(0);
    }

}
