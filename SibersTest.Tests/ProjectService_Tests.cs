using Microsoft.EntityFrameworkCore;
using SibersTest.Core.Entities;
using SibersTest.Infrastructure.Data;
using SibersTest.Services.DTOs;
using SibersTest.Services.Services;

namespace SibersTest.Tests;

[TestFixture]
public class ProjectService_Tests
{

    private DbContextOptions<ApplicationDbContext> _options;
    #region Setup

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
    }

    #endregion

    #region Create project test
    [Test]
    [TestCase(0)]

    public async Task CreateProject_ShoudSaveInDb(int expectedCount)
    {



        using (var context = new ApplicationDbContext(_options))
        {
            var service = new ProjectService(context);
            service.CreateAsync(new Services.DTOs.ProjectFormDto
            {
                Name = ""
            }, "");

            context.SaveChanges(); // Сохраняем изменения в базе данных

        }




        using (var context = new ApplicationDbContext(_options))
        {
            Assert.That(context.Projects.Count(), Is.EqualTo(expectedCount), $"Shoud be {expectedCount}, now {context.Projects.Count()}");
        }

    }
    #endregion


}