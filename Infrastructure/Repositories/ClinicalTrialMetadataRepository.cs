using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Infrastructure.Repositories;

public sealed class ClinicalTrialMetadataRepository : IClinicalTrialMetadataRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ClinicalTrialMetadataRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClinicalTrialMetadata?> GetClinicalTrialByIdAsync(Guid trialId, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<ClinicalTrialMetadata>()
        .FirstOrDefaultAsync(x => x.Id == trialId, cancellationToken);
    }

    public void Insert(ClinicalTrialMetadata metadata) => _dbContext.Set<ClinicalTrialMetadata>().Add(metadata);
}