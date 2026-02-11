using SibersTest.Core.Entities;
using SibersTest.Services.DTOs;


namespace SibersTest.Services.Interfaces
{

/// <summary>
/// Interface for project service, providing methods for managing projects, including retrieval, creation, updating, deletion, and searching by name.
/// </summary>
    public interface IProjectService
    {
        /// <summary>
        /// Asynchronously retrieves a list of projects, optionally filtered by a search term.
        /// </summary>
        /// <param name="search">Optional search term to filter projects by name.</param>
        /// <returns>Task that returns an enumerable collection of ProjectListItemDto objects.</returns>
        Task<IEnumerable<ProjectListItemDto>> GetAllAsync(string search);

        /// <summary>
        /// Asynchronously creates a new project based on the provided ProjectFormDto and saves any associated files to the specified root path.
        /// </summary>
        /// <param name="dto">The ProjectFormDto object containing the details of the project to be created.</param>
        /// <param name="rootPath">The root path where associated files should be saved.</param>
        /// <returns>Task that represents the asynchronous operation.</returns>
        Task CreateAsync(ProjectFormDto dto, string rootPath);

        /// <summary>
        /// Asynchronously updates an existing project identified by the provided ID using the details from the ProjectFormDto, and saves any associated files to the specified root path.
        /// </summary>
        /// <param name="dto">The ProjectFormDto object containing the updated details of the project.</param>
        /// <param name="id">The unique identifier of the project to be updated.</param>
        /// <param name="rootPath">The root path where associated files should be saved.</param>
        /// <returns>Task that represents the asynchronous operation.</returns>
        Task UpdateAsync(ProjectFormDto dto, int id, string rootPath);

        /// <summary>
        /// Asynchronously deletes a project identified by the provided ID.
        /// </summary>
        /// <param name="id">The unique identifier of the project to be deleted.</param>
        /// <returns>Task that represents the asynchronous operation.</returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// Asynchronously searches for projects by name using the provided search term and returns a list of matching ProjectListItemDto objects.
        /// </summary>
        /// <param name="term">The search term to match against project names.</param>
        /// <returns>Task that returns a list of ProjectListItemDto objects matching the search term, or null if no matches are found.</returns>
        Task<List<ProjectListItemDto>?> SearchByNameAsync(string term);

        /// <summary>
        /// Asynchronously retrieves a project by its unique identifier and returns a ProjectFormDto containing the project's details, or null if the project is not found.
        /// </summary>
        /// <param name="id">The unique identifier of the project to be retrieved.</param>
        /// <returns>Task that returns a ProjectFormDto object containing the project's details, or null if the project is not found.</returns>
        Task<ProjectFormDto> GetByIdAsync(int id);
    }

}