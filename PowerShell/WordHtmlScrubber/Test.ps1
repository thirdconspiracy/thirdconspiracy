Param (
    $Folder,
    [switch]$Log
)

Write-Host "Param 1, Folder = '$Folder'"

Set-Variable -Name LogIT -value $Log -scope Global

if ( -not($PSBoundParameters.ContainsKey('Folder')))
{
    $Folder = "C:\TranslationSupportTool\HTML\"
    Write-Host "Param 1, Folder(default) = '$Folder'"
}
Write-Host "Param Flag -Log = '$Log'"


$Folders = Get-ChildItem $Folder

Write-Host $Folders