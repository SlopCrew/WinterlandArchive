Set-Location $PSScriptRoot/..
$ErrorActionPreference = 'Stop'

# `dotnet clean` doesn't delete everything.

Write-Output `
    .\libs\BRC-CommonAPI\bin `
    .\libs\BRC-CommonAPI\obj `
    .\libs\CrewBoomAPI\bin `
    .\libs\CrewBoomAPI\obj `
    .\libs\SlopCrew\SlopCrew.API\bin `
    .\libs\SlopCrew\SlopCrew.API\obj `
    .\Winterland.MapStation.Common\bin `
    .\Winterland.MapStation.Common\obj `
    .\Winterland.MapStation.Plugin\bin `
    .\Winterland.MapStation.Plugin\obj `
    .\Winterland.MustLoadInEditor\obj `
    .\Winterland.MustLoadInEditor\bin `
    .\Winterland.Common\obj `
    .\Winterland.Common\bin `
    .\Winterland.Common_Editor\obj `
    .\Winterland.Common_Editor\bin `
    .\Winterland.Plugin\obj `
    .\Winterland.Plugin\bin `
| ForEach-Object {
    if(test-path $_) { remove-item -recurse $_ }
}
