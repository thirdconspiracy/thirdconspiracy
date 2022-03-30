#Requires -RunAsAdministrator

$json = Get-Content -Raw -Path '\\ad.garmin.com\IBAM\Scratch\DEV\LocalEnvironmentVariables.json' | ConvertFrom-Json
#$json = $raw | ConvertTo-Json -Compress

# Add System Variables

$systemVars = $json.System
$systemVars | foreach {
   $var = $_.Variable
   $val = $_.Value
   Write-Output "[Environment]::SetEnvironmentVariable(`"$var`", `"secret`", [System.EnvironmentVariableTarget]::Machine)"
   [Environment]::SetEnvironmentVariable("$var", "$val", [System.EnvironmentVariableTarget]::Machine)
}

# Add User Variables
$userVars = $json.User
$userVars | foreach {
   $var = $_.Variable
   $val = $_.Value
   Write-Output "[Environment]::SetEnvironmentVariable(`"$var`", `"secret`", [System.EnvironmentVariableTarget]::User)"
   [Environment]::SetEnvironmentVariable("$var", "$val", [System.EnvironmentVariableTarget]::User)
}

