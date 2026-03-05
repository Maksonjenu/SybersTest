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


        using (var context = new ApplicationDbContext(_options))
        {
            var service = new EmployeeService(context);
            service.CreateAsync(new Services.DTOs.EmployeeFormDto
            {
                FullName = "Test Employee (Manager)",
                Email = "temp@mail.com"
            });

            service.CreateAsync(new Services.DTOs.EmployeeFormDto
            {
                FullName = "Test Employee (Worker 1)",
                Email = "temp@mail.com",
            });

            service.CreateAsync(new Services.DTOs.EmployeeFormDto
            {
                FullName = "Test Employee (Worker2)",
                Email = "temp@mail.com",
            });
            context.SaveChanges(); // Сохраняем изменения в базе данных

        }
    }

    #endregion

    #region Create project test


    [Test]
    [TestCase("Test Project", "Test Company", "Test Company", "1-1-1", "2-2-2", 0, 0, "Manager Full Name")]
    public async Task CreateProject_ShoudThrowException(
            string projectName,
            string customerCompany,
            string executorCompany,
            DateTime startDate,
            DateTime endDate,
            int priority,
            int managerId,
            string managerFullName
            )
    {

        using (var context = new ApplicationDbContext(_options))
        {
            var projectService = new ProjectService(context);
            var employeeService = new EmployeeService(context);

            List<EmployeeDto> employees = (List<EmployeeDto>)await employeeService.GetAllAsync();

            var ex = Assert.CatchAsync<Exception>(async () =>
                    {
                        await projectService.CreateAsync(new ProjectFormDto
                        {
                            Name = projectName,
                            CustomerCompany = customerCompany,
                            ExecutorCompany = executorCompany,
                            StartDate = startDate,
                            EndDate = endDate,
                            Priority = priority,
                            ManagerId = managerId,
                            Employees = employees,
                            ManagerFullName = managerFullName,
                        }, "");

                        context.SaveChanges(); // Сохраняем изменения в базе данных
                    });

            TestContext.WriteLine($"\nИсключение было поймано: {ex.Message}\n");
            TestContext.WriteLine($"\nТип исключения: {ex.GetType().Name}\n");
        }
    }


    [Test]
    [TestCase("Test Project", "Test Company", "Test Company", "1-1-1", "2-2-2", 0, 1, "Manager Full Name", 1)]
    [TestCase("Test Project", "Test Company", "Test Company", "1-1-1", "2-2-2", 0, 0, "Manager Full Name", 0)]
    [TestCase("Test Project", "Test Company", "Test Company", "2-2-2", "1-1-1", 0, 0, "Manager Full Name", 0)]
    [TestCase("", "Test Company", "Test Company", "1-1-1", "2-2-2", 0, 0, "Manager Full Name", 0)]
    [TestCase("Test Project", "", "Test Company", "1-1-1", "2-2-2", 0, 0, "Manager Full Name", 0)]
    [TestCase("Test Project", "Test Company", "", "1-1-1", "2-2-2", 0, 0, "Manager Full Name", 0)]

    public async Task CreateProject_ShoudSaveInDb(
        string projectName,
        string customerCompany,
        string executorCompany,
        DateTime startDate,
        DateTime endDate,
        int priority,
        int managerId,
        string managerFullName,
        int expectedCount
        )
    {

            


        using (var context = new ApplicationDbContext(_options))
        {


            var employeeService = new EmployeeService(context);

            List<EmployeeDto> employees = (List<EmployeeDto>)await employeeService.GetAllAsync();

            var service = new ProjectService(context);
            service.CreateAsync(new ProjectFormDto
            {
                Name = projectName,
                CustomerCompany = customerCompany,
                ExecutorCompany = executorCompany,
                StartDate = startDate,
                EndDate = endDate,
                Priority = priority,
                ManagerId = managerId,
                Employees = employees,
                ManagerFullName = managerFullName,
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