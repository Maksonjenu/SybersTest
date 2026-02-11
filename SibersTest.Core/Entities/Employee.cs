namespace SibersTest.Core.Entities
{

    /// <summary>
    /// Employee entity represents an employee in the system. It contains properties for the employee's personal information and their relationships with projects.
    /// The Employee entity has a many-to-many relationship with the Project entity, allowing an employee to be associated with multiple projects and a project to have multiple employees.
    /// </summary>
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Patronymic { get; set; }
        public string Email { get; set; }

        //many to many 
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

        //many to one 
        public virtual ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
    }
}
