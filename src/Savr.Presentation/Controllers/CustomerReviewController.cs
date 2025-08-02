using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savr.Application.Features.CustomerReview.Commands;
using Savr.Application.Features.CustomerReview.Queries;
using Serilog;


namespace Savr.Presentation.Controllers
{
    [ApiController]
    [Route("customer-review")]
    [Authorize(Roles = "User")]
    public class CustomerReviewController : ControllerBase
    {
        private readonly ISender _sender;

        public CustomerReviewController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReview(CreateCustomerReviewCommand command, CancellationToken cancellationToken)
        {
                var result = await _sender.Send(command, cancellationToken);
                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }

                return BadRequest(Helpers.ResultErrorParser.ParseResultError(result.Errors));
           
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateReview(UpdateCustomerReviewCommand command, CancellationToken cancellationToken)
        {
                var result = await _sender.Send(command, cancellationToken);

                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }

                return BadRequest(Helpers.ResultErrorParser.ParseResultError(result.Errors));
            
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllReviews([FromQuery] long listingId, CancellationToken cancellationToken)
        {
                var result = await _sender.Send(new GetReviewsByListingQuery(listingId), cancellationToken);
                return Ok(result.Value);
           
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteReview(DeleteCustomerReviewCommand command, CancellationToken cancellationToken)
        {
                var result = await _sender.Send(command, cancellationToken);
                if (result.IsSuccess)
                {
                    return Ok();
                }

                return BadRequest(Helpers.ResultErrorParser.ParseResultError(result.Errors));
          
        }
    }
}
