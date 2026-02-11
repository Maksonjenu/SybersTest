using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SibersTest.Services.DTOs
{

    public class ProjectEditDto
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
        public int ManagerId { get; set; } 

        public List<int> EmployeeIds { get; set; } = new List<int>();

    }
}