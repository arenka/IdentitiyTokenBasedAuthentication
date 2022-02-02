using IdentitiyTokenBasedAuthentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentitiyTokenBasedAuthentication.Security.Token
{
    public interface ITokenHandler
    {
        AccessToken CreateAccessToken(AppUser user);

        void RevokeRefreshToken(AppUser user);
    }
}
