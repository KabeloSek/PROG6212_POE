using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;

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

            Claims_Queries claims = new Claims_Queries();
            claims.CreateClaimsTable();

            return View();
        }

        [HttpPost]
        public IActionResult Register(Register user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Create an instance of the Register model
                    Register register = new Register();

                    //check the role entered by the user and call the appropriate method to insert data
                    switch (user.role.ToLower())
                    {
                        case "lecturer":
                            register.storeLecturer(user.name, user.surname, user.email, user.password, user.role);
                            ViewBag.SuccessMessage = "Registration successful!";
                            break;

                        case "programmanager":
                            register.storePM(user.name, user.surname, user.email, user.password, user.role);
                            ViewBag.SuccessMessage = "Registration successful!";
                            break;

                        case "programcoordinator":
                            register.storePC(user.name, user.surname, user.email, user.password, user.role);
                            ViewBag.SuccessMessage = "Registration successful!";
                            break;

                        default:
                            Console.WriteLine("role", "Invalid role. ");
                            break;
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine("Error during registration." + error.Message);
                    ViewBag.ErrorMessage = "Registration unsuccessful!";
                    ViewBag.SuccessMessage = null;
                }
                
                
            }
            return View(user);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(Login user)
        {
            if (ModelState.IsValid)
            {
                Login login = new Login();
                int userId = login.GetUserID(user.email, user.password, user.role); // get ID instead of bool

                if (userId > 0)
                {
                    // store session variables
                    HttpContext.Session.SetInt32("UserID", userId);
                    HttpContext.Session.SetString("UserRole", user.role);

                    switch (user.role.ToLower())
                    {
                        case "lecturer":
                            HttpContext.Session.SetInt32("LecturerID", userId);
                            return RedirectToAction("HomePage", "Home");

                        case "programcoordinator":
                            HttpContext.Session.SetInt32("CoordinatorID", userId);
                            return RedirectToAction("Review", "Home");

                        case "programmanager":
                            HttpContext.Session.SetInt32("ManagerID", userId);
                            return RedirectToAction("Approve", "Home");

                        default:
                            Console.WriteLine("Invalid role specified.");
                            break;
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid login credentials.";
                }
            }

            return View(user);
        }
        public IActionResult HomePage()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ClaimPage()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ClaimPage(Claims_Queries claim, IFormFile documentFile)
        {
            int? lecturerID = HttpContext.Session.GetInt32("LecturerID");
            if (lecturerID == null)
                return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                try
                {
                    string uniqueFileName = "No document uploaded";
                    if (documentFile != null && documentFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(documentFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                            documentFile.CopyTo(fileStream);
                    }

                    claim.storeClaim(lecturerID.Value, claim.name, claim.sessions, claim.hoursWorked, claim.hourlyRate, uniqueFileName);
                    ViewBag.Message = "Claim submitted successfully.";
                }
                catch (Exception error)
                {
                    Console.WriteLine("Unable to submit claim: " + error.Message);
                }
            }
            else
            {
                ViewBag.Message = "Please fill in all fields correctly.";
            }

            return View();
        }
        public IActionResult ReviewPage()
        {
            int? lecturerID = HttpContext.Session.GetInt32("LecturerID");
            if (lecturerID == null)
            {
                try
                {
                    return RedirectToAction("Login", "Home");
                }
                catch (Exception error) { 
                Console.WriteLine("LecturerID not found in session." + error.Message);
                }
                    
            }

            Claims_Queries claim = new Claims_Queries();
            var claimsList = claim.GetAllClaims(lecturerID.Value);
            
            if(claimsList.Count == 0)
            {
                ViewBag.Message = "No claims found.";
            }
            return View(claimsList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
