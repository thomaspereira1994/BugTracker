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

    public class BTTicketService : IBTTicketService
    {
        #region PRIVATE READ ONLY VARIALBES
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;
        private readonly IBTProjectService _projectsService;
        #endregion


        #region CONSTRUCTOR
        public BTTicketService(ApplicationDbContext context, IBTRolesService rolesService, IBTProjectService projectsService)
        {
            _context = context;
            _rolesService = rolesService;
            _projectsService = projectsService;
        }
        #endregion

        #region SERVICE METHODS
        #region CRUD METHODS
        //CRUD - CREATE
        public async Task AddNewTicketAsync(Ticket ticket)
        {

            try
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region ADD TICKET COMMENT
        public async Task AddTicketCommentAsync(TicketComment ticketComment)
        {
            try
            {
                await _context.AddAsync(ticketComment);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        } 
        #endregion
        //CRUD - READ
        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            try
            {
                return await _context.Tickets.Include(t => t.DeveloperUser)
                                             .Include(t => t.OwnerUser)
                                             .Include(t => t.Project)
                                             .Include(t => t.TicketPriority)
                                             .Include(t => t.TicketStatus)
                                             .Include(t => t.TicketType)
                                             .Include(t => t.Comments)
                                             .Include(t => t.Attachments)
                                             .Include(t => t.History)
                                             .FirstOrDefaultAsync(t => t.Id == ticketId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        //CRUD - UPDATE
        public async Task UpdateTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        //CRUD - ARCHIVE(DELETE)
        public async Task ArchiveTicketAsync(Ticket ticket)
        {
            try
            {
                ticket.Archived = true;
                await UpdateTicketAsync(ticket);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RestoreTicketAsync(Ticket ticket)
        {
            try
            {
                ticket.Archived = false;
                await UpdateTicketAsync(ticket);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            Ticket ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            try
            {
                if(ticket != null)
                {
                    try
                    {
                        ticket.DeveloperUserId = userId;

                        //REVISIT THIS WHEN STATUS ENUM CREATED
                        ticket.TicketStatusId = (await LookupTicketStatusIdAsync("Development")).Value;
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region GET ALL TICKETS BY X METHODS
        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            try
            {
                List<Ticket> ticketsList = await _context.Projects
                                                         .Where(p => p.Id == companyId)
                                                         .SelectMany(p => p.Tickets).Include(t => t.Attachments)
                                                                                    .Include(t => t.Comments)
                                                                                    .Include(t => t.DeveloperUser)
                                                                                    .Include(t => t.History)
                                                                                    .Include(t => t.OwnerUser)
                                                                                    .Include(t => t.TicketPriority)
                                                                                    .Include(t => t.TicketStatus)
                                                                                    .Include(t => t.TicketType)
                                                                                    .Include(t => t.Project)
                                                                                    .ToListAsync();

                return ticketsList;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            int priorityId = (await LookupTicketPriorityIdAsync(priorityName)).Value;

            try
            {
                List<Ticket> ticketsList = await _context.Projects
                                                         .Where(p => p.CompanyId == companyId)
                                                         .SelectMany(p => p.Tickets).Include(t => t.Attachments)
                                                                                    .Include(t => t.Comments)
                                                                                    .Include(t => t.DeveloperUser)
                                                                                    .Include(t => t.History)
                                                                                    .Include(t => t.OwnerUser)
                                                                                    .Include(t => t.TicketPriority)
                                                                                    .Include(t => t.TicketStatus)
                                                                                    .Include(t => t.TicketType)
                                                                                    .Include(t => t.Project)
                                                        .Where(t => t.TicketPriorityId == priorityId).ToListAsync();
                return ticketsList;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            int statusId = (await LookupTicketStatusIdAsync(statusName)).Value;

            try
            {
                List<Ticket> ticketsList = await _context.Projects
                                                         .Where(p => p.CompanyId == companyId)
                                                         .SelectMany(p => p.Tickets).Include(t => t.Attachments)
                                                                                    .Include(t => t.Comments)
                                                                                    .Include(t => t.DeveloperUser)
                                                                                    .Include(t => t.History)
                                                                                    .Include(t => t.OwnerUser)
                                                                                    .Include(t => t.TicketPriority)
                                                                                    .Include(t => t.TicketStatus)
                                                                                    .Include(t => t.TicketType)
                                                                                    .Include(t => t.Project)
                                                        .Where(t => t.TicketStatusId == statusId).ToListAsync();
                return ticketsList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            int typeId = (await LookupTicketTypeIdAsync(typeName)).Value;

            try
            {
                List<Ticket> ticketsList = await _context.Projects
                                                         .Where(p => p.CompanyId == companyId)
                                                         .SelectMany(p => p.Tickets).Include(t => t.Attachments)
                                                                                    .Include(t => t.Comments)
                                                                                    .Include(t => t.DeveloperUser)
                                                                                    .Include(t => t.History)
                                                                                    .Include(t => t.OwnerUser)
                                                                                    .Include(t => t.TicketPriority)
                                                                                    .Include(t => t.TicketStatus)
                                                                                    .Include(t => t.TicketType)
                                                                                    .Include(t => t.Project)
                                                        .Where(t => t.TicketTypeId == typeId).ToListAsync();
                return ticketsList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        public async Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
        {
            try
            {
                List<Ticket> ticketsList = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => t.Archived).ToList();
                return ticketsList;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
        {
            List<Ticket> ticketsList = new();

            try
            {
                ticketsList = (await GetAllTicketsByPriorityAsync(companyId, priorityName)).Where(t => t.ProjectId == projectId).ToList();
                return ticketsList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
        {
            List<Ticket> ticketsList = new();

            try
            {
                ticketsList = (await GetTicketsByRoleAsync(role, userId, companyId)).Where(t => t.ProjectId == projectId).ToList();
                return ticketsList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {
            List<Ticket> ticketsList = new();

            try
            {
                ticketsList = (await GetAllTicketsByStatusAsync(companyId, statusName)).Where(t => t.ProjectId == projectId).ToList();
                return ticketsList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
        {
            List<Ticket> ticketsList = new();

            try
            {
                ticketsList = (await GetAllTicketsByTypeAsync(companyId, typeName)).Where(t => t.ProjectId == projectId).ToList();
                return ticketsList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<BTUser> GetTicketDeveloperAsync(int ticketId, int companyId)
        {
            BTUser developer = new();

            try
            {
                Ticket ticket = (await GetAllTicketsByCompanyAsync(companyId)).FirstOrDefault(t => t.Id == ticketId);

                if(ticket?.DeveloperUserId != null)
                {
                    developer = ticket.DeveloperUser;
                }

                return developer;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
        {
            List<Ticket> ticketsList = new();
            try
            {
                if (role == Roles.Admin.ToString())
                {
                    ticketsList = await GetAllTicketsByCompanyAsync(companyId);
                }
                else if (role == Roles.Developer.ToString())
                {
                    ticketsList = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => t.DeveloperUserId == userId).ToList();
                }
                else if (role == Roles.Submitter.ToString())
                {

                    ticketsList = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => t.OwnerUserId == userId).ToList();
                }
                else if (role == Roles.ProjectManager.ToString())
                {
                    ticketsList = await GetTicketsByUserIdAsync(userId, companyId);
                }

                return ticketsList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
        {
            BTUser bTUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            List<Ticket> ticketsList = new();


            try
            {
                if (await _rolesService.IsUserInRoleAsync(bTUser, Roles.Admin.ToString()))
                {
                    ticketsList = (await _projectsService.GetAllProjectsByCompanyAsync(companyId)).SelectMany(p => p.Tickets).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(bTUser, Roles.Developer.ToString()))
                {
                    ticketsList = (await _projectsService.GetAllProjectsByCompanyAsync(companyId)).SelectMany(p => p.Tickets)
                                                                                             .Where(t => t.DeveloperUserId == userId).ToList();
                }

                else if (await _rolesService.IsUserInRoleAsync(bTUser, Roles.Submitter.ToString()))
                {
                    ticketsList = (await _projectsService.GetAllProjectsByCompanyAsync(companyId)).SelectMany(p => p.Tickets)
                                                                                             .Where(t => t.OwnerUserId == userId).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(bTUser, Roles.ProjectManager.ToString()))
                {
                    ticketsList = (await _projectsService.GetUserProjectsAsync(userId)).SelectMany(t => t.Tickets).ToList();
                }

                return ticketsList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region LOOKUP METHODS
        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            try
            {
                TicketPriority ticketPriority = await _context.TicketPriorities.FirstOrDefaultAsync(p => p.Name == priorityName);
                return ticketPriority?.Id;
            }
            catch
            {
                throw;
            }
        }

        public async Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            try
            {
                TicketStatus ticketStatus = await _context.TicketStatuses.FirstOrDefaultAsync(s => s.Name == statusName);
                return ticketStatus?.Id;
            }
            catch
            {
                throw;
            }
        }

        public async Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            try
            {
                TicketType ticketType = await _context.TicketTypes.FirstOrDefaultAsync(t => t.Name == typeName);
                return ticketType?.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment)
        {
            try
            {
                await _context.AddAsync(ticketAttachment);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId)
        {
            try
            {
                TicketAttachment ticketAttachment = await _context.TicketAttachments.Include(t => t.User)
                                                                                    .FirstOrDefaultAsync(t => t.Id == ticketAttachmentId);

                return ticketAttachment;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #endregion
    }
}
