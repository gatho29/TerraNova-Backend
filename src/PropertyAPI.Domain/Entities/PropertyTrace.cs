using MongoDB.Bson.Serialization.Attributes;

namespace PropertyAPI.Domain.Entities;

/// <summary>
/// Entidad que registra la trazabilidad de la propiedad (ventas e impuestos)
/// </summary>
[BsonIgnoreExtraElements]
public class PropertyTrace
{
    public string IdPropertyTrace { get; set; } = string.Empty;
    public string IdProperty { get; set; } = string.Empty;
    public DateTime DateSale { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal Tax { get; set; }
}


