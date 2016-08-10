using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.IO;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;
using Microsoft.Win32.SafeHandles;
//https://blogs.msdn.microsoft.com/kebab/2014/04/28/executing-powershell-scripts-from-c/

namespace Powershdll
{
    static class Program
    {
        [DllImport("kernel32.dll",
            EntryPoint = "GetStdHandle",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();
        private const int STD_OUTPUT_HANDLE = -11;
        private const int MY_CODE_PAGE = 437;

        public static void getPSTerm()
        {
           
            AllocConsole();
            IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
            StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            Console.WriteLine("PowerShdll.dll v0.1");
            string cmd = "";
            PS ps = new PS();
            while (true) {
                Console.Write("PS " + ps.exe("$(get-location).Path").Replace(System.Environment.NewLine, String.Empty) +">");
                cmd = Console.ReadLine();
                Console.WriteLine(ps.exe(cmd));
            } 

            //MessageBox.Show("Now I'm happy!");
        }
    }

    public class PS {
        Runspace runspace;

        public PS() {
            this.runspace = RunspaceFactory.CreateRunspace();
            // open it
            this.runspace.Open();

        }
        public string exe(string cmd) {
            try {
                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(cmd);
                pipeline.Commands.Add("Out-String");
                Collection<PSObject> results = pipeline.Invoke();
                StringBuilder stringBuilder = new StringBuilder();
                foreach (PSObject obj in results)
                {
                    stringBuilder.AppendLine(obj.ToString());
                }
                return stringBuilder.ToString();
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.

                string errorText = e.Message + "\n";
                return (errorText);
            }
        }
        public void close() {
            this.runspace.Close();
        }
    }

    public static class UtilDLL
    {
        public static void useless() {
        }
        [DllExport("main", CallingConvention = CallingConvention.Cdecl)]
        public static void main()
        {
            Program.getPSTerm();
        }
    }
}

