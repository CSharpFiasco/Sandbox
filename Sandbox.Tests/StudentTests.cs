using Microsoft.EntityFrameworkCore;
using Sandbox.Tests.Fixtures;
using SdkBuild.Domain;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sandbox.Tests
{
    public class StudentTests : IClassFixture<SdkBuildContextFixture>
    {
        private readonly SdkBuildContextFixture _fixture;
        public StudentTests(SdkBuildContextFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task UpdatingUntrackedAndInsertingUntracked_SavesBoth()
        {
            await using SdkBuildContext context = _fixture.CreateContext();
            await using var _ = context.Database.BeginTransaction();

            var existingUntrackedStudent = context.Student.AsNoTracking().First(i => i.Id == 1);
            existingUntrackedStudent.FirstName = "Jane";
            existingUntrackedStudent.LastName = "Doe";

            var newStudent = new Student { FirstName = "John", LastName = "Doe" };
            context.Student.Update(existingUntrackedStudent);

            await context.Student.AddAsync(newStudent);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();
            var result = await context.Student.AsNoTracking().OrderBy(s => s.Id).ToListAsync();

            Assert.Collection(result, (student) =>
            {
                Assert.Equal(1, student.Id);
                Assert.Equal("Jane", student.FirstName);
                Assert.Equal("Doe", student.LastName);
            }, (student) =>
            {
                Assert.Equal(2, student.Id);
            }, (student) =>
            {
                Assert.Equal(3, student.Id);
                Assert.Equal("John", student.FirstName);
                Assert.Equal("Doe", student.LastName);
            });
        }

    }
}
