using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using System.Data;

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
                using (var cmd = new NpgsqlCommand("select public.adddepartment(@in_department_name,@in_created_by)", _connection))
                {
                    cmd.Parameters.AddWithValue("in_department_name", model.DepartmentName);
                    cmd.Parameters.AddWithValue("in_created_by", 1);
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

                var sql = "select public.updatedepartment(@in_department_id,@in_department_name,@in_modify_by)";

                using (var cmd = new NpgsqlCommand(sql, _connection))
                {
                    cmd.Parameters.AddWithValue("in_department_id", dept.DepartmentId);
                    cmd.Parameters.AddWithValue("in_department_name", dept.DepartmentName);
                    cmd.Parameters.AddWithValue("in_modify_by", 1);
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

                var sql = "SELECT public.deletedepartment(@in_department_id, @in_modify_by)";
                using (var cmd2 = new NpgsqlCommand(sql, _connection))
                {
                    cmd2.Parameters.AddWithValue("in_department_id", model.DepartmentId);
                    cmd2.Parameters.AddWithValue("in_modify_by", 1);
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

            var sql = "update departments set modifiedby = @modifiedby,modifiedtime = @modifiedtime, isDeleted = false where departmentid = @departmentid";
            using (var cmd = new NpgsqlCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("departmentid", DepartmentId);
                cmd.Parameters.AddWithValue("modifiedby", Convert.ToInt32(uid));
                cmd.Parameters.AddWithValue("modifiedtime", DateTime.Now);
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
            };

            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.getdepartmentdetails(@departmentid)", _connection))
            {
                cmd.Parameters.AddWithValue("@departmentid", departmentId);

                using (NpgsqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        department.DepartmentName = dr["departmentname"].ToString()!;
                        department.totalprojects = Convert.ToInt32(dr["totalprojects"]);
                        department.completedprojects = Convert.ToInt32(dr["completedprojects"])!;
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

        public List<Employee> GetDepartmentEmployees(int departmentId)
        {
            var departmentEmployees = new List<Employee>();

            using (var cmd = new NpgsqlCommand("SELECT * FROM public.getdepartmentemployees(@departmentId)", _connection))
            {
                Console.WriteLine(Convert.ToInt64(departmentId));
                cmd.Parameters.AddWithValue("@departmentId", Convert.ToInt64(departmentId));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var employee = new Employee
                        {
                            EmployeeAipl = reader.GetString(0),
                            EmployeeName = reader.GetString(1),
                            EmployeeDesignation = reader.GetString(2),
                            EmployeeSkills = reader["employeeskills"] != DBNull.Value ? reader["employeeskills"].ToString() : string.Empty,
                            IsAllocated = (bool)reader["isallocated"],
                        };
                        departmentEmployees.Add(employee);
                    }
                }
            }

            return departmentEmployees;
        }

        public List<Project> GetDepartmentProjects(int departmentId)
        {
            var departmentProject = new List<Project>();
            using (var cmd = new NpgsqlCommand("SELECT * FROM public.getdepartmentprojects(@departmentId)", _connection))
            {
                Console.WriteLine(Convert.ToInt64(departmentId));
                cmd.Parameters.AddWithValue("@departmentId", Convert.ToInt64(departmentId));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var project = new Project
                        {
                            ProjectName = Convert.ToString(reader["projectname"])!,
                            StartDate = Convert.IsDBNull(reader["projectstartdate"]) ? default : DateOnly.FromDateTime((DateTime)reader["projectstartdate"]),
                            EndDate = Convert.IsDBNull(reader["projectenddate"]) ? default : DateOnly.FromDateTime((DateTime)reader["projectenddate"]),
                            SkillName = reader["projectskills"]!= DBNull.Value ? reader["projectskills"].ToString()! : "Not found",
                            Status = reader["projectstatus"] != DBNull.Value ? reader["projectstatus"].ToString()! : "Not found",
                        };
                        departmentProject.Add(project);
                    }
                }
            }

            return departmentProject;
        }

        public IActionResult GetDepartmentname(int departmentId)
        {
            var departmentname = "";
            using (var cmd = new NpgsqlCommand("SELECT * FROM public.departments where departmentid = @departmentId", _connection))
            {
                Console.WriteLine(Convert.ToInt64(departmentId));
                cmd.Parameters.AddWithValue("@departmentId", Convert.ToInt64(departmentId));

                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) // Check if there are rows returned
                {
                    departmentname = reader.GetString(1); // Index 0 for the first column
                }
            }

            return Json(new { departmentname  = departmentname });
        }


        [HttpGet]

        public IActionResult deptProjects(int departmentId)
        {
            ViewData["DepartmentId"] = departmentId;
            var departmentEmployees = GetDepartmentProjects(departmentId);

            return View(departmentEmployees);
        }
        [HttpGet]
        public IActionResult deptEmployees(int departmentId)
        {
            ViewData["DepartmentId2"] = departmentId;
            Console.WriteLine("In controller " + departmentId);
            var departmentEmployees = GetDepartmentEmployees(departmentId);


            return View(departmentEmployees);
        }
    }
}
