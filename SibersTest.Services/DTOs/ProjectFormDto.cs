using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SibersTest.Services.DTOs
{
    /// <summary>
    /// Universal DTO for Project Wizard (Create, Edit and Display)
    /// </summary>
    public class ProjectFormDto
    {
        // 0 or null for Create mode, specific ID for Edit mode
        public int Id { get; set; }

        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Customer company is required")]
        public string CustomerCompany { get; set; }

        [Required(ErrorMessage = "Executor company is required")]
        public string ExecutorCompany { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Range(0, 100, ErrorMessage = "Priority must be between 0 and 100")]
        public int Priority { get; set; }

        [Required(ErrorMessage = "Please select a project manager")]
        public int ManagerId { get; set; }


        // Full name for display on the summary or edit steps
        public string? ManagerFullName { get; set; }

        public List<int> EmployeeIds { get; set; } = new List<int>();

        public ICollection<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();

        // Optional count for easy display in UI
        public int EmployeeCount => Employees?.Count ?? 0;

        // --- File Handling ---

        public ICollection<IFormFile>? Documents { get; set; }

        public ICollection<string> ExistingFiles { get; set; } = new List<string>();
    }
}