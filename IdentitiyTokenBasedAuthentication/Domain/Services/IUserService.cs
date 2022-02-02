using IdentitiyTokenBasedAuthentication.Domain.Responses;
using IdentitiyTokenBasedAuthentication.Models;
using IdentitiyTokenBasedAuthentication.ResourceViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentitiyTokenBasedAuthentication.Domain.Services
{
    public interface IUserService
    {
        Task<BaseResponse<UserViewModel>> UpdateUser(UserViewModel userViewModel, string userName);
        Task<AppUser> GetUserByUserName(string userName);
        Task<BaseResponse<AppUser>> UploadUserImage(string picturePath, string userName);
        Task<Tuple<AppUser, IList<Claim>>> GetUserByRefreshToken(string refreshToken);
        Task<bool> RevokeRefreshToken(string refreshToken);
    }
}
