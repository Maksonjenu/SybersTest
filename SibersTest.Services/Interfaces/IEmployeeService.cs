using SibersTest.Services.DTOs;


namespace SibersTest.Services.Interfaces
{

    /// <summary>
    /// Interface for employee service, which defines methods for managing employees, such as creating, updating, deleting, and retrieving employee information. It also includes a method for searching employees by name.
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Asynchronously retrieves a list of all employees. This method returns an enumerable collection of EmployeeDto objects, which contain the details of each employee. The method is designed to be used in scenarios where you need to display or process a list of all employees in the system.
        /// </summary>
        /// <returns>An enumerable collection of EmployeeDto objects.</returns>
        Task<IEnumerable<EmployeeDto>> GetAllAsync();

        /// <summary>
        /// Asynchronously creates a new employee based on the provided EmployeeFormDto object. This method takes in the details of the employee to be created, such as their name, position, and other relevant information encapsulated in the EmployeeFormDto. Upon successful creation, the new employee will be added to the system. This method is typically used when adding new employees through a user interface or an API endpoint.
        /// </summary>
        /// <param name="dto">EmployeeFormDto object containing the details of the employee to be created.</param>
        /// <returns>Task that represents the asynchronous operation.</returns>
        Task CreateAsync(EmployeeFormDto dto);

        /// <summary>
        /// Asynchronously updates an existing employee's information based on the provided EmployeeFormDto object. This method takes in the updated details of the employee, such as their name, position, and other relevant information encapsulated in the EmployeeFormDto. The method will locate the existing employee record in the system and update it with the new information. This is typically used when editing employee details through a user interface or an API endpoint.
        /// </summary>
        /// <param name="dto">EmployeeFormDto object containing the updated details of the employee.</param>
        /// <returns>Task that represents the asynchronous operation.</returns>
        Task UpdateAsync(EmployeeFormDto dto);

        /// <summary>
        /// Asynchronously deletes an employee from the system based on the provided employee ID. This method takes in the unique identifier of the employee to be deleted and removes the corresponding record from the system. This is typically used when an employee leaves the organization or when their information needs to be removed for any reason. The method ensures that the specified employee is properly deleted from the database or data store.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to be deleted.</param>
        /// <returns>Task that represents the asynchronous operation.</returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// Asynchronously searches for employees by their name based on the provided search term. This method takes in a string term, which is used to match against the names of employees in the system. The method returns an enumerable collection of EmployeeDto objects that match the search criteria. This is typically used in scenarios where you want to find specific employees by their name, such as in a search functionality within a user interface or an API endpoint.
        /// </summary>
        /// <param name="term">Search term to match against employee names.</param>
        /// <returns>Task that returns an enumerable collection of EmployeeDto objects matching the search criteria.</returns>
        Task<IEnumerable<EmployeeDto>> SearchByNameAsync(string term);

        /// <summary>
        /// Asynchronously retrieves an employee's details based on the provided employee ID. This method takes in the unique identifier of the employee and returns an EmployeeDto object containing the details of the specified employee. This is typically used when you need to view or edit the information of a specific employee, such as in a user interface or an API endpoint that displays employee details.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to retrieve.</param>
        /// <returns>Task that returns an EmployeeDto object containing the employee's details.</returns>
        Task<EmployeeDto> GetByIdAsync(int id);
    }

}