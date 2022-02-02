using IdentitiyTokenBasedAuthentication.Domain.Responses;
using IdentitiyTokenBasedAuthentication.Domain.Services;
using IdentitiyTokenBasedAuthentication.Models;
using IdentitiyTokenBasedAuthentication.ResourceViewModel;
using IdentitiyTokenBasedAuthentication.Security.Token;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentitiyTokenBasedAuthentication.Services
{
    public class AuthenticationService : BaseService, IAuthenticationService
    {
        private readonly ITokenHandler _tokenHandler;
        private readonly CustomTokenOptions _customTokenOptions;
        private readonly IUserService _userService;
        public AuthenticationService(IUserService userService, ITokenHandler tokenHandler, IOptions<CustomTokenOptions> customTokenOptions, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager) : base(userManager, signInManager, roleManager)
        {
            _tokenHandler = tokenHandler;
            _customTokenOptions = customTokenOptions.Value;
            _userService = userService;
        }

        public async Task<BaseResponse<AccessToken>> CreateAccessTokenByRefreshToken(RefreshTokenViewModel refreshTokenViewModel)
        {
            var userClaim = await _userService.GetUserByRefreshToken(refreshTokenViewModel.RefteshToken);
            if (userClaim.Item1 != null)
            {
                AccessToken accessToken = _tokenHandler.CreateAccessToken(userClaim.Item1);
                Claim refreshTokenClaims = new Claim("refreshToken", accessToken.RefreshToken);
                Claim refreshTokenEndDate = new Claim("refreshTokenEndDate", DateTime.Now.AddMinutes(_customTokenOptions.RefreshTokenExpiration).ToString());

                await _userManager.ReplaceClaimAsync(userClaim.Item1, userClaim.Item2[0], refreshTokenClaims);
                await _userManager.ReplaceClaimAsync(userClaim.Item1, userClaim.Item2[1], refreshTokenEndDate);
                return new BaseResponse<AccessToken>(accessToken);
            }
            return new BaseResponse<AccessToken>("Kullanıcı bulunamadı.");
        }

        public async Task<BaseResponse<AccessToken>> RevokeRefreshToken(RefreshTokenViewModel refreshTokenViewModel)
        {
            bool result = await _userService.RevokeRefreshToken(refreshTokenViewModel.RefteshToken);
            if (result)
            {
                return new BaseResponse<AccessToken>(new AccessToken());
            }
            return new BaseResponse<AccessToken>("RefreshToken bulunamadı.");
        }

        public async Task<BaseResponse<AccessToken>> SignIn(SignInViewModel signInViewModel)
        {
            AppUser user = await _userManager.FindByEmailAsync(signInViewModel.Email);
            if (user != null)
            {
                bool isUser = await _userManager.CheckPasswordAsync(user, signInViewModel.Password);
                if (isUser)
                {
                    AccessToken accessToken = _tokenHandler.CreateAccessToken(user);
                    Claim refreshTokenClaims = new Claim("refreshToken", accessToken.RefreshToken);
                    Claim refreshTokenEndDate = new Claim("refreshTokenEndDate", DateTime.Now.AddMinutes(_customTokenOptions.RefreshTokenExpiration).ToString());

                    List<Claim> refreshClaimList = _userManager.GetClaimsAsync(user).Result.Where(x => x.Type.Contains("refreshToken")).ToList();

                    if (refreshClaimList.Any())
                    {
                        await _userManager.ReplaceClaimAsync(user, refreshClaimList[0], refreshTokenClaims);
                        await _userManager.ReplaceClaimAsync(user, refreshClaimList[1], refreshTokenEndDate);
                    }
                    else
                    {
                        await _userManager.AddClaimsAsync(user, new[] { refreshTokenClaims, refreshTokenEndDate });
                    }
                    return new BaseResponse<AccessToken>(accessToken);
                }
                return new BaseResponse<AccessToken>("Email veya Şifre hatalıdır.");

            }
            return new BaseResponse<AccessToken>("Kullanıcı Bulunamadı.");
        }

        public async Task<BaseResponse<UserViewModel>> SignUp(UserViewModel userViewModel)
        {
            AppUser user = new AppUser
            {
                UserName = userViewModel.UserName,
                Email = userViewModel.Email,
            };
            IdentityResult result = await _userManager.CreateAsync(user,userViewModel.Password);
            if (result.Succeeded)
            {
                return new BaseResponse<UserViewModel>(user.Adapt<UserViewModel>());
            }
            return new BaseResponse<UserViewModel>(result.Errors.First().Description);
        }
    }
}
