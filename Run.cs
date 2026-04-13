using System;
using System.Diagnostics;
using System.IO;

public static class Run
{
    private static void runProcess(string exe, string args)
    {
        var psi = new ProcessStartInfo(exe, args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false
        };

        var p = Process.Start(psi)!;
        string stdout = p.StandardOutput.ReadToEnd();
        string stderr = p.StandardError.ReadToEnd();
        p.WaitForExit();

        if (p.ExitCode != 0)
        {
            Console.Error.WriteLine(stderr);
            Console.Error.WriteLine(stdout);
            Environment.Exit(1);
        }
    }

    public static void compile(string asmFile)
    {
        string workDir  = Directory.GetCurrentDirectory();
        string baseName = Path.GetFileNameWithoutExtension(asmFile);
        string objFile  = baseName + ".o";

        string asmFull   = Path.Combine(workDir, asmFile);
        string objFull   = Path.Combine(workDir, objFile);
        string runtimeC  = Path.Combine(workDir, "runtime.c");
        string runtimeO  = Path.Combine(workDir, "runtime.o");
        string kernelDef = Path.Combine(workDir, "kernel32.def");
        string kernelLib = Path.Combine(workDir, "kernel32.lib");
        string exeFull   = Path.Combine(workDir, "out.exe");

        runProcess(Config.Configuration.dlltool,
            $"-m i386:x86-64 -d \"{kernelDef}\" -l \"{kernelLib}\"");
        runProcess(Config.Configuration.clang,
            $"-c -g \"{asmFull}\" -o \"{objFull}\"");
        runProcess(Config.Configuration.clang,
            $"-c -g \"{runtimeC}\" -o \"{runtimeO}\"");
        runProcess(Config.Configuration.linker,
            $"/debug /entry:main /subsystem:console /out:\"{exeFull}\" \"{objFull}\" \"{runtimeO}\" \"{kernelLib}\"");
    }
}