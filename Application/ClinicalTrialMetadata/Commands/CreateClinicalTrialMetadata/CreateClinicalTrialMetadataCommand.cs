using System;
using Application.Abstractions.Messaging;
using Domain.Enums;

namespace Application.ClinicalTrialMetadata.Commands.CreateClinicalTrialMetadata;

public record CreateClinicalTrialMetadataCommand(string Title, DateTime StartDate, DateTime? EndDate, int Participants, TrialStatus Status) : ICommand<Guid>
{

}
