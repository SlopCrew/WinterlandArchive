param(
    $projectDirectory,
    $packagedOutputDirectory
)

# As an alternative to cleaning a project in-place, which would require me to re-import yet again,
# I can xcopy it to a new location, then clean the copy.

# xcopy was giving me error for long paths: https://en.wikipedia.org/wiki/XCOPY#:~:text=directory%202%5Cmyfile%22-,Limitations,is%20longer%20than%20254%20characters.
# used robocopy instead https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/robocopy
robocopy "$projectDirectory" "$packagedOutputDirectory" /mir /ndl /nfl
.\clean-trimmed-project.ps1 -projectDirectory $packagedOutputDirectory