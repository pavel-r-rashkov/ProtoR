namespace ProtoR.Web.Infrastructure.Swagger
{
    using System.Linq;
    using System.Reflection;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class SwaggerExcludeFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context == null)
            {
                return;
            }

            var excludedProperties = context.Type
                .GetProperties()
                .Where(t => t.GetCustomAttribute<SwaggerExcludeAttribute>() != null);

            foreach (PropertyInfo excludedProperty in excludedProperties)
            {
                var excludedPropertyName = excludedProperty.Name.ToCamelCase();

                if (schema.Properties.ContainsKey(excludedPropertyName))
                {
                    schema.Properties.Remove(excludedPropertyName);
                }
            }
        }
    }
}
