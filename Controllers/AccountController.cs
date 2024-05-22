using Amnex_Project_Resource_Mapping_System.Repo.Classes;
using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;
using System.Net.Mail;
using Newtonsoft.Json;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
namespace Amnex_Project_Resource_Mapping_System.Controllers
{
    namespace Amnex_Project_Resource_Mapping_System.Controllers
    {
        public class AccountController : Controller
        {
            private readonly NpgsqlConnection _connection;
            private readonly Account account = new Account();
            private IConfiguration _configuration;

            public AccountController(NpgsqlConnection connection, IConfiguration configuration)
            {
                _configuration = configuration;
                _connection = connection;
                connection.Open();
            }


            public IActionResult Login()
            {
                HttpContext.Session.Clear();
                return View();
            }
            [HttpPost]
            public async Task<IActionResult> Login(Login data, string recaptchaResponse)
            {
                bool isRecaptchaValid = await ValidateRecaptcha(recaptchaResponse);
                //if (!isRecaptchaValid)
                    if (!true)
                    {
                    ModelState.AddModelError(string.Empty, "reCAPTCHA validation failed.");
                    return Json(new { success = false, message = "reCAPTCHA validation failed." });
                }

                using (var cmd = new NpgsqlCommand($"SELECT * FROM LOGIN('{data.UserName}', '{data.Password}');", _connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetInt32(2) == 1)
                            {
                                HttpContext.Session.SetString("userId", reader.GetInt32(0).ToString());
                                HttpContext.Session.SetString("userName", reader.GetString(1));
                                return Json(new { success = true });
                            }
                            else
                            {
                                return Json(new { success = false,message = "Only admins are allowed to login this portal." });
                            }

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid username or password.");
                            return Json(new { success = false, message = "Invalid username or password." });
                        }
                    }
                }
            }


            private async Task<bool> ValidateRecaptcha(string recaptchaResponse)
            {
                try
                {
                    var secretKey = _configuration["GoogleRecaptcha:SecretKey"];
                    var client = new HttpClient();
                    var response = await client.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptchaResponse}");
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(responseBody);
                    return captchaResponse!.Success;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public class CaptchaResponse
            {
                [JsonProperty("success")]
                public bool Success { get; set; }
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
            public async Task<IActionResult> ForgotPassword(string EmployeeUserName, string Email, string recaptchaResponse)
            {
                bool flag = false;
                bool isRecaptchaValid = await ValidateRecaptcha(recaptchaResponse);
                if (!isRecaptchaValid)
                {
                    return Json(new { success = false, message = "reCAPTCHA validation failed." });
                }
                using (var cmd = new NpgsqlCommand($"select * from validate('{EmployeeUserName}','{Email}');",_connection))
                {
                    using(var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            flag = reader.GetBoolean(0);
                        }
                        else
                        {
                            return Json(new { success = false, message = "Invalid User Name or Email." });
                        }
                        
                    }
                }

                if (flag)
                {
                    string resetToken = Guid.NewGuid().ToString();
                    SaveResetToken(Email, resetToken);
                    string resetPasswordLink = Url.Action("ResetPassword", "Home", new { email = Email!, token = resetToken }, protocol: HttpContext.Request.Scheme)!;

                    using (SmtpClient smtpClient = new SmtpClient(_configuration["SMTPConfiguration:Server"]))
                    {
                        smtpClient.Port = Convert.ToInt32(_configuration["SMTPConfiguration:Port"]);
                        smtpClient.Credentials = new NetworkCredential(_configuration["SMTPConfiguration:HostName"], _configuration["SMTPConfiguration:Password"]);
                        smtpClient.EnableSsl = true;
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress(_configuration["SMTPConfiguration:HostName"]!);
                        mailMessage.To.Add(Email);
                        mailMessage.Subject = "PRMS Account Credentials";
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = $"<a href='{resetPasswordLink}'>{resetPasswordLink}</a>";

                        smtpClient.Send(mailMessage);

                        return Json(new { success = true, message = "Reset password link sended." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Invalid User Name or Email." });
                }
                //using (var cmd = new NpgsqlCommand($"select * from GetPassword('{EmployeeUserName}','{Email}');", _connection))
                //{
                //    using (var reader = cmd.ExecuteReader())
                //    {
                //        if (reader.Read())
                //        {
                //            using (SmtpClient smtpClient = new SmtpClient(_configuration["SMTPConfiguration:Server"]))
                //            {
                //                smtpClient.Port = Convert.ToInt32(_configuration["SMTPConfiguration:Port"]);
                //                smtpClient.Credentials = new NetworkCredential(_configuration["SMTPConfiguration:HostName"], _configuration["SMTPConfiguration:Password"]);
                //                smtpClient.EnableSsl = true;
                //                MailMessage mailMessage = new MailMessage();
                //                mailMessage.From = new MailAddress(_configuration["SMTPConfiguration:HostName"]!);
                //                mailMessage.To.Add(Email);
                //                mailMessage.Subject = "PRMS Account Credentials";
                //                mailMessage.IsBodyHtml = true;
                //                mailMessage.Body = $"<h3>Hello {reader.GetString(1)},</h3></br><p>Your PRMS UserName is \"<b>{EmployeeUserName}</b>\" and Password is \"<b>{reader.GetString(0)}</b>\".";

                //                smtpClient.Send(mailMessage);

                //            }
                //            return Json(new { success = true });
                //        }
                //        else
                //        {
                //            return Json(new { success = false, message = "Invalid User Name or Email." });
                //        }


                //    }
                //}
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


            internal static bool SendCredentials(Employee employee, IConfiguration configuration)
            {
                using (SmtpClient smtpClient = new SmtpClient(configuration["SMTPConfiguration:Server"]))
                {
                    smtpClient.Port = Convert.ToInt32(configuration["SMTPConfiguration:Port"]);
                    smtpClient.Credentials = new NetworkCredential(configuration["SMTPConfiguration:HostName"], configuration["SMTPConfiguration:Password"]);
                    smtpClient.EnableSsl = true;
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(configuration["SMTPConfiguration:HostName"]!);
                    mailMessage.To.Add(employee.Email);
                    mailMessage.Subject = "PRMS Account Credentials";
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = $"<h3>Hello {employee.EmployeeName},</h3></br><p>Your PRMS account is created. your UserName is \"<b>{employee.EmployeeUserName}</b>\" and Password is \"<b>{employee.Password}</b>\".";
                    smtpClient.Send(mailMessage);

                }
                return true;
            }


            private void SaveResetToken(string email, string token)
            {
                try
                {

                    using (var cmd = new NpgsqlCommand("select saveresettoken(@Email, @Token);", _connection))
                    {
                        cmd.Parameters.AddWithValue("Email", email);
                        cmd.Parameters.AddWithValue("Token", token);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException e)
                {
                    Console.WriteLine(e.Message);
                }
                {

                }
                
            }



        }
    }
}