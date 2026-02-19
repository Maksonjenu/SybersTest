using Microsoft.EntityFrameworkCore;
using SibersTest.Core.Entities;
using SibersTest.Infrastructure.Data;
using SibersTest.Services.DTOs;
using SibersTest.Services.Services;

namespace SibersTest.Tests;

[TestFixture]
public class EmployeeService_Tests
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

    #region CreateEmployeeTests
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

    #endregion

    #region GetAllEmployeesTest

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

    #endregion

    #region SearchByNameTests

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

    #endregion

    #region UpdateEmployeeTest

/// <summary>
/// Test case for <see cref="EmployeeService.UpdateAsync(EmployeeFormDto)"/>.
/// Throws <see cref="ArgumentException"/> if the input is invalid.
/// Verify that the employee is updated correctly.
///
/// <param name="updateFullName">New full name</param>
/// <param name="updateEmail">New email</param>
/// <param name="shouldThrow">Indicates whether the test method should throw an exception.</param>
/// <param name="expectedFirst">Expected first name after update</param>
/// <param name="expectedLast">Expected last name after update</param>
/// <param name="expectedPatronymic">Expected patronymic after update</param>
/// <returns></returns>
    [Test]
    // invalid full name (single token)
    [TestCase("Jane", "jane.doe@example.com", true, "", "", "")]
    // valid two-part name
    [TestCase("Jane Doe", "jane.doe@example.com", false, "Jane", "Doe", "")]
    // valid three-part name
    [TestCase("Ivan Ivanov Ivanovich", "ivan.ivanov@example.com", false, "Ivan", "Ivanov", "Ivanovich")]
    // empty full name
    [TestCase("", "john.doe@example.com", true, "", "", "")]
    // trailing space only / invalid
    [TestCase("John ", "john.doe@example.com", true, "", "", "")]
    // invalid email
    [TestCase("John Doe", "invalid-email", true, "", "", "")]
    // russian two-part valid
    [TestCase("Мария Иванова", "maria.ivanova@example.com", false, "Мария", "Иванова", "")]
    // multiple spaces should be normalized
    [TestCase("John   Doe", "john.doe@example.com", false, "John", "Doe", "")]
    // hyphen and apostrophe
    [TestCase("Anna-Marie O'Neil", "anna.oneil@example.com", false, "Anna-Marie", "O'Neil", "")]
    // three-part with middle name separated by space
    [TestCase("Jean Luc Picard", "jean.picard@example.com", false, "Jean", "Luc", "Picard")]
    public async Task UpdateEmployee_ParametrizedTests(string updateFullName, string updateEmail, bool shouldThrow, string expectedFirst, string expectedLast, string expectedPatronymic)
     {
         // Arrange - seed one employee
         using (var context = new ApplicationDbContext(_options))
         {
             var employee = new Employee
             {
                 FirstName = "John",
                 LastName = "Doe",
                 Patronymic = "Smith",
                 Email = "john.doe@example.com"
             };
             context.Employees.Add(employee);
             context.SaveChanges();
         }
 
         using (var context = new ApplicationDbContext(_options))
         {
             var service = new EmployeeService(context);
             var existing = await service.GetByIdAsync(1);
 
             var dto = new EmployeeFormDto
             {
                 Id = existing.Id,
                 FullName = updateFullName,
                 Email = updateEmail
             };
 
             // Act & Assert
             if (shouldThrow)
             {
                 Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(dto));
             }
             else
             {
                 await service.UpdateAsync(dto);
                 var updated = await service.GetByIdAsync(existing.Id);

                string[] splittedNames = updated.FullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

                string firstName = splittedNames.Length > 0 ? splittedNames[0] : "";
                string lastName = splittedNames.Length > 1 ? splittedNames[1] : "";
                string patronymic = splittedNames.Length > 2 ? splittedNames[2] : "";

                 // Проверяем разделённые поля DTO
                Assert.That(firstName ?? "", Is.EqualTo(expectedFirst ?? ""));
                Assert.That(lastName ?? "", Is.EqualTo(expectedLast ?? ""));
                Assert.That(patronymic ?? "", Is.EqualTo(expectedPatronymic ?? ""));
                Assert.That(updated.Email, Is.EqualTo(updateEmail));

                // Дополнительно проверяем корректную склейку FullName (если поле присутствует в DTO)
                 var expectedFullName = string.Join(" ", new[] { expectedFirst, expectedLast, expectedPatronymic }.Where(s => !string.IsNullOrEmpty(s)));
                 if (updated.GetType().GetProperty("FullName") != null)
                 {
                     Assert.That(((updated.FullName ?? "").Trim()), Is.EqualTo(expectedFullName));
                 }
             }
         }
     }
 
     #endregion
 }