namespace ProtoR.Web.Infrastructure.Swagger
{
    using System;
    using System.Linq;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class IgnoreParameterPrefixFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters != null)
            {
                foreach (OpenApiParameter parameter in operation.Parameters.Where(p => p.Name.Contains('.', StringComparison.InvariantCultureIgnoreCase)))
                {
                    parameter.Name = parameter.Name
                        .Split('.', StringSplitOptions.RemoveEmptyEntries)
                        .Last();
                }
            }
        }
    }
}
