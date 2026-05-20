using MediatR;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Auth;

namespace ProjectTaskManagement.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password)
    : IRequest<ApiResponse<AuthResponseDto>>;
