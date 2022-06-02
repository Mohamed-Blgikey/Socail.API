using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Socail.BL.Authentcation;
using Socail.BL.Dtos;
using Socail.BL.Helper;

namespace Socail.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices auth;
        #region fields

        #endregion

        #region Ctor
        public AuthController(IAuthServices auth)
        {
            this.auth = auth;
        }
        #endregion

        #region Register
        [HttpPost]
        [Route("~/Register")]
        public async Task<IActionResult> Register(RegisterDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await auth.Register(dTO);

            if (!result.IsAuthencated)
                return Ok(new { message = result.Message });

            return Ok(new { message = result.Message, token = result.Token, expiresOn = result.ExpiresOn });
        }
        #endregion

        #region Login
        [HttpPost]
        [Route("~/Login")]
        public async Task<IActionResult> Login(LoginDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await auth.Login(dTO);

            if (!result.IsAuthencated)
                return Ok(new { message = result.Message });

            return Ok(new { message = result.Message, token = result.Token, expiresOn = result.ExpiresOn });
        }
        #endregion
    }
}
