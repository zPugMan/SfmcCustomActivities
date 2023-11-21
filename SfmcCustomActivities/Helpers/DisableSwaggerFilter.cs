using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace SfmcCustomActivities.Helpers
{
    public class DisableSwaggerFilter : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
    {

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var api in context.ApiDescriptions)
            {
                var action = (ControllerActionDescriptor)api.ActionDescriptor;
                if (action.ControllerTypeInfo.GetCustomAttributes<DisableSwaggerAttribute>().Any()
                    || action.MethodInfo.GetCustomAttributes<DisableSwaggerAttribute>().Any())
                {
                    var uri = "/" + (string.IsNullOrEmpty(api.RelativePath) ? "" : api.RelativePath.TrimEnd('/'));

                    var httpMethod = api.HttpMethod;
                    if (string.IsNullOrEmpty(httpMethod))
                    {
                        return;
                    }
                
                    var operation = (OperationType)Enum.Parse(typeof(OperationType), httpMethod, true);

                    swaggerDoc.Paths[uri].Operations.Remove(operation);

                    if (!swaggerDoc.Paths[uri].Operations.Any())
                    {
                        swaggerDoc.Paths.Remove(uri);
                    }
                }
                
            }
        }
    }
}
