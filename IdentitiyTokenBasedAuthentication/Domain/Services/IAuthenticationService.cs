using IdentitiyTokenBasedAuthentication.Domain.Responses;
using IdentitiyTokenBasedAuthentication.ResourceViewModel;
using IdentitiyTokenBasedAuthentication.Security.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentitiyTokenBasedAuthentication.Domain.Services
{
    public interface IAuthenticationService
    {
        Task<BaseResponse<UserViewModel>> SignUp(UserViewModel userViewModel);
        Task<BaseResponse<AccessToken>> SignIn(SignInViewModel signInViewModel);
        Task<BaseResponse<AccessToken>> CreateAccessTokenByRefreshToken(RefreshTokenViewModel refreshTokenViewModel);
        Task<BaseResponse<AccessToken>> RevokeRefreshToken(RefreshTokenViewModel refreshTokenViewModel);
    }
}
