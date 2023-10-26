using Contracts.DoctorDto;
using Contracts.PatientDto;
using Contracts.ReceptionistDto;
using FluentValidation;
using Services.Abstractions;
using Services.FluentValidation.Validators.CreateDto;
using Services.FluentValidation.Validators.UpdateDto;

namespace Services
{
    public sealed class ValidatorManager : IValidatorManager
    {
        private readonly Lazy<IValidator<PatientForCreationDto>> _lazyPatientCreationValidator;
        private readonly Lazy<IValidator<PatientForUpdateDto>> _lazyPatientUpdateValidator;

        private readonly Lazy<IValidator<DoctorForCreationDto>> _lazyDoctorCreationValidator;
        private readonly Lazy<IValidator<DoctorForUpdateDto>> _lazyDoctorUpdateValidator;

        private readonly Lazy<IValidator<ReceptionistForCreationDto>> _lazyReceptionistCreationValidator;
        private readonly Lazy<IValidator<ReceptionistForUpdateDto>> _lazyReceptionistUpdateValidator;
        public ValidatorManager()
        {
            _lazyPatientCreationValidator = new Lazy<IValidator<PatientForCreationDto>>(() => new PatientCreationDtoValidator());
            _lazyPatientUpdateValidator = new Lazy<IValidator<PatientForUpdateDto>>(() => new PatientUpdateDtoValidator());

            _lazyDoctorCreationValidator = new Lazy<IValidator<DoctorForCreationDto>>(() => new DoctorCreationDtoValidator());
            _lazyDoctorUpdateValidator = new Lazy<IValidator<DoctorForUpdateDto>>(() => new DoctorUpdateDtoValidator());

            _lazyReceptionistCreationValidator = new Lazy<IValidator<ReceptionistForCreationDto>>(() => new ReceptionistCreationDtoValidator());
            _lazyReceptionistUpdateValidator = new Lazy<IValidator<ReceptionistForUpdateDto>>(() => new ReceptionistUpdateDtoValidator());
        }

        public IValidator<PatientForCreationDto> PatientCreationValidator => _lazyPatientCreationValidator.Value;
        public IValidator<PatientForUpdateDto> PatientUpdateValidator => _lazyPatientUpdateValidator.Value;

        public IValidator<DoctorForCreationDto> DoctorCreationValidator => _lazyDoctorCreationValidator.Value;
        public IValidator<DoctorForUpdateDto> DoctorUpdateValidator => _lazyDoctorUpdateValidator.Value;

        public IValidator<ReceptionistForCreationDto> ReceptionistCreationValidator => _lazyReceptionistCreationValidator.Value;
        public IValidator<ReceptionistForUpdateDto> ReceptionistUpdateValidator => _lazyReceptionistUpdateValidator.Value;
    }
}
