using MongoDB.Bson;
using MongoDB.Driver;
using PropertyAPI.Domain.Entities;
using PropertyAPI.Domain.Interfaces;
using PropertyAPI.Infrastructure.Data;
using PropertyAPI.Infrastructure.Helpers;
using System.Linq;

namespace PropertyAPI.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de propiedades con MongoDB
/// </summary>

public class PropertyRepository : IPropertyRepository
{
    private readonly IMongoCollection<Property> _propertiesCollection;
    private readonly IMongoCollection<Owner> _ownersCollection;
    private readonly IMongoCollection<PropertyImage> _imagesCollection;
    private readonly IMongoCollection<PropertyTrace> _tracesCollection;

    public PropertyRepository(MongoDbContext context)
    {
        _propertiesCollection = context.GetCollection<Property>("Properties");
        _ownersCollection = context.GetCollection<Owner>("Owners");
        _imagesCollection = context.GetCollection<PropertyImage>("PropertyImages");
        _tracesCollection = context.GetCollection<PropertyTrace>("PropertyTraces");

        CreateIndexes();
    }

    private void CreateIndexes()
    {
        try
        {
            var existingIndexes = _propertiesCollection.Indexes.List().ToList();
            var existingNames = existingIndexes
                .Select(idx => idx.TryGetValue("name", out var value) ? value?.AsString : null)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var indexesToCreate = new List<CreateIndexModel<Property>>();

            void AddIndex(CreateIndexModel<Property> indexModel)
            {
                var indexName = indexModel.Options?.Name;
                if (!string.IsNullOrWhiteSpace(indexName) && existingNames.Contains(indexName))
                {
                    return;
                }
                indexesToCreate.Add(indexModel);
            }

            AddIndex(new CreateIndexModel<Property>(
                Builders<Property>.IndexKeys.Ascending(p => p.Name),
                new CreateIndexOptions { Name = "Name_Index" }
            ));

            AddIndex(new CreateIndexModel<Property>(
                Builders<Property>.IndexKeys.Ascending(p => p.Address),
                new CreateIndexOptions { Name = "Address_Index" }
            ));

            AddIndex(new CreateIndexModel<Property>(
                Builders<Property>.IndexKeys.Ascending(p => p.Price),
                new CreateIndexOptions { Name = "Price_Index" }
            ));

            AddIndex(new CreateIndexModel<Property>(
                Builders<Property>.IndexKeys.Ascending(p => p.IdOwner),
                new CreateIndexOptions { Name = "IdOwner_Index" }
            ));

            AddIndex(new CreateIndexModel<Property>(
                Builders<Property>.IndexKeys.Ascending(p => p.CodeInternal),
                new CreateIndexOptions { Name = "CodeInternal_Index", Unique = true }
            ));

            if (indexesToCreate.Count > 0)
            {
                _propertiesCollection.Indexes.CreateMany(indexesToCreate);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear índices: {ex.Message}");
        }
    }

    public async Task<IEnumerable<Property>> GetPropertiesAsync(PropertyFilter? filters = null, CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<Property>.Filter;
        var filterDefinitions = new List<FilterDefinition<Property>>();

        if (!string.IsNullOrWhiteSpace(filters?.Name))
        {
            // Normalizar el término de búsqueda para ignorar acentos y mayúsculas/minúsculas
            var normalizedNamePattern = TextNormalizer.CreateAccentInsensitiveRegex(filters.Name);
            var nameFilter = filterBuilder.Regex(
                p => p.Name,
                new MongoDB.Bson.BsonRegularExpression(normalizedNamePattern, "i")
            );
            filterDefinitions.Add(nameFilter);
        }

        if (!string.IsNullOrWhiteSpace(filters?.Address))
        {
            // Normalizar el término de búsqueda para ignorar acentos y mayúsculas/minúsculas
            var normalizedAddressPattern = TextNormalizer.CreateAccentInsensitiveRegex(filters.Address);
            var addressFilter = filterBuilder.Regex(
                p => p.Address,
                new MongoDB.Bson.BsonRegularExpression(normalizedAddressPattern, "i")
            );
            filterDefinitions.Add(addressFilter);
        }

        if (filters?.MinPrice.HasValue == true || filters?.MaxPrice.HasValue == true)
        {
            FilterDefinition<Property> priceFilterBuilder = filterBuilder.Empty;

            if (filters.MinPrice.HasValue && filters.MaxPrice.HasValue)
            {
                priceFilterBuilder = filterBuilder.And(
                    filterBuilder.Gte(p => p.Price, filters.MinPrice.Value),
                    filterBuilder.Lte(p => p.Price, filters.MaxPrice.Value)
                );
            }
            else if (filters.MinPrice.HasValue)
            {
                priceFilterBuilder = filterBuilder.Gte(p => p.Price, filters.MinPrice.Value);
            }
            else if (filters.MaxPrice.HasValue)
            {
                priceFilterBuilder = filterBuilder.Lte(p => p.Price, filters.MaxPrice.Value);
            }

            filterDefinitions.Add(priceFilterBuilder);
        }

        var finalFilter = filterDefinitions.Count > 0
            ? filterBuilder.And(filterDefinitions)
            : filterBuilder.Empty;

        var properties = await _propertiesCollection
            .Find(finalFilter)
            .SortBy(p => p.Name)
            .ToListAsync(cancellationToken);

        await LoadRelatedEntitiesAsync(properties, cancellationToken);

        return properties;
    }

    public async Task<Property?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Property>.Filter.Eq(p => p.Id, id);
        var property = await _propertiesCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

        if (property == null)
        {
            return null;
        }

        await LoadRelatedEntitiesAsync(new List<Property> { property }, cancellationToken);
        return property;
    }

