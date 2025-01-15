using Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Domain.Abstractions;

public interface IClinicalTrialMetadataRepository
{
    void Insert(ClinicalTrialMetadata clinicalTrialMetadata);
    Task<ClinicalTrialMetadata> GetClinicalTrialByIdAsync(Guid trialId, CancellationToken cancellationToken);
}