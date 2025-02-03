using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Enums;

namespace Application.ClinicalTrialMetadata.Commands.CreateClinicalTrialMetadata;

internal sealed class CreateClinicalTrialMetadataCommandHandler : ICommandHandler<CreateClinicalTrialMetadataCommand, Guid>
{
    private readonly IClinicalTrialMetadataRepository _clinicalTrialMetadataRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateClinicalTrialMetadataCommandHandler(IClinicalTrialMetadataRepository clinicalTrialMetadataRepository, IUnitOfWork unitOfWork)
    {
        _clinicalTrialMetadataRepository = clinicalTrialMetadataRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateClinicalTrialMetadataCommand request, CancellationToken cancellationToken)
    {
        DateTime? endDateUtc = null;
        if (request.EndDate != null)
        {
            endDateUtc = request.EndDate.Value;
        }
        else if (request.Status == TrialStatus.Ongoing)
        {
            endDateUtc = request.StartDate.AddMonths(1);
        }

        var durationInDays = endDateUtc.HasValue ? (endDateUtc.Value - request.StartDate).Days : 0;

        var metadata = new Domain.Entities.ClinicalTrialMetadata(
            Guid.NewGuid(), 
            request.Title, 
            request.StartDate, 
            endDateUtc, 
            request.Participants, 
            request.Status, 
            durationInDays);

        _clinicalTrialMetadataRepository.Insert(metadata);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return metadata.Id;
    }
}