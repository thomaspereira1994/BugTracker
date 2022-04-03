using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        #region READ ONLY PRIVATE VARIABLES
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;
        #endregion

        #region CONSTRUCTOR
        public BTProjectService(ApplicationDbContext context, IBTRolesService rolesService)
        {
            _context = context;
            _rolesService = rolesService;
        }
        #endregion

        #region SERVICE METHODS

        #region CRUD METHODS
        // CRUD - CREATE
        public async Task AddNewProjectAsync(Project project)
        {
            _context.Add(project);
            await _context.SaveChangesAsync();
        }
        // CRUD - READ
        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project project = await _context.Projects.Include(p => p.Tickets)
                                                     .Include(p => p.Members)
                                                     .Include(p => p.ProjectPriority)
                                                     .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);
            return project;
        }
        //CRUD - UPDATE
        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }
        //CRUD - ARCHIVE(DELETE)
        public async Task ArchiveProjectAsync(Project project)
        {
            try
            {
                project.Archived = true;
                await UpdateProjectAsync(project);

                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = true;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            BTUser currentProjectManager = await GetProjectManagerAsync(projectId);

            //remove current pm if project already has one.
            if (currentProjectManager != null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error removing current Project Manager. - Error: {exception.Message}");
                    throw;
                }
            }

            try
            {
                await AddUserToProjectAsync(userId, projectId);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error adding new Project Manager. - Error : {exception.Message}");
                throw;
            }
        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                if (!await IsUserOnProjectAsync(userId, projectId))
                {
                    try
                    {
                        project.Members.Add(user);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            List<BTUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
            List<BTUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
            List<BTUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

            List<BTUser> teamMembers = developers.Concat(submitters).Concat(admins).ToList();

            return teamMembers;
        }

        public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            List<Project> projectsList = new();
            projectsList = await _context.Projects.Where(c => c.Id == companyId && c.Archived == false).Include(p => p.Members)
                                                                                                       .Include(p => p.Tickets).ThenInclude(t => t.Comments)
                                                                                                       .Include(p => p.Tickets).ThenInclude(t => t.Attachments)
                                                                                                       .Include(p => p.Tickets).ThenInclude(t => t.History)
                                                                                                       .Include(p => p.Tickets).ThenInclude(t => t.DeveloperUser)
                                                                                                       .Include(p => p.Tickets).ThenInclude(t => t.OwnerUser)
                                                                                                       .Include(p => p.Tickets).ThenInclude(t => t.Notifications)
                                                                                                       .Include(p => p.Tickets).ThenInclude(t => t.TicketStatus)
                                                                                                       .Include(p => p.Tickets).ThenInclude(t => t.TicketPriority)
                                                                                                       .Include(p => p.Tickets).ThenInclude(t => t.TicketType)
                                                                                                       .Include(p => p.ProjectPriority)
                                                                                                       .ToListAsync();
            return projectsList;
        }

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Project> projectsList = await GetAllProjectsByCompany(companyId);
            int priorityId = await LookupProjectPriorityId(priorityName);

            return projectsList.Where(p => p.ProjectPriorityId == priorityId).ToList();

        }

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            List<Project> projectsList = await GetAllProjectsByCompany(companyId);

            return projectsList.Where(p => p.Archived).ToList();
        }

        public Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

      

        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects.Include(p => p.Members)
                                                     .FirstOrDefaultAsync(p => p.Id == projectId);

            foreach (BTUser member in project?.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                {
                    return member;
                }
            }

            return null;
        }

        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

            List<BTUser> members = new();

            foreach (var user in project.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(user, role))
                {
                    members.Add(user);
                }
            }

            return members;
        }

        public Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users
                                                           .Include(u => u.Projects).ThenInclude(p => p.Company)
                                                           .Include(u => u.Projects).ThenInclude(p => p.Members)
                                                           .Include(u => u.Projects).ThenInclude(p => p.Tickets)
                                                           .Include(u => u.Projects).ThenInclude(t => t.Tickets).ThenInclude(t => t.DeveloperUser)
                                                           .Include(u => u.Projects).ThenInclude(t => t.Tickets).ThenInclude(t => t.OwnerUser)
                                                           .Include(u => u.Projects).ThenInclude(t => t.Tickets).ThenInclude(t => t.TicketPriority)
                                                           .Include(u => u.Projects).ThenInclude(t => t.Tickets).ThenInclude(t => t.TicketStatus)
                                                           .Include(u => u.Projects).ThenInclude(t => t.Tickets).ThenInclude(t => t.TicketType)
                                                           .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();

                return userProjects;

            }
            catch (Exception exception)
            {
                Console.WriteLine($"**** ERROR **** - Eror Getting user projects list --> {exception.Message}");
                throw;
            }
        }

        public async Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            List<BTUser> users = await _context.Users.Where(u => u.Projects
                                                     .All(p => p.Id != projectId))
                                                     .ToListAsync();

            return users.Where(u => u.CompanyId == companyId).ToList();
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

            bool result = false;

            if (project != null)
            {
                result = project.Members.Any(m => m.Id == userId);
            }

            return result;
        }

        public async Task<int> LookupProjectPriorityId(string priorityName)
        {
            int priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;

            return priorityId;
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects.Include(p => p.Members)
                                                     .FirstOrDefaultAsync(p => p.Id == projectId);

            try
            {
                foreach (BTUser member in project?.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                    {
                        await RemoveUserFromProjectAsync(member.Id, projectId);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                try
                {
                    if (await IsUserOnProjectAsync(userId, projectId))
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {

                    throw;
                }


            }
            catch (Exception exception)
            {
                Console.WriteLine($"**** ERROR **** - Error removing user from project. ---> {exception.Message}");

            }
        }

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                List<BTUser> members = await GetProjectMembersByRoleAsync(projectId, role);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (BTUser bTUser in members)
                {
                    try
                    {
                        project.Members.Remove(bTUser);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"**** ERROR **** - Error removing user from project. ---> {exception.Message}");
                throw;
            }
        }

        public async Task RestoreProjectAsync(Project project)
        {
            try
            {
                project.Archived = false;
                await UpdateProjectAsync(project);

                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = false;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
