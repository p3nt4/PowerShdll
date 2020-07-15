using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.IO;

//https://blogs.msdn.microsoft.com/kebab/2014/04/28/executing-powershell-scripts-from-c/

namespace Powershdll
{
   
    class PowerShdll
    {
        PS ps;
        public PowerShdll()
        {
            ps = new PS();
        }
        public void interact()
        {
            Console.WriteLine("PowerShdll.exe");
            string cmd = "";
            while (cmd.ToLower() != "exit")
            {
                Console.Write("PS " + ps.exe("$(get-location).Path").Replace(System.Environment.NewLine, String.Empty) + ">");
                cmd = Console.ReadLine();
                Console.WriteLine(ps.exe(cmd));
            }
        }
        public static string LoadScript(string filename)
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
                Console.WriteLine(errorText);
                return ("error");
            }

        }
        public void usage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("PowerShdll.exe <script>");
            Console.WriteLine("PowerShdll.exe -h\t Display this messages");
            Console.WriteLine("PowerShdll.exe -f <path>\t Run the script passed as argument");
            Console.WriteLine("PowerShdll.exe -i\t Start an interactive console (Default)");
        }
        public void start(string[] args)
        {
            // Place payload here for embeded payload:
            string payload = "";
            if (payload.Length != 0) {
                Console.Write(ps.exe(payload));
                ps.close(); 
                return; 
            }
            if (args.Length==0) { this.interact(); return; }
            else if (args[0] == "-h")
            {
                usage();
            }
            else if (args[0] == "-w")
            {
                this.interact();
            }
            else if (args[0] == "-i")
            {
                Console.Title = "PowerShdll";
                this.interact();
                ps.close();
            }
            else if (args[0] == "-f")
            {
                if (args.Length < 2) { usage(); return; }
                string script = PowerShdll.LoadScript(args[1]);
                if (script != "error")
                {
                    Console.Write(ps.exe(script));
                }
            }
            else
            {
                string script = string.Join(" ", args);
                Console.Write(ps.exe(script));
                ps.close();
            }
            return;
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


