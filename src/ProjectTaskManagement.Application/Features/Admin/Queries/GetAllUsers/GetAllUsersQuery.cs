using MediatR;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Admin;

namespace ProjectTaskManagement.Application.Features.Admin.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<ApiResponse<IEnumerable<UserDto>>>;
