using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.ViewModels
{
    public class ProjectMembersViewModel
    {
        #region PROPERTIES
        public Project Project { get; set; }
        public MultiSelectList Users { get; set; }
        public List<string> SelectedUsers { get; set; } 
        #endregion
    }
}
