using Contracts.ReceptionistDto;
using FluentValidation;

namespace Services.FluentValidation.Validators.CreateDto
{
    public class ReceptionistCreationDtoValidator : ProfileCreationDtoValidator<ReceptionistForCreationDto>
    {
        public ReceptionistCreationDtoValidator()
        {
            RuleFor(p => p.OfficeId).NotNull().Must(ValidateGuid).WithErrorCode("Invalid office ID");
        }
    }
}
