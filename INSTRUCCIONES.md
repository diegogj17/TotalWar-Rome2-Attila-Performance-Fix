# Instrucciones de despliegue

Estos launchers sustituyen el `.exe` que Steam/el launcher de Creative Assembly
arrancan, para forzar el `SetProcessAffinity*.exe` correspondiente antes de lanzar
el ejecutable real del juego. Esto evita los cuelgues en pantallas de carga de
batalla en Rome II y Attila en hardware moderno.

## Flujo

```
Steam -> Launcher de Creative Assembly -> <Juego>.exe (nuestro launcher)
       -> SetProcessAffinity*.exe -> <Juego>_original.exe -> Partida
```

## Por qué rutas absolutas

`Process.Start()` con ruta relativa o solo el nombre del archivo fallaba con:

```
System.ComponentModel.Win32Exception (2): The system cannot find the file specified
```

...aunque el ejecutable existía y abría bien con doble clic. Usando la ruta
absoluta completa tanto para el afinador de CPU como para el ejecutable del
juego, el problema se resolvió de forma fiable. Por eso los launchers llevan
la ruta de instalación de Steam escrita directamente en el código, en vez de
depender de `AppContext.BaseDirectory` + un nombre relativo.

## Compilar

Desde la raíz del repo, para cada proyecto:

```
dotnet publish <Carpeta del proyecto> -c Release
```

Todos los proyectos son self-contained, single-file, `win-x64`. El `.exe`
publicado queda en `<Carpeta>/bin/Release/<tfm>/win-x64/publish/`.

## Desplegar en la carpeta del juego

### Total War: Rome II
Carpeta: `C:\Program Files (x86)\Steam\steamapps\common\Total War Rome II`

1. Renombrar el `Rome2.exe` original a `Rome2_original.exe`.
2. Copiar `Rome2Launcher/bin/Release/net8.0-windows/win-x64/publish/Rome2Launcher.exe`
   a esa carpeta como `Rome2.exe`.
3. Copiar `SetProcessAffinityRome2/bin/Release/net8.0/win-x64/publish/SetProcessAffinityRome2.exe`
   a esa carpeta (puede mantener su nombre, no hace falta renombrarlo).

### Total War: Attila
Carpeta: `C:\Program Files (x86)\Steam\steamapps\common\Total War Attila`

1. Renombrar el `Attila.exe` original a `Attila_original.exe`.
2. Copiar `AttilaLauncher/bin/Release/net8.0-windows/win-x64/publish/AttilaLauncher.exe`
   a esa carpeta como `Attila.exe`.
3. Copiar `SetProcessAffinityAttila/bin/Release/net8.0/win-x64/publish/SetProcessAffinityAttila.exe`
   a esa carpeta.

## Si Steam verifica/actualiza el juego

Steam sobrescribirá `<Juego>.exe` con el original en una verificación de
archivos, deshaciendo el despliegue. Si eso pasa, repetir los pasos de
despliegue de arriba (los `_original.exe` seguirán intactos salvo que Steam
también los borre, en cuyo caso hay que volver a copiarlos de una instalación
limpia antes de renombrarlos).

## Añadir un nuevo juego

1. Duplicar `SetProcessAffinityRome2/` (o `SetProcessAffinityAttila/`) y cambiar
   la constante `GAME_PROCESS_NAME` en `AffinitySetter.cs` al nombre del proceso
   del juego (sin `.exe`), tras renombrarlo con el sufijo `_original`.
2. Duplicar `Rome2Launcher/` (o `AttilaLauncher/`) y cambiar las dos rutas
   absolutas en `Program.cs` a la carpeta de instalación del nuevo juego.
3. Publicar ambos proyectos y desplegar siguiendo los mismos pasos de arriba.
