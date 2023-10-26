using Contracts.ReceptionistDto;
using FluentValidation;

namespace Services.FluentValidation.Validators.UpdateDto
{
    public class ReceptionistUpdateDtoValidator : ProfileUpdateDtoValidator<ReceptionistForUpdateDto>
    {
        public ReceptionistUpdateDtoValidator()
        {
            RuleFor(p => p.OfficeId).NotNull().Must(ValidateGuid).WithErrorCode("Invalid office ID");
        }
    }
}
