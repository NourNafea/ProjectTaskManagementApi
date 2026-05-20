using MediatR;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Projects;

namespace ProjectTaskManagement.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand(string Name, string Description)
    : IRequest<ApiResponse<ProjectDto>>;
