using Microsoft.AspNetCore.Identity;
using ProjectTaskManagement.Domain.Entities;

namespace ProjectTaskManagement.Infrastructure.Persistence;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
