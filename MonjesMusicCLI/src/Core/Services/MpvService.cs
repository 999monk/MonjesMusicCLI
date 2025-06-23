using System.Diagnostics;

namespace MonjesMusicCLI.Core.Services;

public class MpvService
{
    private Process? _procesoMpv;
    private bool _pausado = false;
    public async Task PlayAsync(string url)
    {
        try
        {
            // Si ya hay un proceso reproduciéndose, detenerlo
            if (_procesoMpv != null && !_procesoMpv.HasExited)
            {
                Stop();
            }
            // Cfg comando mpv
            string comando = "mpv";
            string argumentos = $"--no-video --cache=yes --demuxer-max-bytes=100M --demuxer-max-back-bytes=50M --cache-secs=60 --no-terminal --force-seekable=yes --input-ipc-server=/tmp/mpv-socket --idle \"{url}\"";
            
            var startInfo = new ProcessStartInfo
            {
                FileName = comando,
                Arguments = argumentos,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            };
            // Crear y iniciar el proceso
            _procesoMpv = new Process { StartInfo = startInfo };
                
            // Manejar eventos del proceso
            _procesoMpv.EnableRaisingEvents = true;
            _procesoMpv.Exited += OnMpvExited;
                
            // Iniciar el proceso
            _procesoMpv.Start();
                
            Console.WriteLine($"reproduciendo: {url}");
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error iniciando MPV: {ex.Message}");
            _procesoMpv = null;
        }
        
    }
    public void Stop()
    {
        try
        {
            if (_procesoMpv != null && !_procesoMpv.HasExited)
            {
                Console.WriteLine("deteniendo reproducción...");
                
                _procesoMpv.CloseMainWindow();
                
                if (!_procesoMpv.WaitForExit(3000)) // 3 segundos
                {
                    // Si no se cierra, forzar la terminación
                    _procesoMpv.Kill();
                    Console.WriteLine("proceso MPV terminado.");
                }
                else
                {
                    Console.WriteLine("proceso MPV detenido correctamente.");
                }
            }
            else
            {
                Console.WriteLine("no hay reproducción activa.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deteniendo MPV: {ex.Message}");
        }
        finally
        {
            _procesoMpv?.Dispose();
            _procesoMpv = null;
        }
    }

    public async Task PausarAsync()
    {
        try
        {
            if (_procesoMpv != null && !_procesoMpv.HasExited)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        _procesoMpv.StandardInput.WriteLine("cycle pause");
                        _procesoMpv.StandardInput.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error enviando comando: {ex.Message}");
                    }
                });
                
                _pausado = !_pausado;
                string estado = _pausado ? "pausado" : "reanudado";
                Console.WriteLine($"Reproducción {estado}.");
            }
            else
            {
                Console.WriteLine("MPV no está corriendo.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al pausar/reanudar MPV: {ex.Message}");
        }
    }
    public void Pausar()
    {
        PausarAsync().Wait();
    }
    
    /// <summary>
    /// Verifica si MPV está reproduciendo actualmente
    /// </summary>
    public bool EstaReproduciendo
    {
        get
        {
            return _procesoMpv != null && !_procesoMpv.HasExited;
        }
    }
    /// <summary>
    /// Verifica si la reproducción está pausada
    /// </summary>
    public bool EstaPausado => _pausado;
    /// <summary>
    /// Verifica si MPV está disponible en el sistema
    /// </summary>
    public async Task<bool> EstaDisponibleAsync()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "mpv",
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
                
            using var proceso = new Process { StartInfo = startInfo };
            proceso.Start();
            await proceso.WaitForExitAsync();
                
            return proceso.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
        
    /// <summary>
    /// Evento que se ejecuta cuando MPV termina
    /// </summary>
    private void OnMpvExited(object? sender, EventArgs e)
    {
        Console.WriteLine("Reproducción terminada.");
        _procesoMpv?.Dispose();
        _procesoMpv = null;
    }
        
    /// <summary>
    /// Liberar recursos al destruir el objeto
    /// </summary>
    public void Dispose()
    {
        Stop();
    }
}