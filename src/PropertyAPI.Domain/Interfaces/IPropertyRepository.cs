using PropertyAPI.Domain.Entities;

namespace PropertyAPI.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para operaciones de base de datos con propiedades
/// </summary>
/// 
/// 
public interface IPropertyRepository
{
    Task<IEnumerable<Property>> GetPropertiesAsync(PropertyFilter? filters = null, CancellationToken cancellationToken = default);

    Task<Property?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}

