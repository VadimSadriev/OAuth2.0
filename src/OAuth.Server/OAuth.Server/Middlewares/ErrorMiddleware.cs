using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OAuth.Server.Extensions;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace OAuth.Server.Middlewares
{
    public class ErrorMiddleware 
    {
        private RequestDelegate _next;

        public ErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleAsync(httpContext, ex);
            }
        }

        private async Task HandleAsync(HttpContext httpContext, Exception ex)
        {
            var code = StatusCodes.Status400BadRequest;

            httpContext.Response.StatusCode = code;
            httpContext.Response.ContentType = MediaTypeNames.Application.Json;

            var error = new
            {
                ex.Message
            };

            var result = error.Serialize();

            await httpContext.Response.WriteAsync(result);
        }
    }

    public static class ErrorMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorMiddleware>();
        }
    }
}
