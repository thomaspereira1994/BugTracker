using System;
using System.ComponentModel;

namespace BugTracker.Models
{
    public class TicketComment
    {
        //Primary Key
        public int Id { get; set; }

        [DisplayName("Member Comment")]
        public string Comment { get; set; }


        [DisplayName("Date")]
        public DateTime Created { get; set; }


        //Foreign Key
        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        //Foreign Key
        [DisplayName("Team Member")]
        public int UserId { get; set; }


        //Navigation properties
        public virtual Ticket Ticket { get; set; }
        public virtual BTUser User { get; set; }


    }
}
