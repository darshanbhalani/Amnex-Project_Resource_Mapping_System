using Amnex_Project_Resource_Mapping_System.Models;
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
        public IActionResult Departments()
        {
            List<Department> departments = GetDepartment();
            ViewData["DepartmentData"] = JsonConvert.SerializeObject(departments);
            return View();
        }
        [HttpGet]
        public List<Department> GetDepartment()
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
            return departments;
        }
        [HttpPost]
        public IActionResult AddDepartment(Department model)
        {
            try
            {
                using (var cmd = new NpgsqlCommand("select public.insertdepartment(@in_department_name,@in_created_by,@in_modify_by,@in_create_time,@in_modify_time,@in_is_deleted)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_department_name", model.DepartmentName);
                    cmd.Parameters.AddWithValue("in_created_by", 1);
                    cmd.Parameters.AddWithValue("in_modify_by", 1);
                    cmd.Parameters.AddWithValue("in_create_time", DateTime.Now);
                    cmd.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                    cmd.Parameters.AddWithValue("in_is_deleted", false);
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true, department = model });
            }
            catch (NpgsqlException ex)
            {
                if (ex.SqlState == "23505")
                {
                    var fetchdeptid = checkId(model.DepartmentName);
                    var checkdata = checkDelete(model.DepartmentName);
                    if (checkdata)
                    {
                        changeIsDeleted(fetchdeptid);
                        return Json(new { success = true });

                    }

                    Response.StatusCode = 400;
                    return Json(new { success = false, error = "Department already exists" });
                }
                else
                {
                    Response.StatusCode = 500;
                    return Json(new { success = false, error = ex.Message });
                }
            }
        }
        [HttpPut]
        public IActionResult Editdepartment(Department dept)
        {
            //string uid = HttpContext.Session.GetString("userId")!;

            try
            {

                var sql = "select public.updatedepartment(@in_department_id,@in_department_name,@in_modify_time,@in_modify_by)";

                using (var cmd = new NpgsqlCommand(sql, _connection))
                {
                    cmd.Parameters.AddWithValue("in_department_id", dept.DepartmentId);
                    cmd.Parameters.AddWithValue("in_department_name", dept.DepartmentName);
                    cmd.Parameters.AddWithValue("in_modify_by", 1);
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
            try
            {

                var sql = "SELECT public.deletedepartment(@in_department_id, @in_modify_time, @in_modify_by)";
                using (var cmd2 = new NpgsqlCommand(sql, _connection))
                {
                    cmd2.Parameters.AddWithValue("in_department_id", model.DepartmentId);
                    cmd2.Parameters.AddWithValue("in_modify_by", 1);
                    cmd2.Parameters.AddWithValue("in_modify_time", DateTime.Now);
                    cmd2.ExecuteNonQuery();
                }

                // Return success JSON response
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                // Return error JSON response with exception details
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }
        public int checkId(string departmentname)
        {
            int departmentId = 0;

            var sql = "select departmentid from departments where departmentname = @departmentname";
            using (var cmd = new NpgsqlCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("departmentname", departmentname);

                object result = cmd.ExecuteScalar()!;

                if (result != null)
                {
                    departmentId = Convert.ToInt32(result);
                    Console.WriteLine("DepartmentId: " + departmentId);
                    return departmentId;
                }
                else
                {
                    Console.WriteLine("Department not found.");
                    return 0;
                }
            }

        }
        public void changeIsDeleted(int DepartmentId)
        {
            string uid = HttpContext.Session.GetString("userId")!;

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
        public bool checkDelete(string departmentname)
        {
            Console.WriteLine(departmentname);
            var sql = "select isdeleted from departments where departmentname = @departmentname";

            using (var cmd = new NpgsqlCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("departmentname", departmentname);
                var count = (bool)cmd.ExecuteScalar()!;
                return count;
            }
        }
        internal static List<dynamic> getDepartmentList(NpgsqlConnection _connection)
        {
            List<dynamic> departments = [];
            using (NpgsqlCommand cmd = new NpgsqlCommand("select * from public.displayalldepartments();", _connection))
            {
                using (NpgsqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        List<dynamic> x = [];
                        x.Add(Convert.ToInt32(dr.GetValue(0)));
                        x.Add(dr.GetValue(1));
                        departments.Add(x);

                    }
                }
            }
            return departments;
        }
        public IActionResult GetDetails(int departmentId)
        {
            Department department = new Department
            {
                DepartmentId = departmentId,
                // Add other properties as needed
            };

            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.getdepartmentdetails(@departmentid)", _connection))
            {
                cmd.Parameters.AddWithValue("@departmentid", departmentId);

                using (NpgsqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        // Read department details from the reader
                        department.DepartmentName = dr["departmentname"].ToString()!;
                        department.totalprojects = Convert.ToInt32(dr["totalprojects"]);
                        department.completedprojects= Convert.ToInt32(dr["completedprojects"])!;
                        department.runningprojects = Convert.ToInt32(dr["runningprojects"])!;
                        department.pendingprojects = Convert.ToInt32(dr["pendingprojects"])!;
                        department.totalemployees = Convert.ToInt32(dr["totalemployees"])!;
                        department.allocatedemployees = Convert.ToInt32(dr["allocatedemployees"])!;
                        department.unallocatedemployees = Convert.ToInt32(dr["unallocatedemployees"])!;

                        return View(department);
                    }
                    else
                    {
                        Console.WriteLine("Department not found.");
                        return View();
                    }
                }
            }
        }

        public IActionResult deptProjects(int departmentId)
        {
            ViewData["DepartmentId"] = departmentId;

            return View();
        }

    }
}
