﻿using Domain.Entities;
using Domain.Repositories;
using Infrastructure.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus
{
    public class RabbitMQServicesMessageHandler
    {
        public static async Task HandleMessage(IRepositoryManager _repositoryManager, object? model, BasicDeliverEventArgs eventArgs, CancellationToken cancellationToken)
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var specialization = JsonConvert.DeserializeObject<Specialization>(message);

            var doctors = await _repositoryManager.DoctorRepository.GetAllAsync(cancellationToken);
            foreach (Doctor doctor in doctors)
            {
                if (doctor.SpecializationId == specialization.Id)
                {
                    doctor.Status = specialization.IsActive ? DoctorStatus.AtWork : DoctorStatus.Inactive;
                    _repositoryManager.DoctorRepository.Update(doctor, cancellationToken);
                    Console.WriteLine(doctor);
                }
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
