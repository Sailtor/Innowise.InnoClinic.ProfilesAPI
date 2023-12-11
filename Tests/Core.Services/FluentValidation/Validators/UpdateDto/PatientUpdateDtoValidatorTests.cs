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

        [Theory, MemberData(nameof(InvalidPatients))]
        public async Task Validate_WithInvalidModel_ShouldNotValidate(
            string Name,
            string LastName,
            string Middlename,
            Guid PhotoId,
            DateOnly DateOfBirth)
        {
            var patient = new PatientForUpdateDto()
            {
                Name = Name,
                LastName = LastName,
                MiddleName = Middlename,
                PhotoId = PhotoId,
                DateOfBirth = DateOfBirth
            };
            var result = await _validator.TestValidateAsync(patient);
            result.ShouldHaveAnyValidationError();
        }

        public static IEnumerable<object?[]> InvalidPatients
        {
            get
            {
                yield return new object[] { "", "TestLastname", "TestMiddlename", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50) };
                yield return new object[] { "TestFirstName", "", "TestMiddlename", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50) };
                yield return new object[] { "TestFirstName", "TestLastname", "", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50) };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.Empty, DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50) };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1024) };
            }
        }
    }
}
