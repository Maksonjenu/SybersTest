namespace SibersTest.Services.DTOs
{

    /// <summary>
    /// DTO for listing projects with basic info, used in project list views.
    /// </summary>
    public class ProjectListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CustomerCompany { get; set; }
        public string ExecutorCompany { get; set; }
        public string ManagerFullName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public int EmployeeCount { get; set; } 
        public ICollection<int> EmployeeIds { get; set; } 

        // temporary field for linking to manager details, not ideal but works for now
        public int? ManagerId { get; set; }
        
        public ICollection<string> Files { get; set; }
    }
}