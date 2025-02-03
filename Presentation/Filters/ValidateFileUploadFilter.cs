using Application.Behaviors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Presentation.Filters;
public class ValidateFileUploadFilter : IAsyncActionFilter
{
    private static readonly string _schema;

    static ValidateFileUploadFilter()
    {
        _schema = System.IO.File.ReadAllText("Schemas/ClinicalTrialMetadataSchema.json");
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var file = context.ActionArguments["file"] as IFormFile;

        if (file == null || file.Length == 0)
        {
            context.Result = new BadRequestObjectResult("No file was uploaded.");
            return;
        }

        if (!file.ContentType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new BadRequestObjectResult("Only JSON files are allowed.");
            return;
        }

        // Validate JSON schema
        using var streamReader = new StreamReader(file.OpenReadStream());
        var jsonString = await streamReader.ReadToEndAsync();
        
        var validator = new JsonSchemaValidator();
        var validationResult = validator.Validate(jsonString, _schema);
        
        if (!validationResult.IsValid)
        {
            context.Result = new BadRequestObjectResult(validationResult.Errors);
            return;
        }

        // Reset the position of the stream for the next reader
        file.OpenReadStream().Position = 0;
        
        await next();
    }
} 