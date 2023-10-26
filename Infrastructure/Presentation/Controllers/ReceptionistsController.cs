using Contracts.ReceptionistDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Data;
using Services.Abstractions;

namespace Presentation.Controllers
{
    [Authorize(Roles = UserRoles.Receptionist)]
    [ApiController]
    [Route("api/receptionists")]
    public class ReceptionistsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public ReceptionistsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetReceptionists(CancellationToken cancellationToken)
        {
            var receptionistsDto = await _serviceManager.ReceptionistService.GetAllAsync(cancellationToken);
            return Ok(receptionistsDto);
        }

        [HttpGet("{receptionistId:guid}")]
        public async Task<IActionResult> GetReceptionistById(Guid receptionistId, CancellationToken cancellationToken)
        {
            var receptionistDto = await _serviceManager.ReceptionistService.GetByIdAsync(receptionistId, cancellationToken);
            return Ok(receptionistDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReceptionist([FromBody] ReceptionistForCreationDto receptionistForCreationDto)
        {
            var receptionistDto = await _serviceManager.ReceptionistService.CreateAsync(receptionistForCreationDto);
            return CreatedAtAction(nameof(GetReceptionistById), new { receptionistId = receptionistDto.Id }, receptionistDto);
        }

        [HttpPut("{receptionistId:guid}")]
        public async Task<IActionResult> UpdateReceptionist(Guid receptionistId, [FromBody] ReceptionistForUpdateDto receptionistForUpdateDto, CancellationToken cancellationToken)
        {
            await _serviceManager.ReceptionistService.UpdateAsync(receptionistId, receptionistForUpdateDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{receptionistId:guid}")]
        public async Task<IActionResult> DeletePatient(Guid receptionistId, CancellationToken cancellationToken)
        {
            await _serviceManager.ReceptionistService.DeleteAsync(receptionistId, cancellationToken);
            return NoContent();
        }
    }
}
