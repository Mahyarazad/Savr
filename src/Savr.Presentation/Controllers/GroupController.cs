using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Savr.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/group")]
    public class GroupController : ControllerBase
    {
    }
}
