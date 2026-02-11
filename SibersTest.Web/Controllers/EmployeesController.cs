using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SibersTest.Services.DTOs;
using SibersTest.Services.Interfaces;
using SibersTest.Web.Models;

namespace SibersTest.Web.Controllers;

/// <summary>
/// Controller for employees views. Handles requests related to employee management, such as displaying the list of employees and creating new employees. Utilizes the IEmployeeService to perform operations on employee data.
/// </summary>
public class EmployeesController : Controller
{
    private readonly IEmployeeService _service;

    /// <summary>
    /// Constructor for EmployeesController. Initializes the controller with the specified employee service.
    /// </summary>
    /// <param name="service">The employee service instance to use for managing employees.</param>
    public EmployeesController(IEmployeeService service) { _service = service; }

    /// <summary>
    /// Index page with list of all employees. Retrieves the employee data from the service and passes it to the view for display.
    /// </summary>
    /// <returns>Page with list of employees</returns>
    public async Task<IActionResult> Index()
    {
        var data = await _service.GetAllAsync();
        return View(data);

    }

    /// <summary>
    /// Page for creating a new employee. Displays a form for entering employee details and handles the form submission to create a new employee using the service.
    /// </summary>
    /// <returns>View with employee creation form</returns>
    [HttpGet]
    public IActionResult Create()
    {
        var model = new EmployeeFormDto();
        return View(model);
    }

    /// <summary>
    /// Page for handling the submission of the employee creation form. Validates the form data and, if valid, creates a new employee using the service. If the form data is invalid, redisplays the form with validation errors.
    /// </summary>
    /// <param name="dto">The form DTO containing employee data to create.</param>
    /// <returns>Redirects to the Index page if successful, otherwise returns the view with validation errors.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(EmployeeFormDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        await _service.CreateAsync(dto);
        return RedirectToAction("Index");
    }

}
