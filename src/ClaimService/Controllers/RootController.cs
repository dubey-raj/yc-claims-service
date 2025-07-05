using Microsoft.AspNetCore.Mvc;

namespace ClaimService.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Route("")]
    public class RootController : ControllerBase
    {
        [HttpGet]
        public RedirectResult Get() => Redirect("/swagger");

        /// <summary>
        /// Returns response for healthz query
        /// </summary>
        /// <returns>OK 200</returns>
        [Route("healthz")]
        [HttpGet]
        public ObjectResult GetHealth() => Ok("Success");
    }
}
