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
            try
            {
                var result = await _sender.Send(command, cancellationToken);
                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }

                return BadRequest(Helpers.ResultErrorParser.ParseResultError(result.Errors));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CreateReview failed unexpectedly.");
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateReview(UpdateCustomerReviewCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _sender.Send(command, cancellationToken);

                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }

                return BadRequest(Helpers.ResultErrorParser.ParseResultError(result.Errors));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UpdateReview failed unexpectedly.");
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllReviews([FromQuery] long listingId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _sender.Send(new GetReviewsByListingQuery(listingId), cancellationToken);
                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetAllReviews failed unexpectedly.");
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteReview(DeleteCustomerReviewCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _sender.Send(command, cancellationToken);
                if (result.IsSuccess)
                {
                    return Ok();
                }

                return BadRequest(Helpers.ResultErrorParser.ParseResultError(result.Errors));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DeleteReview failed unexpectedly.");
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }
    }
}
