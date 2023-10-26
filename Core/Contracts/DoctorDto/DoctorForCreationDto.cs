﻿using Contracts.ProfileDto;
using Domain.Entities;

namespace Contracts.DoctorDto
{
    public class DoctorForCreationDto : ProfileForCreationDto
    {
        public DateOnly DateOfBirth { get; set; }
        public Guid SpecializationId { get; set; }
        public Guid OfficeId { get; set; }
        public int CareerStartYear { get; set; }
        public DoctorStatus Status { get; set; }
    }
}
