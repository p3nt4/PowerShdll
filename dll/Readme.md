# PowerShdll
Run PowerShell with dlls only.
Does not require access to powershell.exe as it uses powershell automation dlls.

## dll mode:

```
Usage:
rundll32 PowerShdll,main <script>
rundll32 PowerShdll,main -f <path>       Run the script passed as argument
rundll32 PowerShdll,main -w      Start an interactive console in a new window
rundll32 PowerShdll,main -i      Start an interactive console in this console
```

## exe mode

```
Usage:
PowerShdll.exe <script>
PowerShdll.exe -f <path>       Run the script passed as argument
PowerShdll.exe -i      Start an interactive console in this console
```

## Known Issues

Some errors do not seem to show in the output. May be confusing as commands such as Import-Module do not output an error on failure.
Make sure you have typed your commands correctly!
