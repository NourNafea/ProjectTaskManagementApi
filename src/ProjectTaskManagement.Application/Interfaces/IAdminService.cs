using ProjectTaskManagement.Application.DTOs.Admin;

namespace ProjectTaskManagement.Application.Interfaces;

public interface IAdminService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
}
