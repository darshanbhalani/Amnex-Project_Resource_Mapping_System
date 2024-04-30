using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;

using Npgsql;
using System.Net.Mail;
using System.Net;
using System.Text;
using static System.Net.WebRequestMethods;

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

        private string GenerateRandomPassword()
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

        internal void SendCredentialsToUser(string to, string password)
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("connect.nextgentechnology@gmail.com", "tcllfvvxodcydksv");
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("connect.nextgentechnology@gmail.com");
                mailMessage.To.Add(to);
                mailMessage.Subject = "PRMS Credentials";
                mailMessage.Body = $"Your credentials for PRMS are as follows:\n\nUsername: {to}\nPassword: {password}";

                smtpClient.Send(mailMessage);



            }
        }

        [HttpPost]
        public IActionResult AddEmployee(Employee empObj)
        {
            string password = GenerateRandomPassword();
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
                command.Parameters.AddWithValue("@in_createdby", 1); // Assuming a default value for created by
                command.Parameters.AddWithValue("@in_createdtime", DateTime.Now); // Current timestamp
                                                                                  // Assuming modify by and modify time are set to the same as created by and created time initially
                command.Parameters.AddWithValue("@in_modifyby", 1);
                command.Parameters.AddWithValue("@in_modifytime", DateTime.Now);

                command.ExecuteNonQuery();
            }
            SendCredentialsToUser(empObj.Email, password);
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
                                EmployeeName = reader["employeename"].ToString(),
                                EmployeeUserName = reader["employeeusername"].ToString(),
                                DepartmentId = Convert.ToInt32(reader["departmentid"]), // Note the column name correction
                                SkillsId = reader["skillsid"].ToString(), // Note the column name correction
                                LoginRoleId = Convert.ToInt32(reader["loginroleid"]), // Note the column name correction
                                IsAllocated = Convert.ToBoolean(reader["isallocated"]), // Note the column name correction
                                Email = reader["email"].ToString(), // Note the column name correction
                                                                    // Assuming other properties are present in the reader
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
                command.Parameters.AddWithValue("@ModifyBy", 1); // Assuming a default value for modify by
                command.Parameters.AddWithValue("@ModifyTime", DateTime.Now); // Current timestamp

                command.ExecuteNonQuery();
            }

            return Json(new { success = true });
        }




        public IActionResult DeleteEmployee()
        {
            return Ok();
        }
    }
}