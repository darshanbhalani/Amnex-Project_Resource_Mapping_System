using Amnex_Project_Resource_Mapping_System.Models;
using Amnex_Project_Resource_Mapping_System.Repo.Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;

namespace Amnex_Project_Resource_Mapping_System.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly NpgsqlConnection _connection;

        public DepartmentsController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }
        [HttpGet]
        public IActionResult Departments()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetDepartments()
        {
            List<Department> departments = new List<Department>();

            // Prepare SQL command to call the function
            var sql = "SELECT * FROM public.displayalldepartments()";

            using (var cmd = new NpgsqlCommand(sql, _connection))
            {
                // Execute the query
                using (var reader = cmd.ExecuteReader())
                {
                    // Read the data and populate the list
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
            var Json1 = JsonConvert.SerializeObject(departments);
            //return View(departments);
            return Json(Json1);
        }

        [HttpPost]
        public IActionResult Add(Department model)
        {
            string uid = HttpContext.Session.GetString("userId");

            try
            {
                var sql = "select public.insertdepartment(@in_department_name,@in_created_by,@in_modify_by,@in_create_time,@in_modify_time,@in_is_deleted)";

                using (var cmd = new NpgsqlCommand(sql, _connection))
                {
                    cmd.Parameters.AddWithValue("in_department_name", model.DepartmentName);
                    cmd.Parameters.AddWithValue("in_created_by", Convert.ToInt32(uid));
                    cmd.Parameters.AddWithValue("in_modify_by", Convert.ToInt32(uid));
                    cmd.Parameters.AddWithValue("in_create_time", DateTime.Now);
                    cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                    cmd.Parameters.AddWithValue("in_is_deleted", false);
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true });
            }
            catch (NpgsqlException ex)
            {
                if (ex.SqlState == "23505") // Unique Violation SQLState
                {
                    var fetchdeptid = CheckId(model.DepartmentName);
                    var checkdata = Checkdelete(model.DepartmentName);
                    if (checkdata)
                    {
                        changeIsDeleted(fetchdeptid);
                        return Json(new { success = true });

                    }
                    // Handle duplicate entry error

                    Response.StatusCode = 400; // Set HTTP status code to 400 Bad Request
                    return Json(new { success = false, error = "Department already exists" });
                }
                else
                {
                    // Handle other Npgsql exceptions
                    Response.StatusCode = 500; // Set HTTP status code to 500 Internal Server Error
                    return Json(new { success = false, error = ex.Message });
                }
            }
        }
        public int CheckId(string departmentname)
        {
            int departmentId = 0;

            var sql = "select departmentid from departments where departmentname = @departmentname";
            using (var cmd = new NpgsqlCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("departmentname", departmentname);

                object result = cmd.ExecuteScalar();

                // Check if the result is not null and can be converted to int
                if (result != null)
                {
                    departmentId = Convert.ToInt32(result);
                    Console.WriteLine("DepartmentId: " + departmentId);
                    return departmentId;
                }
                else
                {
                    // Handle case where no departmentid was found
                    Console.WriteLine("Department not found.");
                    return 0;
                }
            }

        }
        public void changeIsDeleted(int DepartmentId)
        {
            string uid = HttpContext.Session.GetString("userId");

            var sql = "update departments set modifyby = @modifyby,modifytime = @modifytime, isDeleted = false where departmentid = @departmentid";
            using (var cmd = new NpgsqlCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("departmentid", DepartmentId);
                cmd.Parameters.AddWithValue("modifyby", Convert.ToInt32(uid));
                cmd.Parameters.AddWithValue("modifytime", DateTime.Now);
                cmd.Parameters.AddWithValue("isDeleted", false);
                cmd.ExecuteNonQuery();
            }


        }
        public bool Checkdelete(string departmentname)
        {
            Console.WriteLine(departmentname);
            var sql = "select isdeleted from departments where departmentname = @departmentname";

            using (var cmd = new NpgsqlCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("departmentname", departmentname);
                var count = (bool)cmd.ExecuteScalar();
                return count;
            }
        }
        [HttpPut]
        public IActionResult Editdepartment(Department model)
        {
            string uid = HttpContext.Session.GetString("userId");

            try
            {

                var sql = "select public.updatedepartment(@in_department_id,@in_department_name,@in_modify_time,@in_modify_by)";

                using (var cmd = new NpgsqlCommand(sql, _connection))
                {
                    cmd.Parameters.AddWithValue("in_department_id", model.DepartmentId);
                    cmd.Parameters.AddWithValue("in_department_name", model.DepartmentName);
                    cmd.Parameters.AddWithValue("in_modify_by", Convert.ToInt32(uid));
                    cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }


                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Handle exception
                return Json(new { success = false, error = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult Deletedepartment(Department model)
        {
            string uid = HttpContext.Session.GetString("userId");

            try
            {

                var sql = "select public.deletedepartment(@in_department_id,@in_modify_time,@in_modify_by)";

                using (var cmd = new NpgsqlCommand(sql, _connection))
                {
                    cmd.Parameters.AddWithValue("in_department_id", model.DepartmentId);
                    cmd.Parameters.AddWithValue("in_modify_by", Convert.ToInt32(uid));
                    cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }


                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Handle exception
                return Json(new { success = false, error = ex.Message });
            }
        }



    }
}
