using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using API.DTO;

namespace API
{
     public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var formParameters = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile) || p.ParameterType == typeof(AddProductDto))
                .ToList();

            if (formParameters.Any())
            {
                // Create a request body that includes both file and other form parameters
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = formParameters.ToDictionary(p => p.Name, p => new OpenApiSchema
                                {
                                    Type = p.ParameterType == typeof(IFormFile) ? "string" : "string", 
                                    Format = p.ParameterType == typeof(IFormFile) ? "binary" : null
                                }),
                                Required = formParameters.Where(p => p.ParameterType != typeof(IFormFile)).Select(p => p.Name).ToHashSet()
                            }
                        }
                    }
                };
            }
        }
    }

}