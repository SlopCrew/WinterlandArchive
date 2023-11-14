Tracking cspotcode's experiment to create a Unity Editor project that is playable but much smaller than full game rip.
*Different from the proxy mesh that LazyDuchess made*

## TODOs

- [ ] fix exclusion filter to understand asset bundles, not just directories [can skip if limited time]
- [ ] Upload complete assetripper export to OneDrive, for stable GUIDs in future
- [ ] Prep stripped project, upload to OneDrive

## Download

TODO upload to OneDrive.  Was hoping to finish tonight, ran out of time.

https://1drv.ms/f/s!AigtPPaAHhPfp8gbt3OGc2bZ4iXy7g?e=LnnVxW (currently empty)

## Sales Pitch

Why the heck would we want to load a big project into the editor?

Having the game in editor has some benefits.

Want to augment the map w/ a camera trigger while grinding up the christmas tree? How do we build a camera trigger?  
Or mix in other vanilla game elements: pedestrians, birds, additional grind walls, etc?  
We can copy-paste existing map elements from the base game, they should "just work."  
For example, copy-paste a trashcan, replace the texture, modify the award prefab in inspector,
now you have a custom Christmas gift.

You can't add a pedestrian in the editor if you don't have the `Pedestrian` script in the editor.
Same for meshes, audio references*, textures, etc.

\*We keep the audio *references* but replace the .oggs with blanks: faster import, smaller filesize.

## Overview

Rough outline of how to accomplish this:

- [x] Export from assetripper, ~~with yaml shaders~~
- [x] Delete a bunch of files to shrink the project
- ~~fix shaders by fixing parse errors for arrays of ints~~
- [ ] Remove Steam DLLs
- [ ] Apply my code changes that enable Play button in editor
- [ ] Add mapping toolkit as well-defined subdirectory (upgrades should mean import new `.unitypackage` on top of old, or equivalent)
- [x] Modify `manifest.json` to auto-install Unity UI, Timeline, VSCode, and VS extensions
- [x] Other steps from my guide: https://github.com/cspotcode/bomb-rush-cyberfunk-modding
- [x] Turn off domain reload
- [x] Load it once to run API conversion on the DLLs
- [x] Clean it -- delete `Library` et al
- Zip it up for the team

- Test import (do not ship tested import, b/c will include massive `Library` directory)
  - Does play button work?
  - ~~Does lighting work?~~
  - ~~Do shaders render?~~

## Benchmark

Initial import takes ~7m10s
Subsequent opening the project w/Square scene takes 30s

## Manifest fixing

```json
    "com.unity.ide.visualstudio": "2.0.22",
    "com.unity.ide.vscode": "1.2.5",
    "com.unity.timeline": "1.6.5",
    "com.unity.ugui": "1.0.0",
```

## Shader fixing

AssetRipper's yaml export spits out arrays of ints in a form that is valid yaml, but Unity balks at.

https://github.com/AssetRipper/AssetRipper/issues/695

I wrote a find-and-replace script to fix them, but the results are pitch black everywhere.  So something else
must be going wrong.

## Trimming unnecessary assets

Still to be trimmed:

- Assets/meshes/Combined*??
  - No, I tried, it broke stuff

Instead of excluding audio, we neuter it.  We find every .ogg and replace it with a copy of empty.ogg.  This preserves references,
so referenced audio *should* play correctly in game even though editor is silent.

  Assets\audio
  Assets\AudioClip

Some assets' directory does not match their assetbundle.  E.g. they are in Stages/Tower but save into `common_assets` bundle.

A more robust asset stripping method would be to dump a huge list of every asset and its assigned AssetBundle.
Then can strip all mall assets, all pyramid assets, etc regardless of directory.

## Fixing missing references

I used https://github.com/edcasillas/unity-missing-references-finder

Add this to manifest.json `"com.ecasillas.missingrefsfinder": "https://github.com/edcasillas/unity-missing-references-finder.git"`

## Re-adding missing assets

AssetRipper does not generate consistent GUIDs across multiple extractions.  When I generate the final project,
it's important to keep a backup of the *full* export, for GUIDs.

Alternatively, scripts could map between GUIDs and paths and thus "repair" references in the future. That wouldn't be fun.

## Questions

Assetripper: Static mesh separation?
- no, cuz they're not separated in-game, so we gotta roll with it
