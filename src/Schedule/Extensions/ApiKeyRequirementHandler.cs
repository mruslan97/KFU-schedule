using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Schedule.Extensions
{
    public class ApiKeyRequirementHandler : AuthorizationHandler<ApiKeyRequirement>
    {
        private const string ApiKeyHeaderName = "X-API-KEY";

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
        {
            await SucceedRequirementIfApiKeyPresentAndValid(context, requirement);
        }

        private async Task SucceedRequirementIfApiKeyPresentAndValid(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext authorizationFilterContext)
            {
                var apiKey = authorizationFilterContext.HttpContext.Request.Headers[ApiKeyHeaderName].FirstOrDefault();
                if (apiKey != null && requirement.ApiKeys.Any(requiredApiKey => apiKey == requiredApiKey))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    authorizationFilterContext.HttpContext.Response.StatusCode = 403;
                    await authorizationFilterContext.HttpContext.Response.WriteAsync("Invalid api key", Encoding.UTF8);
                }
            }
        }
    }
}