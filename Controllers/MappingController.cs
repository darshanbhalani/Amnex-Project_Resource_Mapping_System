using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Amnex_Project_Resource_Mapping_System.Controllers
{
    public class MappingController : Controller
    {
        private readonly NpgsqlConnection _connection;

        public MappingController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }

        [HttpPost]
        public IActionResult GetProjectEmployeeMapping()
        {
            List<ProjectDetails> projects = new List<ProjectDetails>();

            using (var command = new NpgsqlCommand("select * from getprojectformapping();", _connection))
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    projects.Add(new ProjectDetails
                    {
                        ProjectId = reader.GetInt32(0),
                        ProjectName = reader.GetString(1),
                        ProjectDepartment = reader.GetString(2),
                        ProjectSkills = reader.IsDBNull(3)! ? null : reader.GetString(3)!,
                        NumberOfEmployees = reader.GetInt32(4),
                        StartDate = reader.GetDateTime(5).Date,
                        EndDate = reader.GetDateTime(6).Date,
                        ProjectStatus = reader.GetString(7)
                    });
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


        [HttpPost]
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


        [HttpPost]
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
                using (var command = new NpgsqlCommand("SELECT public.removeemployeefromproject(@projectId, @employeeId, @endDate, @userId, @modifyTime)", _connection))
                {
                    command.Parameters.AddWithValue("@projectId", projectId);
                    command.Parameters.AddWithValue("@employeeId", employeeId);
                    command.Parameters.AddWithValue("@endDate", DateTime.Now.Date);
                    command.Parameters.AddWithValue("@userId", Convert.ToInt32(HttpContext.Session.GetString("userId")!));
                    command.Parameters.AddWithValue("@modifyTime", DateTime.Now);


                    command.ExecuteNonQuery();

                }
                return Json(new { success = true });
        }


        [HttpPost]
        public ActionResult AssignEmployees([FromBody] AssignEmployeeModel data)
        {
            
                Console.WriteLine("Received data:");
                Console.WriteLine($"ProjectId: {data.ProjectId}");

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
                        command.Parameters.AddWithValue("in_createdby", Convert.ToInt32(HttpContext.Session.GetString("userId")!));
                        command.Parameters.AddWithValue("in_createdtime", DateTime.Now);
                        command.Parameters.AddWithValue("in_projectid", data.ProjectId);

                        command.ExecuteNonQuery();
                    }

                    Console.WriteLine($"EmployeeId: {employeeId}, ProjectRoleId: {projectRoleId}, StartDate: {startDate}, EndDate: {endDate}");
                }

                return Json(new { success = true, message = "Data saved successfully" });
            }

    }
}
