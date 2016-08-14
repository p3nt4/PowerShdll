# PowerShdll
Run PowerShell with dlls only.
Does not require access to powershell.exe as it uses powershell automation dlls.

#dll mode:

rundll32 PowerShdll.dll,main

#exe mode

powershdll.exe

#Known Issues

Some errors do not seem to show in the output. May be confusing as commands such as Import-Module do not output an error on fail.
Make sure you have typed your commands correctly!
