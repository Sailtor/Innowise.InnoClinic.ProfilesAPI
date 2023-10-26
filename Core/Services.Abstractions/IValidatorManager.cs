using Contracts.DoctorDto;
using Contracts.PatientDto;
using Contracts.ReceptionistDto;
using FluentValidation;

namespace Services.Abstractions
{
    public interface IValidatorManager
    {
        IValidator<PatientForCreationDto> PatientCreationValidator { get; }
        IValidator<PatientForUpdateDto> PatientUpdateValidator { get; }
        IValidator<DoctorForCreationDto> DoctorCreationValidator { get; }
        IValidator<DoctorForUpdateDto> DoctorUpdateValidator { get; }
        IValidator<ReceptionistForCreationDto> ReceptionistCreationValidator { get; }
        IValidator<ReceptionistForUpdateDto> ReceptionistUpdateValidator { get; }

    }
}
