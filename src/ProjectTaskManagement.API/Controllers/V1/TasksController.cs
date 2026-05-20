using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTaskManagement.Application.DTOs.Tasks;
using ProjectTaskManagement.Application.Features.Tasks.Commands.CreateTask;
using ProjectTaskManagement.Application.Features.Tasks.Commands.DeleteTask;
using ProjectTaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using ProjectTaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

namespace ProjectTaskManagement.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator) => _mediator = mediator;

    [HttpGet("project/{projectId:guid}")]
    public async Task<IActionResult> GetByProject(Guid projectId)
    {
        var result = await _mediator.Send(new GetTasksByProjectQuery(projectId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskItemDto dto)
    {
        var result = await _mediator.Send(new CreateTaskCommand(dto.Title, dto.Description, dto.DueDate, dto.Priority, dto.ProjectId));
        return StatusCode(201, result);
    }

    [HttpPatch("{taskId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid taskId, [FromBody] UpdateTaskStatusDto dto)
    {
        var result = await _mediator.Send(new UpdateTaskStatusCommand(taskId, dto.Status));
        return Ok(result);
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> Delete(Guid taskId)
    {
        var result = await _mediator.Send(new DeleteTaskCommand(taskId));
        return Ok(result);
    }
}
