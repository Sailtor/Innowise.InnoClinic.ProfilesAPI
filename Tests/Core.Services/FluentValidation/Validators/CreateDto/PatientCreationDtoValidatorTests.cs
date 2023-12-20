using Contracts.PatientDto;
using FluentValidation.TestHelper;
using Services.FluentValidation.Validators.CreateDto;

namespace Tests.Core.Services.FluentValidation.Validators.CreateDto
{
    public class PatientCreationDtoValidatorTests
    {
        private readonly PatientCreationDtoValidator _validator;
        public PatientCreationDtoValidatorTests()
        {
            _validator = new PatientCreationDtoValidator();
        }

        [Fact]
        public async Task Validate_WithValidModel_ShouldValidate()
        {
            PatientForCreationDto patient = new()
            {
                Name = "TestName",
                LastName = "TestLastname",
                MiddleName = "TestMiddlename",
                AccountId = Guid.NewGuid(),
                PhotoId = Guid.NewGuid(),
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50),
                IsLinkedToAccount = true,
            };
            var result = await _validator.TestValidateAsync(patient);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_WithInvalidModel_ShouldNotValidate()
        {
            PatientForCreationDto patient = new()
            {
                Name = "",
                LastName = "",
                MiddleName = "m",
                AccountId = Guid.Empty,
                PhotoId = Guid.Empty,
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1024)
            };

            var result = await _validator.TestValidateAsync(patient);

            result.ShouldHaveValidationErrorFor(d => d.Name);
            result.ShouldHaveValidationErrorFor(d => d.LastName);
            result.ShouldHaveValidationErrorFor(d => d.MiddleName);
            result.ShouldHaveValidationErrorFor(d => d.AccountId);
            result.ShouldHaveValidationErrorFor(d => d.PhotoId);
            result.ShouldHaveValidationErrorFor(d => d.DateOfBirth);
        }

        [Fact]
        public async Task Validate_WithInvalidAccountProperties_ShouldNotValidate()
        {
            PatientForCreationDto patient = new()
            {
                Name = "TestName",
                LastName = "TestLastname",
                MiddleName = "TestMiddlename",
                AccountId = Guid.NewGuid(),
                PhotoId = Guid.NewGuid(),
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50),
                IsLinkedToAccount = false
            };

            var result = await _validator.TestValidateAsync(patient);

            result.ShouldNotHaveValidationErrorFor(d => d.Name);
            result.ShouldNotHaveValidationErrorFor(d => d.LastName);
            result.ShouldNotHaveValidationErrorFor(d => d.MiddleName);
            result.ShouldNotHaveValidationErrorFor(d => d.AccountId);
            result.ShouldNotHaveValidationErrorFor(d => d.PhotoId);
            result.ShouldNotHaveValidationErrorFor(d => d.DateOfBirth);
            result.ShouldHaveValidationErrorFor(d => d.IsLinkedToAccount);
        }

        [Fact]
        public async Task Validate_WithNullAccountId_ShouldNotValidate()
        {
            PatientForCreationDto patient = new()
            {
                Name = "TestName",
                LastName = "TestLastname",
                MiddleName = "TestMiddlename",
                AccountId = null,
                PhotoId = Guid.NewGuid(),
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50),
                IsLinkedToAccount = true
            };

            var result = await _validator.TestValidateAsync(patient);

            result.ShouldNotHaveValidationErrorFor(d => d.Name);
            result.ShouldNotHaveValidationErrorFor(d => d.LastName);
            result.ShouldNotHaveValidationErrorFor(d => d.MiddleName);
            result.ShouldNotHaveValidationErrorFor(d => d.AccountId);
            result.ShouldNotHaveValidationErrorFor(d => d.PhotoId);
            result.ShouldNotHaveValidationErrorFor(d => d.DateOfBirth);
            result.ShouldHaveValidationErrorFor(d => d.IsLinkedToAccount);
        }
    }
}
