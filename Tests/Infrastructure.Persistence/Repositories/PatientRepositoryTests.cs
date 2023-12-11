﻿using Bogus;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repositories;
using Services.Data;

namespace Tests.Infrastructure.Persistence.Repositories
{
    public class PatientRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<RepositoryDbContext> _contextOptions;
        public PatientRepositoryTests()
        {
            _contextOptions = new DbContextOptionsBuilder<RepositoryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            using var context = new RepositoryDbContext(_contextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ActionExecutes_ReturnsPatients()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(patients);
                context.SaveChanges();
            }

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                PatientRepository patientRepository = new(context);
                var recievedPatients = await patientRepository.GetAllAsync(cancellationToken: default);

                recievedPatients.Count().Should().Be(3, "because we put 3 patients in the collection");
            }
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsPatient()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(patients);
                context.SaveChanges();
            }
            var patientId = patients[0].Id;

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                PatientRepository patientRepository = new(context);
                var recievedPatient = await patientRepository.GetByIdAsync(patientId, cancellationToken: default);

                recievedPatient.Should().NotBe(null, "because we put this patient in the collection");
            }
        }

        [Fact]
        public async Task Update_WithValidUpdateModel_UpdatesPatient()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(patients);
                context.SaveChanges();
            }

            var patientForUpdate = patients[0];
            patientForUpdate.Name = "TestName";
            patientForUpdate.MiddleName = "TestMiddlename";
            patientForUpdate.LastName = "TestLastName";
            patientForUpdate.DateOfBirth = new DateOnly(2002, 11, 13);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                PatientRepository PatientRepository = new(context);
                PatientRepository.Update(patientForUpdate);
                var updatedPatient = await PatientRepository.GetByIdAsync(patientForUpdate.Id);

                updatedPatient.Should().NotBe(null, "because we put patient with this id in the collection");
                updatedPatient.Name.Should().Be("TestName", "because we changed patient name");
                updatedPatient.MiddleName.Should().Be("TestMiddlename", "because we changed patient middlename");
                updatedPatient.LastName.Should().Be("TestLastName", "because we changed patient last name");
                updatedPatient.DateOfBirth.Should().Be(new DateOnly(2002, 11, 13), "because we changed patient date of birth");
            }
        }

        [Fact]
        public async Task AddAsync_WithValidPatient_AddsPatientToDatabase()
        {
            List<Patient> patients = GenerateRandomPatients(3);
            List<Patient> newPatient = GenerateRandomPatients(1);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(patients);
                context.SaveChanges();
            }

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                PatientRepository patientRepository = new(context);
                await patientRepository.AddAsync(newPatient.First());
                await context.SaveChangesAsync();
                var patientsInRepository = await patientRepository.GetAllAsync(cancellationToken: default);

                patientsInRepository.Count().Should().Be(4, "because we added new patient to database");
            }
        }

        [Fact]
        public async Task Remove_WithValidId_RemovesPatientFromDatabase()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(patients);
                context.SaveChanges();
            }

            var patientForRemoval = patients[1];

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                PatientRepository patientRepository = new(context);

                patientRepository.Remove(patientForRemoval);
                await context.SaveChangesAsync();
                var patientsInRepository = await patientRepository.GetAllAsync(cancellationToken: default);

                patientsInRepository.Count().Should().Be(2, "because we deleted 1 patient from database");
            }
        }

        public void Dispose()
        {
            using var context = new RepositoryDbContext(_contextOptions);
            context.Database.EnsureDeleted(); //ensure deleting db on every test
            context.SaveChanges();
        }

        private static List<Patient> GenerateRandomPatients(int count)
        {
            var patients = new List<Patient>();
            var faker = new Faker();

            for (int i = 0; i < count; i++)
            {
                DateOnly dateOfBirth = faker.Date.BetweenDateOnly(
                        DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-AllowedAge.Max)),
                        DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-AllowedAge.Min)));

                var patient = new Patient
                {
                    Id = Guid.NewGuid(),
                    Name = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    MiddleName = faker.Name.FirstName(),
                    IsLinkedToAccount = faker.Random.Bool(),
                    DateOfBirth = dateOfBirth,
                    AccountId = Guid.NewGuid(),
                    PhotoId = Guid.NewGuid(),
                };

                patients.Add(patient);
            }

            return patients;
        }

    }
}
