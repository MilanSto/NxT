using Domain.Abstractions;
using Domain.Primitives;
using NJsonSchema;
using System.Linq;

namespace Application.Behaviors
{
    public class JsonSchemaValidator : IJsonSchemaValidator
    {
        public JsonValidationResult Validate(string json, string schema)
        {
            var schemaObject = JsonSchema.FromJsonAsync(schema).Result;
            var validationErrors = schemaObject.Validate(json);

            return new JsonValidationResult
            {
                IsValid = !validationErrors.Any(),
                Errors = validationErrors.Select(e => $"{e.Path}: {e.Kind}")
            };
        }
    }
}
