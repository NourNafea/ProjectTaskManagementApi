using Microsoft.AspNetCore.Identity;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Domain.Constants;
using ProjectTaskManagement.Infrastructure.Persistence;

namespace ProjectTaskManagement.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<(bool Succeeded, string[] Errors, string UserId, string Email, string FullName, IList<string> Roles)> RegisterAsync(
        string fullName, string email, string password)
    {
        var user = new ApplicationUser { FullName = fullName, Email = email, UserName = email };
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description).ToArray(), string.Empty, string.Empty, string.Empty, []);

        await _userManager.AddToRoleAsync(user, AppRoles.User);
        var roles = await _userManager.GetRolesAsync(user);
        return (true, [], user.Id, user.Email!, user.FullName, roles);
    }

    public async Task<(bool Succeeded, string UserId, string Email, string FullName, IList<string> Roles)> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return (false, string.Empty, string.Empty, string.Empty, []);

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        if (!result.Succeeded) return (false, string.Empty, string.Empty, string.Empty, []);

        var roles = await _userManager.GetRolesAsync(user);
        return (true, user.Id, user.Email!, user.FullName, roles);
    }
}
