using AutoMapper;
using Contracts.ReceptionistDto;
using Domain.Entities;

namespace Services.Automapper.Profiles
{
    public sealed class ReceptionistProfile : Profile
    {
        public ReceptionistProfile()
        {
            CreateMap<ReceptionistForCreationDto, Receptionist>();
            CreateMap<ReceptionistForUpdateDto, Receptionist>();
            CreateMap<Receptionist, ReceptionistForResponseDto>();
        }
    }
}
