using SibersTest.Core.Entities;
using SibersTest.Services.Interfaces;
using SibersTest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SibersTest.Services.DTOs;


namespace SibersTest.Services.Services
{


    /// <summary>
    /// Service for managing employees. Implements IEmployeeService interface and provides methods for CRUD operations and searching employees by name.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {


        #region Fields and constructor
        // Database context for accessing employee data.
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for EmployeeService. Initializes the service with the provided database context.
        /// </summary>
        /// <param name="context">The ApplicationDbContext instance to be used for employee data access.</param>
        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion


        #region IEmployeeService implementation


        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            return await _context.Employees
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FullName = $"{e.FirstName} {e.LastName} {e.Patronymic}",
                    Email = e.Email
                })
                .ToListAsync();
        }


        /// <summary>
        /// Create new employee from DTO.
        /// </summary>
        /// <param name="dto">Data transfer object with employee information.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throw if FullName is empty or null.</exception>
        public async Task CreateAsync(EmployeeFormDto dto)
        {

            ValidateFullName(dto.FullName);

            var (firstName, lastName, patronymic) = ParseFullName(dto.FullName);

            var employee = new Employee
            {
                FirstName = firstName,
                LastName = lastName,
                Patronymic = patronymic,
                Email = dto.Email
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmployeeFormDto dto)
        {

            ValidateFullName(dto.FullName);

            var employee = await _context.Employees.FindAsync(dto.Id);
            if (employee == null)
                throw new InvalidOperationException($"Employee with ID {dto.Id} not found.");

            var (firstName, lastName, patronymic) = ParseFullName(dto.FullName);

            employee.FirstName = firstName;
            employee.LastName = lastName;
            employee.Patronymic = patronymic;
            employee.Email = dto.Email;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<EmployeeDto>> SearchByNameAsync(string term)
        {
            var lowerTerm = term?.ToLower() ?? string.Empty;
            return await _context.Employees
            .Where(e => e.FirstName.ToLower().Contains(lowerTerm) || e.LastName.ToLower().Contains(lowerTerm) || e.Patronymic.ToLower().Contains(lowerTerm))
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FullName = $"{e.FirstName} {e.LastName} {e.Patronymic}",
                Email = e.Email
            })
            .ToListAsync();
        }

        public async Task<EmployeeDto> GetByIdAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return null;

            return new EmployeeDto
            {
                Id = employee.Id,
                FullName = $"{employee.FirstName} {employee.LastName} {employee.Patronymic}",
                Email = employee.Email
            };
        }


        #endregion


        #region Utility methods

        private (string firstName, string lastName, string patronymic) ParseFullName(string fullName)
        {
            var parts = fullName?.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            string firstName = parts.Length > 0 ? parts[0] : string.Empty;
            string lastName = parts.Length > 1 ? parts[1] : string.Empty;
            string patronymic = parts.Length > 2 ? string.Join(' ', parts.Skip(2)) : string.Empty;

            return (firstName, lastName, patronymic);
        }


        private void ValidateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("FullName cannot be empty.", nameof(fullName));
        }
        #endregion
    }
}