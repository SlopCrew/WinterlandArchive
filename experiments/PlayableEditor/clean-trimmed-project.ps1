param(
    # Directory of Unity project
    $projectDirectory
)
Remove-Item "$projectDirectory/Library"
Remove-Item "$projectDirectory/Logs"
Remove-Item "$projectDirectory/Temp"
Remove-Item "$projectDirectory/UserSettings"
