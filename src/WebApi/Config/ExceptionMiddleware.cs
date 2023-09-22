using System;
using Newtonsoft.Json;
using System.Net;
using Core.Entities;
namespace WebApi.Config
{

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ExceptionMiddleware>();

        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Oooops! Algo salió mal: {ex.Message}");
                await HandleGlobalExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleGlobalExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(new ErrorDetails()
            {
                StatusCode = StatusCodes.Status406NotAcceptable, //Custom error
                Message = $"Algo salió mal. Error!: {exception.Message}",
                StackTrace = exception.StackTrace
            }));
        }
    }


}

