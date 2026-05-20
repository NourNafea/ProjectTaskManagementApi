using MediatR;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Auth;
using ProjectTaskManagement.Application.Interfaces;

namespace ProjectTaskManagement.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponseDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(IIdentityService identityService, IJwtTokenService jwtTokenService)
    {
        _identityService = identityService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<ApiResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, userId, email, fullName, roles) = await _identityService.LoginAsync(request.Email, request.Password);

        if (!succeeded)
            return ApiResponse<AuthResponseDto>.Fail("Invalid email or password.");

        var token = _jwtTokenService.GenerateToken(userId, email, fullName, roles);
        return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto(token, email, fullName, roles));
    }
}
