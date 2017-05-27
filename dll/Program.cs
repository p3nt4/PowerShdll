using System;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;

//https://blogs.msdn.microsoft.com/kebab/2014/04/28/executing-powershell-scripts-from-c/

namespace Powershdll
{
    public static class UtilDLL
    {
        public static void useless() {
        }
        [DllExport("main", CallingConvention = CallingConvention.Cdecl)]
        public static void main(IntPtr hwnd, IntPtr hinst, string lpszCmdLine, int nCmdShow)
        {
            PowerShdll psdl = new PowerShdll();
            psdl.start(lpszCmdLine.Split(' '));
        }
    }

}


