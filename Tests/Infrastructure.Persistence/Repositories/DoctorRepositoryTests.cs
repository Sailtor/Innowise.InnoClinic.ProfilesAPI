using Bogus;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repositories;
using Services.Data;

namespace Tests.Infrastructure.Persistence.Repositories
{
    public class DoctorRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<RepositoryDbContext> _contextOptions;
        public DoctorRepositoryTests()
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
        public async Task GetAllAsync_ActionExecutes_ReturnsDoctors()
        {
            List<Doctor> doctors = GenerateRandomDoctors(3);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(doctors);
                context.SaveChanges();
            }

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                DoctorRepository doctorRepository = new(context);
                var recievedDoctors = await doctorRepository.GetAllAsync(cancellationToken: default);

                recievedDoctors.Count().Should().Be(3, "because we put 3 items in the collection");
            }
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsDoctor()
        {
            List<Doctor> doctors = GenerateRandomDoctors(3);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(doctors);
                context.SaveChanges();
            }
            var doctorId = doctors[0].Id;

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                DoctorRepository doctorRepository = new(context);
                var recievedDoctor = await doctorRepository.GetByIdAsync(doctorId, cancellationToken: default);

                recievedDoctor.Should().NotBe(null, "because we put this doctor in the collection");
            }
        }

        [Fact]
        public async Task Update_WithValidUpdateModel_ShouldUpdateDoctor()
        {
            List<Doctor> doctors = GenerateRandomDoctors(3);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(doctors);
                context.SaveChanges();
            }

            var doctorForUpdate = doctors[0];
            doctorForUpdate.Name = "TestName";
            doctorForUpdate.MiddleName = "TestMiddlename";
            doctorForUpdate.LastName = "TestLastName";
            doctorForUpdate.Status = DoctorStatus.AtWork;

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                DoctorRepository doctorRepository = new(context);
                doctorRepository.Update(doctorForUpdate);
                var updatedDoctor = await doctorRepository.GetByIdAsync(doctorForUpdate.Id);

                updatedDoctor.Should().NotBe(null, "because we put doctor with this id in the collection");
                updatedDoctor.Name.Should().Be("TestName", "because we changed doctor name");
                updatedDoctor.MiddleName.Should().Be("TestMiddlename", "because we changed doctor middlename");
                updatedDoctor.LastName.Should().Be("TestLastName", "because we changed doctor last name");
                updatedDoctor.Status.Should().Be(DoctorStatus.AtWork, "because we changed doctor status");
            }
        }

        [Fact]
        public async Task AddAsync_WithValidDoctor_ShouldAddDoctorToDatabase()
        {
            List<Doctor> doctors = GenerateRandomDoctors(3);
            List<Doctor> newDoctor = GenerateRandomDoctors(1);

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(doctors);
                context.SaveChanges();
            }

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                DoctorRepository doctorRepository = new(context);
                await doctorRepository.AddAsync(newDoctor.First());
                await context.SaveChangesAsync();
                var doctorsInRepository = await doctorRepository.GetAllAsync(cancellationToken: default);

                doctorsInRepository.Count().Should().Be(4, "because we added new doctor to database");
            }
        }

        [Fact]
        public async Task FindAsync_WithValidPredicate_ShouldReturnFilteredDoctors()
        {
            List<Doctor> doctors = GenerateRandomDoctors(3);
            doctors[0].Status = DoctorStatus.AtWork;
            doctors[1].Status = DoctorStatus.AtWork;
            doctors[2].Status = DoctorStatus.SelfIsolation;

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                context.Profiles.AddRange(doctors);
                context.SaveChanges();
            }

            using (var context = new RepositoryDbContext(_contextOptions))
            {
                DoctorRepository doctorRepository = new(context);
                var recievedDoctors = await doctorRepository.FindAsync(d => d.Status == DoctorStatus.AtWork, cancellationToken: default);
                var doctorsInRepository = await doctorRepository.GetAllAsync(cancellationToken: default);

                doctorsInRepository.Count().Should().Be(3, "because we added 3 doctors to database");
                recievedDoctors.Count().Should().Be(2, "because we added 2 doctors with AtWork status to database");
            }
        }

        public void Dispose()
        {
            using var context = new RepositoryDbContext(_contextOptions);
            context.Database.EnsureDeleted(); //ensure deleting db on every test
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
                    MiddleName = faker.Name.FirstName(),
                    DateOfBirth = dateOfBirth,
                    SpecializationId = Guid.NewGuid(),
                    AccountId = Guid.NewGuid(),
                    PhotoId = Guid.NewGuid(),
                    OfficeId = Guid.NewGuid(),
                    CareerStartYear = faker.Random.Int(dateOfBirth.Year - AllowedAge.Min, DateTime.UtcNow.Year),
                    Status = faker.PickRandom<DoctorStatus>()
                };

                doctors.Add(doctor);
            }

            return doctors;
        }
    }
}
