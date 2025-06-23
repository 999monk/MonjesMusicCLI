namespace MonjesMusicCLI.Core.Models;

public class ResultadoBusqueda
{
    public int Indice { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Duracion { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
        
    public override string ToString()
    {
        return $"{Indice}. {Titulo} ({Duracion})";
    }
}