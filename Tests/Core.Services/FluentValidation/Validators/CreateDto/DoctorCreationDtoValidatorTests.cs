using Contracts.DoctorDto;
using Domain.Entities;
using FluentValidation.TestHelper;
using Services.FluentValidation.Validators.CreateDto;

namespace Tests.Core.Services.FluentValidation.Validators.CreateDto
{
    public class DoctorCreationDtoValidatorTests
    {
        private readonly DoctorCreationDtoValidator _validator;
        public DoctorCreationDtoValidatorTests()
        {
            _validator = new DoctorCreationDtoValidator();
        }

        [Fact]
        public async Task Validate_WithValidModel_ShouldValidate()
        {
            DoctorForCreationDto doctor = new()
            {
                Name = "TestName",
                LastName = "TestLastname",
                MiddleName = "TestMiddlename",
                AccountId = Guid.NewGuid(),
                PhotoId = Guid.NewGuid(),
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50),
                SpecializationId = Guid.NewGuid(),
                OfficeId = Guid.NewGuid(),
                CareerStartYear = DateTime.UtcNow.Year - 20,
                Status = DoctorStatus.AtWork
            };
            var result = await _validator.TestValidateAsync(doctor);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_WithInvalidModel_ShouldNotValidate()
        {
            DoctorForCreationDto doctor = new()
            {
                Name = "",
                LastName = "",
                MiddleName = "n",
                AccountId = Guid.Empty,
                PhotoId = Guid.Empty,
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1024),
                SpecializationId = Guid.Empty,
                OfficeId = Guid.Empty,
                CareerStartYear = DateTime.UtcNow.Year - 1024,
                Status = (DoctorStatus)14
            };
            var result = await _validator.TestValidateAsync(doctor);

            result.ShouldHaveValidationErrorFor(d => d.Name);
            result.ShouldHaveValidationErrorFor(d => d.LastName);
            result.ShouldHaveValidationErrorFor(d => d.MiddleName);
            result.ShouldHaveValidationErrorFor(d => d.AccountId);
            result.ShouldHaveValidationErrorFor(d => d.PhotoId);
            result.ShouldHaveValidationErrorFor(d => d.DateOfBirth);
            result.ShouldHaveValidationErrorFor(d => d.SpecializationId);
            result.ShouldHaveValidationErrorFor(d => d.OfficeId);
            result.ShouldHaveValidationErrorFor(d => d.CareerStartYear);
            result.ShouldHaveValidationErrorFor(d => d.Status);
        }

        [Fact]
        public async Task Validate_WithInvalidCareerStartYear_ShouldNotValidate()
        {
            DoctorForCreationDto doctor = new()
            {
                Name = "TestName",
                LastName = "TestLastname",
                MiddleName = "TestMiddlename",
                AccountId = Guid.NewGuid(),
                PhotoId = Guid.NewGuid(),
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50),
                SpecializationId = Guid.NewGuid(),
                OfficeId = Guid.NewGuid(),
                CareerStartYear = DateTime.UtcNow.Year - 51,
                Status = DoctorStatus.AtWork
            };
            var result = await _validator.TestValidateAsync(doctor);

            result.ShouldNotHaveValidationErrorFor(d => d.Name);
            result.ShouldNotHaveValidationErrorFor(d => d.LastName);
            result.ShouldNotHaveValidationErrorFor(d => d.MiddleName);
            result.ShouldNotHaveValidationErrorFor(d => d.AccountId);
            result.ShouldNotHaveValidationErrorFor(d => d.PhotoId);
            result.ShouldNotHaveValidationErrorFor(d => d.DateOfBirth);
            result.ShouldNotHaveValidationErrorFor(d => d.SpecializationId);
            result.ShouldNotHaveValidationErrorFor(d => d.OfficeId);
            result.ShouldHaveValidationErrorFor(d => d.CareerStartYear);
            result.ShouldNotHaveValidationErrorFor(d => d.Status);
        }
    }
}
