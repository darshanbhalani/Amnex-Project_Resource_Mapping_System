using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Amnex_Project_Resource_Mapping_System.Models;
using Amnex_Project_Resource_Mapping_System.Repo.Classes;
namespace Amnex_Project_Resource_Mapping_System.Controllers
{
    namespace Amnex_Project_Resource_Mapping_System.Controllers
    {
        public class AccountController : Controller
        {
            private readonly NpgsqlConnection _connection;
            private readonly Account account = new Account();
            public AccountController(NpgsqlConnection connection)
            {
                _connection = connection;
                connection.Open();
            }
            public IActionResult Login()
            {
                return View();
            }


            [HttpPost]
            public IActionResult Login(Login data)
            {

                using (var cmd = new NpgsqlCommand($"SELECT * FROM validateusercredentials('{data.Username}', '{data.Password}');", _connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            HttpContext.Session.SetString("userId", reader.GetInt32(0).ToString());
                            HttpContext.Session.SetString("userName", reader.GetString(1));
                            return Json(new { success = true });
                        }
                        else
                        {
                            return Json(new { success = false });
                        }
                    }

                }
            }

            [HttpPost]
            public void Logout()
            {
                HttpContext.Session.Clear();
            }

            public IActionResult Profile()
            {
                Employee employee = new Employee();
                using (var command = new NpgsqlCommand($"select * from GetEmployeeData({Convert.ToInt32(HttpContext.Session.GetString("userId"))});", _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                      while (reader.Read())
                        {
                            employee.Employeeid = reader.GetInt32(0);
                            employee.Employeename = reader.GetString(1);
                            employee.Department = reader.GetString(2);
                            employee.Skillsid = reader.GetString(3);
                            employee.Employeerating = reader.GetInt32(4);
                            employee.Employeeroleid = reader.GetString(5);
                            employee.Email = reader.GetString(6);
                        }
                    }
                }

                return View(employee);
            }

            [HttpPost]
            public IActionResult SendOTP(string data)
            {
                account.sendOTP(data, HttpContext);
                return Ok();
            }

            [HttpPost]
            public IActionResult checkOTP(string otp)
            {
                if (HttpContext.Session.GetString("OTP") == otp)
                {
                    if (DateTime.Now - DateTime.Parse(HttpContext.Session.GetString("OTPTime")) <= TimeSpan.FromMinutes(2))
                    {
                        Console.WriteLine("Ok....................");
                        return Json(new { success = true });
                    }
                    else
                    {
                        Console.WriteLine("Expire....................");
                        return Json(new { success = false, message = "OTP Expire..." });
                    }
                }
                else
                {
                    Console.WriteLine("Oops....................");
                    return Json(new { success = false, message = "Invalid OTP..." });
                }
            }

            [HttpPost]
            public IActionResult ForgotPassword(string password)
            {
                    account.forgotPassword(password,HttpContext,_connection);
                    return Json(new { success = true });
            }

            public IActionResult ChangePassword(string currentPassword, string newPassword)
            {
                bool result = account.changePassword(currentPassword,newPassword, HttpContext, _connection);
                Console.WriteLine(result);
                if(result)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }

            }
        }
    }
}