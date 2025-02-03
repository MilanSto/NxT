using Application.ClinicalTrialMetadata.Queries.GetClinicalTrialMetadataById;
using Presentation.DTOs;

namespace Presentation.Mapper;
public class ClinicalTrialMetadataMapper : IMapper<ClinicalTrialMetadataDto, ClinicalTrialMetadataResponse>
{
    public ClinicalTrialMetadataDto Map(ClinicalTrialMetadataResponse model)
    {
        return new ClinicalTrialMetadataDto
        {
            Title = model.Title,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Participants = model.Participants,
            Status = model.Status,
            DurationInDays = model.DurationInDays
        };
    }

    public ClinicalTrialMetadataResponse Map(ClinicalTrialMetadataDto model)
    {
        return new ClinicalTrialMetadataResponse(
            model.Title,
            model.StartDate,
            model.EndDate,
            model.Participants,
            model.Status,
            model.DurationInDays
        );
    }
}

