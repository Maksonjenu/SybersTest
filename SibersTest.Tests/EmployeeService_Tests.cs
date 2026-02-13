using Microsoft.EntityFrameworkCore;
using SibersTest.Core.Entities;
using SibersTest.Infrastructure.Data;
using SibersTest.Services.Services;

namespace SibersTest.Tests;

[TestFixture]
public class EmployeeService_Tests
{

    private DbContextOptions<ApplicationDbContext> _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
    }

    [Test]
    [TestCase("John Doe", "john.doe@example.com", 1)]
    [TestCase("John ", "john.doe@example.com", 0)]
    [TestCase("John", "john.doe@example.com", 0)]
    [TestCase("", "", 0)]
    [TestCase("John Doe", "", 0)]
    [TestCase("", "john.doe@example.com", 0)]
    [TestCase("", "johndoeexample.com", 0)]
    [TestCase("", "johndoe@examplecom", 0)]
    [TestCase("", "@.", 0)]
    // Дополнительные кейсы
    [TestCase("Иван Иванов Иванович", "ivan.ivanov@example.com", 1)]
    [TestCase("Иван Иванов", "ivan.ivanov@example.com", 1)]
    [TestCase("Ivan Ivanov", "ivanov@", 0)]
    [TestCase("Ivan Ivanov", "ivanov@domain", 0)]
    [TestCase("Ivan Ivanov", "ivanov@domain.", 0)]
    [TestCase("Ivan Ivanov", "ivanov@.com", 0)]
    [TestCase("Ivan Ivanov", "ivanov@domain.c", 1)]
    [TestCase("Ivan Ivanov", "ivanov@domain.co.uk", 1)]
    [TestCase("Ivan Ivanov", "ivanov@domain..com", 0)]


    public async Task CreateEmployee_ShoudSaveInDb(string fullName, string email, int expectedCount)

    {
        using (var context = new ApplicationDbContext(_options))
        {
            var service = new EmployeeService(context);
            service.CreateAsync(new Services.DTOs.EmployeeFormDto
            {
                FullName = fullName,
                Email = email
            });

            context.SaveChanges(); // Сохраняем изменения в базе данных

        }
        using (var context = new ApplicationDbContext(_options))
        {
            Assert.That(context.Employees.Count(), Is.EqualTo(expectedCount));
        }

    }
}
