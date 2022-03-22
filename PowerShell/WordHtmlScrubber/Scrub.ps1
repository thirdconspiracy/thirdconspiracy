Param (
    $Folder,
    [switch]$Log
)

# Parameters
Write-Host "Param 1, Folder = '$Folder'"
if ( -not($PSBoundParameters.ContainsKey('Folder')))
{
    $Folder = "C:\TranslationSupportTool\HTML\"
    Write-Host "Param 1, Folder(default) = '$Folder'"
}
Write-Host "Param Flag -Log = '$Log'"
Set-Variable -Name LogIT -value $Log -scope Global

# Get files
$Folders = Get-ChildItem $Folder

Write-Host "Files to scrub:"
ForEach ( $Child in $Folders ) {

    If ( $Child.Extension -eq ".htm" ) {
        write-host " * $Child"
    }
}

$Response = read-host "convert these files? (Y/n)"

If ( $Response -eq "" -or $Response -eq "y" -or $Response -eq "Y" ) {

    Function DoIt {

        Param ( $Folder )

        $Folders = Get-ChildItem $Folder

        ForEach ( $Child in $Folders ) {

            If ( $Child.PSIsContainer ) {
                #DoIt $Child.FullName
                # Do nothing
            }
            Else {

                $Extensions = "/.htm/"

                $FileExtension = "/" + $Child.Extension + "/"

                If ( $Extensions.Contains( $FileExtension ) -and $FileExtension -gt "" ) {
                    .\Actions.ps1 -FullName $Child.FullName
                }
            }
        }
    }
} Else {
    write-host "nothing converted"
    break
}

DoIt $Folder

Write-Host "Scrubbing Complete"
if ( $Log ) {
    $tmp = Get-Date -Format "yyyyMMdd_*"
    Write-Host "Log written to"
    Write-Host "   C:\logs\WordHtmlScrubber\Session_$tmp.log"
}
Write-Host ""