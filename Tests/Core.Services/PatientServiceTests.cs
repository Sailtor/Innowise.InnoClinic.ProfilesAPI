using AutoMapper;
using Bogus;
using Contracts.PatientDto;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repositories;
using Services;
using Services.Abstractions;
using Services.Data;
using ValidationException = FluentValidation.ValidationException;

namespace Tests.Core.Services
{
    public class PatientServiceTests : IDisposable
    {
        private readonly IMapper _mapper;
        private readonly DbContextOptions<RepositoryDbContext> _contextOptions;

        private readonly IRepositoryManager _repositoryManager;
        private readonly IValidatorManager _validatorManager;

        public PatientServiceTests(IMapper mapper)
        {
            _contextOptions = new DbContextOptionsBuilder<RepositoryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            var context = new RepositoryDbContext(_contextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.SaveChanges();

            _mapper = mapper;
            _repositoryManager = new RepositoryManager(context);
            _validatorManager = new ValidatorManager();
        }

        [Fact]
        public async Task GetAllAsync_ActionExecutes_ReturnsPatients()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            foreach (Patient patient in patients)
            {
                await _repositoryManager.PatientRepository.AddAsync(patient);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            PatientService patientService = new(_repositoryManager, _mapper, _validatorManager);
            var recievedPatients = await patientService.GetAllAsync(cancellationToken: default);

            recievedPatients.Count().Should().Be(3, "because we put 3 patients in database");
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsPatient()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            foreach (Patient patient in patients)
            {
                await _repositoryManager.PatientRepository.AddAsync(patient);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var patientId = patients[0].Id;

            PatientService patientService = new(_repositoryManager, _mapper, _validatorManager);

            var recievedPatient = await patientService.GetByIdAsync(patientId, cancellationToken: default);

            recievedPatient.Should().NotBe(null, "because we put this patient in the collection");
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsNotFoundException()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            foreach (Patient patient in patients)
            {
                await _repositoryManager.PatientRepository.AddAsync(patient);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var patientId = Guid.NewGuid();

            PatientService patientService = new(_repositoryManager, _mapper, _validatorManager);
            Func<Task> act = async () => await patientService.GetByIdAsync(patientId, cancellationToken: default);

            await act.Should().ThrowAsync<ProfileNotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_WithValidCreateModel_ReturnsCreatedPatient()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            foreach (Patient patient in patients)
            {
                await _repositoryManager.PatientRepository.AddAsync(patient);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var patientForCreationDto = new PatientForCreationDto()
            {
                Name = "TestName",
                MiddleName = "TestMiddleName",
                LastName = "TestLastName",
                PhotoId = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                IsLinkedToAccount = true,
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-AllowedAge.Min))
            };

            PatientService patientService = new(_repositoryManager, _mapper, _validatorManager);

            PatientForResponseDto? createdPatient = null;

            await patientService.Invoking(async (ds) => createdPatient = await ds.CreateAsync(patientForCreationDto, cancellationToken: default)).Should().NotThrowAsync<ValidationException>();
            var patientsInDatabase = await patientService.GetAllAsync(cancellationToken: default);

            patientsInDatabase.Count().Should().Be(4, "because we added new patient to database");
            createdPatient.Should().NotBe(null, "because we put patient with this id in the collection");
        }

        [Fact]
        public async Task CreateAsync_WithInvalidCreateModel_ThrowsException()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            foreach (Patient patient in patients)
            {
                await _repositoryManager.PatientRepository.AddAsync(patient);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var patientForCreationDto = new PatientForCreationDto()
            {
                Name = "",
                MiddleName = "",
                LastName = "",
                PhotoId = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                IsLinkedToAccount = true,
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-AllowedAge.Min))
            };

            PatientService patientService = new(_repositoryManager, _mapper, _validatorManager);

            await patientService.Invoking(ds => ds.CreateAsync(patientForCreationDto, cancellationToken: default)).Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UpdateAsync_WithValidUpdateModel_UpdatesPatient()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            foreach (Patient patient in patients)
            {
                await _repositoryManager.PatientRepository.AddAsync(patient);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var patientId = patients[0].Id;
            var patientForUpdate = new PatientForUpdateDto
            {
                Name = "TestName",
                MiddleName = "TestMiddleName",
                LastName = "TestLastName",
                PhotoId = patients[0].PhotoId,
                DateOfBirth = patients[0].DateOfBirth
            };

            PatientService patientService = new(_repositoryManager, _mapper, _validatorManager);
            await patientService.UpdateAsync(patientId, patientForUpdate, cancellationToken: default);

            var updatedPatient = await patientService.GetByIdAsync(patientId);

            await patientService.Invoking(y => y.UpdateAsync(patientId, patientForUpdate, cancellationToken: default)).Should().NotThrowAsync<ValidationException>();

            updatedPatient.Should().NotBe(null, "because we put patient with this id in the collection");
            updatedPatient.Name.Should().Be("TestName", "because we changed patient name");
            updatedPatient.MiddleName.Should().Be("TestMiddleName", "because we changed patient middlename");
            updatedPatient.LastName.Should().Be("TestLastName", "because we changed patient last name");
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidUpdateModel_ThrowsException()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            foreach (Patient patient in patients)
            {
                await _repositoryManager.PatientRepository.AddAsync(patient);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var patientId = patients[0].Id;
            var patientForUpdate = new PatientForUpdateDto
            {
                Name = "",
                MiddleName = "",
                LastName = "",
                PhotoId = patients[0].PhotoId,
                DateOfBirth = patients[0].DateOfBirth
            };

            PatientService patientService = new(_repositoryManager, _mapper, _validatorManager);

            await patientService.Invoking(y => y.UpdateAsync(patientId, patientForUpdate, cancellationToken: default)).Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsException()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            foreach (Patient patient in patients)
            {
                await _repositoryManager.PatientRepository.AddAsync(patient);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var patientId = Guid.NewGuid();
            var patientForUpdate = new PatientForUpdateDto
            {
                Name = "TestName",
                MiddleName = "TestMiddleName",
                LastName = "TestLastName",
                PhotoId = patients[0].PhotoId,
                DateOfBirth = patients[0].DateOfBirth
            };

            PatientService patientService = new(_repositoryManager, _mapper, _validatorManager);

            await patientService.Invoking(y => y.UpdateAsync(patientId, patientForUpdate, cancellationToken: default)).Should().ThrowAsync<ProfileNotFoundException>();
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesPatient()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            foreach (Patient patient in patients)
            {
                await _repositoryManager.PatientRepository.AddAsync(patient);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var patientId = patients[0].Id;

            PatientService patientService = new(_repositoryManager, _mapper, _validatorManager);

            await patientService.Invoking(ds => ds.DeleteAsync(patientId, cancellationToken: default)).Should().NotThrowAsync<ProfileNotFoundException>();

            var patientsInDatabase = await patientService.GetAllAsync(cancellationToken: default);

            patientsInDatabase.Count().Should().Be(2, "because we deleted patient from database");
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ThrowsException()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            foreach (Patient patient in patients)
            {
                await _repositoryManager.PatientRepository.AddAsync(patient);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
            var patientId = Guid.NewGuid();
            PatientService patientService = new(_repositoryManager, _mapper, _validatorManager);

            await patientService.Invoking(ds => ds.DeleteAsync(patientId, cancellationToken: default)).Should().ThrowAsync<ProfileNotFoundException>();
        }

        [Fact]
        public async Task LinkUserProfileToAccountAsync_WithValidIds_LinksUserToAccount()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            patients[0].AccountId = null;
            patients[0].IsLinkedToAccount = false;

            foreach (Patient patient in patients)
            {
                await _repositoryManager.PatientRepository.AddAsync(patient);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
            var patientId = patients[0].Id;
            var userAccountId = Guid.NewGuid();
            PatientService patientService = new(_repositoryManager, _mapper, _validatorManager);

            await patientService.Invoking(ds => ds.LinkUserProfileToAccountAsync(patientId, userAccountId, cancellationToken: default)).Should().NotThrowAsync<ProfileNotFoundException>();
            var updatedPatient = await patientService.GetByIdAsync(patientId);

            updatedPatient.AccountId.Should().Be(userAccountId, "because we linked patient to specific account");
            updatedPatient.IsLinkedToAccount.Should().Be(true, "because we linked patient to account");
        }

        [Fact]
        public async Task LinkUserProfileToAccountAsync_WithInvalidUserId_ThrowsException()
        {
            List<Patient> patients = GenerateRandomPatients(3);

            patients[0].AccountId = null;
            patients[0].IsLinkedToAccount = false;

            foreach (Patient patient in patients)
            {
                await _repositoryManager.PatientRepository.AddAsync(patient);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
            var patientId = Guid.NewGuid();
            var userAccountId = Guid.NewGuid();
            PatientService patientService = new(_repositoryManager, _mapper, _validatorManager);

            await patientService.Invoking(ds => ds.LinkUserProfileToAccountAsync(patientId, userAccountId, cancellationToken: default)).Should().ThrowAsync<ProfileNotFoundException>();
        }

        public void Dispose()
        {
            using var context = new RepositoryDbContext(_contextOptions);
            context.Database.EnsureDeleted(); //ensure deleting db after every test
            context.SaveChanges();
        }

        private static List<Patient> GenerateRandomPatients(int count)
        {
            var patients = new List<Patient>();
            var faker = new Faker();
            DateOnly dateOfBirth = faker.Date.BetweenDateOnly(
                DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-AllowedAge.Max)),
                DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-AllowedAge.Min)));
            for (int i = 0; i < count; i++)
            {
                var patient = new Patient
                {
                    Id = Guid.NewGuid(),
                    Name = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    MiddleName = faker.Name.LastName(),
                    AccountId = Guid.NewGuid(),
                    PhotoId = Guid.NewGuid(),
                    IsLinkedToAccount = true,
                    DateOfBirth = dateOfBirth,
                };

                patients.Add(patient);
            }

            return patients;
        }
    }
}
