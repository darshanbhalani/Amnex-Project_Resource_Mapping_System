using Amnex_Project_Resource_Mapping_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Amnex_Project_Resource_Mapping_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Projects()
        {
            return View();
        }
        public IActionResult Employees()
        {
            return View();
        }
        public IActionResult ProjectEmployeeMapping()
        {
            return View();
        }
        public IActionResult Department()
        {
            return View();
        }
        public IActionResult Skills()
        {
            return View();
        }
        public IActionResult Actions()
        {
            return View();
        }

    }
}
