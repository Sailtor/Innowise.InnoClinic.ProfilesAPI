using Domain.Entities;
using Domain.Repositories;
using MassTransit;
using Newtonsoft.Json;
using Shared;
using System.Text.Json;

namespace MessageBus
{
    public class NotificationCreatedConsumer : IConsumer<OfficeStatusChanged>
    {
        private readonly IRepositoryManager _repositoryManager;

        public NotificationCreatedConsumer(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task Consume(ConsumeContext<OfficeStatusChanged> context)
        {
            var serializedMessage = System.Text.Json.JsonSerializer.Serialize(context.Message, new JsonSerializerOptions { });
            var messageObject = JsonConvert.DeserializeObject<OfficeStatusChanged>(serializedMessage);

            var doctors = await _repositoryManager.DoctorRepository.FindAsync(d => d.OfficeId == messageObject.Id);
            foreach (var doctor in doctors)
            {
                doctor.Status = messageObject.IsActive ? DoctorStatus.AtWork : DoctorStatus.Inactive;
                _repositoryManager.DoctorRepository.Update(doctor);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
        }
    }
}
