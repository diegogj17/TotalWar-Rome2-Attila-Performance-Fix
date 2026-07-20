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

## Why absolute paths in the launchers

`Process.Start()` with a relative path or bare filename failed with
`Win32Exception (2): The system cannot find the file specified`, even though the target
exe existed and opened fine via double-click. Using the full absolute path to both the
affinity setter and the game exe resolved it reliably, so the launchers hardcode the
Steam install path rather than relying on `AppContext.BaseDirectory` + a relative name.

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
