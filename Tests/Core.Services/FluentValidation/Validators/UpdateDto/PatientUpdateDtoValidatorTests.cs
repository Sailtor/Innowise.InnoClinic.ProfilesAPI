using Contracts.PatientDto;
using FluentValidation.TestHelper;
using Services.FluentValidation.Validators.UpdateDto;

namespace Tests.Core.Services.FluentValidation.Validators.UpdateDto
{
    public class PatientUpdateDtoValidatorTests
    {
        private readonly PatientUpdateDtoValidator _validator;
        public PatientUpdateDtoValidatorTests()
        {
            _validator = new PatientUpdateDtoValidator();
        }

        [Fact]
        public async Task Validate_WithValidModel_ShouldValidate()
        {
            PatientForUpdateDto patient = new()
            {
                Name = "TestName",
                LastName = "TestLastname",
                MiddleName = "TestMiddlename",
                PhotoId = Guid.NewGuid(),
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50),
            };
            var result = await _validator.TestValidateAsync(patient);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_WithInvalidModel_ShouldNotValidate()
        {
            PatientForUpdateDto patient = new()
            {
                Name = "TestName",
                LastName = "TestLastname",
                MiddleName = "TestMiddlename",
                PhotoId = Guid.NewGuid(),
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50),
            };

            var result = await _validator.TestValidateAsync(patient);

            result.ShouldNotHaveValidationErrorFor(d => d.Name);
            result.ShouldNotHaveValidationErrorFor(d => d.LastName);
            result.ShouldNotHaveValidationErrorFor(d => d.MiddleName);
            result.ShouldNotHaveValidationErrorFor(d => d.PhotoId);
            result.ShouldNotHaveValidationErrorFor(d => d.DateOfBirth);
        }
    }
}
