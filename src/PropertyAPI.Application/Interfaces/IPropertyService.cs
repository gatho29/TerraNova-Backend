using PropertyAPI.Application.DTOs;

namespace PropertyAPI.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de propiedades
/// </summary>

public interface IPropertyService
{
 Task<IEnumerable<PropertyDto>> GetPropertiesAsync(PropertyFilterDto? filters = null, CancellationToken cancellationToken = default);
 Task<PropertyDto?> GetPropertyByIdAsync(string id, CancellationToken cancellationToken = default);
}

