using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using System.Text.Json;

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

        //[HttpPost]
        //public IActionResult GetProjectEmployeeMapping()
        //{
        //    List<ProjectDetails> projects = new List<ProjectDetails>();

        //    using (var command = new NpgsqlCommand("select * from displayprojectsformapping();", _connection))
        //    {
        //        using (var reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                projects.Add(new ProjectDetails
        //                {
        //                    ProjectId = reader.GetInt32(0),
        //                    ProjectName = reader.GetString(1),
        //                    ProjectDepartment = reader.GetString(2),
        //                    ProjectSkills = reader.GetString(3),
        //                    NumberOfEmployees = reader.GetInt32(6),
        //                    StartDate = reader.GetDateTime(4).Date,
        //                    EndDate = reader.GetDateTime(5).Date
        //                });
        //            }
        //        }
        //    }

        //    return Json(JsonConvert.SerializeObject(projects));

        //}


        [HttpPost]
        public IActionResult getAllocatedEmployeesData(int ProjectId)
        {
            List<ProjectDetails> empData = new List<ProjectDetails>();

            using (var command = new NpgsqlCommand($"SELECT * from getworkingemployeesofproject({ProjectId});", _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var assignEmpData = new ProjectDetails
                        {
                            WorkingEmployeeId = reader.GetInt32(0),
                            EmployeeCode = reader.GetString(1),
                            Employeename = reader.GetString(2),
                            EmployeeStartDate = reader.GetDateTime(5).Date,
                            EmployeeEndDate = reader.GetDateTime(6).Date,
                            EmployeeSkills = reader.IsDBNull(4) ? "not defined" : reader.GetString(4)
                        };
                        empData.Add(assignEmpData);
                    }

                }
            }
            return Json(JsonConvert.SerializeObject(empData));
        }


        [HttpPost]
        public IActionResult FetchNotAllocatedEmployees(string departmentName, int projectid)
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
                    string sqlQuery = "select * from public.getemployeesformapping(@deptId,@projectid)";
                    using (var cmd = new NpgsqlCommand(sqlQuery, _connection))
                    {
                        cmd.Parameters.AddWithValue("@deptId", departmentId);
                        cmd.Parameters.AddWithValue("@projectid", projectid);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var assignEmpData = new ProjectDetails
                                {
                                    EmployeeId = reader.GetInt32(0),
                                    EmployeeCode = reader.IsDBNull(1) ? "not found" : reader.GetString(1),
                                    NotAllocatedEmployeeName = reader.IsDBNull(2) ? "not found" : reader.GetString(2),
                                    NotAllocatedEmployeeSkills = reader.IsDBNull(4) ? "not defined" : reader.GetString(4),
                                    EmployeeDesignation = reader.IsDBNull(3) ? "not found" : reader.GetString(3)

                                };
                                empData.Add(assignEmpData);
                            }
                        }
                    }
                }
            }

            return Json(JsonConvert.SerializeObject(empData));
        }


        [HttpPost]
        public IActionResult RemoveFromProject(int employeeId, int projectId)
        {
            using (var command = new NpgsqlCommand("SELECT public.removeemployeefromproject( @employeeId,@projectId)", _connection))
            {
                command.Parameters.AddWithValue("@projectId", projectId);
                command.Parameters.AddWithValue("@employeeId", employeeId);

                command.ExecuteNonQuery();

            }
            return Json(new { success = true });
        }



        [HttpPost]
        public IActionResult AssignEmployees([FromBody] AssignEmployeeModel data)
        {
            // Create a list to hold all skill IDs
            Dictionary<string, long> tempSkills = getProjectSkills(data.ProjectId);

            // Initialize a list to hold the skill IDs for each employee
            List<string> tempEmployeeSkills = new List<string>();

            // Iterate over each employee's assigned skills
            for (var i = 0; i < data.AssignedSkillsOfEmployee.Length; i++)
            {
                List<long> currentEmployeeSkills = new List<long>();
                for (var j = 0; j < data.AssignedSkillsOfEmployee[i].Length; j++)
                {
                    string skillName = data.AssignedSkillsOfEmployee[i][j];
                    if (tempSkills.TryGetValue(skillName, out long skillId))
                    {
                        currentEmployeeSkills.Add(skillId);
                    }
                    else
                    {
                        Console.WriteLine($"Skill '{skillName}' not found in project skills.");
                    }
                }
                tempEmployeeSkills.Add(string.Join(",", currentEmployeeSkills));
            }

            // Use the fetched skill IDs for the stored procedure call
            using (var command = new NpgsqlCommand("select public.assignprojecttoemployee2(@in_employeeids,@in_skillsid,@in_startdate,@in_enddate,@in_createdby,@in_createdtime,@in_projectid)", _connection))
            {
                command.Parameters.Add(new NpgsqlParameter("in_employeeids", NpgsqlDbType.Array | NpgsqlDbType.Bigint) { Value = data.EmployeesId });
                // Assuming tempSkills is a flat list of skill IDs fetched earlier
                command.Parameters.Add(new NpgsqlParameter("in_skillsid", NpgsqlDbType.Array | NpgsqlDbType.Text) { Value = tempEmployeeSkills.ToArray() });
                command.Parameters.Add(new NpgsqlParameter("in_startdate", NpgsqlDbType.Array | NpgsqlDbType.Date) { Value = data.StartDate });
                command.Parameters.Add(new NpgsqlParameter("in_enddate", NpgsqlDbType.Array | NpgsqlDbType.Date) { Value = data.EndDate });
                command.Parameters.Add(new NpgsqlParameter("in_createdby", NpgsqlDbType.Bigint) { Value = Convert.ToInt64(HttpContext.Session.GetString("userId"))});
                command.Parameters.Add(new NpgsqlParameter("in_createdtime", NpgsqlDbType.Timestamp) { Value = DateTime.Now });
                command.Parameters.Add(new NpgsqlParameter("in_projectid", NpgsqlDbType.Bigint) { Value = data.ProjectId });

                command.ExecuteNonQuery();
            }

            string dataJson = System.Text.Json.JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("Received Data: " + dataJson);

            return Json(new { success = true, message = "Data saved successfully" });
        }

        private Dictionary<string, long> getProjectSkills(long projectId)
        {
            Dictionary<string, long> projectSkills = new Dictionary<string, long>();
            using (var cmd = new NpgsqlCommand($"SELECT * FROM GETPROJECTSKILLS({projectId})", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        projectSkills[reader.GetString(1)] = reader.GetInt64(0);
                    }
                }
            }
            return projectSkills;
        }




        [HttpPost]
        public IActionResult UpdateAllocatedEmployeesData([FromBody] List<ProjectDetails> updatedData)
        {
            try
            {
                foreach (var editedEmployee in updatedData)
                {
                    using (var command = new NpgsqlCommand("UPDATE public.projectemployeemaster SET masterstartdate = @StartDate, masterenddate = @EndDate WHERE masteremployeeid = @EmployeeId AND masterisworking=true; ", _connection))
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