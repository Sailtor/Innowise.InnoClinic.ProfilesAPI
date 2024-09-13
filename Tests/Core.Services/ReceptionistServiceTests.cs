using AutoMapper;
using Bogus;
using Contracts.ReceptionistDto;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repositories;
using Services;
using Services.Abstractions;
using ValidationException = FluentValidation.ValidationException;

namespace Tests.Core.Services
{
    public class ReceptionistServiceTests
    {
        private readonly IMapper _mapper;
        private readonly DbContextOptions<RepositoryDbContext> _contextOptions;

        private readonly IRepositoryManager _repositoryManager;
        private readonly IValidatorManager _validatorManager;

        public ReceptionistServiceTests(IMapper mapper)
        {
            _contextOptions = new DbContextOptionsBuilder<RepositoryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            var context = new RepositoryDbContext(_contextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            _mapper = mapper;
            _repositoryManager = new RepositoryManager(context);
            _validatorManager = new ValidatorManager();
        }

        [Fact]
        public async Task GetAllAsync_ActionExecutes_ReturnsReceptionists()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);

            foreach (Receptionist receptionist in receptionists)
            {
                await _repositoryManager.ReceptionistRepository.AddAsync(receptionist);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            ReceptionistService receptionistService = new(_repositoryManager, _mapper, _validatorManager);
            var recievedReceptionists = (await receptionistService.GetAllAsync(cancellationToken: default)).ToList();

            recievedReceptionists.Count().Should().Be(receptionists.Count, "because we put 3 receptionists in database");
            recievedReceptionists[0].Should().BeEquivalentTo(receptionists[0], options => options.ExcludingMissingMembers());
            recievedReceptionists[1].Should().BeEquivalentTo(receptionists[1], options => options.ExcludingMissingMembers());
            recievedReceptionists[2].Should().BeEquivalentTo(receptionists[2], options => options.ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsReceptionist()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);

            foreach (Receptionist receptionist in receptionists)
            {
                await _repositoryManager.ReceptionistRepository.AddAsync(receptionist);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var receptionistId = receptionists[0].Id;

            ReceptionistService receptionistService = new(_repositoryManager, _mapper, _validatorManager);

            var recievedReceptionist = await receptionistService.GetByIdAsync(receptionistId, cancellationToken: default);

            recievedReceptionist.Should().NotBeNull("because we put this receptionist in the collection");
            recievedReceptionist.Should().BeEquivalentTo(receptionists[0], options => options.ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsNotFoundException()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);

            foreach (Receptionist receptionist in receptionists)
            {
                await _repositoryManager.ReceptionistRepository.AddAsync(receptionist);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var receptionistId = Guid.NewGuid();

            ReceptionistService receptionistService = new(_repositoryManager, _mapper, _validatorManager);
            Func<Task> act = async () => await receptionistService.GetByIdAsync(receptionistId, cancellationToken: default);

            await act.Should().ThrowAsync<ProfileNotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_WithValidCreateModel_ReturnsCreatedReceptionist()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);

            foreach (Receptionist receptionist in receptionists)
            {
                await _repositoryManager.ReceptionistRepository.AddAsync(receptionist);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var receptionistForCreationDto = new ReceptionistForCreationDto()
            {
                Name = "TestName",
                MiddleName = "TestMiddleName",
                LastName = "TestLastName",
                PhotoId = Guid.NewGuid(),
                OfficeId = Guid.NewGuid()
            };

            ReceptionistService receptionistService = new(_repositoryManager, _mapper, _validatorManager);

            ReceptionistForResponseDto? createdReceptionist = null;

            await receptionistService.Invoking(async (ds) => createdReceptionist = await ds.CreateAsync(receptionistForCreationDto, cancellationToken: default)).Should().NotThrowAsync<ValidationException>();
            var receptionistsInDatabase = (await receptionistService.GetAllAsync(cancellationToken: default)).ToList();

            receptionistsInDatabase.Count().Should().Be(receptionists.Count + 1, "because we added new receptionist to database");
            createdReceptionist.Should().NotBeNull("because we put receptionist with this id in the collection");

            receptionistsInDatabase[0].Should().BeEquivalentTo(receptionists[0], options => options.ExcludingMissingMembers());
            receptionistsInDatabase[1].Should().BeEquivalentTo(receptionists[1], options => options.ExcludingMissingMembers());
            receptionistsInDatabase[2].Should().BeEquivalentTo(receptionists[2], options => options.ExcludingMissingMembers());
            receptionistsInDatabase[3].Should().BeEquivalentTo(receptionistForCreationDto, options => options.ExcludingMissingMembers());

        }

        [Fact]
        public async Task CreateAsync_WithInvalidCreateModel_ThrowsException()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);

            foreach (Receptionist receptionist in receptionists)
            {
                await _repositoryManager.ReceptionistRepository.AddAsync(receptionist);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var receptionistForCreationDto = new ReceptionistForCreationDto()
            {
                Name = "",
                MiddleName = "",
                LastName = "",
                PhotoId = Guid.NewGuid(),
                OfficeId = Guid.NewGuid()
            };

            ReceptionistService receptionistService = new(_repositoryManager, _mapper, _validatorManager);

            await receptionistService.Invoking(ds => ds.CreateAsync(receptionistForCreationDto, cancellationToken: default)).Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UpdateAsync_WithValidUpdateModel_UpdatesReceptionist()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);

            foreach (Receptionist receptionist in receptionists)
            {
                await _repositoryManager.ReceptionistRepository.AddAsync(receptionist);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var receptionistId = receptionists[0].Id;
            var receptionistForUpdate = new ReceptionistForUpdateDto
            {
                Name = "TestName",
                MiddleName = "TestMiddlename",
                LastName = "TestLastName",
                PhotoId = receptionists[0].PhotoId,
                OfficeId = receptionists[0].OfficeId,
            };

            ReceptionistService receptionistService = new(_repositoryManager, _mapper, _validatorManager);
            await receptionistService.UpdateAsync(receptionistId, receptionistForUpdate, cancellationToken: default);

            var updatedReceptionist = await receptionistService.GetByIdAsync(receptionistId);

            await receptionistService.Invoking(y => y.UpdateAsync(receptionistId, receptionistForUpdate, cancellationToken: default)).Should().NotThrowAsync<ValidationException>();

            updatedReceptionist.Should().NotBeNull("because we put receptionist with this id in the collection");
            updatedReceptionist.Name.Should().Be("TestName", "because we changed receptionist name");
            updatedReceptionist.MiddleName.Should().Be("TestMiddlename", "because we changed receptionist middlename");
            updatedReceptionist.LastName.Should().Be("TestLastName", "because we changed receptionist last name");
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidUpdateModel_ThrowsException()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);

            foreach (Receptionist receptionist in receptionists)
            {
                await _repositoryManager.ReceptionistRepository.AddAsync(receptionist);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var receptionistId = receptionists[0].Id;
            var receptionistForUpdate = new ReceptionistForUpdateDto
            {
                Name = "",
                MiddleName = "",
                LastName = "",
                PhotoId = receptionists[0].PhotoId,
                OfficeId = receptionists[0].OfficeId,
            };

            ReceptionistService receptionistService = new(_repositoryManager, _mapper, _validatorManager);

            await receptionistService.Invoking(y => y.UpdateAsync(receptionistId, receptionistForUpdate, cancellationToken: default)).Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsException()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);

            foreach (Receptionist receptionist in receptionists)
            {
                await _repositoryManager.ReceptionistRepository.AddAsync(receptionist);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var receptionistId = Guid.NewGuid();
            var receptionistForUpdate = new ReceptionistForUpdateDto
            {
                Name = "TestName",
                MiddleName = "TestMiddlename",
                LastName = "TestLastName",
                PhotoId = receptionists[0].PhotoId,
                OfficeId = receptionists[0].OfficeId,
            };

            ReceptionistService receptionistService = new(_repositoryManager, _mapper, _validatorManager);

            await receptionistService.Invoking(y => y.UpdateAsync(receptionistId, receptionistForUpdate, cancellationToken: default)).Should().ThrowAsync<ProfileNotFoundException>();
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesReceptionist()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);

            foreach (Receptionist receptionist in receptionists)
            {
                await _repositoryManager.ReceptionistRepository.AddAsync(receptionist);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var receptionistId = receptionists[2].Id;

            ReceptionistService receptionistService = new(_repositoryManager, _mapper, _validatorManager);

            await receptionistService.Invoking(ds => ds.DeleteAsync(receptionistId, cancellationToken: default)).Should().NotThrowAsync<ProfileNotFoundException>();

            var receptionistsInDatabase = (await receptionistService.GetAllAsync(cancellationToken: default)).ToList();

            receptionistsInDatabase.Count().Should().Be(receptionists.Count-1, "because we deleted receptionist from database");
            receptionistsInDatabase[0].Should().BeEquivalentTo(receptionists[0], options => options.ExcludingMissingMembers());
            receptionistsInDatabase[1].Should().BeEquivalentTo(receptionists[1], options => options.ExcludingMissingMembers());

        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ThrowsException()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);

            foreach (Receptionist receptionist in receptionists)
            {
                await _repositoryManager.ReceptionistRepository.AddAsync(receptionist);
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
            var receptionistId = Guid.NewGuid();
            ReceptionistService receptionistService = new(_repositoryManager, _mapper, _validatorManager);

            await receptionistService.Invoking(ds => ds.DeleteAsync(receptionistId, cancellationToken: default)).Should().ThrowAsync<ProfileNotFoundException>();
        }

        private static List<Receptionist> GenerateRandomReceptionists(int count)
        {
            var receptionists = new List<Receptionist>();
            var faker = new Faker();

            for (int i = 0; i < count; i++)
            {
                var receptionist = new Receptionist
                {
                    Id = Guid.NewGuid(),
                    Name = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    MiddleName = faker.Name.FirstName(),
                    AccountId = Guid.NewGuid(),
                    PhotoId = Guid.NewGuid(),
                    OfficeId = Guid.NewGuid(),
                };

                receptionists.Add(receptionist);
            }

            return receptionists;
        }
    }
}
