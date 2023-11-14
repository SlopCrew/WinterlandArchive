param(
    # output of AssetRipper, will be copied to new destination
    $assetRipperDirectory,
    # Directory to create the output Unity project
    $outputDirectory
)

$assetRipperUnityProject = $assetRipperDirectory + "\Bomb Rush Cyberfunk\ExportedProject"

# https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/xcopy
xcopy /q /i "$assetRipperUnityProject" "$outputDirectory" /e /h /c /EXCLUDE:xcopy-exclusions.txt
# For vending machines
mkdir "$outputDirectory\Assets\Stages\Tower\materials"
get-item "$assetRipperUnityProject\Assets\Stages\Tower\materials\TowerProps01_*" | ForEach-Object {
    Copy-Item $_.fullname "$outputDirectory\Assets\Stages\Tower\materials\$($_.name)"
}

# Add package dependencies
node ./fix-manifest.mjs $outputDirectory/Packages/manifest.json

# Replace every audio file with an empty file.
# They import faster, take less space, and avoid breaking references.
# So if you load them in the game, they should still play correctly.
& {
    Get-Item "$outputDirectory\Assets\audio\*.ogg"
    Get-Item "$outputDirectory\Assets\audio\*\*.ogg"
    Get-Item "$outputDirectory\Assets\audio\*\*\*.ogg"
    Get-Item "$outputDirectory\Assets\audio\*\*\*\*\*.ogg"
    Get-Item "$outputDirectory\Assets\AudioClip\*.ogg"
} | ForEach-Object {
    Write-Host "Overwriting with empty.ogg: " $_.fullname
    Copy-Item ./empty.ogg $_.fullname
}