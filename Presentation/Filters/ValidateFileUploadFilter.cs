using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Presentation.Filters;
public class ValidateFileUploadFilter : ActionFilterAttribute
{
    private readonly IConfiguration _configuration;
    private readonly long _maxSize;
    private readonly string[] _allowedTypes;

    public ValidateFileUploadFilter(IConfiguration configuration)
    {
        _configuration = configuration;
        _maxSize = _configuration.GetValue<long>("FileUploadSettings:MaxFileSizeInBytes");
        _allowedTypes = _configuration.GetSection("FileUploadSettings:AllowedFileExtensions").Get<string[]>();
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var file = context.ActionArguments["file"] as IFormFile;
        if (file == null)
        {
            context.Result = new BadRequestObjectResult("No file uploaded.");
            return;
        }

        if (!_allowedTypes.Contains(file.ContentType))
        {
            context.Result = new BadRequestObjectResult($"File type not allowed. Allowed types: {string.Join(", ", _allowedTypes)}");
            return;
        }

        if (file.Length > _maxSize)
        {
            context.Result = new BadRequestObjectResult($"File size exceeds the limit of {_maxSize} bytes.");
            return;
        }

        base.OnActionExecuting(context);
    }
}
