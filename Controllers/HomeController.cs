using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;

namespace PROG6212_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Create an instance of the Register model
            Register register = new Register();

            // Call the method to create user tables
            register.CreateUserTables();

            return View();
        }

        [HttpPost]
        public IActionResult Register(Register user)
        {
            if (ModelState.IsValid)
            {
                // Create an instance of the Register model
                Register register = new Register();

                //check the role entered by the user and call the appropriate method to insert data
                switch (user.role.ToLower())
                {
                    case "lecturer":
                        register.storeLecturer(user.name, user.surname, user.email, user.password, user.role);
                    break;

                    case "programmanager":
                        register.storePM(user.name, user.surname, user.email, user.password, user.role);
                    break;

                    case "programcoordinator":
                        register.storePC(user.name, user.surname, user.email, user.password, user.role);
                    break;

                    default:
                        ModelState.AddModelError("role", "Invalid role. ");
                    break;
                }
                ViewBag.SuccessMessage = "Registration successful!";
                ViewBag.ErrorMessage = "Registration unsuccessful!";
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
