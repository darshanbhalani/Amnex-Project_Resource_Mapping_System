using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

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

            DashboardModal dashboardGraphs = new DashboardModal()
            {
                Inserts = inserts,
                Updates = updates,
                Deletes = deletes,
                Employees = employees,
                Projects = projects,
                Departments = departments,
                DepartmentProject = departmentProject,
            };

            return View(dashboardGraphs);
        }


        public IActionResult Projects()
        {
            int completed = 0;
            int running = 0;
            using (var cmd = new NpgsqlCommand("SELECT * from public.count_running_completed_projects()", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        running = reader.GetInt32(0);
                        completed = reader.GetInt32(1);
                    }
                }
            }

            List<Project> projects = new List<Project>();
            using (NpgsqlCommand cmd = new NpgsqlCommand("select * from public.displayactiveprojects();", _connection))
            {
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        projects.Add(new Project
                        {
                            ProjectId = Convert.ToInt32(reader.GetValue(0).ToString()),
                            ProjectName = reader.GetValue(1).ToString()!,
                            Status = reader.GetValue(6).ToString()!
                        });
                    }
                }
            }

            ProjectsModal projectsModal = new ProjectsModal()
            {
                Projects = projects,
                Skills = SkillsController.getSkillList(_connection),
                Departments = DepartmentsController.getDepartmentList(_connection),
                RunningProjects = running,
                CompletedProjects = completed,
            };

            return View(projectsModal);
        }


        public IActionResult Employees()
        {
            int allocated = 0;
            int unallocated = 0;
            using (var cmd = new NpgsqlCommand("select * from count_allocated_unallocated_employees()", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allocated = reader.GetInt32(0);
                        unallocated = reader.GetInt32(1);
                    }
                }
            }

            EmployeesModel employeesModel = new EmployeesModel()
            {
                Employees = EmployeesController.getEmployeesList(_connection),
                EmployeeDepartments = getEmployeeDepartmentList(_connection),
                EmployeeSkills = getEmployeeSkillsList(_connection),
                AllocatedEmployees = allocated,
                UnallocatedEmployees = unallocated
            };

            return View(employeesModel);
        }


        public IActionResult ProjectEmployeeMapping()
        {
            return View();
        }


        public IActionResult Departments()
        {
            List<Department> departments = new List<Department>();

            using (var cmd = new NpgsqlCommand("SELECT * FROM public.displayalldepartments()", _connection))
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


        public IActionResult Actions()
        {
            List<Log> logs = new List<Log>();
            using (var cmd = new NpgsqlCommand("select * from getLogDetails()", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(new Log
                        {
                            EntityName = reader.IsDBNull(1) ? "Not Found":reader.GetString(1),
                            EntityType = reader.IsDBNull(2) ? "Not Found" : reader.GetString(2),
                            Description = reader.IsDBNull(3) ? "Not Found" : reader.GetString(3),
                            Action = reader.IsDBNull(4) ? "Not Found" : reader.GetString(4),
                            CreateTime = reader.GetDateTime(5),
                            LogBy = reader.IsDBNull(6) ? "Not Found" : reader.GetString(6),
                        });
                    }
                }
            }
            return View(logs);
        }


        private static List<Department> getEmployeeDepartmentList(NpgsqlConnection _connection)
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
                            DepartmentName = Convert.ToString(reader[1])!
                        };
                        employeeDepartment.Add(employeeDepartmentObj);

                    }

                }
            }
            return employeeDepartment;
        }


        private static List<Skill> getEmployeeSkillsList(NpgsqlConnection _connection)
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
                            Skillname = Convert.ToString(reader[1])!
                        };
                        employeeSkill.Add(employeeSkillObj);

                    }

                }
            }
            return employeeSkill;
        }


        public IActionResult Error()
        {
            return View();
        }
    }
}
