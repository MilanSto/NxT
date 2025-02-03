using Application.Behaviors;
using Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Controllers;

/// <summary>
/// Represents the base API controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
    private ISender _sender;
    private IJsonSchemaValidator _jsonSchemaValidator;

    /// <summary>
    /// Gets the sender.
    /// </summary>
    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetService<ISender>();//DI inject trough constructor

    protected IJsonSchemaValidator JsonSchemaValidator => _jsonSchemaValidator ??= new JsonSchemaValidator();//DI inject trough constructor
}