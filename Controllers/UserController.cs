using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Amnex_Project_Resource_Mapping_System.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NpgsqlConnection _connection;

        public UserController(IHttpContextAccessor httpContextAccessor, NpgsqlConnection connection)
        {
            _httpContextAccessor = httpContextAccessor;
            _connection = connection;
            connection.Open();

        }

        public IActionResult UserDashboard()
        {
            return View();
        }
        public IActionResult UserProjects(Project data)
        {
            int uid = GetUserId();
            var workingProjects = new List<Project>();
            var notWorkingProjects = new List<Project>();

            using (var cmd = new NpgsqlCommand($"SELECT * FROM getprojectsbyemployeeid({uid});", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var project = new Project
                        {

                            ProjectName = reader["projectname"] != DBNull.Value ? (string)reader["projectname"] : "",
                            StartDate = reader["startdate"] != DBNull.Value ? DateOnly.FromDateTime((DateTime)reader["startdate"]) : default,
                            EndDate = reader["enddate"] != DBNull.Value ? DateOnly.FromDateTime((DateTime)reader["enddate"]) : default,
                            ProjectId = reader["projectid"] != DBNull.Value ? (Int64)reader["projectid"] : null

                        };

                        // Check if the project is working or not
                        bool isWorking = reader["isworking"] != DBNull.Value ? (bool)reader["isworking"] : false;

                        if (isWorking)
                        {
                            workingProjects.Add(project);
                        }
                        else
                        {
                            notWorkingProjects.Add(project);
                        }
                    }
                }
            }

            // Pass both lists to the view
            ViewBag.WorkingProjects = workingProjects;
            ViewBag.NotWorkingProjects = notWorkingProjects;

            return View();
        }
        public IActionResult userProjectDetails(int projectId)
        {
            var projectDetails = GetProjectDetails(projectId);
            return View(projectDetails);
        }

        private ProjectDetailsView GetProjectDetails(int projectId)
        {
            ProjectDetailsView projectDetails = null!;

            using (var cmd = new NpgsqlCommand($"SELECT projectstartdate, projectenddate FROM projects where projectid = @projectid;", _connection))
            {
                cmd.Parameters.AddWithValue("@projectid", projectId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        projectDetails = new ProjectDetailsView
                        {
                            StartDate = reader["projectstartdate"] != DBNull.Value ? DateOnly.FromDateTime((DateTime)reader["projectstartdate"]) : default,
                            EndDate = reader["projectenddate"] != DBNull.Value ? DateOnly.FromDateTime((DateTime)reader["projectenddate"]) : default
                        };
                    }
                }

                return projectDetails;
            }
        }

        public IActionResult UserAnalysis()
        {
            return View();
        }
        public int GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext.Session.GetString("userId");
            var uid = Convert.ToInt32(userId);
            return uid;
        }
    }
}
