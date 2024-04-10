using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Amnex_Project_Resource_Mapping_System.Models;
namespace Amnex_Project_Resource_Mapping_System.Controllers
{
    namespace Amnex_Project_Resource_Mapping_System.Controllers
    {
        public class AccountController : Controller
        {
            private readonly NpgsqlConnection _connection;
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
                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
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
                return View();
            }
        }
    }
}