using Application.ClinicalTrialMetadata.Commands.CreateClinicalTrialMetadata;
using Application.ClinicalTrialMetadata.Queries.GetClinicalTrialMetadataById;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Presentation.DTOs;
using Presentation.Filters;
using Presentation.Mapper;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.Controllers;

/// <summary>
/// Represents the clinicalTrialMetadatas controller.
/// </summary>
public sealed class ClinicalTrialMetadataController(
    IMapper<ClinicalTrialMetadataDto, ClinicalTrialMetadataResponse> mapper)
    : ApiController
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
    public async Task<IActionResult> GetClinicalTrialMetadata(Guid trialId, CancellationToken cancellationToken)
    {
        var query = new GetClinicalTrialMetadataByIdQuery(trialId);

        var response = await Sender.Send(query, cancellationToken);
        var dto = mapper.Map(response);

        return Ok(dto);
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

            var command = JsonConvert.DeserializeObject<CreateClinicalTrialMetadataCommand>(jsonString);
            if (command == null)
            {
                return BadRequest("Could not deserialize the file into the command.");
            }

            var result = await Sender.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}



