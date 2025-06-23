using System;
using System.Collections.Generic;
using MonjesMusicCLI.Core.Models;

namespace MonjesMusicCLI.UI;

public class ConsoleMenu
{
    public void Bienvenida()
    {
        Console.Clear();
        Console.WriteLine("================================================");
        Console.WriteLine("                    MONJES                      ");
        Console.WriteLine("             música en la terminal              ");
        Console.WriteLine("================================================");
        Console.WriteLine();
        Console.WriteLine();
    }
    public string ObtenerBusqueda()
    {
        Console.Write("búsqueda: ");
        string? busqueda = Console.ReadLine();
            
        while (string.IsNullOrWhiteSpace(busqueda))
        {
            Console.WriteLine("por favor ingresa una búsqueda válida.");
            Console.Write("búsqueda: ");
            busqueda = Console.ReadLine();
        }
            
        return busqueda.Trim();
    }
    public void MostrarResultados(List<ResultadoBusqueda> resultados)
    {
        Console.WriteLine();
        Console.WriteLine("=======================");
        Console.WriteLine("RESULTADOS:");
            
        if (resultados.Count == 0)
        {
            Console.WriteLine("no se encontraron resultados.");
            return;
        }
            
        foreach (var resultado in resultados)
        {
            Console.WriteLine($"{resultado.Indice}. {resultado.Titulo}");
            Console.WriteLine($"   Duración: {resultado.Duracion}");
            Console.WriteLine();
        }
    }
    public int OpcionUsuario(int maxOpciones)
    {
        Console.WriteLine($"selecciona una opción (1-{maxOpciones}) o 0 para salir:");
        Console.Write("tu elección: ");
            
        string? input = Console.ReadLine();
            
        while (true)
        {
            if (int.TryParse(input, out int opcion))
            {
                if (opcion >= 0 && opcion <= maxOpciones)
                {
                    return opcion;
                }
            }
                
            Console.WriteLine($"opción inválida. Ingresa un número entre 0 y {maxOpciones}:");
            Console.Write("tu elección: ");
            input = Console.ReadLine();
        }
    }
    public void MostrarReproduciendo(ResultadoBusqueda resultado)
    {
        Console.WriteLine();
        Console.WriteLine("========================");
        Console.WriteLine("ESCUCHANDO:");
        Console.WriteLine($"Título: {resultado.Titulo}");
        Console.WriteLine($"Duración: {resultado.Duracion}");
        Console.WriteLine("========================");
        Console.WriteLine();
        Console.WriteLine("presiona ENTER para hacer una nueva búsqueda o 'q' + ENTER para salir...");
    }
    public void MostrarBuscando()
    {
        Console.WriteLine();
        Console.WriteLine("buscando...");
    }

    public void MostrarError(string mensaje)
    {
        Console.WriteLine();
        Console.WriteLine($"Error: {mensaje}");
        Console.WriteLine();
    }

    public void MostrarMensaje(string mensaje)
    {
        Console.WriteLine();
        Console.WriteLine($"ℹ️  {mensaje}");
        Console.WriteLine();
    }

    public bool PreguntarContinuar()
    {
        Console.WriteLine();
        Console.Write("¿deseas hacer otra búsqueda? (s/n): ");
        string? respuesta = Console.ReadLine()?.ToLower().Trim();
            
        return respuesta == "s" || respuesta == "si" || respuesta == "y" || respuesta == "yes";
    }

    public void Despedida()
    {
        Console.WriteLine();
        Console.WriteLine("cerrando...");
        Console.WriteLine("¡hasta luego!");
    }
        
    public void LimpiarPantalla()
    {
        Console.Clear();
    }
        
    public void EsperarTecla()
    {
        Console.WriteLine();
        Console.WriteLine("presiona cualquier tecla para continuar...");
        Console.ReadKey();
    }
}