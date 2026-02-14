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


    [Test]
    [TestCase(2, "John", "Doe", "Smith", "john.doe@example.com", "Jane", "Smith", "Johnson", "jane.smith@example.com")]
    [TestCase(1, "Иван", "Иванов", "Иванович", "ivan.ivanov@example.com")]
    [TestCase(3, "Alex", "Brown", "", "alex.brown@example.com", "Maria", "White", "", "maria.white@example.com", "Sergey", "Petrov", "", "sergey.petrov@example.com")]
    [TestCase(0)]
    public async Task GetAllEmployees_ShouldReturnAllEmployees(int expectedCount,
        params string[] employeeData)
    {
        using (var context = new ApplicationDbContext(_options))
        {
            var employees = new List<Employee>();
            for (int i = 0; i + 3 < employeeData.Length; i += 4)
            {
                if (!string.IsNullOrEmpty(employeeData[i]))
                {
                    employees.Add(new Employee
                    {
                        FirstName = employeeData[i],
                        LastName = employeeData[i + 1],
                        Patronymic = employeeData[i + 2],
                        Email = employeeData[i + 3]
                    });
                }
            }
            context.Employees.AddRange(employees);
            context.SaveChanges();
        }

        using (var context = new ApplicationDbContext(_options))
        {
            var service = new EmployeeService(context);
            var employees = await service.GetAllAsync();
            Assert.That(employees.Count(), Is.EqualTo(expectedCount));
        }
    }

    [Test]
    [TestCase(1, "John", "John", "Doe", "Smith", "john.doe@example.com")]
    [TestCase(1, "john", "John", "Doe", "Smith", "john.doe@example.com")]
    [TestCase(
        2,
        "John", "John", "Doe", "Smith", "john.doe@example.com",
        "Jane", "Smith", "Johnson", "jane.smith@example.com")
    ]
    [TestCase(
        2,
        "Иван", "Иван", "Иванов", "Петрович", "ivan.ivanov@example.com",
        "Мария", "Мария", "Иванова", "Петровна", "maria.ivanova@example.com")
    ]
    public async Task SearchByName_ShouldReturnMatchingEmployees(int expectedCount,
        string searchName, params string[] employeeData)
    {
        using (var context = new ApplicationDbContext(_options))
        {
            var employees = new List<Employee>();
            for (int i = 0; i + 3 < employeeData.Length; i += 4)
            {
                if (!string.IsNullOrEmpty(employeeData[i]))
                {
                    employees.Add(new Employee
                    {
                        FirstName = employeeData[i],
                        LastName = employeeData[i + 1],
                        Patronymic = employeeData[i + 2],
                        Email = employeeData[i + 3]
                    });
                }
            }
            context.Employees.AddRange(employees);
            context.SaveChanges();
        }

        using (var context = new ApplicationDbContext(_options))
        {
            var service = new EmployeeService(context);
            var employees = await service.SearchByNameAsync(searchName);
            Assert.That(employees.Count(), Is.EqualTo(expectedCount));
        }
    }
}