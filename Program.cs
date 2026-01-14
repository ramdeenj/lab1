//program.cs

using System.Text.Encodings.Web;
using System.Text.Unicode;

public class Program
{
    public static void Main(string[] args)
    {
        // Argument validation

        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: tokenizer <filename>");
            Environment.Exit(1);
        }

        string input;
        try
        {
            input = File.ReadAllText(args[0]);
        }
        catch
        {
            // File could not be opened
            Environment.Exit(1);
            return;
        }

        // Initialize terminals
        Terminals.init();

        // Tokenize
        var T = new Tokenizer();
        T.setInput(input);

        List<Token> tokens = new();

        try
        {
            while (true)
            {
                Token tok = T.next();
                if (tok.sym == "$")
                    break;
                tokens.Add(tok);
            }
        }
        catch
        {
            // Tokenization failure
            Environment.Exit(1);
        }
        // Output JSON
        var opts = new System.Text.Json.JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true,
            MaxDepth = 1000000,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        string json = System.Text.Json.JsonSerializer.Serialize(tokens, opts);
        Console.WriteLine(json);

        Environment.Exit(0);
    }
}
