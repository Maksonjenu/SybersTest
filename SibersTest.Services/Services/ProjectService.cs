using SibersTest.Core.Entities;
using SibersTest.Services.Interfaces;
using SibersTest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SibersTest.Services.DTOs;


namespace SibersTest.Services.Services
{

    public class ProjectService : IProjectService
    {

        #region Fields and constructor

        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion


        public async Task CreateAsync(ProjectFormDto dto, string rootPath)
        {
            string uploadsPath = Path.Combine(rootPath, "uploads");
            var project = new Project
            {
                Name = dto.Name,
                CustomerCompany = dto.CustomerCompany,
                ExecutorCompany = dto.ExecutorCompany,
                ManagerId = dto.ManagerId,
                StartDate = (DateTime)dto.StartDate,
                EndDate = (DateTime)dto.EndDate,
                Priority = dto.Priority,

                Employees = await _context.Employees.Where(e => dto.Employees.Select(emp => emp.Id).Contains(e.Id)).ToListAsync()
            };

            // File save logic with rootPath
            if (dto.Documents != null && dto.Documents.Any())
            {
                var uploadedFiles = new List<string>();


                if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

                foreach (var file in dto.Documents)
                {

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    var filePath = Path.Combine(uploadsPath, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    uploadedFiles.Add(uniqueFileName);
                }


                project.AttachedFiles = string.Join(";", uploadedFiles);
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();


        }

        public Task DeleteAsync(int id)
        {
            var project = _context.Projects.FirstOrDefault(p => p.Id == id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                return _context.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ProjectListItemDto>> GetAllAsync(string search)
        {
            var query = _context.Projects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(s) ||
                    p.CustomerCompany.ToLower().Contains(s) ||
                    p.ExecutorCompany.ToLower().Contains(s));
            }

            var projectsData = await query
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.CustomerCompany,
                    p.ExecutorCompany,

                    MFirstName = p.Manager != null ? p.Manager.FirstName : "Not",
                    MLastName = p.Manager != null ? p.Manager.LastName : "Assigned",
                    MPatronymic = p.Manager != null ? p.Manager.Patronymic : "",

                    p.StartDate,
                    p.EndDate,
                    p.Priority,

                    EmployeeCount = p.Employees != null ? p.Employees.Count() : 0,
                    ManagerId = p.Manager != null ? (int?)p.Manager.Id : null,
                    RawFiles = p.AttachedFiles ?? ""
                })
                .ToListAsync();

            return projectsData.Select(p => new ProjectListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                CustomerCompany = p.CustomerCompany ?? "",
                ExecutorCompany = p.ExecutorCompany ?? "",
                ManagerFullName = $"{p.MFirstName} {p.MLastName} {p.MPatronymic}".Trim(),
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Priority = p.Priority,
                EmployeeCount = p.EmployeeCount,
                ManagerId = p.ManagerId,
                Files = !string.IsNullOrEmpty(p.RawFiles)
                    ? p.RawFiles.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                    : new List<string>()
            }).ToList();
        }

        public async Task<ProjectFormDto> GetByIdAsync(int id)
        {



            var project = await _context.Projects
                    .Include(p => p.Employees)
                    .Include(p => p.Manager)
                    .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return null;


            return new ProjectFormDto
            {
                Id = project.Id,
                Name = project.Name,
                CustomerCompany = project.CustomerCompany,
                ExecutorCompany = project.ExecutorCompany,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Priority = project.Priority,

                ManagerId = project.ManagerId ?? 0,
                ManagerFullName = project.Manager != null
                    ? $"{project.Manager.FirstName} {project.Manager.LastName}"
                    : "Не назначен",

                Employees = project.Employees.Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FullName = $"{e.FirstName} {e.LastName}",
                    Email = e.Email
                }).ToList()
            };


        }

        public Task<List<ProjectListItemDto>?> SearchByNameAsync(string term)
        {
            return _context.Projects
            .Where(p => p.Name.ToLower().Contains(term.ToLower()))
            .Select(p => new ProjectListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                CustomerCompany = p.CustomerCompany,
                ExecutorCompany = p.ExecutorCompany,
                ManagerFullName = $"{p.Manager.FirstName} {p.Manager.LastName} {p.Manager.Patronymic}",
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Priority = p.Priority,
                EmployeeCount = p.Employees.Count()
            })
            .ToListAsync();
        }

        public async Task UpdateAsync(ProjectFormDto dto, int id, string webRootPath)
        {
            var project = await _context.Projects
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return;

            project.Name = dto.Name;
            project.CustomerCompany = dto.CustomerCompany;
            project.ExecutorCompany = dto.ExecutorCompany;
            project.ManagerId = dto.ManagerId;
            project.StartDate = dto.StartDate;
            project.EndDate = (DateTime)dto.EndDate;
            project.Priority = dto.Priority;

            project.Employees.Clear();

            if (dto.EmployeeIds != null && dto.EmployeeIds.Any())
            {
                var selectedEmployees = await _context.Employees
                    .Where(e => dto.EmployeeIds.Contains(e.Id))
                    .ToListAsync();

                foreach (var emp in selectedEmployees)
                {
                    project.Employees.Add(emp);
                }
            }



            if (dto.Documents != null && dto.Documents.Any())
            {
                var uploadDir = Path.Combine(webRootPath, "uploads");
                if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                List<string> newFileNames = new List<string>();

                foreach (var file in dto.Documents)
                {
                    var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    var filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    newFileNames.Add(fileName);
                }

                string oldFiles = project.AttachedFiles ?? "";
                string addedFiles = string.Join(";", newFileNames);

                project.AttachedFiles = string.IsNullOrEmpty(oldFiles)
                    ? addedFiles
                    : oldFiles.TrimEnd(';') + ";" + addedFiles;
            }

            await _context.SaveChangesAsync();
        }
    }

}