using IdentitiyTokenBasedAuthentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentitiyTokenBasedAuthentication.Services
{
    public class BaseService : ControllerBase
    {
        protected UserManager<AppUser> _userManager { get; }
        protected SignInManager<AppUser> _signInManager { get; }
        protected RoleManager<AppRole> _roleManager { get; }

        public BaseService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
    }
}
