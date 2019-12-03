Import-Module -Name "./Build-Versioning.psm1"

$projects = "..\src\Extensions.EntityFramework.DataMigration\Extensions.EntityFramework.DataMigration.csproj"

New-NugetPackages `
    -Projects $projects `
    -NugetServerUrl "http://www.nuget.org/api/v2" `
    -VersionPackage "Extensions.EntityFramework.DataMigration" `
    -VersionFilePath "..\version-ef-full.json" `
    -OutputPath "..\dist\nuget\" `
    -MsBuildParams "SourceLinkCreate=true;SignAssembly=true;AssemblyOriginatorKeyFile=..\..\private\signkey.snk"