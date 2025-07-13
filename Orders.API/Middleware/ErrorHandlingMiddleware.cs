using Orders.Application.DTOs;
using Orders.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace Orders.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = GetStatusCodeAndMessage(exception);
            
            _logger.LogError(exception, "Erro na requisição {Method} {Path}: {Message}", 
                context.Request.Method, context.Request.Path, exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Data = null,
                Timestamp = DateTime.UtcNow
            };

            var result = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            });

            await context.Response.WriteAsync(result);
        }

        private static (int statusCode, string message) GetStatusCodeAndMessage(Exception exception)
        {
            return exception switch
            {
                InvalidStatusTransitionException => (StatusCodes.Status400BadRequest, GetUserFriendlyStatusMessage(exception)),
                InvalidOrderDataException => (StatusCodes.Status400BadRequest, exception.Message),
                OrderNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
                ArgumentException => (StatusCodes.Status400BadRequest, "Dados inválidos fornecidos"),
                InvalidOperationException => (StatusCodes.Status400BadRequest, exception.Message),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Acesso não autorizado"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
                _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
            };
        }

        private static string GetUserFriendlyStatusMessage(Exception exception)
        {
            var message = exception.Message;
            
            // Extrair informações da mensagem de erro
            if (message.Contains("Transição de status inválida:"))
            {
                var parts = message.Split("->");
                if (parts.Length == 2)
                {
                    var currentStatus = parts[0].Split(":").Last().Trim();
                    var newStatus = parts[1].Trim();
                    
                    return $"Não é possível alterar o status de '{GetStatusDescription(currentStatus)}' para '{GetStatusDescription(newStatus)}'. " +
                           $"Consulte as regras de transição de status.";
                }
            }
            
            return "Transição de status inválida. Consulte as regras de negócio.";
        }

        private static string GetStatusDescription(string status)
        {
            return status switch
            {
                "Created" => "Criado",
                "Paid" => "Pago",
                "Shipped" => "Enviado",
                "Cancelled" => "Cancelado",
                _ => status
            };
        }
    }
} 