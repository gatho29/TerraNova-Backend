using MongoDB.Bson.Serialization.Attributes;

namespace PropertyAPI.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa una propiedad
/// </summary>
[BsonIgnoreExtraElements]
public class Property
{
    public string Id { get; set; } = string.Empty;
    public string IdOwner { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    [BsonElement("AddressProperty")]
    public string Address { get; set; } = string.Empty;
    [BsonElement("PriceProperty")]
    public decimal Price { get; set; }
    public string CodeInternal { get; set; } = string.Empty;
    public int Year { get; set; }
    public Owner? Owner { get; set; }
    public List<PropertyImage> Images { get; set; } = new();
    public List<PropertyTrace> Traces { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

