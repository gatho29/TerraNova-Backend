using MongoDB.Driver;
using PropertyAPI.Domain.Entities;
using PropertyAPI.Infrastructure.Data;
using System.Linq;

namespace PropertyAPI.Scripts;

/// <summary>
/// Script para poblar la base de datos con la estructura solicitada:
/// Owner, Property, PropertyImage y PropertyTrace
/// </summary>
public class SeedData
{
    private readonly IMongoCollection<Property> _propertiesCollection;
    private readonly IMongoCollection<Owner> _ownersCollection;
    private readonly IMongoCollection<PropertyImage> _imagesCollection;
    private readonly IMongoCollection<PropertyTrace> _tracesCollection;

    public SeedData(MongoDbContext context)
    {
        _propertiesCollection = context.GetCollection<Property>("Properties");
        _ownersCollection = context.GetCollection<Owner>("Owners");
        _imagesCollection = context.GetCollection<PropertyImage>("PropertyImages");
        _tracesCollection = context.GetCollection<PropertyTrace>("PropertyTraces");
    }

    public async Task SeedAsync()
    {
        var ownersCountTask = _ownersCollection.CountDocumentsAsync(FilterDefinition<Owner>.Empty);
        var propertiesCountTask = _propertiesCollection.CountDocumentsAsync(FilterDefinition<Property>.Empty);
        var imagesCountTask = _imagesCollection.CountDocumentsAsync(FilterDefinition<PropertyImage>.Empty);
        var tracesCountTask = _tracesCollection.CountDocumentsAsync(FilterDefinition<PropertyTrace>.Empty);

        await Task.WhenAll(ownersCountTask, propertiesCountTask, imagesCountTask, tracesCountTask);

        var hasAnyData = ownersCountTask.Result > 0
            || propertiesCountTask.Result > 0
            || imagesCountTask.Result > 0
            || tracesCountTask.Result > 0;

        if (hasAnyData)
        {
            Console.WriteLine("La base de datos ya contiene datos. Omitiendo seed.");
            return;
        }

        await InsertAllAsync();
    }

    public async Task RepopulateAsync()
    {
        await _ownersCollection.DeleteManyAsync(FilterDefinition<Owner>.Empty);
        await _propertiesCollection.DeleteManyAsync(FilterDefinition<Property>.Empty);
        await _imagesCollection.DeleteManyAsync(FilterDefinition<PropertyImage>.Empty);
        await _tracesCollection.DeleteManyAsync(FilterDefinition<PropertyTrace>.Empty);
        Console.WriteLine("Datos existentes eliminados. Repoblando la base de datos...");

        await InsertAllAsync();
    }

