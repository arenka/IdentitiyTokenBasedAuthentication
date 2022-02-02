using IdentitiyTokenBasedAuthentication.Domain.Responses;
using IdentitiyTokenBasedAuthentication.Domain.Services;
using IdentitiyTokenBasedAuthentication.ResourceViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentitiyTokenBasedAuthentication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        [HttpGet]
        public IActionResult IsAuthentication()
        {
            return Ok(User.Identity.IsAuthenticated);
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(UserViewModel userViewModel)
        {
            BaseResponse<UserViewModel> response = await _authenticationService.SignUp(userViewModel);
            if (response.Success)
            {
                return Ok(response.Extra);
            }
            return BadRequest(response.Message);
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel signInViewModel)
        {
            var response = await _authenticationService.SignIn(signInViewModel);
            if (response.Success)
            {
                return Ok(response.Extra);
            }
            return BadRequest(response.Message);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAccessTokenByRefreshToken(RefreshTokenViewModel refreshTokenViewModel)
        {
            var response = await _authenticationService.CreateAccessTokenByRefreshToken(refreshTokenViewModel);
            if (response.Success)
            {
                return Ok(response.Extra);
            }
            return BadRequest(response.Message);
        }
        [HttpDelete]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenViewModel refreshTokenViewModel)
        {
            var response = await _authenticationService.RevokeRefreshToken(refreshTokenViewModel);
            if (response.Success)
            {
                return Ok();
            }
            return BadRequest(response.Message);
        }
    }
}
