using Microsoft.AspNetCore.Mvc;
using SibersTest.Services.DTOs;
using SibersTest.Services.Interfaces;

/// <summary>
/// API controller for managing employees. Provides endpoints for searching, retrieving, creating, updating, and deleting employee records.
/// </summary>
[Route("api/employees")]

public class EmployeesApiController : ControllerBase
{


    private readonly IEmployeeService _service;

    /// <summary>
    /// Constructor for EmployeesApiController. Initializes the controller with the provided employee service.
    /// </summary>
    /// <param name="service">The employee service to be used by this controller.</param>
    public EmployeesApiController(IEmployeeService service)
    {
        _service = service;
    }

    /// <summary>
    /// Searches for employees by their full name. The search term must be at least 4 characters long to avoid unnecessary database queries. Returns a list of employee DTOs containing the ID and full name of matching employees.
    /// </summary>
    /// <param name="term">The search term for employee full name.</param>
    /// <returns>A list of employee DTOs matching the search term.</returns>
    [HttpGet("search")]
    public async Task<IActionResult> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Length < 4)
            return Ok(new List<EmployeeDto>());

        var results = await _service.SearchByNameAsync(term);
        Console.WriteLine($"Search term: '{term}', Results count: {results.Count()}");
        return Ok(results ?? new List<EmployeeDto>());
    }

    /// <summary>
    /// Retrieves an employee by their ID. If the employee is found, returns the employee DTO; otherwise, returns a 404 Not Found response.
    /// </summary>
    /// <param name="id">The ID of the employee to retrieve.</param>
    /// <returns>The employee DTO if found, otherwise a 404 Not Found response.</returns>

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _service.GetByIdAsync(id);
        if (employee == null) return NotFound();
        return Ok(employee);
    }


    /// <summary>
    /// Deletes an employee by their ID. If the ID is invalid (less than or equal to 0), returns a 400 Bad Request response. If the deletion is successful, returns a 200 OK response; otherwise, returns a 500 Internal Server Error response in case of exceptions.
    /// </summary>
    /// <param name="id">The ID of the employee to delete.</param>
    /// <returns>A 200 OK response if deletion is successful, otherwise a 400 Bad Request or 500 Internal Server Error response.</returns>
    [HttpPost("delete/{id}")] 
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0) return BadRequest("Invalid ID.");

        try
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error.");
        }
    }

    /// <summary>
    /// Saves an employee record. If the DTO contains an ID greater than 0, it updates the existing employee; otherwise, it creates a new employee. Validates the model state and returns appropriate responses based on the success or failure of the operation.
    /// </summary>
    /// <param name="dto">The employee form DTO to save.</param>
    /// <returns>A 200 OK response if successful, otherwise a 400 Bad Request or 500 Internal Server Error response.</returns>

    [HttpPost("save")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Save([FromBody] EmployeeFormDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            if (dto.Id > 0)
            {
                await _service.UpdateAsync(dto);
            }
            else
            {
                await _service.CreateAsync(dto);
            }
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
