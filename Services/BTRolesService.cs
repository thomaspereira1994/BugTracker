using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTRolesService : IBTRolesService
    {
        #region PRIVATE READ ONLY VARIABLES
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BTUser> _userManager;
        #endregion

        #region CONSTRUCTOR
        public BTRolesService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<BTUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        #endregion

        #region GET ROLES
        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            try
            {
                List<IdentityRole> result = new();

                result = await _context.Roles.ToListAsync();

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region ADD USER TO ROLE
        public async Task<bool> AddUserToRoleAsync(BTUser user, string roleName)
        {
            bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;
            return result;
        }
        
        #endregion

        #region GET ROLES BY ID
        public async Task<string> GetRolesByIdAsync(string roleId)
        {
            IdentityRole identityRole = _context.Roles.Find(roleId);
            string result = await _roleManager.GetRoleNameAsync(identityRole);
            return result;
        }
        #endregion

        #region GET USER ROLES
        public async Task<IEnumerable<string>> GetUserRolesAsync(BTUser user)
        {
            IEnumerable<string> result = await _userManager.GetRolesAsync(user);
            return result;
        }
        #endregion

        #region GET USERS IN ROLE
        public async Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            List<BTUser> users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
            List<BTUser> result = users.Where(u => u.CompanyId == companyId).ToList();
            return result;
        }
        #endregion

        #region GET USERS NOT IN ROLE
        public async Task<List<BTUser>> GetUsersNotInRoleAsync(string roleName, int companyId)
        {
            List<string> usersIds = (await _userManager.GetUsersInRoleAsync(roleName)).Select(u => u.Id).ToList();
            List<BTUser> roleUsers = _context.Users.Where(u => !usersIds.Contains(u.Id)).ToList();

            List<BTUser> result = roleUsers.Where(u => u.CompanyId == companyId).ToList();

            return result;
        }
        #endregion

        #region IS USER IN ROLE
        public async Task<bool> IsUserInRoleAsync(BTUser user, string roleName)
        {
            bool result = (await _userManager.IsInRoleAsync(user, roleName));
            return result;
        }
        #endregion

        #region REMOVE USER FROM ROLE
        public async Task<bool> RemoveUserFromRoleAsync(BTUser user, string roleName)
        {
            bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;
            return result;
        }
        #endregion

        #region REMOVE USER FROM ROLES
        public async Task<bool> RemoveUserFromRolesAsync(BTUser user, IEnumerable<string> roles)
        {
            bool result = (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;
            return result;
        }
        #endregion
    }
}
