using Contracts.DoctorDto;
using FluentValidation;

namespace Services.FluentValidation.Validators.UpdateDto
{
    public class DoctorUpdateDtoValidator : ProfileUpdateDtoValidator<DoctorForUpdateDto>
    {
        public DoctorUpdateDtoValidator()
        {
            RuleFor(p => p.DateOfBirth).Must(BeAValidAge).WithErrorCode("Invalid age");
            RuleFor(p => p.SpecializationId).NotNull().Must(ValidateGuid).WithErrorCode("Invalid specialization ID");
            RuleFor(p => p.OfficeId).NotNull().Must(ValidateGuid).WithErrorCode("Invalid office ID");
            RuleFor(p => p.CareerStartYear).GreaterThanOrEqualTo(p => p.DateOfBirth.Year + 18).WithErrorCode("Invalid career start year");
        }
    }
}
