using System;
using Application.Abstractions.Messaging;

namespace Application.ClinicalTrialMetadata.Queries.GetClinicalTrialMetadataById;

public sealed record GetClinicalTrialMetadataByIdQuery(Guid TrialId) : IQuery<ClinicalTrialMetadataResponse>
{

}
