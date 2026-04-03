using System;
using System.Collections.Generic;
using System.IO;

namespace ASM
{
    public enum Register
    {
        rax, rbx, rcx, rdx, rsi, rdi, rbp, rsp,
        xmm0, xmm1
    }

    public enum StorageClass
    {
        STATIC = 12345
    }

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
        public Label() { this.lbl = $"lbl{counter_++}"; }
        public Label(string name) { this.lbl = name; }
        public override string ToString() => $"{lbl}:";
    }

    public class Comment : Op
    {
        public readonly string comment;
        public Comment(string s) { this.comment = s; }
        public override string ToString() => $"    // {comment}";
    }

    public class RawOp : Op
    {
        string text;
        public RawOp(string text) { this.text = text; }
        public override string ToString() => text;
    }

    public class OpPushReg : Op
    {
        Register reg;
        public OpPushReg(Register reg) { this.reg = reg; }
        public override string ToString() => $"    pushq %{reg}";
    }

    public class OpPopReg : Op
    {
        Register reg;
        public OpPopReg(Register reg) { this.reg = reg; }
        public override string ToString() => $"    popq %{reg}";
    }

    public class OpMovRegReg : Op
    {
        Register src, dst;
        public OpMovRegReg(Register src, Register dst) { this.src = src; this.dst = dst; }
        public override string ToString() => $"    movq %{src}, %{dst}";
    }

    public class OpMovConstReg : Op
    {
        long value;
        Register dst;
        public OpMovConstReg(long value, Register dst) { this.value = value; this.dst = dst; }
        public override string ToString()
        {
            if (value >= int.MinValue && value <= int.MaxValue)
                return $"    movq ${value}, %{dst}";
            else
                return $"    movabsq ${value}, %{dst}";
        }
    }

    public class OpMovRegRegInd : Op
    {
        Register src, dst;
        int offset;
        public OpMovRegRegInd(Register src, int offset, Register dst)
        { this.src = src; this.offset = offset; this.dst = dst; }
        public override string ToString() => $"    movq %{src}, {offset}(%{dst})";
    }

    public class OpMovRegIndReg : Op
    {
        Register src, dst;
        int offset;
        public OpMovRegIndReg(int offset, Register src, Register dst)
        { this.offset = offset; this.src = src; this.dst = dst; }
        public override string ToString() => $"    movq {offset}(%{src}), %{dst}";
    }

    public class OpMovConstRegInd : Op
    {
        long value;
        int offset;
        Register dst;
        public OpMovConstRegInd(long value, int offset, Register dst)
        { this.value = value; this.offset = offset; this.dst = dst; }
        public override string ToString() => $"    movq ${value}, {offset}(%{dst})";
    }

    public class OpSubRegConstant : Op
    {
        long value;
        Register dst;
        public OpSubRegConstant(long value, Register dst) { this.value = value; this.dst = dst; }
        public override string ToString() => $"    subq ${value}, %{dst}";
    }

    public class OpAdd : Op
    {
        Register left, right;
        public OpAdd(Register left, Register right) { this.left = left; this.right = right; }
        public override string ToString() => $"    addq %{right}, %{left}";
    }

    public class OpSub : Op
    {
        Register left, right;
        public OpSub(Register left, Register right) { this.left = left; this.right = right; }
        public override string ToString() => $"    subq %{right}, %{left}";
    }

    public class OpMul : Op
    {
        Register left, right;
        public OpMul(Register left, Register right) { this.left = left; this.right = right; }
        public override string ToString() => $"    imulq %{right}, %{left}";
    }

    public class OpIDiv : Op
    {
        Register reg;
        public OpIDiv(Register reg) { this.reg = reg; }
        public override string ToString() => $"    idivq %{reg}";
    }

    public class OpCqo : Op
    {
        public override string ToString() => "    cqo";
    }

    public class OpNeg : Op
    {
        Register reg;
        public OpNeg(Register reg) { this.reg = reg; }
        public override string ToString() => $"    negq %{reg}";
    }

    public class OpNot : Op
    {
        Register reg;
        public OpNot(Register reg) { this.reg = reg; }
        public override string ToString() => $"    notq %{reg}";
    }

    public class OpAnd : Op
    {
        Register left, right;
        public OpAnd(Register left, Register right) { this.left = left; this.right = right; }
        public override string ToString() => $"    andq %{right}, %{left}";
    }

    public class OpOr : Op
    {
        Register left, right;
        public OpOr(Register left, Register right) { this.left = left; this.right = right; }
        public override string ToString() => $"    orq %{right}, %{left}";
    }

    public class OpXor : Op
    {
        Register left, right;
        public OpXor(Register left, Register right) { this.left = left; this.right = right; }
        public override string ToString() => $"    xorq %{right}, %{left}";
    }

    public class OpShl : Op
    {
        Register reg;
        public OpShl(Register reg) { this.reg = reg; }
        public override string ToString() => $"    shlq %cl, %{reg}";
    }

    public class OpSar : Op
    {
        Register reg;
        public OpSar(Register reg) { this.reg = reg; }
        public override string ToString() => $"    sarq %cl, %{reg}";
    }

    public class OpShr : Op
    {
        Register reg;
        public OpShr(Register reg) { this.reg = reg; }
        public override string ToString() => $"    shrq %cl, %{reg}";
    }

    public class OpMovsdMemXmm : Op
    {
        int offset; Register src, xmm;
        public OpMovsdMemXmm(int offset, Register src, Register xmm)
        { this.offset = offset; this.src = src; this.xmm = xmm; }
        public override string ToString() => $"    movsd {offset}(%{src}), %{xmm}";
    }

    public class OpMovsdXmmMem : Op
    {
        Register xmm; int offset; Register dst;
        public OpMovsdXmmMem(Register xmm, int offset, Register dst)
        { this.xmm = xmm; this.offset = offset; this.dst = dst; }
        public override string ToString() => $"    movsd %{xmm}, {offset}(%{dst})";
    }

    public class OpMovqRegXmm : Op
    {
        Register reg, xmm;
        public OpMovqRegXmm(Register reg, Register xmm) { this.reg = reg; this.xmm = xmm; }
        public override string ToString() => $"    movq %{reg}, %{xmm}";
    }

    public class OpMovqXmmReg : Op
    {
        Register xmm, reg;
        public OpMovqXmmReg(Register xmm, Register reg) { this.xmm = xmm; this.reg = reg; }
        public override string ToString() => $"    movq %{xmm}, %{reg}";
    }

    public class OpAddsd : Op
    {
        Register left, right;
        public OpAddsd(Register left, Register right) { this.left = left; this.right = right; }
        public override string ToString() => $"    addsd %{right}, %{left}";
    }

    public class OpSubsd : Op
    {
        Register left, right;
        public OpSubsd(Register left, Register right) { this.left = left; this.right = right; }
        public override string ToString() => $"    subsd %{right}, %{left}";
    }

    public class OpMulsd : Op
    {
        Register left, right;
        public OpMulsd(Register left, Register right) { this.left = left; this.right = right; }
        public override string ToString() => $"    mulsd %{right}, %{left}";
    }

    public class OpDivsd : Op
    {
        Register left, right;
        public OpDivsd(Register left, Register right) { this.left = left; this.right = right; }
        public override string ToString() => $"    divsd %{right}, %{left}";
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
            outputFile.WriteLine("    .extern SetErrorMode");
            foreach (var op in opcodes)
                outputFile.WriteLine(op);
            outputFile.WriteLine(".section .data");
            outputFile.WriteLine(".section .bss");
        }
    }
}