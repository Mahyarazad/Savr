using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Savr.Application.Abstractions;
using Savr.Application.Features.Listings.Commands;
using Savr.Application.Features.Products.Commands.UpdateProduct;
using Savr.Application.Features.Products.Queries;
using Savr.Presentation.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Savr.Presentation.Controllers
{
    [ApiController]
    //[Authorize(Roles = "Admin,Merchant")]
    [Route("api/v1/listing")]
    public class ListingController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IDistributedCache _distributedCache;
        //private readonly IDataProtectionProvider _dataProtectionProvider;

        public ListingController(IHttpContextAccessor httpContextAccessor, ISender sender)
        {
            _sender = sender;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpPost("create")]
        public async Task<IActionResult> AddProduct(CreateListingCommand command, CancellationToken cancellationToken)
        {
            return await ControllerActionExecutor.SafeExecute(
                () => _sender.Send(command, cancellationToken),
                result => Created("/", result)
            );
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct(UpdateListingCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            if(result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(ResultErrorParser.ParseResultError(result.Errors));
        }

        [AllowAnonymous]
        [HttpPost("get-listing")]
        public async Task<IActionResult> GetProductList(
            [FromQuery, Required] int pageNumber,
            [FromQuery, Required] int pageSize,
            [FromBody] IEnumerable<SqlFilter> filters,
            CancellationToken cancellationToken)
        {
            var ipConfig = HttpContext.Connection.RemoteIpAddress;
            var ip = HttpContext.Request.Headers;
            var sessionId = HttpContext.Request.Cookies[".AspNetCore.Session"];
            if(!string.IsNullOrEmpty(sessionId))
            {
                var protectedData = Convert.FromBase64String(Pad(sessionId));

                //var unprotectedData = _dataProtector.Unprotect(protectedData);

                //var humanReadableData = System.Text.Encoding.UTF8.GetString(unprotectedData);
                //var sessionData = await _distributedCache.GetStringAsync(humanReadableData);
                // sessionData will contain the data stored in Redis for the session
            }

            var result = await _sender.Send(new GetListingListQuery(pageNumber, pageSize, filters), cancellationToken);
            return Ok(result);
        }

        [HttpDelete("soft-delete")]
        public async Task<IActionResult> DeleteProduct(DeleteListingCommand command, CancellationToken cancellationToken)
        {
            var result =  await _sender.Send(command, cancellationToken);
            if(result.IsSuccess)
            {
                return Ok(HttpStatusCode.Accepted);
            }

            if(result.HasError(x => x.Message == HttpStatusCode.Unauthorized.ToString()))
            {
                return Unauthorized();
            }

            if(result.HasError(x => x.Message == HttpStatusCode.Forbidden.ToString()))
            {
                return Forbid();
            }

            if(result.HasError(x => x.Message == HttpStatusCode.NotFound.ToString()))
            {
                return NotFound();
            }

            return BadRequest(result.Errors);
        }

        private string Pad(string text)
        {
            var padding = 3 - ((text.Length + 3) % 4);
            if(padding == 0)
            {
                return text;
            }
            return text + new string('=', padding);
        }
    }
}
