using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
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
            string query = "SELECT public.addemployee(@in_employeename, @in_employeedepartmentid, @in_employeeemail, @in_employeedesignationid, @in_password, @in_employeeloginroleid, @in_skillsid, @in_createdby)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@in_employeename", empObj.EmployeeName);
                command.Parameters.AddWithValue("@in_employeedepartmentid", empObj.DepartmentId);
                command.Parameters.AddWithValue("@in_employeeemail", empObj.Email);
                command.Parameters.AddWithValue("@in_employeedesignationid", empObj.DesignationId);
                command.Parameters.AddWithValue("@in_password", password);
                command.Parameters.AddWithValue("@in_employeeloginroleid", empObj.LoginRoleId);
                command.Parameters.AddWithValue("@in_skillsid", empObj.SkillsId.Split(',').Select(int.Parse).ToArray()); // Split the string into an array
                command.Parameters.AddWithValue("@in_createdby", 1); // Assuming CreatedById is the correct property

                var result = command.ExecuteScalar();
                string aipl = result != null ? result.ToString() : null;

                if (!string.IsNullOrEmpty(aipl))
                {
                    return Json(new { success = true });
                }
                else
                {
                    // Handle error
                    return Json(new { success = false, message = "Failed to add employee." });
                }
            }
        }
        [HttpGet]
        public ActionResult EmployeeDetails(int employeeId)
        {
            Employee employeeDetails;

            using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM public.getemployeedetails(@employeeId)", _connection))
            {
                command.Parameters.AddWithValue("@employeeId", employeeId);

                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        employeeDetails = new Employee
                        {
                            EmployeeId = employeeId,
                            EmployeeName = reader["employeename"].ToString()!,
                            EmployeeAipl = reader["employeeaipl"].ToString()!,
                            DepartmentName = reader["employeedepartmentname"].ToString()!,
                            Email = reader["employeeemail"].ToString()!,
                            DesignationName = reader["employeedesignation"].ToString()!,
                            SkillsName = reader["employeeskills"].ToString()!,
                            LoginRole = reader["employeeloginrole"].ToString()!,
                            TotalCompletedProjects = Convert.ToInt32(reader["totalcompletedprojects"])
                        };
                    }
                    else
                    {
                        return Json(new { success = false, message = "Employee not found." });
                    }
                }
            }

            ViewBag.EmployeeProjectDetails = EmployeesController.getEmployeeProjectDetailsList(_connection, employeeId);

            return View(employeeDetails);
        }

        [HttpPost]
        public IActionResult UpdateEmployee(Employee employeeObj)
        {
            // Check if employee details are updateable
            string checkQuery = "SELECT public.isemployeedetailsupdateable(@EmployeeId, @DesignationId, @DepartmentId)";
            bool isUpdateable = false;

            using (NpgsqlCommand checkCommand = new NpgsqlCommand(checkQuery, _connection))
            {
                checkCommand.Parameters.AddWithValue("@EmployeeId", employeeObj.EmployeeId);
                checkCommand.Parameters.AddWithValue("@DesignationId", employeeObj.DesignationId);
                checkCommand.Parameters.AddWithValue("@DepartmentId", employeeObj.DepartmentId);

                // Execute scalar to get the result
                isUpdateable = (bool)checkCommand.ExecuteScalar();
            }

            if (isUpdateable)
            {
                // Update employee details
                string updateQuery = "SELECT public.updateemployeedetails(@EmployeeId, @EmployeeName, @DepartmentId, @Email, @DesignationId, @LoginRoleId, @SkillsId, @ModifyBy)";

                using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, _connection))
                {
                    updateCommand.Parameters.AddWithValue("@EmployeeId", employeeObj.EmployeeId);
                    updateCommand.Parameters.AddWithValue("@EmployeeName", employeeObj.EmployeeName);
                    updateCommand.Parameters.AddWithValue("@DepartmentId", employeeObj.DepartmentId);
                    updateCommand.Parameters.AddWithValue("@Email", employeeObj.Email);
                    updateCommand.Parameters.AddWithValue("@DesignationId", employeeObj.DesignationId);
                    updateCommand.Parameters.AddWithValue("@LoginRoleId", employeeObj.LoginRoleId);
                    updateCommand.Parameters.AddWithValue("@SkillsId", employeeObj.SkillsId.Split(',').Select(int.Parse).ToArray());
                    updateCommand.Parameters.AddWithValue("@ModifyBy", 1);

                    updateCommand.ExecuteNonQuery();
                }

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Employee details cannot be updated." });
            }
        }



        public IActionResult DeleteEmployee(Employee employee)
        {
            try
            {
                bool isDeletable = IsEmployeeDeletable(employee.EmployeeId);

                if (isDeletable == false)
                {
                    return Json(new { success = false, message = "Note: Employee is not deletable." });
                }

                // Proceed with the delete operation
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT public.deleteemployee(:employeeId, :modifyBy);", _connection))
                {
                    cmd.Parameters.AddWithValue("employeeId", employee.EmployeeId);
                    cmd.Parameters.AddWithValue("modifyBy", Convert.ToInt32(HttpContext.Session.GetString("userId")));
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine("Error: " + ex.Message);

                return Json(new { success = false, message = "An error occurred while deleting the employee." });
            }
        }

        public bool IsEmployeeDeletable(int employeeId)
        {
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT public.isemployeedeletable(@employeeId)", _connection))
            {
                command.Parameters.AddWithValue("@employeeId", employeeId);
                bool isDeletable = (bool)command.ExecuteScalar();
                return isDeletable;
            }
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

        public static List<Skill> getEmployeeSkillsList(NpgsqlConnection _connection)
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

        internal static List<Employee> getEmployeesList(NpgsqlConnection _connection)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.displayallemployees()", _connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Employee employee = new Employee
                            {
                                EmployeeId = Convert.ToInt32(reader["employeeid"]),
                                EmployeeAipl = Convert.ToString(reader["employeeaipl"])!,
                                EmployeeName = Convert.ToString(reader["employeename"])!,
                                Email = Convert.ToString(reader["employeeemail"])!,
                                IsAllocated = Convert.ToBoolean(reader["isallocated"]),
                                DepartmentName = Convert.ToString(reader["employeedepartmentname"])!,
                                DesignationName = Convert.ToString(reader["employeedesignationname"])!,
                                SkillsId = Convert.ToString(reader["employeeskills"])!,
                                LoginRole = Convert.ToString(reader["employeesloginrolename"])!
                            };

                            employees.Add(employee);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                // Handle exception appropriately, e.g., log or throw
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

        internal static List<EmployeeRecord> getEmployeeProjectDetailsList(NpgsqlConnection _connection, long employeeId)
        {
            List<EmployeeRecord> employeesRecord = new List<EmployeeRecord>();
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.getemployeeprojects(@employeeId)", _connection))
                {
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EmployeeRecord empRecord = new EmployeeRecord
                            {
                                ProjectName = reader["projectname"].ToString(),
                                EmployeeProjectSkill = reader["employeeprojectskill"].ToString(),
                                StartDate = Convert.ToDateTime(reader["masterstartdate"]),
                                EndDate = Convert.ToDateTime(reader["masterenddate"]),
                                IsWorking = Convert.ToBoolean(reader["masterisworking"])
                            };

                            employeesRecord.Add(empRecord);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                // Handle exception appropriately, e.g., log or throw
            }
            return employeesRecord;
        }


        internal static List<Designation> getEmployeeDesignationList(NpgsqlConnection _connection)
        {
            List<Designation> employeeDesignation = new List<Designation>();
            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.getalldesignations();", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Designation employeeDesignationObj = new Designation
                        {
                            DesignationId = Convert.ToInt32(reader[0]),
                            DesignationName = Convert.ToString(reader[1])!
                        };
                        employeeDesignation.Add(employeeDesignationObj);

                    }

                }
            }
            return employeeDesignation;
        }

        internal static List<LoginRole> getEmployeeLoginRoleList(NpgsqlConnection _connection)
        {
            List<LoginRole> employeeLoginRole = new List<LoginRole>();
            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.getallloginroles();", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LoginRole employeeLoginRoleObj = new LoginRole
                        {
                            LoginRoleId = Convert.ToInt32(reader[0]),
                            LoginRoleName = Convert.ToString(reader[1])!
                        };
                        employeeLoginRole.Add(employeeLoginRoleObj);

                    }

                }
            }
            return employeeLoginRole;
        }
    }
}