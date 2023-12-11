using AutoMapper;
using Bogus;
using Contracts.DoctorDto;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;
using Persistence.Repositories;
using Services;
using Services.Abstractions;
using Services.Data;
using Shared;
using ValidationException = FluentValidation.ValidationException;

namespace Tests.Core.Services
{
    public class DoctorServiceTests : IDisposable
    {
        private readonly IMapper _mapper;
        private readonly DbContextOptions<RepositoryDbContext> _contextOptions;

        private readonly IRepositoryManager _repositoryManager;

        private readonly IValidatorManager _validatorManager;
        private readonly Mock<IPublishEndpoint> _publishEndpoint;

        public DoctorServiceTests(IMapper mapper)
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

            _publishEndpoint = new Mock<IPublishEndpoint>();
        }

        [Fact]
        public async Task GetAllAsync_ActionExecutes_ReturnsDoctors()
        {
            List<Doctor> doctors = GenerateRandomDoctors(3);

            foreach (Doctor doctor in doctors)
            {
                await _repositoryManager.DoctorRepository.AddAsync(doctor);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            DoctorService doctorService = new(_repositoryManager, _mapper, _validatorManager, _publishEndpoint.Object);
            var recievedDoctors = await doctorService.GetAllAsync(cancellationToken: default);

            recievedDoctors.Count().Should().Be(3, "because we put 3 items in the collection");
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsDoctor()
        {
            List<Doctor> doctors = GenerateRandomDoctors(3);

            foreach (Doctor doctor in doctors)
            {
                await _repositoryManager.DoctorRepository.AddAsync(doctor);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var doctorId = doctors[0].Id;


            DoctorService doctorService = new(_repositoryManager, _mapper, _validatorManager, _publishEndpoint.Object);
            var recievedDoctor = await doctorService.GetByIdAsync(doctorId, cancellationToken: default);

            recievedDoctor.Should().NotBe(null, "because we put this doctor in the collection");
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsNotFoundException()
        {
            List<Doctor> doctors = GenerateRandomDoctors(3);

            foreach (Doctor doctor in doctors)
            {
                await _repositoryManager.DoctorRepository.AddAsync(doctor);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var doctorId = Guid.NewGuid();

            DoctorService doctorService = new(_repositoryManager, _mapper, _validatorManager, _publishEndpoint.Object);
            Func<Task> act = async () => await doctorService.GetByIdAsync(doctorId, cancellationToken: default);

            await act.Should().ThrowAsync<ProfileNotFoundException>();
        }

        [Fact]
        public async Task UpdateAsync_WithValidUpdateModel_UpdatesDoctor()
        {

            List<Doctor> doctors = GenerateRandomDoctors(3);

            foreach (Doctor doctor in doctors)
            {
                await _repositoryManager.DoctorRepository.AddAsync(doctor);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var doctorId = doctors[0].Id;
            var specializationId = Guid.NewGuid();
            var doctorForUpdate = new DoctorForUpdateDto
            {
                Name = "TestName",
                MiddleName = "TestMiddlename",
                LastName = "TestLastName",
                PhotoId = doctors[0].PhotoId,
                DateOfBirth = doctors[0].DateOfBirth,
                OfficeId = doctors[0].OfficeId,
                CareerStartYear = doctors[0].CareerStartYear,
                SpecializationId = specializationId
            };

            DoctorService doctorService = new(_repositoryManager, _mapper, _validatorManager, _publishEndpoint.Object);
            await doctorService.UpdateAsync(doctorId, doctorForUpdate, cancellationToken: default);

            var updatedDoctor = await doctorService.GetByIdAsync(doctorId);

            await doctorService.Invoking(y => y.UpdateAsync(doctorId, doctorForUpdate, cancellationToken: default)).Should().NotThrowAsync<ValidationException>();

            _publishEndpoint.Verify(pe => pe.Publish<DoctorNameChanged>(It.IsAny<object>(), It.IsAny<CancellationToken>()), "Method didn't publish message to message queue");

            updatedDoctor.Should().NotBe(null, "because we put doctor with this id in the collection");
            updatedDoctor.Name.Should().Be("TestName", "because we changed doctor name");
            updatedDoctor.MiddleName.Should().Be("TestMiddlename", "because we changed doctor middlename");
            updatedDoctor.LastName.Should().Be("TestLastName", "because we changed doctor last name");
            updatedDoctor.SpecializationId.Should().Be(specializationId, "because we changed doctor status");
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidUpdateModel_ThrowsException()
        {
            List<Doctor> doctors = GenerateRandomDoctors(3);

            foreach (Doctor doctor in doctors)
            {
                await _repositoryManager.DoctorRepository.AddAsync(doctor);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var doctorId = doctors[0].Id;
            var doctorForUpdate = new DoctorForUpdateDto
            {
                Name = "",
                MiddleName = "",
                LastName = "",
                PhotoId = doctors[0].PhotoId,
                DateOfBirth = doctors[0].DateOfBirth,
                OfficeId = doctors[0].OfficeId,
                CareerStartYear = doctors[0].CareerStartYear,
                SpecializationId = doctors[0].SpecializationId
            };

            DoctorService doctorService = new(_repositoryManager, _mapper, _validatorManager, _publishEndpoint.Object);

            await doctorService.Invoking(y => y.UpdateAsync(doctorId, doctorForUpdate, cancellationToken: default)).Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task CreateAsync_WithValidCreateModel_ReturnsCreatedDoctor()
        {
            List<Doctor> doctors = GenerateRandomDoctors(3);

            foreach (Doctor doctor in doctors)
            {
                await _repositoryManager.DoctorRepository.AddAsync(doctor);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var doctorForCreationDto = new DoctorForCreationDto()
            {
                Name = "TestName",
                MiddleName = "TestMiddleName",
                LastName = "TestLastName",
                PhotoId = Guid.NewGuid(),
                DateOfBirth = new DateOnly(2002, 11, 11),
                OfficeId = Guid.NewGuid(),
                CareerStartYear = 2021,
                SpecializationId = Guid.NewGuid()
            };

            DoctorService doctorService = new(_repositoryManager, _mapper, _validatorManager, _publishEndpoint.Object);

            DoctorForResponseDto? createdDoctor = null;

            await doctorService.Invoking(async (ds) => createdDoctor = await ds.CreateAsync(doctorForCreationDto, cancellationToken: default)).Should().NotThrowAsync<ValidationException>();
            var doctorsInDatabase = await doctorService.GetAllAsync(cancellationToken: default);

            doctorsInDatabase.Count().Should().Be(4, "because we added new doctor to database");

            createdDoctor.Should().NotBe(null, "because we put doctor with this id in the collection");
        }

        [Fact]
        public async Task CreateAsync_WithInvalidCreateModel_ThrowsException()
        {
            List<Doctor> doctors = GenerateRandomDoctors(3);

            foreach (Doctor doctor in doctors)
            {
                await _repositoryManager.DoctorRepository.AddAsync(doctor);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var doctorForCreationDto = new DoctorForCreationDto()
            {
                Name = "",
                MiddleName = "",
                LastName = "",
                PhotoId = Guid.NewGuid(),
                DateOfBirth = new DateOnly(1980, 11, 11),
                OfficeId = Guid.NewGuid(),
                CareerStartYear = 1991,
                SpecializationId = Guid.NewGuid()
            };

            DoctorService doctorService = new(_repositoryManager, _mapper, _validatorManager, _publishEndpoint.Object);

            await doctorService.Invoking(ds => ds.CreateAsync(doctorForCreationDto, cancellationToken: default)).Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task ChangeDoctorStatusAsync_WithValidDoctorStatus_ChangesDoctorStatus()
        {
            List<Doctor> doctors = GenerateRandomDoctors(3);
            doctors[0].Status = DoctorStatus.Inactive;
            foreach (Doctor doctor in doctors)
            {
                await _repositoryManager.DoctorRepository.AddAsync(doctor);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var doctorId = doctors[0].Id;

            DoctorService doctorService = new(_repositoryManager, _mapper, _validatorManager, _publishEndpoint.Object);

            await doctorService.Invoking(ds => ds.ChangeDoctorStatusAsync(doctorId, DoctorStatus.AtWork, cancellationToken: default)).Should().NotThrowAsync();
            var updatedDoctor = await doctorService.GetByIdAsync(doctorId);

            updatedDoctor.Should().NotBe(null, "because we put doctor with this id in the collection");
            updatedDoctor.Status.Should().Be(DoctorStatus.AtWork, "because we changed doctor status");
        }

        public void Dispose()
        {
            using var context = new RepositoryDbContext(_contextOptions);
            context.Database.EnsureDeleted(); //ensure deleting db after every test
            context.SaveChanges();
        }

        private static List<Doctor> GenerateRandomDoctors(int count)
        {
            var doctors = new List<Doctor>();
            var faker = new Faker();

            for (int i = 0; i < count; i++)
            {
                DateOnly dateOfBirth = faker.Date.BetweenDateOnly(
                        DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-AllowedAge.Max)),
                        DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-AllowedAge.Min)));

                var doctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    Name = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    MiddleName = faker.Name.LastName(),
                    DateOfBirth = dateOfBirth,
                    SpecializationId = Guid.NewGuid(),
                    AccountId = Guid.NewGuid(),
                    PhotoId = Guid.NewGuid(),
                    OfficeId = Guid.NewGuid(),
                    CareerStartYear = faker.Random.Int(dateOfBirth.Year + AllowedAge.Min, DateTime.UtcNow.Year),
                    Status = faker.PickRandom<DoctorStatus>()
                };

                doctors.Add(doctor);
            }

            return doctors;
        }
    }
}
