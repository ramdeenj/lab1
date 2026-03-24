using System;
using System.Collections.Generic;
using System.IO;

namespace ASM
{
    public abstract class Op
    {
        public abstract override string ToString();
    }

    public class Ret : Op
    {
        public override string ToString() => "    ret";
    }

    public class Label : Op
    {
        private static int counter_ = 0;
        public readonly string lbl;

        public Label()
        {
            this.lbl = $"lbl{counter_++}";
        }

        public Label(string name)
        {
            this.lbl = name;
        }

        public override string ToString() => $"{lbl}:";
    }

    public class Comment : Op
    {
        public readonly string comment;
        public Comment(string s) { this.comment = s; }
        public override string ToString() => $"    /* {comment} */";
    }

    public class Asm
    {
        private static List<Op> opcodes = new List<Op>();

        public static void emit(Op op)
        {
            opcodes.Add(op);
        }

        public static void clear()
        {
            opcodes.Clear();
        }

        public static void write(TextWriter outputFile, string entryLabel)
        {
            outputFile.WriteLine(".section .text");
            outputFile.WriteLine($"    .globl {entryLabel}");
            foreach (var op in opcodes)
                outputFile.WriteLine(op);
            outputFile.WriteLine(".section .data");
            outputFile.WriteLine(".section .bss");
        }
    }
}