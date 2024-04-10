using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Amnex_Project_Resource_Mapping_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly NpgsqlConnection _connection;

        public HomeController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }

        public IActionResult Dashboard()
        {
            DashboardGraphs dashboardGraphs = new DashboardGraphs();
            Graph employees = new Graph();
            List<string> employeesLable = new List<string>();
            List<int> employeesData = new List<int>();

            Graph projects = new Graph();
            List<string> projectsLable = new List<string>();
            List<int> projectsData = new List<int>();

            Graph departments = new Graph();
            List<string> departmentsLable = new List<string>();
            List<int> departmentsData = new List<int>();

            Graph departmentProject = new Graph();
            List<string> departmentProjecLable = new List<string>();
            List<int> departmentProjectData = new List<int>();

            int employeesSum = 0;
            int projectsSum = 0;
            int departmentsSum = 0;

            int currentYear = DateTime.Now.Year;

            using (var command = new NpgsqlCommand($"SELECT * FROM countEmployeesByYear({currentYear-4});", _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employeesLable.Add(reader.GetInt32(0).ToString());
                        employeesData.Add(reader.GetInt32(1));
                    }
                    for (int i = 0; i < employeesData.Count; i++)
                    {
                        employeesSum += employeesData[i];
                        employeesData[i] = employeesSum;
                    }
                    employees.Data = employeesData;
                    employees.Label = employeesLable;
                }

            }

            using (var command = new NpgsqlCommand($"SELECT * FROM countProjectsByYear({currentYear - 4});", _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        projectsLable.Add(reader.GetInt32(0).ToString());
                        projectsData.Add(reader.GetInt32(1));
                    }
                   
                    for (int i = 0; i < projectsData.Count; i++)
                    {
                        projectsSum += projectsData[i];
                        projectsData[i] = projectsSum;
                    }
                    projects.Data = projectsData;
                    projects.Label = projectsLable;
                }
            }

            using (var command = new NpgsqlCommand($"SELECT * FROM countDepartmentsByYear({currentYear - 4});", _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        departmentsLable.Add(reader.GetInt32(0).ToString());
                        departmentsData.Add(reader.GetInt32(1));
                    }
                    for (int i = 0; i < departmentsData.Count; i++)
                    {
                        departmentsSum += departmentsData[i];
                        departmentsData[i] = departmentsSum;
                    }
                    departments.Data = departmentsData;
                    departments.Label = departmentsLable;
                }
            }

            using (var command = new NpgsqlCommand($"SELECT * FROM getDepartmentProjectCounts();", _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        departmentProjecLable.Add(reader.GetString(0));
                        departmentProjectData.Add(reader.GetInt32(1));
                    }

                    departmentProject.Data = departmentProjectData;
                    departmentProject.Label = departmentProjecLable;
                }
            }
            dashboardGraphs.Employees = employees;
            dashboardGraphs.Projects = projects;
            dashboardGraphs.Departments = departments;
            dashboardGraphs.DepartmentProject = departmentProject;
            return View(dashboardGraphs);
        }
        public IActionResult Projects()
        {
            return View();
        }
        public IActionResult Employees()
        {
            return View();
        }
        public IActionResult ProjectEmployeeMapping()
        {
            return View();
        }
        public IActionResult Departments()
        {
            return View();
        }
        public IActionResult Skills()
        {
            return View();
        }
        public IActionResult Actions()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

    }
}
