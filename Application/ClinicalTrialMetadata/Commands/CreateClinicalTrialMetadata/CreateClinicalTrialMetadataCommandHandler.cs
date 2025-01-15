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
        // Ensure dates are in UTC
        var startDateUtc = request.StartDate.Kind == DateTimeKind.Utc
            ? request.StartDate
            : request.StartDate.ToUniversalTime();

        DateTime? endDateUtc = null;
        if (request.EndDate != null)
        {
            if (request.EndDate <= request.StartDate)
            {
                throw new ArgumentException("The end date must be after the start date.");
            }

            endDateUtc = request.EndDate.Value.Kind == DateTimeKind.Utc
                ? request.EndDate.Value
                : request.EndDate.Value.ToUniversalTime();
        }
        else if (request.Status == TrialStatus.Ongoing)
        {
            endDateUtc = startDateUtc.AddMonths(1);
        }

        var durationInDays = endDateUtc.HasValue ? (endDateUtc.Value - startDateUtc).Days : 0;

        var metadata = new Domain.Entities.ClinicalTrialMetadata(Guid.NewGuid(), request.Title, startDateUtc, endDateUtc, request.Participants, request.Status, durationInDays);

        _clinicalTrialMetadataRepository.Insert(metadata);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return metadata.Id;
    }
}