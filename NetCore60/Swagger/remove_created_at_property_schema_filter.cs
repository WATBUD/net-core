
using Microsoft.OpenApi.Models;
using NetCoreSpace.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
namespace NetCoreSpace.Swagger
{

    public class RemoveCreatedAtPropertySchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            Type modelType = context.Type;
            if (modelType.FullName != null && modelType.FullName.Contains("NetCore60.Models"))
            {
                if (model.Properties.ContainsKey("createdAt"))
                {
                    model.Properties.Remove("createdAt");
                    model.Properties.Remove("updatedAt");
                }
                if (modelType.FullName.Contains("VUsersDetail"))
                {
                    model.Properties.Remove("udUserId");
                }

            }

        }
    }

}

