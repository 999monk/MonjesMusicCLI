using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using MonjesMusicCLI.Core.Models;

namespace MonjesMusicCLI.Core.Services;

public class YoutubeDlService
{
    public async Task<List<ResultadoBusqueda>> BusquedaAsync(string query)
    {
        var resultados = new  List<ResultadoBusqueda>();
        try
        {
            // comando yt-dlp
            string comando = "yt-dlp";
            string argumentos = $"--print \"%(title)s|%(duration_string)s|%(webpage_url)s\" \"ytsearch5:{query}\"";
            
            // cfg proceso
            var startInfo = new ProcessStartInfo
            {
                FileName = comando,
                Arguments = argumentos,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            };
            using var proceso = new Process { StartInfo = startInfo };
            
            proceso.Start();
            
            // Leer la salida
            string salida = await proceso.StandardOutput.ReadToEndAsync();
            string error = await proceso.StandardError.ReadToEndAsync();
                
            await proceso.WaitForExitAsync();
            
            if (proceso.ExitCode != 0)
            {
                throw new Exception($"Error ejecutando yt-dlp: {error}");
            }
                
            // Parsear resultados
            resultados = ParsearResultados(salida);
                
            return resultados;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en búsqueda: {ex.Message}");
            return new List<ResultadoBusqueda>();
        }
    }

    private List<ResultadoBusqueda> ParsearResultados(string salida)
    {
        var resultados = new List<ResultadoBusqueda>();
            
        if (string.IsNullOrWhiteSpace(salida))
            return resultados;
        
        string[] lineas = salida.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lineas.Length && i < 5; i++)
        {
            try
            {
                string linea = lineas[i].Trim();
                
                string[] partes = linea.Split('|', 3); 
                    
                if (partes.Length >= 3)
                {
                    var resultado = new ResultadoBusqueda
                    {
                        Indice = i + 1,
                        Titulo = partes[0].Trim(),
                        Duracion = partes[1].Trim(),
                        Url = partes[2].Trim()
                    };
                        
                    // Validar 
                    if (!string.IsNullOrWhiteSpace(resultado.Titulo) && 
                        !string.IsNullOrWhiteSpace(resultado.Url))
                    {
                        resultados.Add(resultado);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parseando línea {i + 1}: {ex.Message}");
                continue;
            }
        }
        return resultados;
    }
    
    /// <summary>
    /// Verifica si yt-dlp está disponible en el sistema
    /// </summary>
    public async Task<bool> EstaDisponibleAsync()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
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
}