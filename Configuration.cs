namespace Config
{
    public class Configuration
    {
        //WINDOWS
        public static string llvmDir = @"c:\program files\llvm\bin";
        public static string clang   = llvmDir + @"\clang.exe";
        public static string linker  = llvmDir + @"\lld-link.exe";
        public static string dlltool = llvmDir + @"\llvm-dlltool.exe";
        public static string kernelLib = "kernel32.lib";
    }
}