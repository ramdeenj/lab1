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
        string baseName = Path.GetFileNameWithoutExtension(asmFile);
        string objFile  = baseName + ".o";
        string exeFile  = "out.exe";

        runProcess(Config.Configuration.dlltool,
            $"-m i386:x86-64 -d kernel32.def -l kernel32.lib");
        runProcess(Config.Configuration.clang,
            $"-c -g {asmFile} -o {objFile}");
        runProcess(Config.Configuration.linker,
            $"/debug /entry:main /subsystem:console /out:{exeFile} {objFile} kernel32.lib");
    }
}