using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services.Interfaces
{
    public interface IBTTicketService
    {
        
        #region ADD NEW TICKET
        //CREATE
        Task AddNewTicketAsync(Ticket ticket);

        #endregion

        #region UPDATE TICKET
        //UPDATE
        Task UpdateTicketAsync(Ticket ticket);
        #endregion

        #region GET TICKET BY ID
        //READ
        Task<Ticket> GetTicketByIdAsync(int ticketId);
        #endregion

        #region GET TICKET ATTACHMENT BY ID
        Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId);

        #endregion

        #region ARCHIVE TICKET
        //DELETE (ARCHIVE)
        Task ArchiveTicketAsync(Ticket ticket);
        #endregion

        #region RESTORE TICKET
        //RESTORE FROM ARCHIVED
        Task RestoreTicketAsync(Ticket ticket);
        #endregion

        #region ADD TICKET COMMENT
        Task AddTicketCommentAsync(TicketComment ticketComment);

        #endregion

        #region ADD TICKET ATTACHMENT
        Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment);

        #endregion
        #region ASSIGN TICKET
        Task AssignTicketAsync(int ticketId, string userId);

        #endregion 

        #region GET ALL TICKETS BY COMPANY
        Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId);
        #endregion

        #region GET ALL TICKETS BY PRIORITY
        Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName);

        #endregion

        #region GET ALL TICKETS BY STATUS
        Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName);

        #endregion

        #region GET ALL TICKETS BY TYPE
        Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName);

        #endregion

        #region GET ARCHIVED TICKETS
        public Task<List<Ticket>> GetArchivedTicketsAsync(int companyId);
        #endregion

        #region GET TICKET DEVELOPER
        Task<BTUser> GetTicketDeveloperAsync(int ticketId, int companyId);

        #endregion

        #region GET TICKETS BY ROLE
        Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId);

        #endregion

        #region GET TICKETS BY USER ID
        Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId);

        #endregion

        #region GET PROJECT TICKETS BY ROLE
        Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId);

        #endregion

        #region GET PROJECT TICKETS BY STATUS
        Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId);

        #endregion

        #region GET PROJECT TICKETS BY PRIORITY
        Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId);

        #endregion

        #region GET PROJECT TICKETS BY TYPE
        Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId);

        #endregion

        #region GET UNASSIGNED TICKETS
        public Task<List<Ticket>> GetUnassignedTicketsAsync(int companyId);

        #endregion

        #region LOOKUP TICKET PRIORITY ID
        Task<int?> LookupTicketPriorityIdAsync(string priorityName);

        #endregion

        #region LOOKUP TICKET STATUS ID
        Task<int?> LookupTicketStatusIdAsync(string statusName);

        #endregion

        #region LOOKUP TICKET TYPE ID
        Task<int?> LookupTicketTypeIdAsync(string typeName);
        #endregion
    }

    }
