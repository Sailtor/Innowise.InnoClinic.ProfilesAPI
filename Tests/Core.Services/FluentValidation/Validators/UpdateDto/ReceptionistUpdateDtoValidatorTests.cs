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

        [Theory, MemberData(nameof(InvalidReceptionists))]
        public async Task Validate_WithInvalidModel_ShouldNotValidate(
            string Name,
            string LastName,
            string Middlename,
            Guid PhotoId,
            Guid OfficeId
            )
        {
            var receptionist = new ReceptionistForUpdateDto()
            {
                Name = Name,
                LastName = LastName,
                MiddleName = Middlename,
                PhotoId = PhotoId,
                OfficeId = OfficeId,
            };
            var result = await _validator.TestValidateAsync(receptionist);
            result.ShouldHaveAnyValidationError();
        }

        public static IEnumerable<object[]> InvalidReceptionists
        {
            get
            {
                yield return new object[] { "", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid() };
                yield return new object[] { "TestFirstName", "", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid() };
                yield return new object[] { "TestFirstName", "TestLastname", "", Guid.NewGuid(), Guid.NewGuid() };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.Empty, Guid.NewGuid() };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.Empty };
            }
        }
    }
}
