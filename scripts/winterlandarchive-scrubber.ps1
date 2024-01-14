#Requires -PSEdition Core
#Requires -Version 7.4

# This script uses BFG Repo Cleaner, similar to `git filter-branch`, to remove
# all art assets from git history. This process requires rewriting *all* past
# commits, since deleting these files via a new commit would still preserve
# them in the history.

# Download BFG Repo Cleaner .jar from here:
# https://rtyley.github.io/bfg-repo-cleaner/

cd $PSScriptRoot
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

git clone --mirror https://github.com/SlopCrew/MilleniumWinterland MilleniumWinterland.git

java -jar .\bfg-1.14.0.jar --delete-files '*.{png,psd,csp,fbx,obj,mtl,asset,anim,wav,ogg,mp3,dll}' --replace-text bfg-replacements.txt --no-blob-protection MilleniumWinterland.git

# File extensions being removed:
# .png - textures, images
# .psd - textures, images (Photoshop)
# .csp - textures, images (ClipStudio)
# .fbx - meshes, textures
# .obj - meshes (Probuilder, maybe others)
# .mtl - textures(?) (Probuilder)
# .asset - *some* of these are meshes, others are objectives, etc.
# .anim - animations
# .wav - audio
# .ogg - audio
# .mp3 - audio
# .dll - in case any third-party DLLs accidentally snuck into git

# File extensions *not* being removed:
#   .playable - Timelines, animations created in Unity Editor
#   .timeline - Timelines, animations created in Unity Editor
#   .mat - Do not contain imagery
#   .prefab - contains Dialogue configured in the editor, but we are ok publishing this

cd MilleniumWinterland.git
git reflog expire --expire=now --all && git gc --prune=now --aggressive

# Safely, avoid accidentally pushing back to same origin
git remote remove origin
git remote add scrubbed https://github.com/SlopCrew/WinterlandArchive

git push --mirror scrubbed
