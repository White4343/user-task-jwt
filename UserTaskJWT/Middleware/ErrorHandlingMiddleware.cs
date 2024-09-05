using FluentValidation;
using Newtonsoft.Json;

namespace UserTaskJWT.Web.Api.Middleware
{
    public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            try
            {
                await next(context).ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException e)
            {
                await CreateContext(context, e, 401).ConfigureAwait(false);
            }
            catch (BadHttpRequestException e)
            {
                await CreateContext(context, e, 400).ConfigureAwait(false);
            }
            catch (ValidationException e)
            {
                await CreateContext(context, e, 400).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await CreateContext(context, e, 500).ConfigureAwait(false);
            }
        }

        private async Task CreateContext(HttpContext context, Exception e, int statusCode)
        {
            logger.LogError(e,"{Message}", e.Message);

            var result = JsonConvert.SerializeObject(new
            {
                error = e.Message
            });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(result).ConfigureAwait(false);
        }
    }
}