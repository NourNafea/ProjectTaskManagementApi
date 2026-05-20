using MediatR;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Admin;
using ProjectTaskManagement.Application.Interfaces;

namespace ProjectTaskManagement.Application.Features.Admin.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ApiResponse<IEnumerable<UserDto>>>
{
    private readonly IAdminService _adminService;

    public GetAllUsersQueryHandler(IAdminService adminService) => _adminService = adminService;

    public async Task<ApiResponse<IEnumerable<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _adminService.GetAllUsersAsync();
        return ApiResponse<IEnumerable<UserDto>>.Ok(users);
    }
}
