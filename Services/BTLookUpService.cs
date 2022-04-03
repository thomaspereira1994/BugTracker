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
    public class BTLookUpService : IBTLookUpService
    {
        #region INJECTION VARIABLES
        private readonly ApplicationDbContext _context;
        #endregion

        #region CONSTRUCTOR
        public BTLookUpService(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion

        #region GET PROJECT PRIORITIES
        public async Task<List<ProjectPriority>> GetProjectPrioritiesAsync()
        {
            try
            {
                return await _context.ProjectPriorities.ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region GET TICKET PRIORITIES
        public Task<List<TicketPriority>> GetTicketPrioritiesAsync()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GET TICKET STATUSES
        public Task<List<TicketStatus>> GetTicketStatusesAsync()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GET TICKET TYPES
        public Task<List<TicketType>> GetTicketTypesAsync()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
