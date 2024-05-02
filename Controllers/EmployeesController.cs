using Amnex_Project_Resource_Mapping_System.Controllers.Amnex_Project_Resource_Mapping_System.Controllers;
using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Amnex_Project_Resource_Mapping_System.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly NpgsqlConnection _connection;

        public EmployeesController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }


        [HttpPost]
        public IActionResult AddEmployee(Employee empObj)
        {
            string password = generateRandomPassword();
            string query = "SELECT * FROM public.insertemployee(@in_employeename, @in_employeeusername, @in_departmentid, @in_skillsid, @in_employeeloginroleid, @in_email, @in_password, @in_createdby, @in_createdtime)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@in_employeename", empObj.EmployeeName);
                command.Parameters.AddWithValue("@in_employeeusername", empObj.EmployeeUserName);
                command.Parameters.AddWithValue("@in_departmentid", empObj.DepartmentId);
                command.Parameters.AddWithValue("@in_skillsid", empObj.SkillsId);
                command.Parameters.AddWithValue("@in_employeeloginroleid", empObj.LoginRoleId);
                command.Parameters.AddWithValue("@in_email", empObj.Email);
                command.Parameters.AddWithValue("@in_password", password);
                command.Parameters.AddWithValue("@in_createdby", Convert.ToInt32(HttpContext.Session.GetString("userId")!)); 
                command.Parameters.AddWithValue("@in_createdtime", DateTime.Now); 
                command.Parameters.AddWithValue("@in_modifyby", Convert.ToInt32(HttpContext.Session.GetString("userId")!));
                command.Parameters.AddWithValue("@in_modifytime", DateTime.Now);
                command.ExecuteNonQuery();
            }
            Employee employee = new Employee { 
                EmployeeName = empObj.EmployeeName,
                EmployeeUserName = empObj.EmployeeUserName,
                Password = password,
                Email = empObj.Email,
            };
            AccountController.SendCredentials(employee);
            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult GetEmployeeById(int employeeId)
        {
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM public.getemployeeprofile(@employeeId)", _connection))
                {
                    command.Parameters.AddWithValue("@employeeId", employeeId);

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Employee employee = new Employee
                            {
                                EmployeeId = Convert.ToInt32(reader["employeeid"]),
                                EmployeeName = reader["employeename"].ToString()!,
                                EmployeeUserName = reader["employeeusername"].ToString()!,
                                DepartmentId = Convert.ToInt32(reader["departmentid"]),
                                SkillsId = reader["skillsid"].ToString()!,
                                LoginRoleId = Convert.ToInt32(reader["loginroleid"]),
                                IsAllocated = Convert.ToBoolean(reader["isallocated"]),
                                Email = reader["email"].ToString()!,
                            };

                            return Json(new { success = true, employee = employee });
                        }
                        else
                        {

                            return Json(new { success = false, message = "Employee not found." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error: " + ex.Message);

                return Json(new { success = false, message = "An error occurred while fetching employee data." });
            }
        }


        [HttpPost]
        public IActionResult UpdateEmployee(Employee empUpdateObj)
        {
            string query = "SELECT public.updateemployee(@EmployeeId, @EmployeeName, @DepartmentId, @Email, @SkillsId, @LoginRoleId, @ModifyBy, @ModifyTime)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@EmployeeId", empUpdateObj.EmployeeId);
                command.Parameters.AddWithValue("@EmployeeName", empUpdateObj.EmployeeName);
                command.Parameters.AddWithValue("@DepartmentId", empUpdateObj.DepartmentId);
                command.Parameters.AddWithValue("@Email", empUpdateObj.Email);
                command.Parameters.AddWithValue("@SkillsId", empUpdateObj.SkillsId);
                command.Parameters.AddWithValue("@LoginRoleId", empUpdateObj.LoginRoleId);
                command.Parameters.AddWithValue("@ModifyBy", Convert.ToInt32(HttpContext.Session.GetString("userId")!)); 
                command.Parameters.AddWithValue("@ModifyTime", DateTime.Now); // Current timestamp

                command.ExecuteNonQuery();
            }

            return Json(new { success = true });
        }


        public IActionResult DeleteEmployee(Employee employee)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand("select public.deleteemployee(:employeeId,:modifyBy,:modifyTime );", _connection))
            {
                cmd.Parameters.AddWithValue("employeeId", employee.EmployeeId);
                cmd.Parameters.AddWithValue("modifyBy", Convert.ToInt32(HttpContext.Session.GetString("userId")!));
                cmd.Parameters.AddWithValue("modifyTime", DateTime.Now);
                cmd.ExecuteNonQuery();
            }

            return Json(new { success = true });
        }


        private string generateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder passwordBuilder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < 8; i++)
            {
                passwordBuilder.Append(chars[random.Next(chars.Length)]);
            }
            return passwordBuilder.ToString();
        }


        internal static List<Employee> getEmployeesList(NpgsqlConnection _connection)
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
                                EmployeeName = Convert.ToString(reader["employee_name"])!,
                                DepartmentId = Convert.ToInt32(reader["department_id"]),
                                SkillsId = Convert.ToString(reader["skills_id"])!,
                                LoginRoleId = Convert.ToInt32(reader["login_role_id"]),
                                IsAllocated = Convert.ToBoolean(reader["is_allocated"]),
                                Email = Convert.ToString(reader["email"])!,
                                CreatedbyName = Convert.ToString(reader["created_by"])!,
                                CreatedTime = Convert.ToDateTime(reader["created_time"]),
                                ModifybyName = Convert.ToString(reader["modify_by"])!,
                                ModifyTime = Convert.ToDateTime(reader["modify_time"])
                            };

                            employees.Add(employee);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return employees;
        }


        internal static List<Department> getEmployeeDepartmentList(NpgsqlConnection _connection)
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

    }
}