using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savr.Application.Features.Group.Commands;
using Savr.Application.Features.Group.Queries;

namespace Savr.Presentation.Controllers
{
    [ApiController]
    //[Authorize(Roles = "Admin,Merchant")]
    [Route("api/v1/group")]
    public class GroupController : ControllerBase
    {
        private readonly ISender _sender;

        public GroupController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup(CreateGroupCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateGroup(UpdateGroupCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllGroups(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetAllGroupsQuery(), cancellationToken);
            return Ok(result.Value);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteGroup(DeleteGroupCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            if (result.IsSuccess) return Ok();
            return BadRequest(result.Errors);
        }
    }
}
