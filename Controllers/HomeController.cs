using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
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

            Graph inserts = new Graph();
            Graph updates = new Graph();
            Graph deletes = new Graph();
            List<string> date = new List<string>();
            List<int> insert = new List<int>();
            List<int> update = new List<int>();
            List<int> delete = new List<int>();

            int employeesSum = 0;
            int projectsSum = 0;
            int departmentsSum = 0;

            int currentYear = DateTime.Now.Year;

            using (var command = new NpgsqlCommand($"SELECT * FROM countEmployeesByYear({currentYear - 4});", _connection))
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
            using (var command = new NpgsqlCommand($"SELECT * FROM getLogCountsLast7Days();", _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        date.Add(reader.GetDateTime(0).ToString().Split(" ")[0]); 
                        insert.Add(reader.GetInt32(1));
                        update.Add(reader.GetInt32(2));
                        delete.Add(reader.GetInt32(3));
                    }
                    inserts.Label = date;
                    inserts.Data = insert;
                    updates.Label = date;
                    updates.Data = update;
                    deletes.Label = date;
                    deletes.Data = delete;
                }
            }
            dashboardGraphs.Inserts = inserts;
            dashboardGraphs.Updates = updates;
            dashboardGraphs.Deletes = deletes;
            dashboardGraphs.Employees = employees;
            dashboardGraphs.Projects = projects;
            dashboardGraphs.Departments = departments;
            dashboardGraphs.DepartmentProject = departmentProject;
            return View(dashboardGraphs);
        }
        public IActionResult Projects()
        {
            List<Project> lst = new List<Project>();

            using (NpgsqlCommand cmd = new NpgsqlCommand("select * from public.displayactiveprojects();", _connection))
            {
                using (NpgsqlDataReader dr = cmd.ExecuteReader()) {
                    while (dr.Read())
                    {
                        Project project = new Project();
                        project.projectId = Convert.ToInt32(dr.GetValue(0).ToString());
                        project.projectName = dr.GetValue(1).ToString();
                        project.status = dr.GetValue(6).ToString();

                        lst.Add(project);
                    }
                }
            }


            List<dynamic> lst1 = [];

            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.displayallskills();", _connection))
            {
                using(NpgsqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        List<dynamic> x = [];
                        Project project = new Project();
                        x.Add(Convert.ToInt32(dr.GetValue(0)));
                        x.Add(dr.GetValue(1));
                        lst1.Add(x);
                    }
                }
                ;
            }

            List<dynamic> lst2 = [];

            using (NpgsqlCommand cmd = new NpgsqlCommand("select * from public.displayalldepartments();", _connection))
            {
                using (NpgsqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        List<dynamic> x = [];
                        Project project = new Project();
                        x.Add(Convert.ToInt32(dr.GetValue(0)));
                        x.Add(dr.GetValue(1));
                        lst2.Add(x);
                    }
                }
            }


            temp temp = new temp();
            temp.list = lst1;
            temp.project = lst;
            temp.list2 = lst2;
            return View(temp);

        }
        public IActionResult Employees()
        {
            return View();
        }
        public IActionResult ProjectEmployeeMapping()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Departments()
        {
            List<Department> departments = new List<Department>();

            var sql = "SELECT * FROM public.displayalldepartments()";

            using (var cmd = new NpgsqlCommand(sql, _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            DepartmentId = reader.GetInt32(0),
                            DepartmentName = reader.GetString(1),
                        });
                    }
                }
            }
            return View(departments);
        }

        public IActionResult Skills()
        {
            List<Skill> skills = new List<Skill>();
            using (var cmd = new NpgsqlCommand("SELECT skillid, skillname FROM public.skills WHERE isdeleted = false order by skillid", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        skills.Add(new Skill
                        {
                            Skillid = reader.GetInt32(0),
                            Skillname = reader.GetString(1)
                        });
                    }
                }
            }
            return View(skills);
        }

        
        [HttpPost]
        public IActionResult projectDelete(int projectId)
        {
                NpgsqlCommand cmd = new NpgsqlCommand("select public.deleteproject(:projectId,:modifyTime ,:modifyBy);", _connection);
                cmd.Parameters.AddWithValue("projectId", projectId);
                cmd.Parameters.AddWithValue("modifyTime", DateTime.Now);
                cmd.Parameters.AddWithValue("modifyBy", "Bhavya Soni");
                cmd.ExecuteNonQuery();
            
            return Json(new { success = true });
        }

        public IActionResult insertProject(Project project)
        {
            
                NpgsqlCommand cmd = new NpgsqlCommand(" select public.insertproject(:in_project_name::text,:in_department_id,:in_start_date::date ,:in_end_date::date,:in_skills_id::text,:in_status::text,:in_created_by::text,:in_create_time::timestamp,:in_modify_by::text,:in_modify_time::timestamp,:in_is_deleted);", _connection);
                cmd.Parameters.AddWithValue("in_project_name", project.projectName);
                cmd.Parameters.AddWithValue("in_department_id", project.departmentId);
                cmd.Parameters.AddWithValue("in_start_date", project.startDate);
                cmd.Parameters.AddWithValue("in_end_date", project.endDate);
                cmd.Parameters.AddWithValue("in_skills_id", project.skillid);
                cmd.Parameters.AddWithValue("in_status", "Inactive");
                cmd.Parameters.AddWithValue("in_created_by", "Bhavya Soni");
                cmd.Parameters.AddWithValue("in_create_time", DateTime.Now);
                cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                cmd.Parameters.AddWithValue("in_modify_by", "Bhavya Soni");
                cmd.Parameters.AddWithValue("in_is_deleted", false);
                cmd.ExecuteNonQuery();
            
            return Json(new { success = true });
        }

        public IActionResult updateProject(Project project)
        {
            
                NpgsqlCommand cmd = new NpgsqlCommand(" select public.updateproject(:in_project_id::integer ,:in_project_name::text,:in_department_id::integer,:in_start_date::date,:in_end_date::date,:in_skills_id::text,:in_status::text,:in_modify_by::text,:in_modify_time::timestamp);", _connection);
                cmd.Parameters.AddWithValue("in_project_id", project.projectId);
                cmd.Parameters.AddWithValue("in_project_name", project.projectName);
                cmd.Parameters.AddWithValue("in_department_id", project.departmentId);
                cmd.Parameters.AddWithValue("in_start_date", project.startDate);
                cmd.Parameters.AddWithValue("in_end_date", project.endDate);
                cmd.Parameters.AddWithValue("in_skills_id", project.skillid);
                cmd.Parameters.AddWithValue("in_status", project.status);
                cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                cmd.Parameters.AddWithValue("in_modify_by", "Bhavya Soni");
                cmd.Parameters.AddWithValue("in_is_deleted", false);
                cmd.ExecuteNonQuery();
            
            return Json(new { success = true });
        }
        public IActionResult Actions()
        {
            List<Log> logs = new List<Log>();   
            using(var cmd = new NpgsqlCommand("select * from getLogDetails()", _connection))
            {
                using(var reader =  cmd.ExecuteReader()) { 
                    while (reader.Read())
                    {
                        logs.Add(new Log
                        {
                            EntityName = reader.GetString(1),
                            EntityType = reader.GetString(2),
                            Description = reader.GetString(3),
                            Action = reader.GetString(4),
                            CreateTime = reader.GetDateTime(5),
                            LogBy = reader.GetString(6),
                        });
                    }
                }
            }
            return View(logs);
        }

        public IActionResult Error()
        {
            return View();
        }

    }
}
