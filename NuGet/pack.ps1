if($env:APPVEYOR_REPO_BRANCH -eq "release")
{
	$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
	
	$version = [System.Reflection.Assembly]::LoadFile("$root\KLog\KLog\bin\Release\netstandard2.0\KLog.dll").GetName().Version
	$versionStr = "{0}.{1}.{2}.{3}" -f ($version.Major, $version.Minor, $version.Build, $version.Revision)
	$klogVersionStr = $versionStr
	
	#  Main KLog lib no longer packed here. NuGet package is now generated on build following
	#	switch to .NET Standard. See project file for nuget settings.
	
	# KLog.Web lib
	Write-Host "Preparing KLog.Web"
	
	$version = [System.Reflection.Assembly]::LoadFile("$root\KLog\KLog.Web\bin\Release\KLog.Web.dll").GetName().Version
	$versionStr = "{0}.{1}.{2}.{3}" -f ($version.Major, $version.Minor, $version.Build, $version.Revision)

	Write-Host "Setting .nuspec version tag to $versionStr"

	$content = (Get-Content $root\NuGet\KLog.Web.nuspec) 
	$content = $content -replace '\$version\$', $versionStr
	$content = $content -replace '\$klogVersion\$', $klogVersionStr

	$content | Out-File $root\NuGet\KLog.Web.compiled.nuspec

	& nuget pack $root\NuGet\KLog.Web.compiled.nuspec
}
else
{
	Write-Host "Not packaging for nuget. To package & push to nuget, merge onto the release branch"
}