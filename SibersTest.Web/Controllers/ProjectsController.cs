using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SibersTest.Services.DTOs;
using SibersTest.Services.Interfaces;
using SibersTest.Web.Models;

namespace SibersTest.Web.Controllers;

/// <summary>
/// Controller for managing projects. Provides actions for listing, creating, and editing projects.
/// </summary>
public class ProjectsController : Controller
{
    private readonly IProjectService _service;


    public ProjectsController(IProjectService service) { _service = service; }

    /// <summary>
    /// Page for listing projects. Supports optional search functionality to filter projects by name or other criteria.
    /// </summary>
    /// <param name="search">Part of the project name of included fields to search for.</param>
    /// <returns>Views the list of projects matching the search criteria.</returns>
    public async Task<IActionResult> Index(string search)
    {
        var projects = await _service.GetAllAsync(search);
        return View(projects);

    }


    /// <summary>
    /// Page for creating a new project. Accepts a ProjectFormDto model containing the details of the project to be created.
    /// </summary>
    /// <param name="model">Data for the new project to be created.</param>
    /// <returns>View with the form for creating a new project.</returns>
    public async Task<IActionResult> Create(ProjectFormDto model)
    {
        return View(model);
    }


    /// <summary>
    /// Page for editing an existing project. Accepts the ID of the project to be edited and retrieves its details to populate the form.
    /// </summary>
    /// <param name="id">The ID of the project to be edited.</param>
    /// <returns>View with the form for editing an existing project.</returns>
    public async Task<IActionResult> Edit(int id)
    {
        var data = await _service.GetByIdAsync(id);
        if (data == null) return NotFound();

        return View("Create", data);
    }
}
