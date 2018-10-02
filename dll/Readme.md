# PowerShdll
Run PowerShell with dlls only.

Does not require access to powershell.exe as it uses powershell automation dlls.

PowerShdll can be run with: rundll32.exe, installutil.exe, regsvcs.exe, regasm.exe and regsvr32.exe.

## dll mode:

### Rundll32:
```
Usage:
rundll32 PowerShdll,main <script>
rundll32 PowerShdll,main -h      Display this message
rundll32 PowerShdll,main -f <path>       Run the script passed as argument
rundll32 PowerShdll,main -w      Start an interactive console in a new window (Default)
rundll32 PowerShdll,main -i      Start an interactive console in this console
If you do not have an interractive console, use -n to avoid crashes on output
```
### Alternatives (Credit to SubTee for these techniques):

```
1. 
    x86 - C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /logfile= /LogToConsole=false /U PowerShdll.dll
    x64 - C:\Windows\Microsoft.NET\Framework64\v4.0.3031964\InstallUtil.exe /logfile= /LogToConsole=false /U PowerShdll.dll
2. 
    x86 C:\Windows\Microsoft.NET\Framework\v4.0.30319\regsvcs.exe PowerShdll.dll
    x64 C:\Windows\Microsoft.NET\Framework64\v4.0.30319\regsvcs.exe PowerShdll.dll
3. 
    x86 C:\Windows\Microsoft.NET\Framework\v4.0.30319\regasm.exe /U PowerShdll.dll
    x64 C:\Windows\Microsoft.NET\Framework64\v4.0.30319\regasm.exe /U PowerShdll.dll
4. 
    regsvr32 /s  /u PowerShdll.dll -->Calls DllUnregisterServer
    regsvr32 /s PowerShdll.dll --> Calls DllRegisterServer
```
