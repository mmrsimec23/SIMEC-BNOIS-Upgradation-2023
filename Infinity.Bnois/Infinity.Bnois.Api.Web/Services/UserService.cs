using Infinity.Bnois.Configuration;
using Infinity.Bnois.ExceptionHelper;
using Infinity.Bnois.Api.Web.Data;
using Infinity.Bnois.Api.Web.Models;
using Microsoft.AspNet.Identity;

using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace Infinity.Bnois.Api.Web.Services
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UserService'
    public class UserService : UserManager<User, string>, IUserService
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UserService'
    {
        private readonly UserStore userStore;
        private readonly IRoleService roleService;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UserService.UserService(UserStore, IRoleService)'
        public UserService(UserStore userStore, IRoleService roleService) : base(userStore)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UserService.UserService(UserStore, IRoleService)'
        {
            this.userStore = userStore;
            this.roleService = roleService;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UserService.GetUser(string)'
        public UserModel GetUser(string userId)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UserService.GetUser(string)'
        {
            if (!ConfigurationResolver.Get().LoggedInUser.UserRoles.Any(x => x == Roles.SuperAdmin))
            {

            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new UserModel();
            }
            User user = userStore.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new InfinityNotFoundException("User not found.");
            }
            return new UserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CultureCode = user.CultureCode,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                CompanyId = user.CompanyId,
            };
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UserService.GetUsers(int, int, out int)'
        public List<UserModel> GetUsers(int pageSize, int pageNumber, out int total)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UserService.GetUsers(int, int, out int)'
        {
            IQueryable<User> userQuery = userStore.Users.AsQueryable();
            total = userQuery.Count();
            List<User> users = userQuery.OrderBy(x => x.UserName).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            List<UserModel> userModels = users.Select(x => new UserModel
            {
                Id = x.Id,
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                CultureCode = x.CultureCode,
                PhoneNumber = x.PhoneNumber,
                IsActive = x.IsActive,
                CompanyId = x.CompanyId,
            }).ToList();
            return userModels.OrderBy(x => x.UserName).ToList();
        }



#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UserService.ChangePassword(string, string, string)'
        public bool ChangePassword(string userName, string oldPassword, string newPassword)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UserService.ChangePassword(string, string, string)'
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new InfinityArgumentMissingException("User Name missing");
            }

            if (string.IsNullOrWhiteSpace(oldPassword))
            {
                throw new InfinityArgumentMissingException("Current Password missing");
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new InfinityArgumentMissingException("New Password missing");
            }

            var user = base.FindAsync(userName, oldPassword).Result;

            if (user == null)
            {
                throw new InfinityNotFoundException("Current Password is not correct.");
            }

            var result = base.ChangePasswordAsync(user.Id, oldPassword, newPassword).Result;

            if (!result.Succeeded)
            {
                throw new InfinityNotFoundException("Password is Not Updated");
            }
            return true;
        }



#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UserService.SaveUser(string, UserModel)'
        public bool SaveUser(string userId, UserModel userModel)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UserService.SaveUser(string, UserModel)'
        {
            if (userModel == null)
            {
                throw new InfinityArgumentMissingException("User data missing");
            }

            User user = new User
            {
                UserName = userModel.UserName,

            };

            if (!string.IsNullOrWhiteSpace(userId))
            {
                user = userStore.Users.FirstOrDefault(x => x.Id == userId);
            }
            else
            {
                user.IsActive = true;
                user.CreatedBy =ConfigurationResolver.Get().LoggedInUser.UserId;
                user.CreatedDate = DateTime.Now;
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                var userNameExist = base.FindByNameAsync(userModel.UserName).Result;
                if (userNameExist != null)
                {
                    throw new InfinityNotFoundException("User Name Already Exist. Please Change User Name");
                }
            }
            user.Email = userModel.Email;
            user.CultureCode = userModel.CultureCode;
            user.FirstName = userModel.FirstName;
            user.LastName = userModel.LastName;
            user.PhoneNumber = userModel.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                return base.UpdateAsync(user).Result.Succeeded;
            }
            userModel.Id = user.Id;
            return base.CreateAsync(user, userModel.Password).Result.Succeeded;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UserService.DeleteUser(string)'
        public bool DeleteUser(string userId)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UserService.DeleteUser(string)'
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new InfinityArgumentMissingException("Invalide Requiest !!");
            }
            User user = userStore.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                throw new InfinityNotFoundException("User not found.");
            }
            user.IsActive = false;
            user.InActiveBy = ConfigurationResolver.Get().LoggedInUser.UserId;
            user.InActiveDate = DateTime.Now;
            return base.UpdateAsync(user).Result.Succeeded;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UserService.DeleteUserRoles(string)'
        public bool DeleteUserRoles(string userId)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UserService.DeleteUserRoles(string)'
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new InfinityArgumentMissingException("Invalid user !");
            }

            string[] userRoles = base.GetRolesAsync(userId).Result.ToArray();
            if (userRoles.Any())
            {
                return base.RemoveFromRolesAsync(userId, userRoles).Result.Succeeded;
            }

            return true;
        }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UserService.SaveUserRoles(string, UserRoleModel[])'
        public bool SaveUserRoles(string userId, UserRoleModel[] roles)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UserService.SaveUserRoles(string, UserRoleModel[])'
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new InfinityArgumentMissingException("Invalid user !");
            }

            string[] roleNames = roles.Where(x => x.IsAssigned).Select(y => y.RoleName).ToArray();
            if (DeleteUserRoles(userId))
            {
                return base.AddToRolesAsync(userId, roleNames).Result.Succeeded;
            }
            return false;
        }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UserService.GetUserRoles(string)'
        public List<UserRoleModel> GetUserRoles(string userId)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UserService.GetUserRoles(string)'
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new InfinityArgumentMissingException("Invalid user !");
            }
            List<Role> roles = roleService.GetRoles().ToList();
            List<string> userRoles = base.GetRolesAsync(userId).Result.ToList();
            List<UserRoleModel> usrRoles= roles.Select(x => new UserRoleModel { RoleId = x.Id, RoleName = x.Name, IsAssigned = userRoles.Any(y => x.Name == y) }).ToList();
            return usrRoles;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UserService.SaveApplicantUser(UserModel)'
        public bool SaveApplicantUser(UserModel userModel)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UserService.SaveApplicantUser(UserModel)'
        {
            if (userModel == null)
            {
                throw new InfinityArgumentMissingException("User data missing");
            }
            User user = new User();
            user.UserName = userModel.UserName;
            var userNameExist = base.FindByNameAsync(userModel.UserName).Result;
            if (userNameExist != null)
            {
                throw new InfinityNotFoundException("User Name Already Exist. Please Change User Name");
            }
            user.Email = userModel.Email;
            user.FirstName = userModel.FirstName;
            user.LastName = userModel.LastName;
            user.CultureCode = "en-US";
            user.IsActive = true;
            user.TwoFactorEnabled = false;
            bool status= base.CreateAsync(user, userModel.Password).Result.Succeeded;
            userModel.Id = user.Id;
            status=  base.AddToRolesAsync(user.Id,  new string[]{ Roles.User }.ToArray()).Result.Succeeded;
            return status;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'UserService.ResetPassword(string, string)'
        public async Task<bool> ResetPassword(string user, string newPassword)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'UserService.ResetPassword(string, string)'
        {
            var code = await base.GeneratePasswordResetTokenAsync(user);
            IdentityResult result= await base.ResetPasswordAsync(user, code, newPassword);
            if (!result.Succeeded)
            {
                throw new InfinityNotFoundException("Password is Not Updated");
            }
            return result.Succeeded;
        }
    }
}