using MediatR;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Projects;

namespace ProjectTaskManagement.Application.Features.Projects.Queries.GetAllProjects;

public record GetAllProjectsQuery : IRequest<ApiResponse<IEnumerable<ProjectDto>>>;
