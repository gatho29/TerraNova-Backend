using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PropertyAPI.Domain.Entities;
using PropertyAPI.Infrastructure.Data;
using PropertyAPI.Scripts;

namespace PropertyAPI.Controllers;

/// <summary>
/// Controlador para poblar la base de datos con datos de ejemplo (solo desarrollo)
/// </summary>

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SeedController : ControllerBase
{
    private readonly MongoDbContext _context;
    private readonly ILogger<SeedController> _logger;

    public SeedController(MongoDbContext context, ILogger<SeedController> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SeedDatabase([FromQuery] bool repopulate = false)
    {
        try
        {
            var propertiesCollection = _context.GetCollection<Property>("Properties");
            var imagesCollection = _context.GetCollection<PropertyImage>("PropertyImages");
            var tracesCollection = _context.GetCollection<PropertyTrace>("PropertyTraces");
            var ownersCollection = _context.GetCollection<Owner>("Owners");
            var seedData = new SeedData(_context);
            
            if (repopulate)
            {
                await seedData.RepopulateAsync();
                var summary = await GetSummaryAsync(propertiesCollection, ownersCollection, imagesCollection, tracesCollection);
                return Ok(new { message = "Base de datos repoblada exitosamente con datos de ejemplo", summary });
            }
            else
            {
                await seedData.SeedAsync();
                var summary = await GetSummaryAsync(propertiesCollection, ownersCollection, imagesCollection, tracesCollection);
                return Ok(new { message = "Base de datos poblada exitosamente con datos de ejemplo", summary });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al poblar la base de datos");
            return StatusCode(500, new { error = "Ocurrió un error al poblar la base de datos", details = ex.Message });
        }
    }

    [HttpPost("add-more")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddMoreData([FromQuery] int count = 100)
    {
        try
        {
            var propertiesCollection = _context.GetCollection<Property>("Properties");
            var imagesCollection = _context.GetCollection<PropertyImage>("PropertyImages");
            var tracesCollection = _context.GetCollection<PropertyTrace>("PropertyTraces");
            var ownersCollection = _context.GetCollection<Owner>("Owners");
            var seedData = new SeedData(_context);
            
            await seedData.AddMoreDataAsync(count);
            var summary = await GetSummaryAsync(propertiesCollection, ownersCollection, imagesCollection, tracesCollection);
            return Ok(new { message = $"Se agregaron {count} propiedades adicionales exitosamente", summary });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar más datos a la base de datos");
            return StatusCode(500, new { error = "Ocurrió un error al agregar más datos", details = ex.Message });
        }
    }

    private static async Task<object> GetSummaryAsync(
        IMongoCollection<Property> propertiesCollection,
        IMongoCollection<Owner> ownersCollection,
        IMongoCollection<PropertyImage> imagesCollection,
        IMongoCollection<PropertyTrace> tracesCollection)
    {
        var propertiesCountTask = propertiesCollection.CountDocumentsAsync(FilterDefinition<Property>.Empty);
        var ownersCountTask = ownersCollection.CountDocumentsAsync(FilterDefinition<Owner>.Empty);
        var imagesCountTask = imagesCollection.CountDocumentsAsync(FilterDefinition<PropertyImage>.Empty);
        var tracesCountTask = tracesCollection.CountDocumentsAsync(FilterDefinition<PropertyTrace>.Empty);

        await Task.WhenAll(propertiesCountTask, ownersCountTask, imagesCountTask, tracesCountTask);

        return new
        {
            properties = propertiesCountTask.Result,
            owners = ownersCountTask.Result,
            images = imagesCountTask.Result,
            traces = tracesCountTask.Result
        };
    }
}

