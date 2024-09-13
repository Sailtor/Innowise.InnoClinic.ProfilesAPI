using Contracts.ReceptionistDto;
using FluentValidation.TestHelper;
using Services.FluentValidation.Validators.UpdateDto;

namespace Tests.Core.Services.FluentValidation.Validators.UpdateDto
{
    public class ReceptionistUpdateDtoValidatorTests
    {
        private readonly ReceptionistUpdateDtoValidator _validator;
        public ReceptionistUpdateDtoValidatorTests()
        {
            _validator = new ReceptionistUpdateDtoValidator();
        }

        [Fact]
        public async Task Validate_WithValidModel_ShouldValidate()
        {
            ReceptionistForUpdateDto receptionist = new()
            {
                Name = "TestName",
                LastName = "TestLastname",
                MiddleName = "TestMiddlename",
                PhotoId = Guid.NewGuid(),
                OfficeId = Guid.NewGuid(),
            };
            var result = await _validator.TestValidateAsync(receptionist);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_WithInvalidModel_ShouldNotValidate()
        {
            ReceptionistForUpdateDto receptionist = new()
            {
                Name = "",
                LastName = "",
                MiddleName = "m",
                PhotoId = Guid.Empty,
                OfficeId = Guid.Empty,
            };
            var result = await _validator.TestValidateAsync(receptionist);
            result.ShouldHaveValidationErrorFor(d => d.Name);
            result.ShouldHaveValidationErrorFor(d => d.LastName);
            result.ShouldHaveValidationErrorFor(d => d.MiddleName);
            result.ShouldHaveValidationErrorFor(d => d.PhotoId);
            result.ShouldHaveValidationErrorFor(d => d.OfficeId);
        }
    }
}
