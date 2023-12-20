using Contracts.ReceptionistDto;
using FluentValidation.TestHelper;
using Services.FluentValidation.Validators.CreateDto;

namespace Tests.Core.Services.FluentValidation.Validators.CreateDto
{
    public class ReceptionistCreationDtoValidatorTests
    {
        private readonly ReceptionistCreationDtoValidator _validator;
        public ReceptionistCreationDtoValidatorTests()
        {
            _validator = new ReceptionistCreationDtoValidator();
        }

        [Fact]
        public async Task Validate_WithValidModel_ShouldValidate()
        {
            ReceptionistForCreationDto receptionist = new()
            {
                Name = "TestName",
                LastName = "TestLastname",
                MiddleName = "TestMiddlename",
                AccountId = Guid.NewGuid(),
                PhotoId = Guid.NewGuid(),
                OfficeId = Guid.NewGuid(),
            };
            var result = await _validator.TestValidateAsync(receptionist);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_WithInvalidModel_ShouldNotValidate()
        {
            ReceptionistForCreationDto receptionist = new()
            {
                Name = "",
                LastName = "",
                MiddleName = "m",
                AccountId = Guid.Empty,
                PhotoId = Guid.Empty,
                OfficeId = Guid.Empty,
            };
            var result = await _validator.TestValidateAsync(receptionist);
            result.ShouldHaveValidationErrorFor(d => d.Name);
            result.ShouldHaveValidationErrorFor(d => d.LastName);
            result.ShouldHaveValidationErrorFor(d => d.MiddleName);
            result.ShouldHaveValidationErrorFor(d => d.AccountId);
            result.ShouldHaveValidationErrorFor(d => d.PhotoId);
            result.ShouldHaveValidationErrorFor(d => d.OfficeId);
        }
    }
}
