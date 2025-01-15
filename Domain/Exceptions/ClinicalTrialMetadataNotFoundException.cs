using System;
using Domain.Exceptions.Base;

namespace Domain.Exceptions;

public sealed class ClinicalTrialMetadataNotFoundException : NotFoundException
{
    public ClinicalTrialMetadataNotFoundException(Guid trialId)
        : base($"Clinical Trial Metadata with the identifier {trialId} was not found.")
    {
    }
}