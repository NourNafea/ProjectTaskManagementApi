using MediatR;
using ProjectTaskManagement.Application.Common;

namespace ProjectTaskManagement.Application.Features.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(Guid Id) : IRequest<ApiResponse>;
