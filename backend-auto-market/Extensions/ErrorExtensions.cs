using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace backend_auto_market.Extensions;

public static class ErrorExtensions
{
    public static void UseErrorHandler(this IApplicationBuilder appBuilder)
    {
        appBuilder.UseExceptionHandler(error =>
        {
            error.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature == null)
                    return;

                context.Response.StatusCode = contextFeature.Error switch
                {
                    OperationCanceledException => (int)HttpStatusCode.ServiceUnavailable,
                    BadHttpRequestException => (int)HttpStatusCode.BadRequest,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                var errorResponse = new
                {
                    statusCode = context.Response.StatusCode,
                    message = contextFeature.Error.GetBaseException().Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            });
        });
    }
}