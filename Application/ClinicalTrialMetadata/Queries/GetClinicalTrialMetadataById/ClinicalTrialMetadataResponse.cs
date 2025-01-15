using Domain.Enums;
using System;

namespace Application.ClinicalTrialMetadata.Queries.GetClinicalTrialMetadataById;

public sealed record ClinicalTrialMetadataResponse(string Title, DateTime StartDate, DateTime? EndDate, int Participants, TrialStatus Status, int DurationInDays);