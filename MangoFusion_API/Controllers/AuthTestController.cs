using MangoFusion_API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MangoFusion_API.Controllers
{
    /// <summary>
    /// To make this authentication in working state we need to configure it 
    /// meaning we need to add 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthTestController : Controller
    {
        [HttpGet]
        [Authorize]
        public ActionResult<string> GetSomeValues()
        {
            return "Your are authroized";
        }

        [HttpGet("{someValue:int}")]
        [Authorize(Roles =StaticDetailForRoles.Role_Admin)]
        public ActionResult<string> GetSomeValues(int someValue)
        {
            return "Your are authroized";
        }
    }
}
