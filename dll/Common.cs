using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

//https://blogs.msdn.microsoft.com/kebab/2014/04/28/executing-powershell-scripts-from-c/

namespace Powershdll
{
    public static class ProcessExtensions
    {
        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);

        public static void Suspend(this Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                SuspendThread(pOpenThread);
            }
        }
        public static void Resume(this Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                ResumeThread(pOpenThread);
            }
        }
        private static string FindIndexedProcessName(int pid)
        {
            var processName = Process.GetProcessById(pid).ProcessName;
            var processesByName = Process.GetProcessesByName(processName);
            string processIndexdName = null;

            for (var index = 0; index < processesByName.Length; index++)
            {
                processIndexdName = index == 0 ? processName : processName + "#" + index;
                var processId = new PerformanceCounter("Process", "ID Process", processIndexdName);
                if ((int)processId.NextValue() == pid)
                {
                    return processIndexdName;
                }
            }

            return processIndexdName;
        }

        private static Process FindPidFromIndexedProcessName(string indexedProcessName)
        {
            var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
            return Process.GetProcessById((int)parentId.NextValue());
        }

        public static Process Parent(this Process process)
        {
            return FindPidFromIndexedProcessName(FindIndexedProcessName(process.Id));
        }
    }

     class PowerShdll{
        Process pp;
        PS ps;
        public PowerShdll() {
            ps = new PS();
        }
        public void cleanup() {
            pp.Resume();
            System.Environment.Exit(1);
        }
        public void interact()
        {
            Console.WriteLine("PowerShdll.dll");
            string cmd = "";
            while (cmd.ToLower() != "exit")
            {
                Console.Write("PS " + ps.exe("$(get-location).Path").Replace(System.Environment.NewLine, String.Empty) + ">");
                cmd = Console.ReadLine();
                Console.WriteLine(ps.exe(cmd));
            }
        }
        public string LoadScript(string filename)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    StringBuilder fileContents = new StringBuilder();
                    string curLine;
                    while ((curLine = sr.ReadLine()) != null)
                    {
                        fileContents.Append(curLine + "\n");
                    }
                    return fileContents.ToString();
                }
            }
            catch (Exception e)
            {
                string errorText = e.Message + "\n";
                pp = Process.GetCurrentProcess().Parent();
                PSConsole.stealConsole(pp);
                Console.CancelKeyPress += delegate {
                    this.cleanup();
                };
                Console.SetCursorPosition(0, Console.CursorTop + 2);
                Console.WriteLine(errorText);
                return ("error");
            }

        }
        public void usage() {
            pp = Process.GetCurrentProcess().Parent();
            PSConsole.stealConsole(pp);
            Console.CancelKeyPress += delegate {
                this.cleanup();
            };
            Console.SetCursorPosition(0, Console.CursorTop + 2);
            Console.WriteLine("Usage:");
            Console.WriteLine("rundll32 PowerShdll,main <script>");
            Console.WriteLine("rundll32 PowerShdll,main -h\t Display this message");
            Console.WriteLine("rundll32 PowerShdll,main -f <path>\t Run the script passed as argument");
            Console.WriteLine("rundll32 PowerShdll,main -w\t Start an interactive console in a new window (Default)");
            Console.WriteLine("rundll32 PowerShdll,main -i\t Start an interactive console in this console");
            Console.WriteLine("\nIf you do not have an interractive console, use -n to avoid crashes on output");

        }
        public void start(string[] args) {
            // Place payload here for embeded payload:
            string payload = "";
            if (payload.Length != 0)
            {
                Console.Write(ps.exe(payload));
                ps.close();
                return;
            }
            int i=0;
            bool useConsole = true;
            string ret;
            if (args.Length == 0)
            {
                PSConsole.getNewConsole();
                this.interact();
            }
            if (args[i] == "-n")
            {
                i++;
                useConsole = false;
            }
            if (args[i] == "-h")
            {
                usage();return;
            }
            else if (args[i] == "-w" || args[i]=="")
            {
                PSConsole.getNewConsole();
                this.interact();
            }
            else if (args[i] == "-i")
            {
                pp = Process.GetCurrentProcess().Parent();
                pp.Suspend();
                PSConsole.stealConsole(pp);
                Console.Title = "PowerShdll";
                Console.CancelKeyPress += delegate
                {
                    this.cleanup();
                };
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                Console.WriteLine("Press Enter to get started:");
                Console.Write("\n");
                this.interact();
                ps.close();
                pp.Resume();
            }
            else if (args[i] == "-f")
            {
                i++;
                if (args.Length < 2) { usage(); return; }
                if (args[i] == "-n")
                {
                    if (args.Length < 3) { usage(); return; }
                    i++;
                    useConsole = false;
                }
                string script = LoadScript(args[i]);
                if (script != "error")
                {
                    ret = ps.exe(script);
                    if (useConsole)
                    {
                        pp = Process.GetCurrentProcess().Parent();
                        PSConsole.stealConsole(pp);
                        Console.CancelKeyPress += delegate
                        {
                            this.cleanup();
                        };
                        Console.SetCursorPosition(0, Console.CursorTop + 1);
                        Console.WriteLine(ret);
                    }
                }
            }
            else
            {
                string script = string.Join(" ", args, i, args.Length - i);
                if (script[0] == '"' && script[script.Length - 1] == '"')
                {
                    script = script.Substring(1, script.Length - 2);
                }
                ret = ps.exe(script);
                if (useConsole)
                {
                    pp = Process.GetCurrentProcess().Parent();
                    PSConsole.stealConsole(pp);
                    Console.CancelKeyPress += delegate
                    {
                        this.cleanup();
                    };
                    Console.SetCursorPosition(0, Console.CursorTop + 1);
                    Console.WriteLine(ret);
                }
                ps.close();
            }
            return;
    }
    
    }
    static class PSConsole
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

        [DllImport("kernel32", SetLastError = true)]
        static extern bool AttachConsole(uint dwProcessId);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();

        private const int STD_ERROR_HANDLE = -12;
        //private static bool _consoleAttached = false;
        //private static IntPtr consoleWindow;

        public static void getNewConsole()
        {

            AllocConsole();
            IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
            StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
        }
        public static void stealConsole(Process pp)
        {
            int ppid = pp.Id;
            if (AttachConsole((uint)ppid))
            {
                //_consoleAttached = true;
                IntPtr stdHandle = GetStdHandle(STD_ERROR_HANDLE); 
                SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
                FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                Encoding encoding = Encoding.ASCII;
                StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
            }
        }
 
    }

    public class PS
    {
        Runspace runspace;

        public PS()
        {
            this.runspace = RunspaceFactory.CreateRunspace();
            // open it
            this.runspace.Open();

        }
        public string exe(string cmd)
        {
            try
            {
                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(cmd);
                pipeline.Commands.Add("Out-String");
                Collection<PSObject> results = pipeline.Invoke();
                StringBuilder stringBuilder = new StringBuilder();
                foreach (PSObject obj in results)
                {
                    foreach (string line in obj.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
                    {
                        stringBuilder.AppendLine(line.TrimEnd());
                    }
                }
                string cleanOutput = "";
                using (StringReader reader = new StringReader(stringBuilder.ToString()))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        cleanOutput = line.Trim() + "\n";
                    }
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
        public void close()
        {
            this.runspace.Close();
        }
    }
}


