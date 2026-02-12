using Microsoft.EntityFrameworkCore;
using SibersTest.Core.Entities;
using SibersTest.Infrastructure.Data;
using SibersTest.Services.Services;

namespace SibersTest.Tests;

[TestFixture]
public class ServicesTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [TestCase("John Doe", "john.doe@example.com", 1)]
    public async Task CreateEmployee_ShoudSaveInDb(string fullName, string email, int expectedCount)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

        // 2. Юзаем контекст
        using (var context = new ApplicationDbContext(options))
        {
            var service = new EmployeeService(context);
            service.CreateAsync(new Services.DTOs.EmployeeFormDto
            {
                FullName = fullName,
                Email = email
            });

            context.SaveChanges(); // Сохраняем изменения в базе данных

        }
        // 3. Проверяем в новом контексте (имитируем новый запрос)
        using (var context = new ApplicationDbContext(options))
        {
            Assert.That(context.Employees.Count(), Is.EqualTo(expectedCount));
        }

    }
}
