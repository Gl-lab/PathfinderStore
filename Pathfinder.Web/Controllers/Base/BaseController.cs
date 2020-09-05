using Microsoft.AspNetCore.Mvc;

namespace Pathfinder.Web.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BaseController : Controller
    {
    }
}
