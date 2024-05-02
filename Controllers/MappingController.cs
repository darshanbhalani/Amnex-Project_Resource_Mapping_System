using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;

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
                command.Parameters.AddWithValue("@userId", Convert.ToInt32(HttpContext.Session.GetString("userId")));
                command.Parameters.AddWithValue("@modifyTime", DateTime.Now);

                command.ExecuteNonQuery();

            }
            return Json(new { success = true });
        }


        [HttpPost]
        public ActionResult AssignEmployees([FromBody] AssignEmployeeModel data)
        {
            using (var command = new NpgsqlCommand("select public.karaninsertemployeeproject(@in_employeeids,@in_projectroleid,@in_startdate,@in_enddate,@in_createdby,@in_createdtime,@in_projectid)", _connection))
            {
                command.Parameters.Add(new NpgsqlParameter("in_employeeids", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = data.EmployeesId });
                command.Parameters.Add(new NpgsqlParameter("in_projectroleid", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = data.ProjectRoleId });
                command.Parameters.Add(new NpgsqlParameter("in_startdate", NpgsqlDbType.Array | NpgsqlDbType.Date) { Value = data.StartDate });
                command.Parameters.Add(new NpgsqlParameter("in_enddate", NpgsqlDbType.Array | NpgsqlDbType.Date) { Value = data.EndDate });
                command.Parameters.Add(new NpgsqlParameter("in_createdby", NpgsqlDbType.Integer) { Value = Convert.ToInt32(HttpContext.Session.GetString("userId")) });
                command.Parameters.Add(new NpgsqlParameter("in_createdtime", NpgsqlDbType.Timestamp) { Value = DateTime.Now });
                command.Parameters.Add(new NpgsqlParameter("in_projectid", NpgsqlDbType.Integer) { Value = data.ProjectId });

                command.ExecuteNonQuery();
            }


            return Json(new { success = true, message = "Data saved successfully" });
        }


        [HttpPost]
        public IActionResult UpdateAllocatedEmployeesData([FromBody] List<ProjectDetails> updatedData)
        {
            try
            {
                foreach (var editedEmployee in updatedData)
                {
                    using (var command = new NpgsqlCommand("UPDATE mapping SET employeestartdate = @StartDate, employeeenddate = @EndDate WHERE employeeid = @EmployeeId AND isworking=true; ", _connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeId", editedEmployee.WorkingEmployeeId);
                        command.Parameters.AddWithValue("@StartDate", editedEmployee.EmployeeStartDate);
                        command.Parameters.AddWithValue("@EndDate", editedEmployee.EmployeeEndDate);

                        command.ExecuteNonQuery();
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating employee dates: " + ex.Message);
                return Json(new { success = false, message = "Error updating employee dates" });
            }
        }
    }

}

