using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTNotificationService : IBTNotificationService
    {
        ApplicationDbContext _context;
        IEmailSender _emailSender;
        IBTRolesService _rolesService;

        //CONSTRUCTOR
        public BTNotificationService(ApplicationDbContext context, IEmailSender emailSender, IBTRolesService rolesService)
        {
            _context = context;
            _emailSender = emailSender;
            _rolesService = rolesService;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            try
            {
                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Notification>> GetReceivedNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notifications.Include(n => n.Recipient)
                                                                               .Include(n => n.Sender)
                                                                               .Include(n => n.Ticket).ThenInclude(t => t.Project)
                                                                               .Where(n => n.RecipientId == userId)
                                                                               .ToListAsync();
                return notifications;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notifications.Include(n => n.Recipient)
                                                                               .Include(n => n.Sender)
                                                                               .Include(n => n.Ticket).ThenInclude(t => t.Project)
                                                                               .Where(n => n.SenderId == userId)
                                                                               .ToListAsync();
                return notifications;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> SendEmailNotificationsAsync(Notification notification, string emailSubject)
        {
            BTUser bTUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.RecipientId);

            if (bTUser != null)
            {
                string bTUserEmail = bTUser.Email;
                string message = notification.Message;

                //SEND EMAIL
                try
                {
                    await _emailSender.SendEmailAsync(bTUserEmail, emailSubject, message);
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

        public async Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role)
        {
            try
            {
                List<BTUser> members = await _rolesService.GetUsersInRoleAsync(role, companyId);

                foreach (BTUser user in members)
                {
                    notification.RecipientId = user.Id;
                    await SendEmailNotificationsAsync(notification, notification.Title);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task SendMembersEmailNotificationsAsync(Notification notification, List<BTUser> members)
        {
            try
            {
                foreach (BTUser user in members)
                {
                    notification.RecipientId = user.Id;
                    await SendEmailNotificationsAsync(notification, notification.Title);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}









