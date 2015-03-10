$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
$version = [System.Reflection.Assembly]::LoadFile("$root\KLog\KLog\bin\Release\KLog.dll").GetName().Version
$versionStr = "{0}.{1}.{2}.{3}" -f ($version.Major, $version.Minor, $version.Build, $version.Revision)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\NuGet\KLog.nuspec) 
$content = $content -replace '\$version\$', $versionStr

$content | Out-File $root\NuGet\KLog.compiled.nuspec

& nuget pack $root\NuGet\KLog.compiled.nuspec