    private async Task LoadRelatedEntitiesAsync(IReadOnlyCollection<Property> properties, CancellationToken cancellationToken)
    {
        if (!properties.Any())
        {
            return;
        }

        var ownerIds = properties
            .Select(p => p.IdOwner)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList();

        var propertyIds = properties
            .Select(p => p.Id)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList();

        var ownersTask = ownerIds.Count > 0
            ? _ownersCollection
                .Find(Builders<Owner>.Filter.In(o => o.IdOwner, ownerIds))
                .ToListAsync(cancellationToken)
            : Task.FromResult(new List<Owner>());

        var imagesTask = propertyIds.Count > 0
            ? _imagesCollection
                .Find(Builders<PropertyImage>.Filter.In(i => i.IdProperty, propertyIds))
                .ToListAsync(cancellationToken)
            : Task.FromResult(new List<PropertyImage>());

        var tracesTask = propertyIds.Count > 0
            ? _tracesCollection
                .Find(Builders<PropertyTrace>.Filter.In(t => t.IdProperty, propertyIds))
                .ToListAsync(cancellationToken)
            : Task.FromResult(new List<PropertyTrace>());

        await Task.WhenAll(ownersTask, imagesTask, tracesTask);

        var owners = ownersTask.Result.ToDictionary(o => o.IdOwner, o => o);
        var imagesLookup = imagesTask.Result.GroupBy(i => i.IdProperty).ToDictionary(g => g.Key, g => g.OrderByDescending(img => img.Enabled).ToList());
        var tracesLookup = tracesTask.Result.GroupBy(t => t.IdProperty).ToDictionary(g => g.Key, g => g.OrderByDescending(trace => trace.DateSale).ToList());

        foreach (var property in properties)
        {
            if (!string.IsNullOrWhiteSpace(property.IdOwner) && owners.TryGetValue(property.IdOwner, out var owner))
            {
                property.Owner = owner;
            }

            property.Images = imagesLookup.TryGetValue(property.Id, out var propertyImages)
                ? propertyImages
                : new List<PropertyImage>();

            property.Traces = tracesLookup.TryGetValue(property.Id, out var propertyTraces)
                ? propertyTraces
                : new List<PropertyTrace>();
        }
    }
}

