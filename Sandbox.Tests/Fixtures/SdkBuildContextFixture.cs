using Microsoft.EntityFrameworkCore;
using SdkBuild.Domain;

namespace Sandbox.Tests.Fixtures;

public sealed class SdkBuildContextFixture
{
    private readonly string ConnectionString = @$"Server=(localdb)\mssqllocaldb;Database=SdkBuildDatabase;Trusted_Connection=True;MultipleActiveResultSets=true";

    private static readonly object _lock = new();
    private static bool _databaseInitialized = false;

    public SdkBuildContextFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using SdkBuildContext context = CreateContext();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                SeedDatabase(context);
                context.SaveChanges();

                _databaseInitialized = true;
            }
        }
    }

    private static void SeedDatabase(SdkBuildContext context)
    {
        context.Student.AddRange(
            new Student { FirstName = "Test", LastName = "Sku1" },
            new Student { FirstName = "Test", LastName = "Sku2" }
            );
    }

    public SdkBuildContext CreateContext()
        => new SdkBuildContext(
            new DbContextOptionsBuilder<SdkBuildContext>()
                .UseSqlServer(ConnectionString)
                .Options);

}
