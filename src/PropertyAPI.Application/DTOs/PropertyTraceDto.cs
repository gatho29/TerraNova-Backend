namespace PropertyAPI.Application.DTOs;

/// <summary>
/// DTO que representa los registros históricos de una propiedad
/// </summary>
public class PropertyTraceDto
{
    public string IdPropertyTrace { get; set; } = string.Empty;
    public DateTime DateSale { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal Tax { get; set; }
}


