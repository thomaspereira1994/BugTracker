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
    public class BTCompanyInfoService : IBTCompanyInfoService
    {
        #region PRIVATE READ ONLY VARIABLES
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BTUser> _userManager;

        public BTCompanyInfoService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<BTUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        #endregion

        public async Task<List<BTUser>> GetAllMembersAsync(int companyId)
        {
            List<BTUser> membersList = new();

            membersList = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();

            return membersList;
        }

        public async Task<List<Project>> GetAllProjectsAsync(int companyId)
        {
            List<Project> projectsList = new();

            projectsList = await _context.Projects.Where(u => u.CompanyId == companyId).Include(p => p.Members)
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

        public async Task<List<Ticket>> GetallTicketsAsync(int companyId)
        {
            List<Ticket> ticketsList = new();
            List<Project> projects = new();

            projects = await GetAllProjectsAsync(companyId);

            ticketsList = projects.SelectMany(p => p.Tickets).ToList();

            return ticketsList;
        }

        public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {
            Company result = new();

            if (companyId != null)
            {
                result = await _context.Companies
                                       .Include(c => c.Members)
                                       .Include(c => c.Projects)
                                       .Include(c => c.Invites)
                                       .FirstOrDefaultAsync(c => c.Id == companyId);
            }

            return result;

        }
    }
}
