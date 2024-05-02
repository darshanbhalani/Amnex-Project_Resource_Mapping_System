using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Npgsql;
using Project = Amnex_Project_Resource_Mapping_System.Models.Project;

namespace Amnex_Project_Resource_Mapping_System.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly NpgsqlConnection _connection;

        public ProjectsController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }


        [HttpPost]
        public IActionResult AddProject(Project project)
        {

            using (NpgsqlCommand cmd = new NpgsqlCommand(" select public.insertproject(:in_project_name::text,:in_department_id,:in_start_date::date ,:in_end_date::date,:in_skills_id::text,:in_status::text,:in_created_by::integer,:in_create_time::timestamp,:in_modify_by::integer,:in_modify_time::timestamp,:in_is_deleted);", _connection))
            {
                cmd.Parameters.AddWithValue("in_project_name", project.ProjectName);
                cmd.Parameters.AddWithValue("in_department_id", project.DepartmentId);
                cmd.Parameters.AddWithValue("in_start_date", project.StartDate);
                cmd.Parameters.AddWithValue("in_end_date", project.EndDate);
                cmd.Parameters.AddWithValue("in_skills_id", project.SkillId);
                cmd.Parameters.AddWithValue("in_status", "Pending");
                cmd.Parameters.AddWithValue("in_created_by", Convert.ToInt32(HttpContext.Session.GetString("userId")!));
                cmd.Parameters.AddWithValue("in_create_time", DateTime.Now);
                cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                cmd.Parameters.AddWithValue("in_modify_by", Convert.ToInt32(HttpContext.Session.GetString("userId")!));
                cmd.Parameters.AddWithValue("in_is_deleted", false);
                cmd.ExecuteNonQuery();
            }

            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult UpdateProject(Project data)
        {

            using (NpgsqlCommand cmd = new NpgsqlCommand("select public.updateproject(@in_project_id::integer,@in_project_name::text,@in_department_id::integer,@in_start_date::date,@in_end_date::date,@in_skills_id::text,@in_status::text,@in_modify_by::integer,@in_modify_time::timestamp);", _connection))
            {
                cmd.Parameters.AddWithValue("in_project_id", data.ProjectId);
                cmd.Parameters.AddWithValue("in_project_name", data.ProjectName);
                cmd.Parameters.AddWithValue("in_department_id", data.DepartmentId);
                cmd.Parameters.AddWithValue("in_start_date", data.StartDate);
                cmd.Parameters.AddWithValue("in_end_date", data.EndDate);
                cmd.Parameters.AddWithValue("in_skills_id", data.SkillId);
                cmd.Parameters.AddWithValue("in_status", data.Status);

                cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                cmd.Parameters.AddWithValue("in_modify_by", Convert.ToInt32(HttpContext.Session.GetString("userId")!));
                cmd.Parameters.AddWithValue("in_is_deleted", false);
                cmd.ExecuteNonQuery();
            }

            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult DeleteProject(int projectId)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand("select public.deleteproject(:projectId,:modifyTime ,:modifyBy);", _connection))
            {
                cmd.Parameters.AddWithValue("projectId", projectId);
                cmd.Parameters.AddWithValue("modifyTime", DateTime.Now);
                cmd.Parameters.AddWithValue("modifyBy", Convert.ToInt32(HttpContext.Session.GetString("userId")!));
                cmd.ExecuteNonQuery();
            }

            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult GetEmployeeById(int projectId)
        {
            List<Employee> employeeDetailsList = new List<Employee>();

            using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * from public.getemployeesforrating({projectId})", _connection))
            {
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Employee employeeDetails = new Employee();
                        employeeDetails.EmployeeId = Convert.ToInt32(reader.GetValue(0));
                        employeeDetails.EmployeeName = reader.GetString(1);
                        employeeDetailsList.Add(employeeDetails);
                    }
                }
            }
            return Json(new { data = employeeDetailsList });
        }


        [HttpPost]
        public IActionResult GetDepartmentByProjectId(int projectId)
        {
            string skillid = "";
            int departmentId = 0;
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;

            using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT d.departmentid, p.startDate, p.endDate,p.skillsid FROM departments d INNER JOIN projects p ON d.departmentId = p.departmentId WHERE p.projectId = {projectId}", _connection))
            {
                cmd.Parameters.AddWithValue("projectId", projectId);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        departmentId = Convert.ToInt32(reader.GetValue(0));
                        startDate = reader.GetDateTime(1);
                        endDate = reader.GetDateTime(2);
                        skillid = reader.GetString(3);

                    }
                }
            }

            return Json(new { departmentId = departmentId, startDate = startDate.ToString("dd-MM-yyyy"), endDate = endDate.ToString("dd-MM-yyyy"), skillid = skillid });
        }


        [HttpPost]
        public ActionResult completeProject(int projectId)
        {

            using (NpgsqlCommand cmd = new NpgsqlCommand("select public.completeproject(@in_project_id,@in_modify_by,@in_modify_time)", _connection))
            {
                cmd.Parameters.AddWithValue("in_project_id", projectId);
                cmd.Parameters.AddWithValue("in_modify_by", Convert.ToInt32(HttpContext.Session.GetString("userId")!));
                cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult GiveRatingToEmployee(int projectId, int[] employeeIds, int[] employeeRatings)
        {
            using (var command = new NpgsqlCommand(" select public.giveratingtoemployee(@project_id,@employees_id,@employees_rating,@modify_by,@modify_time)", _connection))
            {
                command.Parameters.AddWithValue("project_id", projectId);
                command.Parameters.AddWithValue("employees_id", employeeIds);
                command.Parameters.AddWithValue("employees_rating", employeeRatings);
                command.Parameters.AddWithValue("modify_by", Convert.ToInt32(HttpContext.Session.GetString("userId")!));
                command.Parameters.AddWithValue("modify_time", DateTime.Now);

                command.ExecuteNonQuery();
            }
            return Ok("Employee ratings updated successfully");
        }

    }
}

