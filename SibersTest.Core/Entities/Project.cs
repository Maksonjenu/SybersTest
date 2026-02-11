namespace SibersTest.Core.Entities
{
    /// <summary>
    /// Model representing a project in the system, containing details such as name, 
    /// customer and executor companies, start and end dates, priority, manager, and associated employees.
    /// </summary>
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CustomerCompany { get; set; }
        public string ExecutorCompany { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public int? ManagerId { get; set; }
        public virtual Employee Manager { get; set; }
        // Navigation property for the many-to-many relationship with employees
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public string? AttachedFiles { get; set; }
    }
}
