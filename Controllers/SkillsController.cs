using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Amnex_Project_Resource_Mapping_System.Controllers
{
    public class SkillsController : Controller
    {
        private readonly NpgsqlConnection _connection;

        public SkillsController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }
        public IActionResult AddSkill()
        {
            return Ok();
        }

        public IActionResult UpdateSkill()
        {
            return Ok();
        }

        public IActionResult DeleteSkill()
        {
            return Ok();
        }
    }
}
