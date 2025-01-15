using Domain.Enums;
using System;

namespace Application.ClinicalTrialMetadata.Commands.CreateClinicalTrialMetadata;

public sealed record CreateClinicalTrialMetadataRequest(string Title, DateTime StartDate, DateTime EndDate, int Participants, TrialStatus Status);