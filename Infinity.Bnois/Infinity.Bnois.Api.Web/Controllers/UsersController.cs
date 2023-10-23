﻿using Infinity.Bnois.Api.Core;
using Infinity.Bnois.Api.Web.Data;
using Infinity.Bnois.Api.Web.Models;
using Infinity.Bnois.Api.Web.Services;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Infinity.Bnois.Api.Right;

namespace Infinity.Bnois.Api.Web.Controllers
{
    [RoutePrefix(IdentityRoutePrefix.Users)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.USERS)]

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UsersController'
    public class UsersController : BaseController
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UsersController'
    {
        private readonly IUserService userService;
        private readonly ILanguageService _languageService;
        private readonly UserManager userManager;
        private readonly UserStore userStore;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UsersController.UsersController(IUserService, ILanguageService)'
        public UsersController(IUserService userService, ILanguageService languageService)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UsersController.UsersController(IUserService, ILanguageService)'
        {
            this.userService = userService;
            this._languageService = languageService;
            this.userStore = new UserStore(new InfinityIdentityDbContext("Administration"));
            this.userManager = new UserManager(userStore);
        }

      
      
        [HttpGet]
        [Route("get-users")]
//        
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UsersController.GetUsers(int, int, string)'
        public IHttpActionResult GetUsers(int ps, int pn, string q)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UsersController.GetUsers(int, int, string)'
        {
            int total = 0;
            return Ok(new ResponseMessage<UserViewModel>
            {
                Result = new UserViewModel { Users = userService.GetUsers(ps, pn, out total), LoginUserId = base.UserId},
                Total = total
            });
        }


        [HttpGet]
        [Route("get-user")]
//        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UsersController.GetUser(string)'
        public IHttpActionResult GetUser(string userId)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UsersController.GetUser(string)'
        {
            return Ok(new ResponseMessage<UserViewModel>
            {
                Result = new UserViewModel
                {
                    User = userService.GetUser(userId),
                    Languages = _languageService.GetLanguages()
                }
            });
        }

        [HttpPost]
        [ModelValidation]
        [Route("save-user")]
//        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UsersController.SaveUser(UserModel)'
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<IHttpActionResult> SaveUser([FromBody] UserModel model)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UsersController.SaveUser(UserModel)'
        {
            bool status = userService.SaveUser(null, model);
//            if (status)
//            {
//                string message = "BNOIS USER :" + model.UserName + "\n" + " Password :" + model.Password;
//                await userManager.SendEmailAsync(model.Id, "BNOIS USER CREDENTIAL", message);
//                await userManager.SendSmsAsync(model.Id, message);
//            }
            return Ok(new ResponseMessage<bool>
            {
                Result = status
            });
        }

        [HttpPut]
        [Route("update-user/{userId}")]
//        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UsersController.UpdateUser(string, UserModel)'
        public IHttpActionResult UpdateUser(string userId, [FromBody] UserModel model)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UsersController.UpdateUser(string, UserModel)'
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = userService.SaveUser(userId, model)

            });
        }

        [HttpDelete]
//        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        [Route("delete-user/{userId}")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UsersController.DeleteUser(string)'
        public IHttpActionResult DeleteUser(string userId)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UsersController.DeleteUser(string)'
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = userService.DeleteUser(userId)
            });
        }

        [HttpGet]
//        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        [Route("get-user-roles/{userId}")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UsersController.GetUserRoles(string)'
        public IHttpActionResult GetUserRoles(string userId)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UsersController.GetUserRoles(string)'
        {
            return Ok(new ResponseMessage<UserViewModel>
            {
                Result = new UserViewModel
                {
                    User = userService.GetUser(userId),
                    UserRoles = userService.GetUserRoles(userId)
                }
            });
        }

        [HttpPost]
//        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        [Route("save-user-roles/{userId}")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UsersController.SaveUserRoles(string, UserRoleModel[])'
        public IHttpActionResult SaveUserRoles(string userId, UserRoleModel[] userRoles)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UsersController.SaveUserRoles(string, UserRoleModel[])'
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = userService.SaveUserRoles(userId, userRoles),
            });
        }

    }
}
