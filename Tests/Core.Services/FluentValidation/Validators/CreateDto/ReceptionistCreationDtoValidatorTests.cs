using Contracts.ReceptionistDto;
using FluentValidation.TestHelper;
using Services.FluentValidation.Validators.CreateDto;

namespace Tests.Core.Services.FluentValidation.Validators.CreateDto
{
    public class ReceptionistCreationDtoValidatorTests
    {
        private readonly ReceptionistCreationDtoValidator _validator;
        public ReceptionistCreationDtoValidatorTests()
        {
            _validator = new ReceptionistCreationDtoValidator();
        }

        [Fact]
        public async Task Validate_WithValidModel_ShouldValidate()
        {
            ReceptionistForCreationDto receptionist = new()
            {
                Name = "TestName",
                LastName = "TestLastname",
                MiddleName = "TestMiddlename",
                AccountId = Guid.NewGuid(),
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
            Guid AccountId,
            Guid PhotoId,
            Guid OfficeId
            )
        {
            var receptionist = new ReceptionistForCreationDto()
            {
                Name = Name,
                LastName = LastName,
                MiddleName = Middlename,
                AccountId = AccountId,
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
                yield return new object[] { "", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
                yield return new object[] { "TestFirstName", "", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
                yield return new object[] { "TestFirstName", "TestLastname", "", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.Empty, Guid.NewGuid(), Guid.NewGuid() };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.Empty, Guid.NewGuid() };
                yield return new object[] { "TestFirstName", "TestLastname", "TestMiddlename", Guid.NewGuid(), Guid.NewGuid(), Guid.Empty };
            }
        }
    }
}
