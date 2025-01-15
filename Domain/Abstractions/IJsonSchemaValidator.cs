using Domain.Primitives;

namespace Domain.Abstractions;
public interface IJsonSchemaValidator
    {
        JsonValidationResult Validate(string json, string schema);
    }

