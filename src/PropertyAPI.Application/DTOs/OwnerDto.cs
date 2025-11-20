namespace PropertyAPI.Application.DTOs;

/// <summary>
/// DTO que representa al propietario de la propiedad
/// </summary>
public class OwnerDto
{
    public string IdOwner { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    public DateTime Birthday { get; set; }
}


