$root = (Split-Path -Parent $MyInvocation.MyCommand.Definition)
$version = [System.Reflection.Assembly]::LoadFile("$root\OIDC Library\OIDC Library\bin\Debug\OIDC Library.dll").GetName().Version
$versionStr = "{0}.{1}.{2}-beta" -f ($version.Major, $version.Minor, $version.Build)

$content = (Get-Content "$root\OIDC Library\OIDC Library\OIDC Library.nuspec")
$content = $content -replace '\$version\$', $versionStr

$content | Out-File "$root\OIDC Library\OIDC Library\OIDC Library.nuspec"