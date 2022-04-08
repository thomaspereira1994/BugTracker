using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services.Interfaces
{
    public interface IBTProjectService
    {
        #region ADD NEW PROJECT
        public Task AddNewProjectAsync(Project project);

        #endregion

        #region ADD PROJECT MANAGER
        public Task<bool> AddProjectManagerAsync(string userId, int projectId);

        #endregion

        #region ADD USER TO PROJECT
        public Task<bool> AddUserToProjectAsync(string userId, int projectId);

        #endregion

        #region ARCHIVE PROJECT
        public Task ArchiveProjectAsync(Project project);

        #endregion

        #region GET ALL PROJECTS BY COMPANY
        public Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId);

        #endregion

        #region GET ALL PROJECTS BY PRIORITY
        public Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priorityName);

        #endregion

        #region GET ALL PROJECT MEMBERS EXCEPT PROJECT MANAGER
        public Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId);

        #endregion

        #region GET ARCHIVED PROJECTS BY COMPANY
        public Task<List<Project>> GetArchivedProjectsByCompany(int companyId);

        #endregion

        #region GET DEVELOPERS ON PROJECT
        public Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId);

        #endregion

        #region GET PROJECT MANAGER
        public Task<BTUser> GetProjectManagerAsync(int projectId);

        #endregion

        #region GET PROJECT MEMBERS BY ROLE
        public Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role);

        #endregion

        #region GET PROJECT BY ID
        public Task<Project> GetProjectByIdAsync(int projectId, int companyId);

        #endregion

        #region GET SUBMITTERS ON PROJECT
        public Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId);

        #endregion

        #region GET UNASSIGNED PROJECTS
        public Task<List<Project>> GetUnassignedProjectsAsync(int companyId);
        #endregion

        #region GET USERS NOT ON PROJECT
        public Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId);

        #endregion

        #region GET USER PROJECTS
        public Task<List<Project>> GetUserProjectsAsync(string userId);

        #endregion

        #region IS ASSIGNED PROJECT MANAGER
        public Task<bool> IsAssignedProjectManagerAsync(string userId, int projectId);

        #endregion

        #region IS USER ON PROJECT
        public Task<bool> IsUserOnProjectAsync(string userId, int projectId);

        #endregion

        #region LOOK UP PROJECT PRIORITY ID
        public Task<int> LookupProjectPriorityId(string priorityName);

        #endregion

        #region REMOVE PROJECT MANAGER
        public Task RemoveProjectManagerAsync(int projectId);

        #endregion

        #region REMOVE USERS FROM PROJECT BY ROLE
        public Task RemoveUsersFromProjectByRoleAsync(string role, int projectId);

        #endregion

        #region REMOVE USER FROM PROJECT
        public Task RemoveUserFromProjectAsync(string userId, int projectId);

        #endregion

        #region RESTORE PROJECT 
        public Task RestoreProjectAsync(Project project);

        #endregion

        #region UPDATE PROJECT
        public Task UpdateProjectAsync(Project project);

        #endregion
    }
}
