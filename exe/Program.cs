using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

//https://blogs.msdn.microsoft.com/kebab/2014/04/28/executing-powershell-scripts-from-c/

namespace Powershdll
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PowerShdll.exe v0.1");
            string cmd = "";
            PS ps = new PS();
            while (true)
            {
                Console.Write("PS " + ps.exe("$(get-location).Path").Replace(System.Environment.NewLine, String.Empty) + ">");
                cmd = Console.ReadLine();
                Console.WriteLine(ps.exe(cmd));
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
        public void close()
        {
            this.runspace.Close();
        }
    }

}

