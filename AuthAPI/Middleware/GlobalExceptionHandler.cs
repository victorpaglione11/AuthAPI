using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exceção capturada: {Message}", exception.Message);

        var domainException = exception as BaseException;

        int statusCode = domainException != null
         ? domainException.StatusCode
         : StatusCodes.Status500InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitleForStatus(statusCode),
            Type = GetTypeForStatus(statusCode),

            // 3. A Lógica da Mensagem:
            Detail = domainException != null
            ? exception.Message
            : (_env.IsDevelopment() ? exception.ToString() : "Ocorreu um erro inesperado.")
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private string GetTitleForStatus(int statusCode) => statusCode switch
    {
        401 => "Não Autorizado",
        404 => "Recurso não encontrado",
        409 => "Conflito de Regra de Negócio",
        400 => "Requisição Inválida",
        _ => "Erro Interno no Servidor"
    };

    private string GetTypeForStatus(int statusCode) =>
        $"https://httpstatuses.io/{statusCode}";
}