using AutoMapper;
using PropertyAPI.Application.DTOs;
using PropertyAPI.Application.Interfaces;
using PropertyAPI.Domain.Entities;
using PropertyAPI.Domain.Interfaces;

namespace PropertyAPI.Application.Services;

/// <summary>
/// Servicio de aplicación para gestionar propiedades
/// </summary>

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _repository;
    private readonly IMapper _mapper;

    public PropertyService(IPropertyRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<PropertyDto>> GetPropertiesAsync(PropertyFilterDto? filters = null, CancellationToken cancellationToken = default)
    {
        var domainFilter = filters != null ? new PropertyFilter
        {
            Name = filters.Name,
            Address = filters.Address,
            MinPrice = filters.MinPrice,
            MaxPrice = filters.MaxPrice
        } : null;

        var properties = await _repository.GetPropertiesAsync(domainFilter, cancellationToken);
        return _mapper.Map<IEnumerable<PropertyDto>>(properties);
    }

    public async Task<PropertyDto?> GetPropertyByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("ID cannot be null or empty", nameof(id));
        }

        var property = await _repository.GetByIdAsync(id, cancellationToken);
        return property == null ? null : _mapper.Map<PropertyDto>(property);
    }
}

