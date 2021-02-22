# Append to etc/hosts
Add-Content C:\Windows\System32\drivers\etc\hosts "127.0.0.1 localhost"

# Choco Install cmder
$Env:ChocolateyToolsLocation = "C:\grmn\prj\inreach"
choco install cmder

# Expand Archive
Expand-Archive -LiteralPath 'C:\zip\loc\archive.zip' -DestinationPath 'C:\dest\loc\'

# Creating Shortcut
$WScriptShell = New-Object -ComObject WScript.Shell
$Shortcut = $WScriptShell.CreateShortcut("$env:APPDATA\Microsoft\Windows\Start Menu\Programs\Shortcuts\Cmder.lnk")
$Shortcut.TargetPath = "C:\grmn\prj\inreach\Cmder\Cmder.exe"
$Shortcut.Save()

# Create local admin user
$myUserPw = "enterPassword"
$pass = ConvertTo-SecureString "$myUserPw" -AsPlainText -Force
New-LocalUser -Name "my-user" -Password $pass -PasswordNeverExpires -AccountNeverExpires
Add-LocalGroupMember -Group "Administrators" -Member "my-user"
# Update IIS apppool config
cmd
%windir%\system32\inetsrv\appcmd.exe set apppool ".NET v4.5 Classic" /processModel.identityType:"SpecificUser" /processModel.userName:my-user "/processModel.password:$myUserPw"

