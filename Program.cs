using System.IO;
using System.Threading;
using System.Diagnostics;
using static System.Console;
using static System.ConsoleColor;

namespace AutoAffix
{
    class Program
    {
        static void Main()
        {
            //Check if Keys have been generated
            string keyfile = System.AppDomain.CurrentDomain.BaseDirectory + "lib\\gtav_aes_key.dat";
            if (!File.Exists(keyfile))
            {
                ForegroundColor = Red;
                WriteLine("________________________________________________________");
                WriteLine("You forgot to generate your Keyfiles! - Please read the README.md on GitHub!");
                WriteLine("Tool will close itself automatically in 20 seconds!");
                WriteLine("________________________________________________________");
                Thread.Sleep(20000);
                System.Environment.Exit(0);
            } 


            //Initiate Watcher
            string workdir = System.AppDomain.CurrentDomain.BaseDirectory + "workdir";
            var fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Created += FileDetected;
            fileSystemWatcher.Path = workdir;
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.IncludeSubdirectories = true;

            //Started Output
            ForegroundColor = Yellow;
            WriteLine("________________________________________________________");
            WriteLine("Welcome to Sudonym's AutoAffix Tool.");
            WriteLine("Place any extracted .rpf content into 'workdir' and the Tool will automatically recognize the new files and patch them!");
            WriteLine("Please don't forget to patch your repacked dlc.rpf in the End! (Can also be done by this tool.)");
            WriteLine("________________________________________________________");
            WriteLine("Listening...");
            WriteLine("(Press any key to exit.)");
            WriteLine("________________________________________________________");
            ReadKey();
        }

        private static void FileDetected(object sender, FileSystemEventArgs e)
        {
            //If the File-Name does not contain .rpf - Output that, and proceed
            if (!e.Name.Contains(".rpf"))
            {
                ForegroundColor = Red;
                WriteLine($"A new non-patchable file/folder has been detected - {e.Name}");
                ForegroundColor = Yellow;
                WriteLine("________________________________________________________");
                Thread.Sleep(300);
            }

            //If the File-Name contains .rpf - Output that, and redirect File to Patcher
            if (e.Name.Contains(".rpf"))
            {
                ForegroundColor = Blue;
                WriteLine($"New Patchable File has been found!");
                ForegroundColor = Green;
                Patcher(e.Name);
                ForegroundColor = Yellow;
                WriteLine("________________________________________________________");
            }
        }

        public static void Patcher(string file)
        {
            //Specify WorkDir and LibPath
            string libpath = System.AppDomain.CurrentDomain.BaseDirectory + "lib\\";
            string workdir = System.AppDomain.CurrentDomain.BaseDirectory + "workdir\\";

            //Notify User about Patching
            ForegroundColor = Yellow;
            WriteLine("Patching: "+ file +" ...");

            //Patch
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = libpath + "ArchiveFix.exe";
            p.StartInfo.Arguments = "fix " + workdir + file;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            //Notify User about Success
            ForegroundColor = Green;
            WriteLine("Successfully Patched!");
        }
    }
}