using FluentValidation;
using System;

namespace Application.ClinicalTrialMetadata.Commands.CreateClinicalTrialMetadata;

public class CreateClinicalTrialMetadataCommandValidator : AbstractValidator<CreateClinicalTrialMetadataCommand>
{
    public CreateClinicalTrialMetadataCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();

        RuleFor(x => x.StartDate)
            .Must(date => date.Kind == DateTimeKind.Utc)
            .WithMessage("Start date must be in UTC format.");

        RuleFor(x => x.EndDate)
            .Must(date => !date.HasValue || date.Value.Kind == DateTimeKind.Utc)
            .WithMessage("End date must be in UTC format.")
            .Must((command, endDate) => !endDate.HasValue || endDate.Value > command.StartDate)
            .WithMessage("The end date must be after the start date.");

        RuleFor(x => x.Participants).NotEmpty();

        RuleFor(x => x.Status).NotEmpty();
    }
}