param(
    # Directory of Unity project
    $outputDirectory
)
rm "$outputDirectory/Library"
rm "$outputDirectory/Logs"
rm "$outputDirectory/Temp"
rm "$outputDirectory/UserSettings"
