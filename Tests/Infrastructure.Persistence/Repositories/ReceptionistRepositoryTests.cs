using Bogus;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repositories;

namespace Tests.Infrastructure.Persistence.Repositories
{
    public class ReceptionistRepositoryTests
    {
        private readonly DbContextOptions<RepositoryDbContext> _contextOptions;
        public ReceptionistRepositoryTests()
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
        public async Task GetAllAsync_ActionExecutes_ReturnsReceptionists()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(receptionists);
                context.SaveChanges();
            }

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                ReceptionistRepository receptionistRepository = new(context);
                var recievedReceptionists = (await receptionistRepository.GetAllAsync(cancellationToken: default)).ToList();

                recievedReceptionists.Count.Should().Be(receptionists.Count, "because we put 3 receptionists in the collection");
                recievedReceptionists[0].Should().BeEquivalentTo(receptionists[0]);
                recievedReceptionists[1].Should().BeEquivalentTo(receptionists[1]);
                recievedReceptionists[2].Should().BeEquivalentTo(receptionists[2]);
            }
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsReceptionist()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(receptionists);
                context.SaveChanges();
            }
            var receptionistId = receptionists[0].Id;

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                ReceptionistRepository receptionistRepository = new(context);
                var recievedReceptionist = await receptionistRepository.GetByIdAsync(receptionistId, cancellationToken: default);

                recievedReceptionist.Should().NotBeNull("because we put this receptionist in the collection");
                recievedReceptionist.Should().BeEquivalentTo(receptionists[0]);
            }
        }

        [Fact]
        public async Task Update_WithValidUpdateModel_UpdatesReceptionist()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(receptionists);
                context.SaveChanges();
            }

            var receptionistForUpdate = receptionists[0];
            var receptionistOfficeId = Guid.NewGuid();
            receptionistForUpdate.Name = "TestName";
            receptionistForUpdate.MiddleName = "TestMiddlename";
            receptionistForUpdate.LastName = "TestLastName";
            receptionistForUpdate.OfficeId = receptionistOfficeId;

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                ReceptionistRepository receptionistRepository = new(context);
                receptionistRepository.Update(receptionistForUpdate);
                var updatedReceptionist = await receptionistRepository.GetByIdAsync(receptionistForUpdate.Id);

                updatedReceptionist.Should().NotBeNull("because we put receptionist with this id in the collection");
                updatedReceptionist.Name.Should().Be("TestName", "because we changed receptionist name");
                updatedReceptionist.MiddleName.Should().Be("TestMiddlename", "because we changed receptionist middlename");
                updatedReceptionist.LastName.Should().Be("TestLastName", "because we changed receptionist last name");
                updatedReceptionist.OfficeId.Should().Be(receptionistOfficeId, "because we changed receptionist's officeId");
            }
        }

        [Fact]
        public async Task AddAsync_WithValidReceptionist_AddsReceptionistToDatabase()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);
            List<Receptionist> newReceptionist = GenerateRandomReceptionists(1);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(receptionists);
                context.SaveChanges();
            }

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                ReceptionistRepository receptionistRepository = new(context);
                await receptionistRepository.AddAsync(newReceptionist.First());
                await context.SaveChangesAsync();
                var receptionistsInRepository = (await receptionistRepository.GetAllAsync(cancellationToken: default)).ToList();

                receptionistsInRepository.Count.Should().Be(receptionists.Count + 1, "because we added new receptionist to database");
                receptionistsInRepository[0].Should().BeEquivalentTo(receptionists[0]);
                receptionistsInRepository[1].Should().BeEquivalentTo(receptionists[1]);
                receptionistsInRepository[2].Should().BeEquivalentTo(receptionists[2]);
                receptionistsInRepository[3].Should().BeEquivalentTo(newReceptionist.First());
            }
        }

        [Fact]
        public async Task Remove_WithValidId_RemovesReceptionistFromDatabase()
        {
            List<Receptionist> receptionists = GenerateRandomReceptionists(3);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(receptionists);
                context.SaveChanges();
            }

            var receptionistForRemoval = receptionists[2];

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                ReceptionistRepository receptionistRepository = new(context);

                receptionistRepository.Remove(receptionistForRemoval);
                await context.SaveChangesAsync();
                var receptionistsInRepository = (await receptionistRepository.GetAllAsync(cancellationToken: default)).ToList();

                receptionistsInRepository.Count().Should().Be(receptionists.Count - 1, "because we deleted 1 receptionist from database");
                receptionistsInRepository[0].Should().BeEquivalentTo(receptionists[0]);
                receptionistsInRepository[1].Should().BeEquivalentTo(receptionists[1]);
            }
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
                    OfficeId = Guid.NewGuid()
                };

                receptionists.Add(receptionist);
            }

            return receptionists;
        }

    }
}
