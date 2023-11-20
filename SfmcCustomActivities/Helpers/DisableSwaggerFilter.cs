using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.Swagger;
using System.Reflection;
using System.Web.Http.Description;

namespace SfmcCustomActivities.Helpers
{
    public class DisableSwaggerFilter : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
    {
        //public void Apply(SwaggerDocument doc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        //{
        //    foreach (var api in apiExplorer.ApiDescriptions)
        //    {
        //        if (api.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<DisableSwaggerAttribute>().Any()
        //            || api.ActionDescriptor.GetCustomAttributes<DisableSwaggerAttribute>().Any())
        //        {
        //            var route = "/" + api.Route.RouteTemplate.TrimEnd('/');
        //            doc.paths.Remove(route);
        //        }
        //    }
        //}

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var api in context.ApiDescriptions)
            {
                var action = (ControllerActionDescriptor)api.ActionDescriptor;
                if (action.ControllerTypeInfo.GetCustomAttributes<DisableSwaggerAttribute>().Any()
                    || action.MethodInfo.GetCustomAttributes<DisableSwaggerAttribute>().Any())
                {
                    var uri = "/" + api.RelativePath.TrimEnd('/');
                    var operation = (OperationType)Enum.Parse(typeof(OperationType), api.HttpMethod, true);

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
