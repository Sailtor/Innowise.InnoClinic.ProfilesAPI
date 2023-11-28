using Domain.Entities;
using Domain.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Text;

namespace MessageBus
{
    public class RabbitMQServicesMessageHandler
    {
        public static async Task HandleMessage(IRepositoryManager _repositoryManager, object? model, BasicDeliverEventArgs eventArgs, CancellationToken cancellationToken)
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var specialization = JsonConvert.DeserializeObject<Specialization>(message);

            var doctors = await _repositoryManager.DoctorRepository.FindAsync(d => d.SpecializationId == specialization.Id, cancellationToken);
            foreach (Doctor doctor in doctors)
            {
                doctor.Status = specialization.IsActive ? DoctorStatus.AtWork : DoctorStatus.Inactive;
                _repositoryManager.DoctorRepository.Update(doctor, cancellationToken);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
