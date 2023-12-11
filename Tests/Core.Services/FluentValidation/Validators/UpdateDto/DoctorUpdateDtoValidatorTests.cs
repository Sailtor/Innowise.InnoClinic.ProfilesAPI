using Contracts.DoctorDto;
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

        [Theory, MemberData(nameof(InvalidDoctors))]
        public async Task Validate_WithInvalidModel_ShouldNotValidate(
            string Name,
            string LastName,
            string Middlename,
            Guid PhotoId,
            DateOnly DateOfBirth,
            Guid SpecializationId,
            Guid OfficeId,
            int CareerStartYear
            )
        {
            var doctor = new DoctorForUpdateDto()
            {
                Name = Name,
                LastName = LastName,
                MiddleName = Middlename,
                PhotoId = PhotoId,
                DateOfBirth = DateOfBirth,
                SpecializationId = SpecializationId,
                OfficeId = OfficeId,
                CareerStartYear = CareerStartYear
            };
            var result = await _validator.TestValidateAsync(doctor);
            result.ShouldHaveAnyValidationError();
        }

        public static IEnumerable<object[]> InvalidDoctors
        {
            get
            {
                yield return new object[] { "", "TestLastname", "TestMiddlename", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - 20 };
                yield return new object[] { "TestFirstName", "", "TestMiddlename", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - 20 };
                yield return new object[] { "TestFirstName", "TestLastname", "", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - 20 };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.Empty, DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - 20 };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1024), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - 20 };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.Empty, Guid.NewGuid(), DateTime.UtcNow.Year - 20 };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.Empty, DateTime.UtcNow.Year - 20 };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - 1024 };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.Year - -51 };
            }
        }
    }
}
