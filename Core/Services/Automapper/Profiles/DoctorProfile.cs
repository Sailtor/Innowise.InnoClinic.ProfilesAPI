using AutoMapper;
using Contracts.DoctorDto;
using Domain.Entities;

namespace Services.Automapper.Profiles
{
    public sealed class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<DoctorForCreationDto, Doctor>();
            CreateMap<DoctorForUpdateDto, Doctor>();
            CreateMap<Doctor, DoctorForResponseDto>();
        }
    }

}
