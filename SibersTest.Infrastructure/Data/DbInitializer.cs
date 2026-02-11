using System.Data;
using Microsoft.EntityFrameworkCore;
using SibersTest.Core.Entities;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace SibersTest.Infrastructure.Data;

/// <summary>
/// Class responsible for initializing the database with seed data from embedded CSV files. It checks if the Employees and Projects tables are empty, and if so, it reads the data from the CSV files and populates the database. 
/// The class uses CsvHelper to map CSV columns to entity properties, and it ensures that all relationships are ignored during the mapping process to avoid issues with foreign keys.
/// </summary>
public class DbInitializer
{

    public static void Initialize(ApplicationDbContext context)
    {

        context.Database.Migrate();

        if (context.Employees.Any() || context.Projects.Any())
            return;


        InitEmployeesTable(context).Wait();

        InitProjectsTable(context).Wait();

        context.Database.Migrate();


    }

    #region Utility classes

    private static async Task InitEmployeesTable(ApplicationDbContext _context)
    {
        var assembly = typeof(DbInitializer).Assembly;

        var names = typeof(DbInitializer).Assembly.GetManifestResourceNames();
        Console.WriteLine("Embedded resources:");
        foreach (var name in names)
        {
            Console.WriteLine(name);
        }

        var resourceName = "SibersTest.Infrastructure.Data.SeedData.emp.csv";

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                throw new Exception($"Resource '{resourceName}' not found. Check Build Action: Embedded Resource.");
            }

            using (var reader = new StreamReader(stream))

            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<EmployeeMap>();
                var records = csv.GetRecords<Employee>(); 
                _context.Employees.AddRange(records);
                _context.SaveChanges();
            }
        }
    }

    private static async Task InitProjectsTable(ApplicationDbContext _context)
    {
        var assembly = typeof(DbInitializer).Assembly;

        var names = typeof(DbInitializer).Assembly.GetManifestResourceNames();
        Console.WriteLine("Embedded resources:");
        foreach (var name in names)
        {
            Console.WriteLine(name);
        }

        var resourceName = "SibersTest.Infrastructure.Data.SeedData.proj.csv"; 

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                throw new Exception($"Resource '{resourceName}' not found. Check Build Action: Embedded Resource.");
            }

            using (var reader = new StreamReader(stream))

            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Project>(); 
                csv.Context.RegisterClassMap<ProjectMap>();
                _context.Projects.AddRange(records);
                _context.SaveChanges();
            }
        }
    }


    #endregion

    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Map(m => m.Id).Ignore();
            Map(m => m.FirstName).Name("GivenName");
            Map(m => m.LastName).Name("Surname");
            Map(m => m.Email).Name("EmailAddress");
            Map(m => m.Patronymic).Ignore();

            Map(m => m.Projects).Ignore();
        }
    }

    public class ProjectMap : ClassMap<Project>
    {
        public ProjectMap()
        {
            Map(m => m.Id).Ignore();
            Map(m => m.Name).Name("Name");
            Map(m => m.CustomerCompany).Name("CustomerCompany");
            Map(m => m.ExecutorCompany).Name("ExecutorCompany");
            Map(m => m.StartDate).Name("StartDate");
            Map(m => m.EndDate).Name("EndDate");
            Map(m => m.Priority).Name("Priority");

            Map(m => m.Employees).Ignore();
            Map(m => m.Manager).Ignore();
        }
    }



}