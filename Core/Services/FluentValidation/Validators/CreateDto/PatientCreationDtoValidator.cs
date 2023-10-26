using Contracts.PatientDto;
using FluentValidation;

namespace Services.FluentValidation.Validators.CreateDto
{
    public class PatientCreationDtoValidator : ProfileCreationDtoValidator<PatientForCreationDto>
    {
        public PatientCreationDtoValidator()
        {
            RuleFor(p => p.AccountId).Must(ValidateGuid).Unless(p => p.AccountId is null).WithErrorCode("Invalid accountId");
            RuleFor(p => p.IsLinkedToAccount).Equal(true)
                .When(p => p.AccountId is not null, ApplyConditionTo.CurrentValidator)
                .Equal(false)
                .When(p => p.AccountId is null, ApplyConditionTo.CurrentValidator)
                .WithErrorCode("Account ID was specified, but IsLinkedToAccount property set to false");
            RuleFor(p => p.DateOfBirth).Must(BeAValidAge).WithErrorCode("Invalid age");
        }
    }
}
