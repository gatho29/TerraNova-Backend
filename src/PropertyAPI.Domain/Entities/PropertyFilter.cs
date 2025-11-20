namespace PropertyAPI.Domain.Entities;

/// <summary>
/// Clase para filtrar propiedades en las consultas
/// </summary>
public class PropertyFilter
{
    /// <summary>
    /// Filtro por nombre de la propiedad (búsqueda parcial, case-insensitive)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filtro por dirección de la propiedad (búsqueda parcial, case-insensitive)
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Precio mínimo del rango
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Precio máximo del rango
    /// </summary>
    public decimal? MaxPrice { get; set; }
}

