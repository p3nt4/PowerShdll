using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.IO;

namespace PSUnlock
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage:\nPSUnlock.exe <script>\nPSUnlock.exe -f <path>");
                return;
            }
            if (args[0]=="-f")
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage:\nPSUnlock.exe <script>\nPSUnlock.exe -f <path>");
                    return;
                }
                string script= UtilPS.LoadScript(args[1]);
                if (script != "error") {
                    Console.WriteLine(UtilPS.RunScript(script));
                }
                return;
            }
            Console.WriteLine(UtilPS.RunScript(args[0]));
        }
    }

    public static class UtilPS
    {
        //Thanks to Keith Babinec, who I assume did not know what he unleashed :)
        //https://blogs.msdn.microsoft.com/kebab/2014/04/28/executing-powershell-scripts-from-c/

        // helper method that takes your script path, loads up the script
        // into a variable, and passes the variable to the RunScript method
        // that will then execute the contents
        public static string LoadScript(string filename)
        {

            try
            {
                // Create an instance of StreamReader to read from our file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(filename))
                {


                    // use a string builder to get all our lines from the file
                    StringBuilder fileContents = new StringBuilder();

                    // string to hold the current line
                    string curLine;

                    // loop through our file and read each line into our
                    // stringbuilder as we go along
                    while ((curLine = sr.ReadLine()) != null)
                    {
                        // read each line and MAKE SURE YOU ADD BACK THE
                        // LINEFEED THAT IT THE ReadLine() METHOD STRIPS OFF
                        fileContents.Append(curLine + "\n");
                    }

                    // call RunScript and pass in our file contents
                    // converted to a string
                    return fileContents.ToString();
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
 
                string errorText =e.Message + "\n";
                Console.WriteLine(errorText);
                return ("error");
            }



        }


        // Takes script text as input and runs it, then converts
        // the results to a string to return to the user
        public static string RunScript(string scriptText)
        { 
            try{
                // create Powershell runspace
                Runspace runspace = RunspaceFactory.CreateRunspace();

                // open it
                runspace.Open();

                // create a pipeline and feed it the script text
                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(scriptText);

                // add an extra command to transform the script output objects into nicely formatted strings
                // remove this line to get the actual objects that the script returns. For example, the script
                // "Get-Process" returns a collection of System.Diagnostics.Process instances.
                pipeline.Commands.Add("Out-String");
                // execute the script
                Collection<PSObject> results = pipeline.Invoke();

                // close the runspace
                runspace.Close();
                // convert the script result into a single string
                StringBuilder stringBuilder = new StringBuilder();
                foreach (PSObject obj in results)
                {
                    stringBuilder.AppendLine(obj.ToString());
                }

                // return the results of the script that has
                // now been converted to text
                return stringBuilder.ToString();
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                string errorText = "Error Executing Script:\n";
                errorText += e.Message + "\n";
                return errorText;
            }
        }
    }
}

