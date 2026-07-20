# Total War Launchers

Custom launchers that inject `SetProcessAffinity` between Steam/the CA launcher and the
real game executable, working around loading-screen hangs caused by too many CPU
cores/threads being assigned to these older Total War titles.

## Flow

```
Steam -> Creative Assembly Launcher -> <Game>.exe (custom launcher) -> SetProcessAffinity*.exe
                                                                     -> <Game>_original.exe -> Juego
```

## Projects

- `Rome2Launcher/` — replaces `Rome2.exe` in the Total War: Rome II install folder.
  Original game exe renamed to `Rome2_original.exe`.
- `SetProcessAffinityRome2/` — watches for `Rome2_original.exe` and caps its CPU affinity.
- `AttilaLauncher/` — replaces `Attila.exe` in the Total War: Attila install folder.
  Original game exe renamed to `Attila_original.exe`.
- `SetProcessAffinityAttila/` — watches for `Attila_original.exe` and caps its CPU affinity.

## Why `Process.Start()` needs a resolved absolute path

`Process.Start()` with a relative path or bare filename failed with
`Win32Exception (2): The system cannot find the file specified`, even though the target
exe existed and opened fine via double-click. The fix is to always pass a full path
resolved from `AppContext.BaseDirectory` (the folder the launcher itself lives in,
i.e. the game's install folder) rather than a relative filename — this also makes the
launcher portable to any install location, no hardcoded Steam path needed.

The engine also looks for `used_mods.txt` in the child process's **working directory**,
not its exe directory, so `ProcessStartInfo.WorkingDirectory` is set explicitly to the
game folder and the CA launcher's original CLI args are forwarded through — otherwise
mods silently fail to load even with everything else working.

## Building

```
dotnet publish <ProjectFolder> -c Release
```

Each project is self-contained, single-file, `win-x64`. The published `.exe` lands in
`<ProjectFolder>/bin/Release/<tfm>/win-x64/publish/`.

## Deploying

1. In the game's install folder, rename the original game exe to `<Game>_original.exe`.
2. Copy the published launcher exe in as `<Game>.exe`.
3. Copy the published `SetProcessAffinity*.exe` into the same folder.

See [INSTRUCCIONES.md](INSTRUCCIONES.md) for the full deployment steps (in Spanish),
including the Attila `preferences.script.txt` tweak.

## Credits

The CPU affinity approach (`SetProcessAffinityRome2`/`SetProcessAffinityAttila`) is
based on [serkan-erol/Set-CPU-Affinity-for-Rome-2](https://github.com/serkan-erol/Set-CPU-Affinity-for-Rome-2).
