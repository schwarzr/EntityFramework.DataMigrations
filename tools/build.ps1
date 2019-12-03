Import-Module -Name "./Build-Versioning.psm1"


$projects = "..\src\Extensions.EntityFrameworkCore.DataMigration\Extensions.EntityFrameworkCore.DataMigration.csproj"

New-NugetPackages `
    -Projects $projects `
    -NugetServerUrl "http://www.nuget.org/api/v2" `
    -VersionPackage "Extensions.EntityFrameworkCore.DataMigration" `
    -VersionFilePath "..\version.json" `
    -OutputPath "..\dist\nuget\" `
    -MsBuildParams "SourceLinkCreate=true;SignAssembly=true;AssemblyOriginatorKeyFile=..\..\private\signkey-ef-datamigrations.snk"
