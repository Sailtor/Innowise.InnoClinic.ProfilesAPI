using Contracts.PatientDto;
using FluentValidation;

namespace Services.FluentValidation.Validators.UpdateDto
{
    public class PatientUpdateDtoValidator : ProfileUpdateDtoValidator<PatientForUpdateDto>
    {
        public PatientUpdateDtoValidator()
        {
            RuleFor(p => p.DateOfBirth).Must(BeAValidAge).WithErrorCode("Invalid age");
        }
    }
}
