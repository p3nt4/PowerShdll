$bytes  = [System.IO.File]::ReadAllBytes("powershell.exe");
$offset = 0x6C;
$bytes[$offset]   = 0x4C;
$bytes[$offset+1] = 0x4F;
$bytes[$offset+2] = 0x4C;
[System.IO.File]::WriteAllBytes("powershell2.exe", $bytes);