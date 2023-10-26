using Contracts.PatientDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Data;
using Services.Abstractions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public PatientsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [Authorize(Roles = UserRoles.ReceptionistAndDoctor)]
        [HttpGet]
        public async Task<IActionResult> GetPatients(CancellationToken cancellationToken)
        {
            var patientsDto = await _serviceManager.PatientService.GetAllAsync(cancellationToken);
            return Ok(patientsDto);
        }

        [Authorize(Roles = UserRoles.All, Policy = AuthPolicies.OwnerOrReceptionistOrDoctor)]
        [HttpGet("{patientId:guid}")]
        public async Task<IActionResult> GetPatientById(Guid patientId, CancellationToken cancellationToken)
        {
            var patientDto = await _serviceManager.PatientService.GetByIdAsync(patientId, cancellationToken);
            return Ok(patientDto);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] PatientForCreationDto patientForCreationDto)
        {
            var patientDto = await _serviceManager.PatientService.CreateAsync(patientForCreationDto);
            return CreatedAtAction(nameof(GetPatientById), new { patientId = patientDto.Id }, patientDto);
        }

        [Authorize(Roles = UserRoles.ReceptionistAndPatient, Policy = AuthPolicies.OwnerOrReceptionist)]
        [HttpPut("{patientId:guid}")]
        public async Task<IActionResult> UpdatePatient(Guid patientId, [FromBody] PatientForUpdateDto patientForUpdateDto, CancellationToken cancellationToken)
        {
            await _serviceManager.PatientService.UpdateAsync(patientId, patientForUpdateDto, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = UserRoles.ReceptionistAndPatient, Policy = AuthPolicies.OwnerOrReceptionist)]
        [HttpPut("{patientId:guid}/account")]
        public async Task<IActionResult> LinkPatientProfileToAccount(Guid patientId, [FromBody] Guid userAccountId, CancellationToken cancellationToken)
        {
            await _serviceManager.PatientService.LinkUserProfileToAccountAsync(patientId, userAccountId, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = UserRoles.Receptionist)]
        [HttpDelete("{patientId:guid}")]
        public async Task<IActionResult> DeletePatient(Guid patientId, CancellationToken cancellationToken)
        {
            await _serviceManager.PatientService.DeleteAsync(patientId, cancellationToken);
            return NoContent();
        }
    }
}
