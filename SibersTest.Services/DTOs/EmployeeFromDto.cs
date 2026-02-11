
namespace SibersTest.Services.DTOs;
public class EmployeeFormDto {
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }


    public ICollection<int> ProjectIds { get; set; } = new List<int>();
    public ICollection<int> ManagedProjectIds { get; set; } = new List<int>();
}