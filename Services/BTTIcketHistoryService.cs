using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTTIcketHistoryService : IBTTicketHistoryService
    {
        ApplicationDbContext _context;

        #region CONSTRUCTOR
        public BTTIcketHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion

        #region ADD TICKET HISTORY
        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            //NEW TICKET ADDED SCENARIO
            if (oldTicket == null && newTicket != null)
            {
                TicketHistory ticketHistory = new()
                {
                    TicketId = newTicket.Id,
                    Property = "",
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = "New Ticket Created"
                };
                try
                {
                    await _context.TicketHistories.AddAsync(ticketHistory);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                //CHECK TICKET TITLE
                if (oldTicket.Title != newTicket.Title)
                {
                    TicketHistory ticketHistory = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Title",
                        OldValue = oldTicket.Title,
                        NewValue = newTicket.Title,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket title: {newTicket.Title}"
                    };
                    await _context.TicketHistories.AddAsync(ticketHistory);
                }

                //CHECK TICKET DESCRIPTION
                if (oldTicket.Description != newTicket.Description)
                {
                    TicketHistory ticketHistory = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Description",
                        OldValue = oldTicket.Description,
                        NewValue = newTicket.Description,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket description: {newTicket.Description}"
                    };
                    await _context.TicketHistories.AddAsync(ticketHistory);
                }

                //CHECK TICKET PRIORITY
                if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
                {
                    TicketHistory ticketHistory = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Priority",
                        OldValue = oldTicket.TicketPriority.Name,
                        NewValue = newTicket.TicketPriority.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket description: {newTicket.TicketPriority.Name}"
                    };
                    await _context.TicketHistories.AddAsync(ticketHistory);
                }

                //CHECK TICKET STATUS
                if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
                {
                    TicketHistory ticketHistory = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Status",
                        OldValue = oldTicket.TicketStatus.Name,
                        NewValue = newTicket.TicketStatus.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket description: {newTicket.TicketStatus.Name}"
                    };
                    await _context.TicketHistories.AddAsync(ticketHistory);
                }

                //CHECK TICKET TYPE
                if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
                {
                    TicketHistory ticketHistory = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Type",
                        OldValue = oldTicket.TicketType.Name,
                        NewValue = newTicket.TicketType.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket description: {newTicket.TicketType.Name}"
                    };
                    await _context.TicketHistories.AddAsync(ticketHistory);
                }

                //CHECK TICKET DEVELOPER
                if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
                {
                    TicketHistory ticketHistory = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Developer",
                        OldValue = oldTicket.DeveloperUser?.FullName ?? "Not Assigned",
                        NewValue = newTicket.DeveloperUser?.FullName,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket description: {newTicket.DeveloperUser.FullName}"
                    };
                    await _context.TicketHistories.AddAsync(ticketHistory);
                }

                //SAVE CHANGES TO DATABASE
                try
                {

                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public async Task AddHistoryAsync(int ticketId, string model, string userId)
        {
            try
            {
                Ticket ticket = await _context.Tickets.FindAsync(ticketId);

                string description = model.ToLower().Replace("Ticket", "");

                description = $"New {description} added to ticket: {ticket.Title}";

                TicketHistory history = new()
                {
                    TicketId = ticket.Id,
                    Property = model,
                    OldValue = "",
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = description
                };

                await _context.TicketHistories.AddAsync(history);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region GET COMPANY TICKETS HISTORIES
        public async Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
        {
            try
            {
                List<Project> projectsList = (await _context.Companies
                                                            .Include(c => c.Projects)
                                                            .ThenInclude(p => p.Tickets)
                                                            .ThenInclude(t => t.History)
                                                            .ThenInclude(h => h.User)
                                                            .FirstOrDefaultAsync(c => c.Id == companyId)).Projects.ToList();

                List<Ticket> ticketsList = projectsList.SelectMany(p => p.Tickets).ToList();

                List<TicketHistory> ticketHistoriesList = ticketsList.SelectMany(t => t.History).ToList();

                return ticketHistoriesList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region GET PROJECT TICKETS HISTORIES
        public async Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
        {
            try
            {
                Project project = await _context.Projects.Where(p => p.CompanyId == companyId)
                                                         .Include(p => p.Tickets).ThenInclude(t => t.History).ThenInclude(h => h.User)
                                                         .FirstOrDefaultAsync(p => p.Id == projectId);

                List<TicketHistory> ticketHistoryList = project.Tickets.SelectMany(t => t.History).ToList();

                return ticketHistoryList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion    
    }
}
