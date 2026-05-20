using MediatR;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Auth;

namespace ProjectTaskManagement.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string FullName, string Email, string Password)
    : IRequest<ApiResponse<AuthResponseDto>>;
