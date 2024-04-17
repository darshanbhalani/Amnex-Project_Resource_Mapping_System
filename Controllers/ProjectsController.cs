using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Amnex_Project_Resource_Mapping_System.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly NpgsqlConnection _connection;

        public ProjectsController(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
        }
        public IActionResult AddProject()
        {
            return Ok();
        }

        public IActionResult UpdateProject()
        {
            return Ok();
        }

        public IActionResult DeleteProject()
        {
            return Ok();
        }
    }
}
