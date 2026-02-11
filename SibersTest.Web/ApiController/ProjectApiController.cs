using Microsoft.AspNetCore.Mvc;
using SibersTest.Services.DTOs;
using SibersTest.Services.Interfaces;


/// <summary>
/// API controller for managing projects. Provides endpoints for searching, creating, editing, and deleting projects.
/// </summary>
[Route("api/projects")]
public class ProjectsApiController : ControllerBase
{
    private readonly IProjectService _service;
    private readonly IWebHostEnvironment _env;


    /// <summary>
    /// Constructor for ProjectsApiController. Initializes the project service and web host environment.
    /// </summary>
    /// <param name="service">The project service instance to use for managing projects.</param>
    /// <param name="env">The web host environment instance to use for accessing web root path.</param>
    public ProjectsApiController(IProjectService service, IWebHostEnvironment env)
    {
        _service = service;
        _env = env;
    }

    /// <summary>
    /// Searches for projects by name. The search term must be at least 3 characters long to avoid unnecessary database load.
    /// </summary>
    /// <param name="term">The search term for project name.</param>
    /// <returns>A list of project list item DTOs matching the search term.</returns>
    [HttpGet("search")]
    public async Task<IActionResult> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Length < 3)
            return Ok(new List<ProjectListItemDto>());

        var results = await _service.SearchByNameAsync(term);
        Console.WriteLine($"Search term: '{term}', Results count: {results.Count()}");
        return Ok(results ?? new List<ProjectListItemDto>());
    }

    /// <summary>
    /// Creates a new project using the provided form data. The form data is expected to be sent as multipart/form-data, which allows for file uploads if necessary.
    /// </summary>
    /// <param name="dto">The form DTO containing project data to create.</param>
    /// <returns>Returns an OK response indicating successful creation of the project.</returns>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromForm] ProjectFormDto dto)
    {
        await _service.CreateAsync(dto, _env.WebRootPath);
        return Ok();
    }

    /// <summary>
    /// Deletes a specific project by id.
    /// </summary>
    /// <param name="id">Project unique identifier</param>
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
    /// Edits an existing project. The ID in the URL must match the ID in the form data.
    /// </summary>
    /// <param name="id">Unique identifier of the project to edit</param>
    /// <param name="dto">The form DTO containing updated project data</param>
    /// <returns></returns>
    [HttpPost("edit/{id}")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] ProjectFormDto dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");

        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {

            await _service.UpdateAsync(dto, id, _env.WebRootPath);

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
}
