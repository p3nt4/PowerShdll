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
            PowerShdll psdl = new PowerShdll();
            psdl.start(args);
        }
    }

}

