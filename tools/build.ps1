Import-Module -Name "./Build-Versioning.psm1"


$projects = "..\src\Extensions.EntityFrameworkCore.DataMigration\Extensions.EntityFrameworkCore.DataMigration.csproj"


New-NugetPackages `
    -Projects $projects `
    -NugetServerUrl "https://www.nuget.org/api/v2" `
    -VersionPackage "Extensions.EntityFrameworkCore.DataMigration" `
    -VersionFilePath "..\version-60.json" `
    -OutputPath "..\dist\nuget\" `
    -MsBuildParams "SignAssembly=true;AssemblyOriginatorKeyFile=..\..\private\signkey-ef-datamigrations.snk;EfVersion=6"



$coreVersion = New-NugetPackages `
    -Projects $projects `
    -DoNotCleanOutput `
    -NugetServerUrl "https://www.nuget.org/api/v2" `
    -VersionPackage "Extensions.EntityFrameworkCore.DataMigration" `
    -VersionFilePath "..\version-80.json" `
    -OutputPath "..\dist\nuget\" `
    -MsBuildParams "SignAssembly=true;AssemblyOriginatorKeyFile=..\..\private\signkey-ef-datamigrations.snk;EfVersion=8"


Write-Host "##vso[build.updatebuildnumber]$($coreVersion.NugetVersion)"