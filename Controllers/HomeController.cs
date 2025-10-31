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
            try
            {
                int? lecturerID = HttpContext.Session.GetInt32("LecturerID");
                if (lecturerID == null) return RedirectToAction("Login", "Home");

                string documentFileName = null;
                if (documentFile != null && documentFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    documentFileName = Guid.NewGuid().ToString() + Path.GetExtension(documentFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, documentFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        documentFile.CopyTo(stream);
                    }
                }

                claim.storeClaim(lecturerID.Value, claim.name, claim.sessions, claim.hoursWorked, claim.hourlyRate, documentFileName);

                ViewBag.SuccessMessage = "Claim submitted successfully.";

                // Return a new empty claim object so the form is ready for next submission
                return View(new Claims_Queries());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error submitting claim: " + ex.Message);
                ViewBag.ErrorMessage = "Failed to submit claim.";
                return View(new Claims_Queries());
            }
        }
        public IActionResult TrackClaim()
        {
            try
            {
                int? lecturerID = HttpContext.Session.GetInt32("LecturerID");

                if (lecturerID == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                Claims_Queries claimsQueries = new Claims_Queries();
                List<Claims_Queries> lecturerClaims = claimsQueries.GetClaimsByLecturer(lecturerID.Value);

                if (lecturerClaims == null || lecturerClaims.Count == 0)
                {
                    ViewBag.Message = "No claims found.";
                    return View(new List<Claims_Queries>());
                }

                return View(lecturerClaims);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading lecturer claims: " + ex.Message);
                ViewBag.Message = "Error retrieving claims.";
                return View(new List<Claims_Queries>());
            }
        }
        public IActionResult ReviewPage()
        {
            try
            {
                Claims_Queries claimsQueries = new Claims_Queries();
                List<Claims_Queries> claims = claimsQueries.GetAllClaims(); // can include pending, approved, rejected

                if (claims == null || claims.Count == 0)
                {
                    ViewBag.Message = "No claims found.";
                    return View(new List<Claims_Queries>());
                }

                return View(claims);
            }
            catch (Exception error)
            {
                Console.WriteLine("Error loading Review page: " + error.Message);
                return View(new List<Claims_Queries>());
            }
        }

        public IActionResult ApprovePage()
        {
            try
            {
                Claims_Queries claimsQueries = new Claims_Queries();
                List<Claims_Queries> claims = claimsQueries.GetAllClaims(); 
                if (claims == null || claims.Count == 0)
                {
                    ViewBag.Message = "No claims found.";
                    return View(new List<Claims_Queries>());
                }
                return View(claims);
            }
            catch (Exception error)
            {
                Console.WriteLine("Error loading Approve page: " + error.Message);
                return View(new List<Claims_Queries>());
            }
        }

        [HttpPost]
        public IActionResult UpdateClaimStatus(int claimID, string claimStatus)
        {
            try
            {
                Claims_Queries claims = new Claims_Queries();
                claims.ApproveClaim(claimID, claimStatus);

                TempData["SuccessMessage"] = $"Claim {claimID} updated to {claimStatus}.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating claim status: " + ex.Message);
                TempData["ErrorMessage"] = "Failed to update claim.";
            }

            return RedirectToAction("ReviewPage","Home");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
