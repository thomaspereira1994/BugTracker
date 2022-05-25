#region USING STATEMENTS
using BugTracker.Extensions;
using BugTracker.Models;
using BugTracker.Models.ChartModels;
using BugTracker.Models.Enums;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        #region PRIVATE VARIABLES
        private readonly ILogger<HomeController> _logger;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTProjectService _projectService;
        private readonly IBTTicketService _ticketService;
        private readonly SignInManager<BTUser> _signInManager;
        #endregion

        #region CONSTRUCTOR
        public HomeController(ILogger<HomeController> logger, IBTCompanyInfoService companyService, IBTProjectService projectService, IBTTicketService ticketService, SignInManager<BTUser> signInManager)
        {
            _logger = logger;
            _companyInfoService = companyService;
            _projectService = projectService;
            _ticketService = ticketService;
            _signInManager = signInManager;
        }

        #endregion

        #region INDEX
        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
              await _signInManager.SignOutAsync();
            }
            return View();
        }
        #endregion

        #region DASHBOARD
        #region DASHBOARD
        public async Task<IActionResult> Dashboard()
        {
            DashboardViewModel model = new();

            int companyId = User.Identity.GetCompanyId().Value;

            model.Company = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);

            model.Projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);

            model.Tickets = model.Projects.SelectMany(p => p.Tickets).Where(t => !t.Archived).ToList();

            model.Members = model.Company.Members.ToList();

            return View(model);
        } 
        #endregion

        #region GOOGLE PROJECT TICKETS 
        [HttpPost]
        public async Task<JsonResult> GglProjectTickets()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);

            List<object> chartData = new();
            chartData.Add(new object[] { "ProjectName", "TicketCount" });

            foreach (Project project in projects)
            {
                chartData.Add(new object[] { project.Name, project.Tickets.Count() });
            }

            return Json(chartData);
        }
        #endregion

        #region GOOGLE PROJECT PRIORITY
        [HttpPost]
        public async Task<JsonResult> GglProjectPriority()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);

            List<object> chartData = new();
            chartData.Add(new object[] { "Priority", "Count" });


            foreach (string priority in Enum.GetNames(typeof(BTProjectPriority)))
            {
                int priorityCount = (await _projectService.GetAllProjectsByPriorityAsync(companyId, priority)).Count();
                chartData.Add(new object[] { priority, priorityCount });
            }

            return Json(chartData);
        }
        #endregion

        #region AM CHARTS

        [HttpPost]
        public async Task<JsonResult> AmCharts()
        {

            AmChartData amChartData = new();
            List<AmItem> amItems = new();

            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = (await _companyInfoService.GetAllProjectsAsync(companyId)).Where(p => p.Archived == false).ToList();

            foreach (Project project in projects)
            {
                AmItem item = new();

                item.Project = project.Name;
                item.Tickets = project.Tickets.Count;
                item.Developers = (await _projectService.GetProjectMembersByRoleAsync(project.Id, nameof(Roles.Developer))).Count();

                amItems.Add(item);
            }

            amChartData.Data = amItems.ToArray();


            return Json(amChartData.Data);
        }
        #endregion

        #region PLOTLY CHARTS
        [HttpPost]
        public async Task<JsonResult> PlotlyBarChart()
        {
            PlotlyBarData plotlyData = new();
            List<PlotlyBar> barData = new();

            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);

            //Bar One
            PlotlyBar barOne = new()
            {
                X = projects.Select(p => p.Name).ToArray(),
                Y = projects.SelectMany(p => p.Tickets).GroupBy(t => t.ProjectId).Select(g => g.Count()).ToArray(),
                Name = "Tickets",
                Type = "bar"
            };

            //Bar Two
            PlotlyBar barTwo = new()
            {
                X = projects.Select(p => p.Name).ToArray(),
                Y = projects.Select(async p => (await _projectService.GetProjectMembersByRoleAsync(p.Id, nameof(Roles.Developer))).Count).Select(c => c.Result).ToArray(),
                Name = "Developers",
                Type = "bar"
            };

            barData.Add(barOne);
            barData.Add(barTwo);

            plotlyData.Data = barData;

            return Json(plotlyData);
        }
        #endregion
        #endregion

        #region PRIVACY
        public IActionResult Privacy()
        {
            return View();
        }
        #endregion

        #region ERROR
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        } 
        #endregion
    }
}
