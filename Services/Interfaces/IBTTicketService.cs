using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services.Interfaces
{
    public interface IBTTicketService
    {
        #region CRUD METHODS
        //CREATE
        Task AddNewTicketAsync(Ticket ticket);

        //UPDATE
        Task UpdateTicketAsync(Ticket ticket);

        //READ
        Task<Ticket> GetTicketByIdAsync(int ticketId);

        //DELETE (ARCHIVE)
        Task ArchiveTicketAsync(Ticket ticket);
        //RESTORE FROM ARCHIVED
        Task RestoreTicketAsync(Ticket ticket);
        #endregion

        Task AssignTicketAsync(int ticketId, string userId);
        Task<List<Ticket>> GetArchivedTicketsAsync(int companyId);
        Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId);
        Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName);
        Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName);
        Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName);
        Task<BTUser> GetTicketDeveloperAsync(int ticketId, int companyId);
        Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId);
        Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId);
        Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId);
        Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId);
        Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId);
        Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId);


        Task<int?> LookupTicketPriorityIdAsync(string priorityName);
        Task<int?> LookupTicketStatusIdAsync(string statusName);
        Task<int?> LookupTicketTypeIdAsync(string typeName);
    }
}
