using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PROG6212_POE.Models;
using System.Diagnostics;
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
            //create an instance of the Claims_Queries model 
            Claims_Queries claims = new Claims_Queries();

            //call the method to create claims table it is needed to create user tables because they have claimID as foreign key
            claims.CreateClaimsTable();

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
                
                bool isValidUser = login.getUser(user.email, user.password, user.role);
                if (isValidUser) 
                {
                    switch (user.role.ToLower())
                    {
                        case "lecturer":
                            return RedirectToAction("HomePage", "Home");
                      

                        case "programcoordinator":
                            return RedirectToAction("Review", "Home");
                        
                            
                        case "programmanager":
                            return RedirectToAction("Approve", "Home");
         
                        default:
                            Console.WriteLine("Invalid role specified.");
                            return RedirectToAction("Login", "Home");

                    }
                }
                else
                {
                    Console.WriteLine("User not found or invalid credentials.");
                }

            }
            ViewBag.ErrorMessage = "Login unsuccessful! Incorrect Email, Password or Role.";
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
            if (ModelState.IsValid)
            {
                try
                {
                    if (documentFile != null && documentFile.Length > 0)
                    {
                        claim.document = documentFile.FileName;
                    }
                    else
                    {
                        claim.document = "No document uploaded";
                    }
                    claim.storeClaim( claim.name, claim.sessions, claim.hoursWorked, claim.hourlyRate, claim.document);
                    Console.WriteLine("Claim submitted successfully.");
                    ViewBag.Message = "Claim submitted successfully.";

                }catch(Exception error)
                {
                    Console.WriteLine("Unable to submit claim" + error.Message);
                }
            }
            return View(claim);
        }

        public IActionResult ReviewPage()
        {
            Claims_Queries claim = new Claims_Queries();
            List<Claims_Queries> claims = claim.GetAllClaims();
            return View(claims);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
