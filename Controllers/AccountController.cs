using Amnex_Project_Resource_Mapping_System.Repo.Classes;
using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;
using System.Net.Mail;
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
                HttpContext.Session.Clear();
                return View();
            }


            [HttpPost]
            public IActionResult Login(Login data)
            {
                using (var cmd = new NpgsqlCommand($"SELECT * FROM validateusercredentials('{data.UserName}', '{data.Password}');", _connection))
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
                UserProfileModel userProfileModel = new UserProfileModel();
                List<Skill> skills = new List<Skill>();
                List<Department> departments = new List<Department>();
                Employee employee = new Employee();
                using (var command = new NpgsqlCommand($"select * from getemployeeprofile(1);", _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employee.EmployeeId = reader.GetInt32(0);
                            employee.EmployeeName = reader.GetString(1);
                            employee.EmployeeUserName = reader.GetString(2);
                            employee.DepartmentId = reader.GetInt32(3);
                            employee.DepartmentName = reader.GetString(4);
                            employee.SkillsId = reader.GetString(5);
                            employee.SkillsName = reader.GetString(6);
                            employee.Email = reader.GetString(7);
                            employee.LoginRoleId = reader.GetInt32(8);
                            employee.EmployeeRating = reader.GetInt32(10);
                        }
                    }
                }

                using (var command = new NpgsqlCommand($"select skillid,skillname from skills where isdeleted=false", _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            skills.Add(new Skill
                            {
                                Skillid = reader.GetInt32(0),
                                Skillname = reader.GetString(1),
                            });
                        }
                    }
                }
                using (var command = new NpgsqlCommand($"select departmentid,departmentname from departments where isdeleted=false", _connection))
                {
                    using (var reader = command.ExecuteReader())
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

                userProfileModel.ProfileData = employee;
                userProfileModel.Skills = skills;
                userProfileModel.Departments = departments;
                return View(userProfileModel);

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
                    if (DateTime.Now - DateTime.Parse(HttpContext.Session.GetString("OTPTime")!) <= TimeSpan.FromMinutes(2))
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "OTP Expire..." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Invalid OTP..." });
                }
            }


            [HttpPost]
            public IActionResult ForgotPassword(Employee employee)
            {
                using (var cmd = new NpgsqlCommand($"select * from GetPassword('{employee.EmployeeUserName}','{employee.Email}');", _connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                            {
                                smtpClient.Port = 587;
                                smtpClient.Credentials = new NetworkCredential("connect.nextgentechnology@gmail.com", "tcllfvvxodcydksv");
                                smtpClient.EnableSsl = true;

                                MailMessage mailMessage = new MailMessage();
                                mailMessage.From = new MailAddress("connect.nextgentechnology@gmail.com");
                                mailMessage.To.Add(employee.Email);
                                mailMessage.Subject = "PRMS Account Credentials";
                                mailMessage.IsBodyHtml = true;
                                mailMessage.Body = $"<h3>Hello {reader.GetString(1)},</h3></br><p>Your PRMS UserName is \"<b>{employee.EmployeeUserName}</b>\" and Password is \"<b>{reader.GetString(0)}</b>\".";

                                smtpClient.Send(mailMessage);

                            }
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
            public IActionResult ChangePassword(string currentPassword, string newPassword)
            {
                bool result = account.changePassword(currentPassword, newPassword, HttpContext, _connection);
                Console.WriteLine(result);
                if (result)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }

            }


            internal static bool SendCredentials(Employee employee)
            {
                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("connect.nextgentechnology@gmail.com", "tcllfvvxodcydksv");
                    smtpClient.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("connect.nextgentechnology@gmail.com");
                    mailMessage.To.Add(employee.Email);
                    mailMessage.Subject = "PRMS Account Credentials";
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = $"<h3>Hello {employee.EmployeeName},</h3></br><p>Your PRMS account is created. your UserName is \"<b>{employee.EmployeeUserName}</b>\" and Password is \"<b>{employee.Password}</b>\".";

                    smtpClient.Send(mailMessage);

                }
                return true;
            }

        }
    }
}