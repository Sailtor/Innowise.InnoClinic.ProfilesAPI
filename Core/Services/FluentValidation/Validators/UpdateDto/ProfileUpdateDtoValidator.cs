using Contracts.ProfileDto;
using FluentValidation;
using Services.Data;

namespace Services.FluentValidation.Validators.UpdateDto
{
    public class ProfileUpdateDtoValidator<T> : AbstractValidator<T> where T : ProfileForUpdateDto
    {
        public ProfileUpdateDtoValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleFor(p => p.Name).NotEmpty().Length(2, 1024).WithErrorCode("Invalid first name");
            RuleFor(p => p.LastName).NotEmpty().Length(2, 1024).WithErrorCode("Invalid last name");
            RuleFor(p => p.MiddleName).Length(2, 1024).Unless(p => p.MiddleName is null).WithErrorCode("Invalid middle name");
            RuleFor(p => p.PhotoId).Must(ValidateGuid).Unless(p => p.PhotoId is null).WithErrorCode("Invalid photoId");
        }

        protected bool ValidateGuid(Guid? unvalidatedGuid)
        {
            if (unvalidatedGuid != Guid.Empty)
            {
                if (Guid.TryParse(unvalidatedGuid.ToString(), out _))
                {
                    return true;
                }
            }
            return false;
        }

        //Duct tape for not nullable guids
        protected bool ValidateGuid(Guid unvalidatedGuid)
        {
            if (unvalidatedGuid != Guid.Empty)
            {
                if (Guid.TryParse(unvalidatedGuid.ToString(), out _))
                {
                    return true;
                }
            }
            return false;
        }

        //Age validation for doctor and patient profiles
        protected bool BeAValidAge(DateOnly date)
        {
            int currentYear = DateTime.UtcNow.Year;
            int dobYear = date.Year;

            if (dobYear <= currentYear && dobYear > currentYear - AllowedAge.Max)
            {
                return true;
            }

            return false;
        }
    }
}
