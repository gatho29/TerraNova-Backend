using MongoDB.Bson.Serialization.Attributes;

namespace PropertyAPI.Domain.Entities;

/// <summary>
/// Entidad que representa una imagen asociada a una propiedad
/// </summary>
[BsonIgnoreExtraElements]
public class PropertyImage
{
    public string IdPropertyImage { get; set; } = string.Empty;
    public string IdProperty { get; set; } = string.Empty;
    public string File { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
}


