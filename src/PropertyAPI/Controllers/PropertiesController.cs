using Microsoft.AspNetCore.Mvc;
using PropertyAPI.Application.DTOs;
using PropertyAPI.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PropertyAPI.Controllers;

/// <summary>
/// Controlador para gestionar propiedades
/// </summary>
/// 
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(IPropertyService propertyService, ILogger<PropertiesController> logger)
    {
        _propertyService = propertyService ?? throw new ArgumentNullException(nameof(propertyService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PropertyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PropertyDto>>> GetProperties(
        [FromQuery] string? name = null,
        [FromQuery(Name = "address")] string? address = null,
        [FromQuery] [Range(0, double.MaxValue, ErrorMessage = "El precio mínimo debe ser mayor o igual a 0")] decimal? minPrice = null,
        [FromQuery] [Range(0, double.MaxValue, ErrorMessage = "El precio máximo debe ser mayor o igual a 0")] decimal? maxPrice = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
            {
                return BadRequest(new { error = "El precio mínimo no puede ser mayor que el precio máximo" });
            }

            var filters = new PropertyFilterDto
            {
                Name = name,
                Address = address,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            var properties = await _propertyService.GetPropertiesAsync(filters, cancellationToken);
            return Ok(properties);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener propiedades");
            return StatusCode(500, new { error = "Ocurrió un error al procesar la solicitud" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PropertyDto>> GetPropertyById(
        [Required] string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { error = "El ID no puede estar vacío" });
            }

            var property = await _propertyService.GetPropertyByIdAsync(id, cancellationToken);

            if (property == null)
            {
                return NotFound(new { error = $"No se encontró una propiedad con el ID: {id}" });
            }

            return Ok(property);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la propiedad con ID: {PropertyId}", id);
            return StatusCode(500, new { error = "Ocurrió un error al procesar la solicitud" });
        }
    }
}

