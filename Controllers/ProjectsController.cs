using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Npgsql;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
        public IActionResult AddProject(Project project)
        {

            using (NpgsqlCommand cmd = new NpgsqlCommand(" select public.insertproject(:in_project_name::text,:in_department_id,:in_start_date::date ,:in_end_date::date,:in_skills_id::text,:in_status::text,:in_created_by::integer,:in_create_time::timestamp,:in_modify_by::integer,:in_modify_time::timestamp,:in_is_deleted);", _connection))
            {
                cmd.Parameters.AddWithValue("in_project_name", project.projectName);
                cmd.Parameters.AddWithValue("in_department_id", project.departmentId);
                cmd.Parameters.AddWithValue("in_start_date", project.startDate);
                cmd.Parameters.AddWithValue("in_end_date", project.endDate);
                cmd.Parameters.AddWithValue("in_skills_id", project.skillid);
                cmd.Parameters.AddWithValue("in_status", "Pending");
                cmd.Parameters.AddWithValue("in_created_by", 1);
                cmd.Parameters.AddWithValue("in_create_time", DateTime.Now);
                cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                cmd.Parameters.AddWithValue("in_modify_by", 1);
                cmd.Parameters.AddWithValue("in_is_deleted", false);
                cmd.ExecuteNonQuery();
            }

            return Json(new { success = true });
        }

        public IActionResult UpdateProject(Project data)
        {

            using (NpgsqlCommand cmd = new NpgsqlCommand("select public.updateproject(@in_project_id::integer,@in_project_name::text,@in_department_id::integer,@in_start_date::date,@in_end_date::date,@in_skills_id::text,@in_status::text,@in_modify_by::integer,@in_modify_time::timestamp);", _connection))
            {
                cmd.Parameters.AddWithValue("in_project_id", data.projectId);
                cmd.Parameters.AddWithValue("in_project_name", data.projectName);
                cmd.Parameters.AddWithValue("in_department_id", data.departmentId);
                cmd.Parameters.AddWithValue("in_start_date", data.startDate);
                cmd.Parameters.AddWithValue("in_end_date", data.endDate);
                cmd.Parameters.AddWithValue("in_skills_id", data.skillid);
                cmd.Parameters.AddWithValue("in_status", data.status);

                cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                cmd.Parameters.AddWithValue("in_modify_by", 1);
                cmd.Parameters.AddWithValue("in_is_deleted", false);
                cmd.ExecuteNonQuery();
            }

            return Json(new { success = true });
        }

        public IActionResult DeleteProject(int projectId)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand("select public.deleteproject(:projectId,:modifyTime ,:modifyBy);", _connection))
            {
                cmd.Parameters.AddWithValue("projectId", projectId);
                cmd.Parameters.AddWithValue("modifyTime", DateTime.Now);
                cmd.Parameters.AddWithValue("modifyBy", 1);
                cmd.ExecuteNonQuery();
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult GetEmployeeById(int projectId)
        {

            try
            {

                string employeeName = "";
                List<string> employeeNames = new List<string>();



                using (NpgsqlCommand cmd = new NpgsqlCommand($"select employeename from employees where projectid = {projectId};", _connection))
                {
                    cmd.Parameters.AddWithValue("projectId", projectId);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            // Get Employee Name
                            employeeName = reader.GetString(0);
                            employeeNames.Add(employeeName);
                        }
                    }
                }

                return Json(new { employeeNames = employeeNames });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }); // Handle any exceptions
            }


        }
        public IActionResult GetDepartmentByProjectId(int projectId)
        {
            try
            {

                string skillid = "";
                int departmentId = 0; // Initialize department variable
                DateTime startDate = DateTime.MinValue; // Initialize start date
                DateTime endDate = DateTime.MinValue;

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT d.departmentid, p.startDate, p.endDate,p.skillsid FROM departments d INNER JOIN projects p ON d.departmentId = p.departmentId WHERE p.projectId = {projectId}", _connection))
                {
                    cmd.Parameters.AddWithValue("projectId", projectId);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            departmentId = Convert.ToInt32(reader.GetValue(0)); // Get department name
                            startDate = reader.GetDateTime(1); // Get start date
                            endDate = reader.GetDateTime(2);  // Get start date
                            skillid = reader.GetString(3);

                        }
                    }
                }

                return Json(new { departmentId = departmentId, startDate = startDate.ToString("dd-MM-yyyy"), endDate = endDate.ToString("dd-MM-yyyy"), skillid = skillid });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }); // Handle any exceptions
            }
        }
        [HttpPost]
        public ActionResult completeProject(int projectId1)
        {

            using (NpgsqlCommand cmd = new NpgsqlCommand("select public.completeproject(@in_project_id,@in_modify_by,@in_modify_time)", _connection))
            {

                cmd.Parameters.AddWithValue("in_project_id", projectId1);
                cmd.Parameters.AddWithValue("in_modify_by", 1);
                cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
            return Json(new { success = true });
        }

    }
}
