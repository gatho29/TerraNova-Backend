namespace PropertyAPI.Application.DTOs;

/// <summary>
/// DTO para representar imágenes de una propiedad
/// </summary>
public class PropertyImageDto
{
    public string IdPropertyImage { get; set; } = string.Empty;
    public string File { get; set; } = string.Empty;
    public bool Enabled { get; set; }
}


