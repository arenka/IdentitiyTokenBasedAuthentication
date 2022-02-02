using IdentitiyTokenBasedAuthentication.Domain.Services;
using IdentitiyTokenBasedAuthentication.Models;
using IdentitiyTokenBasedAuthentication.ResourceViewModel;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IdentitiyTokenBasedAuthentication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase, IActionFilter
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            AppUser user = await _userService.GetUserByUserName(User.Identity.Name);
            return Ok(user.Adapt<UserViewModel>());
        }
        [HttpGet]
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }
        [HttpGet]
        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.ModelState.Remove("Password");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserViewModel userViewModel)
        {
            var response = await _userService.UpdateUser(userViewModel, User.Identity.Name);
            if (response.Success)
            {
                return Ok(response.Extra);
            }
            return BadRequest(response.Message);
        }
        [HttpPost]
        public async Task<IActionResult> UploadUserPicture(IFormFile picture)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory() + "wwwroot/UserPictures", fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await picture.CopyToAsync(stream);
            }
            var result = new
            {
                path = "https//" + Request.Host + "/UserPictures" + fileName
            };

            var response = await _userService.UploadUserImage(path, User.Identity.Name);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response.Message);
        }



    }
}
