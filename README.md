## ytmpv-cli

Interfaz de línea de comandos minimalista para reproducir música vía YouTube con mpv + yt-dlp.

### Descripción

Una CLI simple y directa para buscar y reproducir música desde YouTube, sin distracciones. 
Usa `yt-dlp` para obtener el audio y `mpv` como motor de reproducción, integrando todo en un solo flujo desde la consola.
Ideal para acompañarte mientras trabajás, estudiás o te concentrás.

![MONJES en funcionamiento](docs/screenshot.png)

### Características

- Búsqueda por texto en YouTube sin navegador
- Selección simple de pistas desde la consola
- Reproducción por streaming sin archivos temporales
- Comandos básicos para nueva búsqueda y salida
- Funciona en cualquier sistema con .NET + mpv

### Requisitos

- [.NET 6.0](https://dotnet.microsoft.com/) o superior
- [yt-dlp](https://github.com/yt-dlp/yt-dlp) instalado y en el PATH
- [mpv](https://mpv.io/) instalado y en el PATH
- Conexión a Internet

### Instalación

1. Clonar el repositorio
2. Verificar que MPV y yt-dlp estén instalados y accesibles
3. Compilar con `dotnet build`
4. Ejecutar con `dotnet run`

### Cómo usar

1. Ejecutá `dotnet run`
2. Ingresá el nombre del tema/artista
3. Elegí una opción de la lista (1-5)
4. La reproducción comienza automáticamente con `mpv`

Durante la reproducción:
- `ENTER`: buscar otra canción
- `Q`: salir
- `0`: volver al menú principal


