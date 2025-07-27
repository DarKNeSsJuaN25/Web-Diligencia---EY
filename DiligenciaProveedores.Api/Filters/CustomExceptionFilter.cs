using DiligenciaProveedores.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiligenciaProveedores.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;
        private readonly IHostEnvironment _env;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            ProblemDetails problemDetails;
            int statusCode;
            string title;
            string detail;

            if (context.Exception is ValidationException validationException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                title = "Error de Validación";
                detail = "Uno o más errores de validación ocurrieron. Verifique los detalles para más información.";

                problemDetails = CreateValidationProblemDetails(validationException, title, detail, statusCode);
            }
            else if (context.Exception is NotFoundException notFoundException)
            {
                statusCode = (int)HttpStatusCode.NotFound;
                title = "Recurso no Encontrado";
                detail = notFoundException.Message;

                problemDetails = new ProblemDetails
                {
                    Title = title,
                    Status = statusCode,
                    Detail = detail,
                    Type = "https://tools.ietf.org/html/rfc7807#section-3.1"
                };
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                title = "Error Interno del Servidor";
                detail = "Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.";

                if (_env.IsDevelopment())
                {
                    detail += $" Detalles técnicos: {context.Exception.Message}";
                    problemDetails = new ProblemDetails
                    {
                        Title = title,
                        Status = statusCode,
                        Detail = detail,
                        Type = "about:blank",
                        Extensions = {
                            {"stackTrace", context.Exception.StackTrace},
                            {"exceptionType", context.Exception.GetType().Name}
                        }
                    };
                }
                else
                {
                    problemDetails = new ProblemDetails
                    {
                        Title = title,
                        Status = statusCode,
                        Detail = detail,
                        Type = "about:blank"
                    };
                }

                _logger.LogError(context.Exception, "Error no manejado en la API: {ErrorMessage}", context.Exception.Message);
            }

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = statusCode
            };
            context.ExceptionHandled = true;
        }

        private ProblemDetails CreateValidationProblemDetails(ValidationException ex, string title, string detail, int statusCode)
        {
            var problemDetails = new ProblemDetails
            {
                Title = title,
                Status = statusCode,
                Detail = detail,
                Type = "https://tools.ietf.org/html/rfc7807#section-3.1"
            };

            var errors = new Dictionary<string, string[]>();
            foreach (var error in ex.Errors)
            {
                errors[error.Key] = error.Value;
            }
            problemDetails.Extensions.Add("errors", errors);

            return problemDetails;
        }
    }
}
