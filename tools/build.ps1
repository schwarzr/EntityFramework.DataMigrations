Import-Module -Name "./Build-Versioning.psm1"


$projects = "..\src\Extensions.EntityFrameworkCore.DataMigration\Extensions.EntityFrameworkCore.DataMigration.csproj"

New-NugetPackages `
    -Projects $projects `
    -NugetServerUrl "http://www.nuget.org/api/v2" `
    -VersionPackage "Extensions.EntityFrameworkCore.DataMigration" `
    -VersionFilePath "..\version-31.json" `
    -OutputPath "..\dist\nuget\" `
    -MsBuildParams "SignAssembly=true;AssemblyOriginatorKeyFile=..\..\private\signkey-ef-datamigrations.snk;EfVersion=31"

New-NugetPackages `
    -Projects $projects `
    -DoNotCleanOutput `
    -NugetServerUrl "http://www.nuget.org/api/v2" `
    -VersionPackage "Extensions.EntityFrameworkCore.DataMigration" `
    -VersionFilePath "..\version-50.json" `
    -OutputPath "..\dist\nuget\" `
    -MsBuildParams "SignAssembly=true;AssemblyOriginatorKeyFile=..\..\private\signkey-ef-datamigrations.snk;EfVersion=5"

$coreVersion = New-NugetPackages `
    -Projects $projects `
    -DoNotCleanOutput `
    -NugetServerUrl "http://www.nuget.org/api/v2" `
    -VersionPackage "Extensions.EntityFrameworkCore.DataMigration" `
    -VersionFilePath "..\version-60.json" `
    -OutputPath "..\dist\nuget\" `
    -MsBuildParams "SignAssembly=true;AssemblyOriginatorKeyFile=..\..\private\signkey-ef-datamigrations.snk;EfVersion=6"


Write-Host "##vso[build.updatebuildnumber]$($coreVersion.NugetVersion)"