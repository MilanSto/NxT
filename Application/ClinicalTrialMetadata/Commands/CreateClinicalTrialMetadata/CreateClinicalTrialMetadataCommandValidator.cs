using FluentValidation;

namespace Application.ClinicalTrialMetadata.Commands.CreateClinicalTrialMetadata;

public sealed class CreateClinicalTrialMetadataCommandValidator : AbstractValidator<CreateClinicalTrialMetadataCommand>
{
    public CreateClinicalTrialMetadataCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();

        RuleFor(x => x.StartDate).NotEmpty();

        RuleFor(x => x.Participants).NotEmpty();

        RuleFor(x => x.Status).NotEmpty();
    }
}