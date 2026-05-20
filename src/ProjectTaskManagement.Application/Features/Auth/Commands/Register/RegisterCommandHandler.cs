using MediatR;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Auth;
using ProjectTaskManagement.Application.Interfaces;

namespace ProjectTaskManagement.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<AuthResponseDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterCommandHandler(IIdentityService identityService, IJwtTokenService jwtTokenService)
    {
        _identityService = identityService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<ApiResponse<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, errors, userId, email, fullName, roles) = await _identityService.RegisterAsync(request.FullName, request.Email, request.Password);

        if (!succeeded)
            return ApiResponse<AuthResponseDto>.Fail("Registration failed.", errors);

        var token = _jwtTokenService.GenerateToken(userId, email, fullName, roles);
        return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto(token, email, fullName, roles), "Registration successful.");
    }
}
