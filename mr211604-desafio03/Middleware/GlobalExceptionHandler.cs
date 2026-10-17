using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pedidos.BL.Exceptions;

namespace mr211604_desafio03.Middleware
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, titulo) = exception switch
            {
                EntidadNoEncontradaException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                ReglaDeNegocioException => (StatusCodes.Status409Conflict, "Regla de negocio incumplida"),
                ValidacionException => (StatusCodes.Status400BadRequest, "Datos no válidos"),
                _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
            };

            if (status == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(exception, "Error no controlado al procesar {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
            }

            var problema = new ProblemDetails
            {
                Status = status,
                Title = titulo,
                Detail = status == StatusCodes.Status500InternalServerError ? "Ocurrió un error inesperado." : exception.Message,
                Instance = httpContext.Request.Path
            };

            if (exception is ValidacionException validacion)
            {
                problema.Extensions["errors"] = validacion.Errores;
            }

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(problema, cancellationToken);
            return true;
        }
    }
}
