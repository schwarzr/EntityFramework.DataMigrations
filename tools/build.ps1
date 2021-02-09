Import-Module -Name "./Build-Versioning.psm1"


$projects = "..\src\Extensions.EntityFrameworkCore.DataMigration\Extensions.EntityFrameworkCore.DataMigration.csproj"

New-NugetPackages `
    -Projects $projects `
    -NugetServerUrl "http://www.nuget.org/api/v2" `
    -VersionPackage "Extensions.EntityFrameworkCore.DataMigration" `
    -VersionFilePath "..\version-22.json" `
    -OutputPath "..\dist\nuget\" `
    -MsBuildParams "SignAssembly=true;AssemblyOriginatorKeyFile=..\..\private\signkey-ef-datamigrations.snk;Configuration=EF22"

New-NugetPackages `
    -Projects $projects `
    -DoNotCleanOutput `
    -NugetServerUrl "http://www.nuget.org/api/v2" `
    -VersionPackage "Extensions.EntityFrameworkCore.DataMigration" `
    -VersionFilePath "..\version-31.json" `
    -OutputPath "..\dist\nuget\" `
    -MsBuildParams "SignAssembly=true;AssemblyOriginatorKeyFile=..\..\private\signkey-ef-datamigrations.snk;Configuration=EF31"

$coreVersion = New-NugetPackages `
    -Projects $projects `
    -DoNotCleanOutput `
    -NugetServerUrl "http://www.nuget.org/api/v2" `
    -VersionPackage "Extensions.EntityFrameworkCore.DataMigration" `
    -VersionFilePath "..\version-50.json" `
    -OutputPath "..\dist\nuget\" `
    -MsBuildParams "SignAssembly=true;AssemblyOriginatorKeyFile=..\..\private\signkey-ef-datamigrations.snk;EfVersion=50"


Write-Host "##vso[build.updatebuildnumber]$($coreVersion.NugetVersion)"