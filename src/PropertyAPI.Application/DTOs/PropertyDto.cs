namespace PropertyAPI.Application.DTOs;

/// <summary>
/// DTO para representar una propiedad en las respuestas de la API
/// </summary>

public class PropertyDto
{
    public string Id { get; set; } = string.Empty;
    public string IdOwner { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CodeInternal { get; set; } = string.Empty;
    public int Year { get; set; }
    public OwnerDto? Owner { get; set; }
    public IEnumerable<PropertyImageDto> Images { get; set; } = Array.Empty<PropertyImageDto>();
    public IEnumerable<PropertyTraceDto> Traces { get; set; } = Array.Empty<PropertyTraceDto>();
}

