using Contracts.DoctorDto;
using Domain.Entities;
using InnoClinic.ProfilesAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace InnoClinic.ProfilesAPI.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("api/doctors")]
    public class DoctorsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public DoctorsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [Authorize(Roles = UserRoles.All)]
        [HttpGet]
        public async Task<IActionResult> GetDoctors(CancellationToken cancellationToken)
        {
            var doctorsDto = await _serviceManager.DoctorService.GetAllAsync(cancellationToken);
            return Ok(doctorsDto);
        }

        [Authorize(Roles = UserRoles.All)]
        [HttpGet("{doctorId:guid}")]
        public async Task<IActionResult> GetDoctorById(Guid doctorId, CancellationToken cancellationToken)
        {
            var doctorDto = await _serviceManager.DoctorService.GetByIdAsync(doctorId, cancellationToken);
            return Ok(doctorDto);
        }

        [Authorize(Roles = UserRoles.Receptionist)]
        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorForCreationDto doctorForCreationDto)
        {
            var doctorDto = await _serviceManager.DoctorService.CreateAsync(doctorForCreationDto);
            return CreatedAtAction(nameof(GetDoctorById), new { doctorId = doctorDto.Id }, doctorDto);
        }

        [Authorize(Roles = UserRoles.ReceptionistAndDoctor, Policy = AuthPolicies.OwnerOrReceptionist)]
        [HttpPut("{doctorId:guid}")]
        public async Task<IActionResult> UpdateDoctor(Guid doctorId, [FromBody] DoctorForUpdateDto doctorForUpdateDto, CancellationToken cancellationToken)
        {
            await _serviceManager.DoctorService.UpdateAsync(doctorId, doctorForUpdateDto, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = UserRoles.Receptionist)]
        [HttpPut("{doctorId:guid}/status")]
        public async Task<IActionResult> UpdateDoctorStatus(Guid doctorId, [FromBody] DoctorStatus status, CancellationToken cancellationToken)
        {
            await _serviceManager.DoctorService.ChangeDoctorStatusAsync(doctorId, status, cancellationToken);
            return NoContent();
        }
    }
}
