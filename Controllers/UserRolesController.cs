#region USING STATEMENTS
using BugTracker.Extensions;
using BugTracker.Models;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
#endregion

namespace BugTracker.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        #region PRIVATE VARIABLES
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyInfoService _companyInfoService;
        #endregion

        #region CONSTRUCTOR
        public UserRolesController(IBTRolesService rolesService, IBTCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }
        #endregion

        #region MANAGE USER ROLES
        #region GET
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            //ADD AN INSTANCE OF THE VIEWMODEL AS A LIST
            List<ManageUserRolesViewModel> model = new();

            //GET CompanyId
            int companyId = User.Identity.GetCompanyId().Value;

            //GET ALL COMPANY USERS
            List<BTUser> companyUsersList = await _companyInfoService.GetAllMembersAsync(companyId);

            //LOOP OVER USERS TO POPULATE ViewModel
            foreach (BTUser user in companyUsersList)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.BTUser = user;

                IEnumerable<string> selectedRoles = await _rolesService.GetUserRolesAsync(user);

                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selectedRoles);

                model.Add(viewModel);
            }

            return View(model);
        }
        #endregion

        #region POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            //GET CompanyId
            int companyId = User.Identity.GetCompanyId().Value;

            //INSTANTIATE THE BTUSER
            BTUser btUser = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.BTUser.Id);

            //GET ROLES FOR THE USER
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(btUser);

            //GRAB THE SELECTED ROLE
            string userRole = member.SelectedRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole))
            {
                //REMOVE USER FROM ROLE
                if (await _rolesService.RemoveUserFromRolesAsync(btUser, roles))
                {
                    //ADD USER TO THE NEW ROLE
                    await _rolesService.AddUserToRoleAsync(btUser, userRole);
                }
            }

            //NAVIGATE BACK TO VIEW
            return RedirectToAction(nameof(ManageUserRoles));
        } 
        #endregion
        #endregion
    }
}
