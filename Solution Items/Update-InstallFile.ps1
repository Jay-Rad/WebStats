$InstallScript = $args[0]
$DllPath = $args[1]

$DllBytes = [System.IO.File]::ReadAllBytes($DllPath)
$DllBase64 = [System.Convert]::ToBase64String($DllBytes)

[string[]]$InstallContents = Get-Content -Path $InstallScript
$Index = $InstallContents.IndexOf(($InstallContents | Where-Object {$_ -like "`$HttpModuleBase64 =*"}))
$InstallContents[$Index] = "`$HttpModuleBase64 = `"$DllBase64`""
($InstallContents | Out-String).Trim() | Out-File -FilePath $InstallScript