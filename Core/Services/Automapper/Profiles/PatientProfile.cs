using AutoMapper;
using Contracts.PatientDto;
using Domain.Entities;

namespace Services.Automapper.Profiles
{
    public sealed class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<PatientForCreationDto, Patient>();
            CreateMap<PatientForUpdateDto, Patient>();
            CreateMap<Patient, PatientForResponseDto>();
        }
    }

}
