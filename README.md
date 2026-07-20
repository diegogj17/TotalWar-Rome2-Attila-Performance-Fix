# Total War Launchers

Custom launchers for Total War: Rome II and Total War: Attila that fix battle-loading-screen
hangs on modern CPUs by capping the game's CPU affinity, while keeping Steam Workshop mods
working correctly.

## How it works

```
Steam -> Creative Assembly Launcher -> <Game>.exe (this launcher)
      -> SetProcessAffinity*.exe -> <Game>_original.exe -> Game
```

Steam/CA still launch what they think is the game's exe, but that exe has been swapped for
a small launcher which starts the CPU-affinity setter and then the real (renamed) game exe.

## Install

Download the exes from the [latest release](https://github.com/diegogj17/TotalWar-Rome2-Attila-Performance-Fix/releases/latest).

**Rome II** — in `...\Steam\steamapps\common\Total War Rome II`:
1. Rename `Rome2.exe` to `Rome2_original.exe`.
2. Put `Rome2Launcher.exe` in the folder and rename it to `Rome2.exe`.
3. Put `SetProcessAffinityRome2.exe` in the same folder.

**Attila** — in `...\Steam\steamapps\common\Total War Attila`:
1. Rename `Attila.exe` to `Attila_original.exe`.
2. Put `AttilaLauncher.exe` in the folder and rename it to `Attila.exe`.
3. Put `SetProcessAffinityAttila.exe` in the same folder.
4. One-time tweak in `%AppData%\The Creative Assembly\Attila\scripts\preferences.script.txt`
   (run the game once first if it doesn't exist yet): set `number_of_threads` to `8` and
   `gfx_video_memory` to `-4000`.

If Steam verifies/updates the game files, it will overwrite `<Game>.exe` with the original
and undo this — just repeat the steps above.

## Building from source

```
dotnet publish <ProjectFolder> -c Release
```

Each project is self-contained, single-file, `win-x64`; the exe lands in
`<ProjectFolder>/bin/Release/<tfm>/win-x64/publish/`.

## Credits

CPU affinity approach based on
[serkan-erol/Set-CPU-Affinity-for-Rome-2](https://github.com/serkan-erol/Set-CPU-Affinity-for-Rome-2).
