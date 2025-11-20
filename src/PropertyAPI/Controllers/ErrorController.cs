using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PropertyAPI.Controllers;

/// <summary>
/// Controlador para manejar errores globales
/// </summary>
/// 
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [Route("/error")]
    public IActionResult Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = context?.Error;

        if (exception != null)
        {
            _logger.LogError(exception, "Error no manejado en la aplicación");

            return Problem(
                title: "Ocurrió un error al procesar la solicitud",
                detail: exception.Message,
                statusCode: 500
            );
        }

        return Problem(
            title: "Error desconocido",
            statusCode: 500
        );
    }
}

