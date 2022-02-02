using IdentitiyTokenBasedAuthentication.Domain.Responses;
using IdentitiyTokenBasedAuthentication.Domain.Services;
using IdentitiyTokenBasedAuthentication.Models;
using IdentitiyTokenBasedAuthentication.ResourceViewModel;
using Mapster;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentitiyTokenBasedAuthentication.Services
{
    public class UserService : BaseService, IUserService
    {
        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager) : base(userManager, signInManager, roleManager)
        {
        }

        public async Task<Tuple<AppUser, IList<Claim>>> GetUserByRefreshToken(string refreshToken)
        {
            Claim claimRefreshToken = new Claim("refreshToken", refreshToken);

            var users = await _userManager.GetUsersForClaimAsync(claimRefreshToken);
            if (users.Any())
            {
                var user = users.First();
                IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
                string refreshTokenEndDate = userClaims.First(c => c.Type == "refreshTokenEndDate").Value;
                if (DateTime.Parse(refreshTokenEndDate) > DateTime.Now)
                {
                    return new Tuple<AppUser, IList<Claim>>(user, userClaims);
                }
                else
                {
                    return new Tuple<AppUser, IList<Claim>>(null, null);
                }
            }
            return new Tuple<AppUser, IList<Claim>>(null, null);
        }

        public async Task<AppUser> GetUserByUserName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<bool> RevokeRefreshToken(string refreshToken)
        {
            var result = await GetUserByRefreshToken(refreshToken);
            if (result.Item1 == null) return false;

            IdentityResult response = await _userManager.RemoveClaimsAsync(result.Item1, result.Item2);
            if (response.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task<BaseResponse<UserViewModel>> UpdateUser(UserViewModel userViewModel, string userName)
        {
            AppUser user = await _userManager.FindByNameAsync(userName);

            if (_userManager.Users.Any(u => u.PhoneNumber == userViewModel.PhoneNumber))
            {
                return new BaseResponse<UserViewModel>("Telefon numarası başka bir üyeye aittir!");
            }

            user.BirthDay = userViewModel.BirthDay;
            user.City = userViewModel.City;
            user.Gender = (int)userViewModel.Gender;
            user.Email = userViewModel.Email;
            user.UserName = userViewModel.UserName;
            user.PhoneNumber = userViewModel.PhoneNumber;

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return new BaseResponse<UserViewModel>(user.Adapt<UserViewModel>());

            return new BaseResponse<UserViewModel>(result.Errors.First().Description);
        }

        public async Task<BaseResponse<AppUser>> UploadUserImage(string picturePath, string userName)
        {
            AppUser user = await _userManager.FindByNameAsync(userName);
            user.Picture = picturePath;
            IdentityResult result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new BaseResponse<AppUser>(user);
            }
            return new BaseResponse<AppUser>(result.Errors.First().Description);
        }
    }
}
