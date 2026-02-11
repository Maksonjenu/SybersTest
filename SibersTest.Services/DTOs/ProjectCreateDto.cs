using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SibersTest.Services.DTOs
{

    /// <summary>
    /// Not used for update, only for create, because of documents and managerId. 
    /// For update use ProjectFromDto
    /// </summary>
    public class ProjectCreateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string CustomerCompany { get; set; }
        [Required]
        public string ExecutorCompany { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Range(0, 100)]
        public int Priority { get; set; }

        [Required(ErrorMessage = "Please select a project manager")]
        public int ManagerId { get; set; } // Только ID!

        public ICollection<int> EmployeeIds { get; set; } = new List<int>(); // Список ID участников

        public ICollection<IFormFile>? Documents { get; set; } 
    }
}