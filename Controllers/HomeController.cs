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

            EmployeesModel model = new EmployeesModel();
            
            int allocated = 0;
            int unallocated = 0;
            using(var cmd = new NpgsqlCommand("select * from count_allocated_unallocated_employees()", _connection)) {
                using(var reader= cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allocated = reader.GetInt32(0);
                        unallocated = reader.GetInt32(1);
                    }
                }
            }

            model.employees = GetEmployees();
            model.employeeDepartments = getEmployeeDepartment();
            model.employeeSkills = getEmployeeSkills();
            model.allocatedEmployees = allocated;
            model.unallocatedEmployees = unallocated;

            return View(model);
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

        public List<Employee> GetEmployees()
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.displayemployees()", _connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Employee employee = new Employee
                            {
                                EmployeeId = Convert.ToInt32(reader["employee_id"]),
                                EmployeeName = Convert.ToString(reader["employee_name"]),
                                DepartmentId = Convert.ToInt32(reader["department_id"]),
                                SkillsId = Convert.ToString(reader["skills_id"]),
                                LoginRoleId = Convert.ToInt32(reader["login_role_id"]),
                                IsAllocated = Convert.ToBoolean(reader["is_allocated"]),
                                Email = Convert.ToString(reader["email"]),
                                CreatedbyName = Convert.ToString(reader["created_by"]), // Assuming createdBy is of type text
                                CreatedTime = Convert.ToDateTime(reader["created_time"]),
                                ModifybyName = Convert.ToString(reader["modify_by"]), // Assuming modifyBy is of type text
                                ModifyTime = Convert.ToDateTime(reader["modify_time"])
                            };

                            employees.Add(employee);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it, throw it, etc.)
                Console.WriteLine("Error: " + ex.Message);
            }
            return employees;
        }


        public List<Department> getEmployeeDepartment()
        {
            List<Department> employeeDepartment = new List<Department>();
            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.displayalldepartments();", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Department employeeDepartmentObj = new Department
                        {
                            DepartmentId = Convert.ToInt32(reader[0]),
                            DepartmentName = Convert.ToString(reader[1])
                        };
                        employeeDepartment.Add(employeeDepartmentObj);

                    }

                }
            }
            return employeeDepartment;
        }
        public List<Skill> getEmployeeSkills()
        {
            List<Skill> employeeSkill = new List<Skill>();
            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.displayallskills();", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Skill employeeSkillObj = new Skill
                        {
                            Skillid = Convert.ToInt32(reader[0]),
                            Skillname = Convert.ToString(reader[1])
                        };
                        employeeSkill.Add(employeeSkillObj);

                    }

                }
            }
            return employeeSkill;
        }

       
        [HttpGet]
        public IActionResult GetProjectEmployeeMapping()
        {
            List<ProjectDetails> projects = new List<ProjectDetails>();

            using (var command = new NpgsqlCommand("select * from getprojectformapping();", _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        projects.Add(new ProjectDetails
                        {
                            ProjectId = reader.GetInt32(0),
                            ProjectName = reader.GetString(1),
                            ProjectDepartment = reader.GetString(2),
                            ProjectSkills = reader.IsDBNull(3) ? null : reader.GetString(3),
                            NumberOfEmployees = reader.GetInt32(4),
                            StartDate = reader.GetDateTime(5).Date,
                            EndDate = reader.GetDateTime(6).Date,
                            ProjectStatus = reader.GetString(7)
                        });
                    }
                }
            }
            return Json(new { success = true, data = projects });
        }

        [HttpPost]
        public IActionResult getAllocatedEmployeesData(int ProjectId, string skills)
        {
            List<ProjectDetails> empData = new List<ProjectDetails>();

            using (var command = new NpgsqlCommand($"SELECT * from getemployeeprojectdata({ProjectId});", _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var assignEmpData = new ProjectDetails
                        {
                            WorkingEmployeeId = reader.GetInt32(0),
                            Employeename = reader.GetString(1),
                            EmployeeStartDate = reader.GetDateTime(3).Date,
                            EmployeeEndDate = reader.GetDateTime(4).Date,
                            EmployeeSkills = skills,
                            EmployeeRole = reader.GetString(6)
                        };
                        empData.Add(assignEmpData);
                    }

                }
            }
            return Json(new { success = true, data = empData });
        }

        [HttpGet]
        public IActionResult FetchNotAllocatedEmployees(string departmentName)
        {
            string getDepartmentIdByName = "select departmentid from departments where departmentname = @deptName";
            List<ProjectDetails> empData = new List<ProjectDetails>();

            using (var command = new NpgsqlCommand(getDepartmentIdByName, _connection))
            {
                command.Parameters.AddWithValue("@deptName", departmentName);

                int departmentId = 0;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        departmentId = reader.GetInt32(0);
                    }
                }

                if (departmentId > 0)
                {
                    string sqlQuery = "select * from public.displayunallocatedemployees(@deptId)";
                    using (var cmd = new NpgsqlCommand(sqlQuery, _connection))
                    {
                        cmd.Parameters.AddWithValue("@deptId", departmentId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var assignEmpData = new ProjectDetails
                                {
                                    employeeId = reader.GetInt32(0),
                                    NotAllocatedEmployeeName = reader.GetString(1),
                                    NotAllocatedEmployeeSkills = reader.GetString(2),
                                    EmployeeRating = reader.GetInt32(3),

                                };
                                empData.Add(assignEmpData);
                            }
                        }
                    }
                }
            }

            return Json(new { success = true, data = empData });
        }


        [HttpGet]
        public IActionResult getProjectRoles()
        {
            List<ProjectDetails> empData = new List<ProjectDetails>();

            using (var command = new NpgsqlCommand($"SELECT * FROM public.projectroles ORDER BY projectroleid ASC;", _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var assignEmpData = new ProjectDetails
                        {
                            ProjectRoleId = reader.GetInt32(0),
                            ProjectRoleName = reader.GetString(1),
                        };
                        empData.Add(assignEmpData);
                    }
                }
            }
            return Json(new { success = true, data = empData });
        }

        [HttpPost]
        public IActionResult RemoveFromProject(int employeeId, int projectId)
        {

            try
            {
                using (var command = new NpgsqlCommand("SELECT public.removeemployeefromproject(@projectId, @employeeId, @endDate, @userId, @modifyTime)", _connection))
                {
                    command.Parameters.AddWithValue("@projectId", projectId);
                    command.Parameters.AddWithValue("@employeeId", employeeId);
                    command.Parameters.AddWithValue("@endDate", DateTime.Now.Date);
                    command.Parameters.AddWithValue("@userId", Convert.ToInt32(HttpContext.Session.GetString("userId")));
                    command.Parameters.AddWithValue("@modifyTime", DateTime.Now);


                    command.ExecuteNonQuery();

                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult AssignEmployees([FromBody] AssignEmployeeModel data)
        {
            try
            {
                Console.WriteLine("Received data:");
                Console.WriteLine($"ProjectId: {data.ProjectId}");

                // Loop through each employee in the data
                for (int i = 0; i < data.EmployeesId.Count; i++)
                {
                    int employeeId = data.EmployeesId[i];
                    int projectRoleId = data.ProjectRoleId[i];
                    DateOnly? startDate = data.StartDate[i];
                    DateOnly? endDate = data.EndDate[i];

                    using (var command = new NpgsqlCommand("select public.karaninsertemployeeproject(@in_employeeids,@in_projectroleid,@in_startdate,@in_enddate,@in_createdby,@in_createdtime,@in_projectid)", _connection))
                    {
                        command.Parameters.AddWithValue("in_employeeids", new[] { employeeId });
                        command.Parameters.AddWithValue("in_projectroleid", new[] { projectRoleId });
                        command.Parameters.AddWithValue("in_startdate", new[] { startDate });
                        command.Parameters.AddWithValue("in_enddate", new[] { endDate });
                        command.Parameters.AddWithValue("in_createdby", Convert.ToInt32(HttpContext.Session.GetString("userId")));
                        command.Parameters.AddWithValue("in_createdtime", DateTime.Now);
                        command.Parameters.AddWithValue("in_projectid", data.ProjectId);

                        command.ExecuteNonQuery();
                    }

                    Console.WriteLine($"EmployeeId: {employeeId}, ProjectRoleId: {projectRoleId}, StartDate: {startDate}, EndDate: {endDate}");
                }

                return Json(new { success = true, message = "Data saved successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("In error");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
