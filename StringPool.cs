using System;
using System.Collections.Generic;
using System.Text;
 
public static class StringPool
{
    // Maps processed string content -> label
    private static Dictionary<string, string> pool = new Dictionary<string, string>();
    private static int counter = 0;
 
    public static void clear()
    {
        pool.Clear();
        counter = 0;
    }
 
    // Given a raw token lexeme like "foo\nbar", return the label for the pool entry.
    public static string getLabel(string rawLexeme)
    {
        string processed = processEscapes(rawLexeme);
        if (!pool.ContainsKey(processed))
        {
            string label = $"strconst{counter++}";
            pool[processed] = label;
        }
        return pool[processed];
    }
 
    public static string processEscapes(string rawLexeme)
    {
        // Strip surrounding quotes
        string inner = rawLexeme.Substring(1, rawLexeme.Length - 2);
        var sb = new StringBuilder();
        int i = 0;
        while (i < inner.Length)
        {
            if (inner[i] == '\\')
            {
                if (i + 1 >= inner.Length)
                    throw new Exception("Unterminated escape sequence in string");
                char next = inner[i + 1];
                switch (next)
                {
                    case 'n':  sb.Append('\n'); break;
                    case 't':  sb.Append('\t'); break;
                    case '"':  sb.Append('"');  break;
                    case '\\': sb.Append('\\'); break;
                    case '0':  sb.Append('\0'); break;
                    case 'r':  sb.Append('\r'); break;
                    default:
                        throw new Exception($"Invalid escape sequence '\\{next}' in string literal");
                }
                i += 2;
            }
            else
            {
                sb.Append(inner[i]);
                i++;
            }
        }
        return sb.ToString();
    }
 
    public static void emit(System.IO.TextWriter w)
    {
        // Emit emptyString first (the default for uninitialized string vars)
        w.WriteLine("emptyString:");
        w.WriteLine("    .quad 0");   // length = 0
 
        foreach (var kv in pool)
        {
            string content = kv.Key;
            string label   = kv.Value;
 
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            long len = bytes.Length;
 
            w.WriteLine($"{label}:");
            w.WriteLine($"    .quad {len}");
 
            if (bytes.Length > 0)
            {
                var hexBytes = new StringBuilder("    .byte ");
                for (int i = 0; i < bytes.Length; i++)
                {
                    if (i > 0) hexBytes.Append(", ");
                    hexBytes.Append($"0x{bytes[i]:x2}");
                }
                w.WriteLine(hexBytes.ToString());
            }
 
            // Pad to 8-byte boundary
            int mod = bytes.Length % 8;
            if (mod != 0)
            {
                int pad = 8 - mod;
                var zeros = new StringBuilder("    .byte ");
                for (int i = 0; i < pad; i++)
                {
                    if (i > 0) zeros.Append(", ");
                    zeros.Append("0x00");
                }
                w.WriteLine(zeros.ToString());
            }
        }
    }
}