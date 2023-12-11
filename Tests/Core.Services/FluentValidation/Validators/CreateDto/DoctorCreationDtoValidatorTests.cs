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

        [Theory, MemberData(nameof(InvalidDoctors))]
        public async Task Validate_WithInvalidModel_ShouldNotValidate(
            string Name,
            string LastName,
            string Middlename,
            Guid AccountId,
            Guid PhotoId,
            DateOnly DateOfBirth,
            Guid SpecializationId,
            Guid OfficeId,
            int CareerStartYear,
            DoctorStatus Status)
        {
            var doctor = new DoctorForCreationDto()
            {
                Name = Name,
                LastName = LastName,
                MiddleName = Middlename,
                AccountId = AccountId,
                PhotoId = PhotoId,
                DateOfBirth = DateOfBirth,
                SpecializationId = SpecializationId,
                OfficeId = OfficeId,
                CareerStartYear = CareerStartYear,
                Status = Status
            };
            var result = await _validator.TestValidateAsync(doctor);
            result.ShouldHaveAnyValidationError();
        }

        public static IEnumerable<object[]> InvalidDoctors
        {
            get
            {
                yield return new object[] { "", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - 20, DoctorStatus.AtWork };
                yield return new object[] { "TestFirstName", "", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - 20, DoctorStatus.AtWork };
                yield return new object[] { "TestFirstName", "TestLastname", "", Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - 20, DoctorStatus.AtWork };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.Empty, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - 20, DoctorStatus.AtWork };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.Empty, DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - 20, DoctorStatus.AtWork };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-2000), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - 20, DoctorStatus.AtWork };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.Empty, Guid.NewGuid(), DateTime.UtcNow.Year - 20, DoctorStatus.AtWork };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.Empty, DateTime.UtcNow.Year - 20, DoctorStatus.AtWork };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - 2000, DoctorStatus.AtWork };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - 20, 14 };
            }
        }
    }
}
