using Contracts.DoctorDto;
using Domain.Entities;
using FluentValidation.TestHelper;
using Services.FluentValidation.Validators.UpdateDto;

namespace Tests.Core.Services.FluentValidation.Validators.UpdateDto
{
    public class DoctorUpdateDtoValidatorTests
    {
        private readonly DoctorUpdateDtoValidator _validator;
        public DoctorUpdateDtoValidatorTests()
        {
            _validator = new DoctorUpdateDtoValidator();
        }

        [Fact]
        public async Task Validate_WithValidModel_ShouldValidate()
        {
            DoctorForUpdateDto doctor = new()
            {
                Name = "TestName",
                LastName = "TestLastname",
                MiddleName = "TestMiddlename",
                PhotoId = Guid.NewGuid(),
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50),
                SpecializationId = Guid.NewGuid(),
                OfficeId = Guid.NewGuid(),
                CareerStartYear = DateTime.UtcNow.Year - 20
            };
            var result = await _validator.TestValidateAsync(doctor);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_WithInvalidModel_ShouldNotValidate()
        {
            DoctorForUpdateDto doctor = new()
            {
                Name = "",
                LastName = "",
                MiddleName = "m",
                PhotoId = Guid.Empty,
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1024),
                SpecializationId = Guid.Empty,
                OfficeId = Guid.Empty,
                CareerStartYear = DateTime.UtcNow.Year - 1024
            };
            var result = await _validator.TestValidateAsync(doctor);

            result.ShouldHaveValidationErrorFor(d => d.Name);
            result.ShouldHaveValidationErrorFor(d => d.LastName);
            result.ShouldHaveValidationErrorFor(d => d.MiddleName);
            result.ShouldHaveValidationErrorFor(d => d.PhotoId);
            result.ShouldHaveValidationErrorFor(d => d.DateOfBirth);
            result.ShouldHaveValidationErrorFor(d => d.SpecializationId);
            result.ShouldHaveValidationErrorFor(d => d.OfficeId);
            result.ShouldHaveValidationErrorFor(d => d.CareerStartYear);
        }

        [Fact]
        public async Task Validate_WithInvalidCareerStartYear_ShouldNotValidate()
        {
            DoctorForUpdateDto doctor = new()
            {
                Name = "TestName",
                LastName = "TestLastname",
                MiddleName = "TestMiddlename",
                PhotoId = Guid.NewGuid(),
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50),
                SpecializationId = Guid.NewGuid(),
                OfficeId = Guid.NewGuid(),
                CareerStartYear = DateTime.UtcNow.Year - 51,
            };
            var result = await _validator.TestValidateAsync(doctor);

            result.ShouldNotHaveValidationErrorFor(d => d.Name);
            result.ShouldNotHaveValidationErrorFor(d => d.LastName);
            result.ShouldNotHaveValidationErrorFor(d => d.MiddleName);
            result.ShouldNotHaveValidationErrorFor(d => d.PhotoId);
            result.ShouldNotHaveValidationErrorFor(d => d.DateOfBirth);
            result.ShouldNotHaveValidationErrorFor(d => d.SpecializationId);
            result.ShouldNotHaveValidationErrorFor(d => d.OfficeId);
            result.ShouldHaveValidationErrorFor(d => d.CareerStartYear);
        }
    }
}
