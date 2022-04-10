using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
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
        private readonly IBTRolesService _rolesService;
        #endregion

        #region CONSTRUCTOR
        public BTCompanyInfoService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<BTUser> userManager, IBTRolesService rolesService)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _rolesService = rolesService;
        }
        #endregion

        #region ADD COMPANY
        public async Task AddCompanyAsync(Company company)
        {
            try
            {
                await _context.AddAsync(company);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        //#region CREATE NEW COMPANY
        //Company CreateNewCompany(string companyName)
        //{
        //    Company company = new();

            

        //    company.Name = companyName;
        //}
        //#endregion

        #region GET ALL MEMBERS IN A COMPANY
        public async Task<List<BTUser>> GetAllMembersAsync(int companyId)
        {
            List<BTUser> membersList = new();

            membersList = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();

            return membersList;
        }
        #endregion

        #region GET ALL PROJECTS IN A COMPANY
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
        #endregion

        #region GET ALL TICKETS IN A COMPANY
        public async Task<List<Ticket>> GetallTicketsAsync(int companyId)
        {
            List<Ticket> ticketsList = new();
            List<Project> projects = new();

            projects = await GetAllProjectsAsync(companyId);

            ticketsList = projects.SelectMany(p => p.Tickets).ToList();

            return ticketsList;
        }
        #endregion

        #region GET COMPANY INFO BY ID
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
        #endregion

        public async Task<Company> AssignNewUserToCompany(string companyName)
        {
            companyName = companyName.ToLower();



            List<Company> companies = await _context.Companies.ToListAsync();
            
            //CHECK IF COMPANY ALREADY EXISTS, ASSIGN USER TO COMPANY IF IT DOES
            foreach (Company company in companies)
            {
                if (companyName == company.Name.ToLower())
                {
                    return company;
                }
            }

            Company newCompany = new ();

            newCompany.Name = companyName;

            await AddCompanyAsync(newCompany);

            //await  _rolesService.AddUserToRoleAsync(User, nameof(Roles.Admin));


            return newCompany;

        }
    }
}
