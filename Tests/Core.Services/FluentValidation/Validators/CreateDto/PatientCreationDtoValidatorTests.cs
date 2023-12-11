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

        [Theory, MemberData(nameof(InvalidPatients))]
        public async Task Validate_WithInvalidModel_ShouldNotValidate(
            string Name,
            string LastName,
            string Middlename,
            Guid AccountId,
            Guid PhotoId,
            DateOnly DateOfBirth,
            bool IsLinkedToAccount)
        {
            var patient = new PatientForCreationDto()
            {
                Name = Name,
                LastName = LastName,
                MiddleName = Middlename,
                AccountId = AccountId,
                PhotoId = PhotoId,
                DateOfBirth = DateOfBirth,
                IsLinkedToAccount = IsLinkedToAccount,
            };
            var result = await _validator.TestValidateAsync(patient);
            result.ShouldHaveAnyValidationError();
        }

        public static IEnumerable<object?[]> InvalidPatients
        {
            get
            {
                yield return new object[] { "", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), true };
                yield return new object[] { "TestFirstName", "", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), true };
                yield return new object[] { "TestFirstName", "TestLastname", "", Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), true };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.Empty, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), true };
                yield return new object?[] { "TestFirstName", "TestLastname", "TestMiddlename", null, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), true };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.Empty, DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), true };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1024), true };
                yield return new object?[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), null };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-50), false };
            }
        }
    }
}
