using MongoDB.Bson.Serialization.Attributes;

namespace PropertyAPI.Domain.Entities;

/// <summary>
/// Entidad que representa al propietario de una propiedad
/// </summary>
[BsonIgnoreExtraElements]
public class Owner
{
    public string IdOwner { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    public DateTime Birthday { get; set; }
}


