using Application.ClinicalTrialMetadata.Commands.CreateClinicalTrialMetadata;
using Application.ClinicalTrialMetadata.Queries.GetClinicalTrialMetadataById;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Presentation.Filters;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.Controllers;

/// <summary>
/// Represents the clinicalTrialMetadatas controller.
/// </summary>
public sealed class ClinicalTrialMetadataController : ApiController
{
    /// <summary>
    /// Gets the metadata with the specified identifier, if it exists.
    /// </summary>
    /// <param name="trialId">The metadata identifier.</param>
    /// <param name="status"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The metadata with the specified identifier, if it exists.</returns>
    [HttpGet("{trialId:guid}")]
    [ProducesResponseType(typeof(ClinicalTrialMetadataResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClinicalTrialMetadata(Guid trialId, [FromQuery] string status, CancellationToken cancellationToken)
    {
        var query = new GetClinicalTrialMetadataByIdQuery(trialId, status);

        var metadata = await Sender.Send(query, cancellationToken);

        return Ok(metadata);
    }

    /// <summary>
    /// Creates a new metadata based on the specified request.
    /// </summary>
    /// <param name="file">The create metadata request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The identifier of the newly created metadata.</returns>
    [HttpPost("upload")]
    [ServiceFilter(typeof(ValidateFileUploadFilter))]
    public async Task<IActionResult> UploadJsonFile(IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            using var streamReader = new StreamReader(file.OpenReadStream());
            var jsonString = await streamReader.ReadToEndAsync(cancellationToken);

            var schema = await System.IO.File.ReadAllTextAsync("Schemas/ClinicalTrialMetadataSchema.json", cancellationToken);

            var validationResult = JsonSchemaValidator.Validate(jsonString, schema);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var command = JsonConvert.DeserializeObject<CreateClinicalTrialMetadataCommand>(jsonString);
            if (command == null)
            {
                return BadRequest("Could not deserialize the file into the command.");
            }

            var result = await Sender.Send(command, cancellationToken);

            return Ok(result);
        }
        catch (JsonException ex)
        {
            return BadRequest($"JSON deserialization error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}



