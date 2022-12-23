using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using VacationRental.Core.Exceptions;

namespace VacationRental.Core.Middleware;

public class ErrorHendlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHendlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            switch (error)
            {
                case ApplicationException e:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case EntityNotFoundException e:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    // unhandled error
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(new ErrorDetails { Message = error?.Message });
            await response.WriteAsync(result);
        }
    }
}