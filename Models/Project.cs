using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }

        [DisplayName("Comapany")]
        public int? CompanyId { get; set; }


        [Required]
        [StringLength(50)]
        [DisplayName("Project Name")]
        public string Name { get; set; }


        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Project Start Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset StartDate { get; set; }

        [DisplayName("Project End Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset? EndDate { get; set; }

        [DisplayName("Priority")]
        public int? ProjectPriorityId { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile ImageFormFile { get; set; }

        [DisplayName("File Name")]
        public string ImageFileName { get; set; }

        public byte[] ImageFileData { get; set; }

        [DisplayName("File Extension")]
        public string ImageContentType { get; set; }

        [DisplayName("Archive")]
        public bool Archived { get; set; }

        //Navigation properties
        public virtual Company Company { get; set; }
        public virtual ProjectPriority ProjectPriority { get; set; }
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

    }
}
