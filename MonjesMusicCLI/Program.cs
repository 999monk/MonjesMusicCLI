
using System;
using System.Threading;
using System.Threading.Tasks;
using MonjesMusicCLI.Core.Models;
using MonjesMusicCLI.Core.Services;
using MonjesMusicCLI.UI;

namespace MonjesMusicCLI;

public class Program
{
    private static readonly ConsoleMenu _menu = new();
    private static readonly YoutubeDlService _youtubeDlService = new();
    private static readonly MpvService _mpvService = new();

    static async Task Main(string[] args)
    {
        try
        {
            // Verificar dependencias
            if (!await VerificarDependencias())
            {
                return;
            }
            
            _menu.Bienvenida();

            // Bucle principal
            bool continuar = true;
            while (continuar)
            {
                try
                {
                    // Obtener búsqueda del usuario
                    string busqueda = _menu.ObtenerBusqueda();

                    // Realizar búsqueda
                    _menu.MostrarBuscando();
                    var resultados = await _youtubeDlService.BusquedaAsync(busqueda);

                    if (resultados.Count == 0)
                    {
                        _menu.MostrarError("No se encontraron resultados para tu búsqueda.");
                        continuar = _menu.PreguntarContinuar();
                        if (continuar)
                        {
                            _menu.LimpiarPantalla();
                            _menu.Bienvenida();
                        }
                        continue;
                    }
                    
                    _menu.MostrarResultados(resultados);
                    
                    int opcion = _menu.OpcionUsuario(resultados.Count);

                    if (opcion == 0)
                    {
                        continuar = false;
                        break;
                    }

                    // Reproducir 
                    var cancionSeleccionada = resultados[opcion - 1];
                    await ReproducirCancion(cancionSeleccionada);
                    
                    continuar = _menu.PreguntarContinuar();
                    if (continuar)
                    {
                        _menu.LimpiarPantalla();
                        _menu.Bienvenida();
                    }
                }
                catch (Exception ex)
                {
                    _menu.MostrarError($"Ocurrió un error inesperado: {ex.Message}");
                    continuar = _menu.PreguntarContinuar();
                    if (continuar)
                    {
                        _menu.LimpiarPantalla();
                        _menu.Bienvenida();
                    }
                }
            }
            _menu.Despedida();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error crítico: {ex.Message}");
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }
        finally
        {
            // Limpiar recursos
            _mpvService.Dispose();
        }
    }
    private static async Task<bool> VerificarDependencias()
    {
        _menu.MostrarMensaje("Verificando dependencias...");

        // Verificar yt-dlp
        bool ytDlpDisponible = await _youtubeDlService.EstaDisponibleAsync();
        if (!ytDlpDisponible)
        {
            _menu.MostrarError("yt-dlp no está instalado o no está disponible en PATH.");
            _menu.MostrarMensaje("Por favor instala yt-dlp desde: https://github.com/yt-dlp/yt-dlp");
            _menu.EsperarTecla();
            return false;
        }

        // Verificar MPV
        bool mpvDisponible = await _mpvService.EstaDisponibleAsync();
        if (!mpvDisponible)
        {
            _menu.MostrarError("MPV no está instalado o no está disponible en PATH.");
            _menu.MostrarMensaje("Por favor instala MPV desde: https://mpv.io/installation/");
            _menu.EsperarTecla();
            return false;
        }

        _menu.MostrarMensaje("✅ Todas las dependencias están disponibles.");
        await Task.Delay(1000); // Pausa breve para que el usuario vea el mensaje
        _menu.LimpiarPantalla();
        return true;
    }
    private static async Task ReproducirCancion(ResultadoBusqueda cancion)
    {
        try
        {
            _menu.MostrarReproduciendo(cancion);
            
            await _mpvService.PlayAsync(cancion.Url);
            
            await EsperarEntradaUsuario();
        }
        catch (Exception ex)
        {
            _menu.MostrarError($"Error al reproducir: {ex.Message}");
            _menu.EsperarTecla();
        }
    }
    private static async Task EsperarEntradaUsuario()
    {
        await Task.Run(() =>
        {
            while (_mpvService.EstaReproduciendo)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    
                    switch (key.Key)
                    {
                        case ConsoleKey.Q:
                            _mpvService.Stop();
                            Environment.Exit(0);
                            break;

                        case ConsoleKey.Enter:
                            _mpvService.Stop();
                            return;

                        case ConsoleKey.Spacebar:
                            _mpvService.Pausar();
                            break;
                    }
                    
                }
                
                Thread.Sleep(100);
            }
        });
    }
}