    public async Task AddMoreDataAsync(int propertyCount = 100)
    {
        Console.WriteLine($"Agregando {propertyCount} propiedades adicionales...");
        
        var existingOwners = await _ownersCollection.Find(FilterDefinition<Owner>.Empty).ToListAsync();
        var existingOwnerIds = existingOwners.Select(o => o.IdOwner).ToHashSet();
        
        var existingProperties = await _propertiesCollection.Find(FilterDefinition<Property>.Empty).ToListAsync();
        var existingCodes = existingProperties.Select(p => p.CodeInternal).ToHashSet();
        
        // Calcular el número máximo por ciudad
        var maxNumbersByCity = new Dictionary<string, int>();
        foreach (var prop in existingProperties)
        {
            var parts = prop.CodeInternal.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[1], out var number))
            {
                var cityCode = parts[0];
                if (!maxNumbersByCity.ContainsKey(cityCode) || maxNumbersByCity[cityCode] < number)
                {
                    maxNumbersByCity[cityCode] = number;
                }
            }
        }
        
        var newOwners = BuildAdditionalOwners(existingOwnerIds);
        if (newOwners.Any())
        {
            await _ownersCollection.InsertManyAsync(newOwners);
            Console.WriteLine($"Se agregaron {newOwners.Count} propietarios nuevos.");
        }

        var allOwners = existingOwners.Concat(newOwners).ToList();
        var properties = BuildAdditionalProperties(allOwners, propertyCount, existingCodes, maxNumbersByCity);
        await _propertiesCollection.InsertManyAsync(properties);

        var images = BuildPropertyImages(properties);
        if (images.Any())
        {
            await _imagesCollection.InsertManyAsync(images);
        }

        var traces = BuildPropertyTraces(properties);
        if (traces.Any())
        {
            await _tracesCollection.InsertManyAsync(traces);
        }

        Console.WriteLine($"Se agregaron {properties.Count} propiedades, {images.Count} imágenes y {traces.Count} trazas.");
    }

    private async Task InsertAllAsync()
    {
        var owners = BuildOwners();
        await _ownersCollection.InsertManyAsync(owners);

        var properties = BuildProperties();
        await _propertiesCollection.InsertManyAsync(properties);

        var images = BuildPropertyImages(properties);
        if (images.Any())
        {
            await _imagesCollection.InsertManyAsync(images);
        }

        var traces = BuildPropertyTraces(properties);
        if (traces.Any())
        {
            await _tracesCollection.InsertManyAsync(traces);
        }

        Console.WriteLine($"Se insertaron {owners.Count} propietarios, {properties.Count} propiedades, {images.Count} imágenes y {traces.Count} trazas.");
    }

    private static List<Owner> BuildOwners() => new()
    {
        new Owner
        {
            IdOwner = "owner-001",
            Name = "Laura Martínez",
            Address = "Carrera 13 #54-80, Bogotá, Colombia",
            Photo = "https://randomuser.me/api/portraits/women/45.jpg",
            Birthday = new DateTime(1982, 3, 5)
        },
        new Owner
        {
            IdOwner = "owner-002",
            Name = "Alberto Restrepo",
            Address = "Avenida El Poblado 20-45, Medellín, Colombia",
            Photo = "https://randomuser.me/api/portraits/men/32.jpg",
            Birthday = new DateTime(1975, 11, 20)
        },
        new Owner
        {
            IdOwner = "owner-003",
            Name = "Carmen Ospina",
            Address = "Kilómetro 3 vía al Mar, Cali, Colombia",
            Photo = "https://randomuser.me/api/portraits/women/15.jpg",
            Birthday = new DateTime(1968, 7, 14)
        },
        new Owner
        {
            IdOwner = "owner-004",
            Name = "Javier Ruiz",
            Address = "Calle 72 #45-30, Barranquilla, Colombia",
            Photo = "https://randomuser.me/api/portraits/men/14.jpg",
            Birthday = new DateTime(1987, 1, 30)
        },
        new Owner
        {
            IdOwner = "owner-005",
            Name = "María Fernanda Díaz",
            Address = "Calle 9 #1-28, Cartagena, Colombia",
            Photo = "https://randomuser.me/api/portraits/women/55.jpg",
            Birthday = new DateTime(1990, 9, 9)
        },
        new Owner
        {
            IdOwner = "owner-006",
            Name = "Ricardo Páez",
            Address = "Carrera 33 #51-19, Bucaramanga, Colombia",
            Photo = "https://randomuser.me/api/portraits/men/59.jpg",
            Birthday = new DateTime(1972, 6, 2)
        },
        new Owner
        {
            IdOwner = "owner-007",
            Name = "Elena Dueñas",
            Address = "Carrera 10 #6-15, Santa Marta, Colombia",
            Photo = "https://randomuser.me/api/portraits/women/67.jpg",
            Birthday = new DateTime(1985, 12, 25)
        },
        new Owner
        {
            IdOwner = "owner-008",
            Name = "Bruno Cárdenas",
            Address = "Calle 50 #70-32, Pereira, Colombia",
            Photo = "https://randomuser.me/api/portraits/men/77.jpg",
            Birthday = new DateTime(1978, 4, 3)
        },
        new Owner
        {
            IdOwner = "owner-009",
            Name = "Paula Herrera",
            Address = "Carrera 7 #61-21, Bogotá, Colombia",
            Photo = "https://randomuser.me/api/portraits/women/21.jpg",
            Birthday = new DateTime(1993, 8, 17)
        }
    };

    private static List<Property> BuildProperties()
    {
        var now = DateTime.UtcNow;

        var propertyMadrid = new Property
        {
            Id = Guid.NewGuid().ToString(),
            IdOwner = "owner-001",
            Name = "Casa moderna en Chapinero",
            Address = "Carrera 7 #56-12, Bogotá, Colombia",
            Price = 920000000m,
            CodeInternal = "BOG-001",
            Year = 2018,
            CreatedAt = now,
            UpdatedAt = now
        };

        var propertyBarcelona = new Property
        {
            Id = Guid.NewGuid().ToString(),
            IdOwner = "owner-002",
            Name = "Apartamento con vista a Medellín",
            Address = "Loma de los Balsos 23-45, Medellín, Colombia",
            Price = 650000000m,
            CodeInternal = "MED-001",
            Year = 2020,
            CreatedAt = now,
            UpdatedAt = now
        };

        var propertyValencia = new Property
        {
            Id = Guid.NewGuid().ToString(),
            IdOwner = "owner-003",
            Name = "Casa campestre en Pance",
            Address = "Kilómetro 18 vía Cali-Buenaventura, Colombia",
            Price = 780000000m,
            CodeInternal = "CLO-001",
            Year = 2016,
            CreatedAt = now,
            UpdatedAt = now
        };

        var propertySegovia = new Property
        {
            Id = Guid.NewGuid().ToString(),
            IdOwner = "owner-004",
            Name = "Penthouse sobre el Malecón",
            Address = "Carrera 46 #80-23, Barranquilla, Colombia",
            Price = 560000000m,
            CodeInternal = "BAQ-001",
            Year = 2019,
            CreatedAt = now,
            UpdatedAt = now
        };

        var propertyLoftMadrid = new Property
        {
            Id = Guid.NewGuid().ToString(),
            IdOwner = "owner-005",
            Name = "Casablanca frente al mar",
            Address = "Carrera 2 #33-26, Cartagena, Colombia",
            Price = 1180000000m,
            CodeInternal = "CTG-001",
            Year = 2015,
            CreatedAt = now,
            UpdatedAt = now
        };

        var propertyChaletPozuelo = new Property
        {
            Id = Guid.NewGuid().ToString(),
            IdOwner = "owner-006",
            Name = "Casa familiar en Cabecera",
            Address = "Carrera 36 #51-28, Bucaramanga, Colombia",
            Price = 430000000m,
            CodeInternal = "BGA-001",
            Year = 2014,
            CreatedAt = now,
            UpdatedAt = now
        };

        var propertyAticoRetiro = new Property
        {
            Id = Guid.NewGuid().ToString(),
            IdOwner = "owner-007",
            Name = "Villa ecológica en Minca",
            Address = "Vereda El Campano, Santa Marta, Colombia",
            Price = 390000000m,
            CodeInternal = "STM-001",
            Year = 2021,
            CreatedAt = now,
            UpdatedAt = now
        };

        var propertyBarceloneta = new Property
        {
            Id = Guid.NewGuid().ToString(),
            IdOwner = "owner-008",
            Name = "Condominio en Pinares",
            Address = "Carrera 18 #12-50, Pereira, Colombia",
            Price = 480000000m,
            CodeInternal = "PEI-001",
            Year = 2017,
            CreatedAt = now,
            UpdatedAt = now
        };

        var propertyPoblenou = new Property
        {
            Id = Guid.NewGuid().ToString(),
            IdOwner = "owner-009",
            Name = "Apartamento inteligente en Rosales",
            Address = "Carrera 4 #71-25, Bogotá, Colombia",
            Price = 980000000m,
            CodeInternal = "BOG-002",
            Year = 2022,
            CreatedAt = now,
            UpdatedAt = now
        };

        return new List<Property>
        {
            propertyMadrid,
            propertyBarcelona,
            propertyValencia,
            propertySegovia,
            propertyLoftMadrid,
            propertyChaletPozuelo,
            propertyAticoRetiro,
            propertyBarceloneta,
            propertyPoblenou
        };
    }

    private static List<PropertyImage> BuildPropertyImages(IEnumerable<Property> properties)
    {
        var images = new List<PropertyImage>();
        var random = new Random();

        var imageUrls = new[]
        {
            "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800",
            "https://images.unsplash.com/photo-1505691938895-1758d7feb511?w=800",
            "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?w=800",
            "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800",
            "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?w=800",
            "https://images.unsplash.com/photo-1570129477492-45c003edd2be?w=800",
            "https://images.unsplash.com/photo-1554995207-c18c203602cb?w=800",
            "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?w=800",
            "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?w=800",
            "https://images.unsplash.com/photo-1600585154526-990dced4db0d?w=800",
            "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?w=800",
            "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?w=800",
            "https://images.unsplash.com/photo-1600607687644-c7171b42498b?w=800",
            "https://images.unsplash.com/photo-1600566753190-17f0baa2a6c3?w=800",
            "https://images.unsplash.com/photo-1600047509807-ba8f99d2cdde?w=800",
            "https://images.unsplash.com/photo-1600047509358-9dc75507daeb?w=800",
            "https://images.unsplash.com/photo-1600566753086-00f18fb6b3ea?w=800",
            "https://images.unsplash.com/photo-1600607687920-4e2a09cf159d?w=800",
            "https://images.unsplash.com/photo-1600585154084-4e5fe7c39198?w=800",
            "https://images.unsplash.com/photo-1600566753151-384129cf4e3e?w=800"
        };

        void AddImages(Property property, params string[] urls)
        {
            foreach (var url in urls)
            {
                images.Add(new PropertyImage
                {
                    IdPropertyImage = Guid.NewGuid().ToString(),
                    IdProperty = property.Id,
                    File = url,
                    Enabled = true
                });
            }
        }

        var propertyList = properties.ToList();
        var propertyDict = propertyList.ToDictionary(p => p.CodeInternal, p => p);

        // Mantener las imágenes específicas para las propiedades originales si existen
        if (propertyDict.ContainsKey("BOG-001"))
        {
            AddImages(propertyDict["BOG-001"],
                "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800",
                "https://images.unsplash.com/photo-1505691938895-1758d7feb511?w=800");
        }

        if (propertyDict.ContainsKey("MED-001"))
        {
            AddImages(propertyDict["MED-001"],
                "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?w=800",
                "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800");
        }

        if (propertyDict.ContainsKey("CLO-001"))
        {
            AddImages(propertyDict["CLO-001"], "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?w=800");
        }

        if (propertyDict.ContainsKey("BAQ-001"))
        {
            AddImages(propertyDict["BAQ-001"], "https://images.unsplash.com/photo-1570129477492-45c003edd2be?w=800");
        }

        if (propertyDict.ContainsKey("CTG-001"))
        {
            AddImages(propertyDict["CTG-001"], "https://images.unsplash.com/photo-1554995207-c18c203602cb?w=800");
        }

        if (propertyDict.ContainsKey("BGA-001"))
        {
            AddImages(propertyDict["BGA-001"], "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?w=800");
        }

        if (propertyDict.ContainsKey("STM-001"))
        {
            AddImages(propertyDict["STM-001"], "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?w=800");
        }

        if (propertyDict.ContainsKey("PEI-001"))
        {
            AddImages(propertyDict["PEI-001"], "https://images.unsplash.com/photo-1600585154526-990dced4db0d?w=800");
        }

        if (propertyDict.ContainsKey("BOG-002"))
        {
            AddImages(propertyDict["BOG-002"], "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?w=800");
        }

        // Agregar imágenes aleatorias para todas las demás propiedades (1-3 imágenes por propiedad)
        var processedCodes = new HashSet<string> { "BOG-001", "MED-001", "CLO-001", "BAQ-001", "CTG-001", "BGA-001", "STM-001", "PEI-001", "BOG-002" };
        
        foreach (var property in propertyList)
        {
            if (!processedCodes.Contains(property.CodeInternal))
            {
                var imageCount = random.Next(1, 4); // 1 a 3 imágenes
                var selectedUrls = imageUrls.OrderBy(x => random.Next()).Take(imageCount).ToArray();
                AddImages(property, selectedUrls);
            }
        }

        return images;
    }

    private static List<PropertyTrace> BuildPropertyTraces(IEnumerable<Property> properties)
    {
        var now = DateTime.UtcNow;
        var traces = new List<PropertyTrace>();

        void AddTrace(Property property, string buyerName, decimal value, decimal tax, int yearsAgo)
        {
            traces.Add(new PropertyTrace
            {
                IdPropertyTrace = Guid.NewGuid().ToString(),
                IdProperty = property.Id,
                Name = buyerName,
                Value = value,
                Tax = tax,
                DateSale = now.AddYears(-yearsAgo)
            });
        }

        foreach (var property in properties)
        {
            AddTrace(property, "Registro de compra inicial", property.Price, property.Price * 0.08m, 3);
            AddTrace(property, "Actualización fiscal", property.Price * 1.05m, property.Price * 0.085m, 1);
        }

        return traces;
    }

    private static List<Owner> BuildAdditionalOwners(HashSet<string> existingOwnerIds)
    {
        var owners = new List<Owner>();
        var random = new Random();
        var firstNames = new[] { "Carlos", "Ana", "Luis", "María", "Juan", "Sofia", "Diego", "Valentina", "Andrés", "Camila", 
            "Felipe", "Isabella", "Sebastián", "Laura", "Nicolás", "Daniela", "Javier", "Andrea", "Miguel", "Paola" };
        var lastNames = new[] { "García", "Rodríguez", "González", "Fernández", "López", "Martínez", "Sánchez", "Pérez", 
            "Gómez", "Martín", "Jiménez", "Ruiz", "Hernández", "Díaz", "Moreno", "Álvarez", "Muñoz", "Romero", "Alonso", "Gutiérrez" };
        var cities = new[] { "Bogotá", "Medellín", "Cali", "Barranquilla", "Cartagena", "Bucaramanga", "Santa Marta", "Pereira", 
            "Manizales", "Armenia", "Ibagué", "Villavicencio", "Pasto", "Neiva", "Valledupar", "Montería", "Sincelejo", "Popayán", "Tunja", "Riohacha" };
        var streets = new[] { "Carrera", "Calle", "Avenida", "Diagonal", "Transversal" };

        int ownerNumber = existingOwnerIds.Count + 1;
        for (int i = 0; i < 50; i++) // Crear 50 owners nuevos para las 100 propiedades
        {
            var ownerId = $"owner-{ownerNumber:D3}";
            while (existingOwnerIds.Contains(ownerId))
            {
                ownerNumber++;
                ownerId = $"owner-{ownerNumber:D3}";
            }
            existingOwnerIds.Add(ownerId);

            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var city = cities[random.Next(cities.Length)];
            var street = streets[random.Next(streets.Length)];
            var streetNumber = random.Next(1, 200);
            var houseNumber = random.Next(1, 200);

            owners.Add(new Owner
            {
                IdOwner = ownerId,
                Name = $"{firstName} {lastName}",
                Address = $"{street} {streetNumber} #{houseNumber}-{random.Next(10, 99)}, {city}, Colombia",
                Photo = $"https://randomuser.me/api/portraits/{(random.Next(2) == 0 ? "men" : "women")}/{random.Next(1, 99)}.jpg",
                Birthday = new DateTime(1960 + random.Next(40), random.Next(1, 13), random.Next(1, 29))
            });
            ownerNumber++;
        }

        return owners;
    }

    private static List<Property> BuildAdditionalProperties(List<Owner> owners, int count, HashSet<string> existingCodes, Dictionary<string, int> maxNumbersByCity)
    {
        var properties = new List<Property>();
        var random = new Random();
        var now = DateTime.UtcNow;
        
        var propertyTypes = new[] { "Casa", "Apartamento", "Villa", "Penthouse", "Chalet", "Loft", "Estudio", "Dúplex", "Casa campestre", "Condominio" };
        var neighborhoods = new Dictionary<string, string[]>
        {
            { "Bogotá", new[] { "Chapinero", "Rosales", "Usaquén", "La Candelaria", "Teusaquillo", "Zona T", "Parque 93", "Cedritos", "Santa Fe", "Suba" } },
            { "Medellín", new[] { "El Poblado", "Laureles", "Envigado", "Sabaneta", "Belén", "La América", "Robledo", "Bello", "Itagüí", "San Javier" } },
            { "Cali", new[] { "Granada", "San Antonio", "El Peñón", "Ciudad Jardín", "Versalles", "Mengua", "Pance", "La Flora", "San Fernando", "Nueva Granada" } },
            { "Barranquilla", new[] { "El Prado", "Bella Vista", "Villa Santos", "Riomar", "Alto Prado", "Centro", "Las Flores", "El Country", "San José", "Villa Campestre" } },
            { "Cartagena", new[] { "Bocagrande", "El Laguito", "Castillo Grande", "Manga", "Getsemaní", "Centro Histórico", "Marbella", "La Boquilla", "Crespo", "Zona Norte" } },
            { "Bucaramanga", new[] { "Cabecera", "Girón", "Floridablanca", "Piedecuesta", "Centro", "San Francisco", "La Ciudadela", "Mutis", "La Rosita", "Alamos" } },
            { "Santa Marta", new[] { "El Rodadero", "Taganga", "Centro", "Bastidas", "Mamatoco", "Gaira", "Bello Horizonte", "Pozos Colorados", "Minca", "La Quinta" } },
            { "Pereira", new[] { "Pinares", "Centro", "Villavicencio", "El Oso", "La Julita", "Alfonso López", "San Joaquín", "Villa Olímpica", "El Lago", "La Villa" } },
            { "Manizales", new[] { "Palogrande", "La Enea", "Centro", "Chipre", "La Francia", "San José", "Villamaría", "La Estrella", "El Cable", "Los Rosales" } },
            { "Armenia", new[] { "Centro", "La Tebaida", "Circasia", "Montenegro", "Quimbaya", "Calarcá", "Barcelona", "La Patria", "El Edén", "San Rafael" } }
        };

        var cities = neighborhoods.Keys.ToArray();
        var cityCodes = new Dictionary<string, string>
        {
            { "Bogotá", "BOG" }, { "Medellín", "MED" }, { "Cali", "CLO" }, { "Barranquilla", "BAQ" }, { "Cartagena", "CTG" },
            { "Bucaramanga", "BGA" }, { "Santa Marta", "STM" }, { "Pereira", "PEI" }, { "Manizales", "MZL" }, { "Armenia", "ARM" },
            { "Ibagué", "IBG" }, { "Villavicencio", "VVC" }, { "Pasto", "PSO" }, { "Neiva", "NVA" }, { "Valledupar", "VUP" },
            { "Montería", "MTR" }, { "Sincelejo", "SNC" }, { "Popayán", "PPN" }, { "Tunja", "TUN" }, { "Riohacha", "RHC" }
        };

        var propertyCountByCity = new Dictionary<string, int>();

        for (int i = 0; i < count; i++)
        {
            var city = cities[random.Next(cities.Length)];
            var cityCode = cityCodes.ContainsKey(city) ? cityCodes[city] : city.Substring(0, 3).ToUpper();
            
            // Obtener el número máximo existente para esta ciudad o empezar desde 1
            var startNumber = maxNumbersByCity.ContainsKey(cityCode) ? maxNumbersByCity[cityCode] + 1 : 1;
            
            if (!propertyCountByCity.ContainsKey(cityCode))
            {
                propertyCountByCity[cityCode] = startNumber;
            }
            else
            {
                propertyCountByCity[cityCode]++;
            }

            var codeInternal = $"{cityCode}-{propertyCountByCity[cityCode]:D3}";
            
            // Verificar que no exista (por si acaso)
            int attempt = 0;
            while (existingCodes.Contains(codeInternal) && attempt < 1000)
            {
                propertyCountByCity[cityCode]++;
                codeInternal = $"{cityCode}-{propertyCountByCity[cityCode]:D3}";
                attempt++;
            }
            
            existingCodes.Add(codeInternal);
            
            // Actualizar el máximo para esta ciudad
            if (!maxNumbersByCity.ContainsKey(cityCode) || maxNumbersByCity[cityCode] < propertyCountByCity[cityCode])
            {
                maxNumbersByCity[cityCode] = propertyCountByCity[cityCode];
            }

            var neighborhood = neighborhoods.ContainsKey(city) 
                ? neighborhoods[city][random.Next(neighborhoods[city].Length)] 
                : "Centro";

            var propertyType = propertyTypes[random.Next(propertyTypes.Length)];
            var owner = owners[random.Next(owners.Count)];

            var basePrice = random.Next(200000000, 1500000000);
            var year = random.Next(2010, 2024);
            var streetNumber = random.Next(1, 200);
            var houseNumber = random.Next(1, 200);
            var streetType = random.Next(3) == 0 ? "Carrera" : random.Next(3) == 1 ? "Calle" : "Avenida";

            properties.Add(new Property
            {
                Id = Guid.NewGuid().ToString(),
                IdOwner = owner.IdOwner,
                Name = $"{propertyType} en {neighborhood}",
                Address = $"{streetType} {streetNumber} #{houseNumber}-{random.Next(10, 99)}, {city}, Colombia",
                Price = basePrice,
                CodeInternal = codeInternal,
                Year = year,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        return properties;
    }
}

