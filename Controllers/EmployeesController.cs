using Microsoft.AspNetCore.Mvc;
using Npgsql;

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
        public IActionResult AddEmployee()
        {
            return Ok();
        }

        public IActionResult UpdateEmployee()
        {
            return Ok();
        }

        public IActionResult DeleteEmployee()
        {
            return Ok();
        }
    }
}
