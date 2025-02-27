﻿using Application.Abstractions.Messaging;
using Domain.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ClinicalTrialMetadata.Queries.GetClinicalTrialMetadataById;

internal sealed class GetClinicalTrialMetadataQueryHandler : IQueryHandler<GetClinicalTrialMetadataByIdQuery, ClinicalTrialMetadataResponse>
{
    private readonly IClinicalTrialMetadataRepository _clinicalTrialMetadataRepositoryRepository;

    public GetClinicalTrialMetadataQueryHandler(IClinicalTrialMetadataRepository clinicalTrialMetadataRepository)
    {
        _clinicalTrialMetadataRepositoryRepository = clinicalTrialMetadataRepository;
    }

    public async Task<ClinicalTrialMetadataResponse> Handle(GetClinicalTrialMetadataByIdQuery request, CancellationToken cancellationToken)
    {
        var metadata = await _clinicalTrialMetadataRepositoryRepository.GetClinicalTrialByIdAsync(request.TrialId, cancellationToken);

        if (metadata == null)
        {
            return null;
        }

        var response = new ClinicalTrialMetadataResponse(metadata.Title, metadata.StartDate, metadata.EndDate, metadata.Participants, metadata.Status, metadata.DurationInDays);

        return response;
    }
}