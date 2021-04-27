using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ActivityCenter.Models;

namespace ActivityCenter.Controllers
{
    public class HomeController : Controller
    {
        private Context _context;
     
        // here we can "inject" our context service into the constructor
        public HomeController(Context context)
        {
            _context = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            Console.WriteLine("**********I am inside the Register/Index function**********");
            return View();
        }

        [HttpPost("register")]
        public IActionResult Regis(User ruser)
        {
            Console.WriteLine("**********I am inside the Regis function**********");
            if(ModelState.IsValid)
            {
                // If a User exists with provided email
                if(_context.Users.Any(u => u.Email == ruser.Email))
                {
                // Manually add a ModelState error to the Email field, with provided
                // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                _context.Add(ruser);
                ruser.Password = Hasher.HashPassword(ruser, ruser.Password);
                _context.SaveChanges();
                User RegUser = _context.Users
                    .FirstOrDefault(ruse => ruse.Email == ruser.Email);
                HttpContext.Session.SetInt32("UserId", RegUser.UserId);
                HttpContext.Session.SetString("UserName",RegUser.FirstName);
                ViewBag.SessionName = HttpContext.Session.GetString("UserName");
                ViewBag.SessionId = HttpContext.Session.GetInt32("UserId");
                // HttpContext.Session.SetString("UserName", RegUser.FirstName);
                // ViewBag.Session = HttpContext.Session.GetString("UserName");
                return Redirect("/home");
            }
            return View("Index");
        }
        [HttpPost("log")]
        public IActionResult Logis(LoginUser userlogin)
        {
            Console.WriteLine("**********I am inside the Logis function**********");
            if(ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                User  userInDb = _context.Users.FirstOrDefault(u => u.Email == userlogin.logEmail);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("logEmail", "Your Combination of Email and Password is Invalid!");
                    return View("Index");
                }
                
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userlogin, userInDb.Password, userlogin.logPassword);
                
                // result can be compared to 0 for failure
                if(result == 0 || result == null)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError("logPassword", "Your Combination of Email and Password is Invalid!");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserId",userInDb.UserId);
                HttpContext.Session.SetString("UserName",userInDb.FirstName);
                ViewBag.SessionName = HttpContext.Session.GetString("UserName");
                ViewBag.SessionId = HttpContext.Session.GetInt32("UserId");
                // HttpContext.Session.SetString("UserName", userInDb.FirstName);
                // ViewBag.Session = HttpContext.Session.GetString("UserName");
                return Redirect("/home");
            }
            return View("Index");
        }

        [HttpGet("home")]
        public IActionResult HomePage()
        {
            Console.WriteLine("**********I am inside the HomePage function**********");
            ViewBag.SessionName = HttpContext.Session.GetString("UserName");
            ViewBag.SessionId = HttpContext.Session.GetInt32("UserId");
            int? UseId = HttpContext.Session.GetInt32("UserId");
            if (UseId == null)
            {
                return Redirect("/");
            }
            else
            {
                ViewBag.Id = (int)ViewBag.SessionId;
                List<Event> Events = _context.Events
                    .Include(w => w.Coordinator)
                    .Include(w => w.Participators)
                    .ThenInclude(w => w.User)
                    .OrderBy(w => w.EventDate)
                    .Where(w => w.EventDate > DateTime.Now)
                    .ToList();
                return View(Events);
            }
        }

        [HttpGet("new")]
        public IActionResult New()
        {
            Console.WriteLine("**********I am inside the New function**********");
            ViewBag.SessionName = HttpContext.Session.GetString("UserName");
            ViewBag.SessionId = HttpContext.Session.GetInt32("UserId");
            int? UseId = HttpContext.Session.GetInt32("UserId");
            if (UseId == null)
            {
                return Redirect("/");
            }
            else
            {
                ViewBag.Id = (int)ViewBag.SessionId;
                return View();
            }
        }

        [HttpPost("addevent")]
        public IActionResult newEvent(Event newEvent)
        {
            Console.WriteLine("**********I am inside the newEvent function**********");
            if(ModelState.IsValid)
            {
                _context.Add(newEvent);
                _context.SaveChanges();
                return Redirect("/home");
            }
            else
            {
                return View("New");
            }
        }

        [HttpGet("event/{eventId}")]
        public IActionResult Event(int eventId)
        {
            Console.WriteLine("**********I am inside the Event function**********");
            ViewBag.SessionName = HttpContext.Session.GetString("UserName");
            ViewBag.SessionId = HttpContext.Session.GetInt32("UserId");
            int? UseId = HttpContext.Session.GetInt32("UserId");
            if (UseId == null)
            {
                return Redirect("/");
            }
            else
            {
                ViewBag.Id = (int)ViewBag.SessionId;
                ViewBag.Event = _context.Events
                    .Include(w => w.Coordinator)
                    .Include(w => w.Participators)
                    .ThenInclude(w => w.User)
                    .FirstOrDefault(w => w.EventId == eventId);
                return View();
            }
        }

        [HttpGet("join/{eventId}/{userId}")]
        public IActionResult Join( int eventId, int userId)
        {
            Console.WriteLine("**********I am inside the Join function**********");
            Participation Join = new Participation();
            Join.UserId = userId;
            Join.EventId = eventId;
            _context.Add(Join);
            _context.SaveChanges();
            return Redirect("/home");
        }

        [HttpGet("leave/{eventId}/{userId}")]
        public IActionResult Leave( int eventId, int userId)
        {
            Console.WriteLine("**********I am inside the Join function**********");
            Participation RetrievedEvent = _context.Participants
                .FirstOrDefault(ev => ev.UserId == userId &&  ev.EventId == eventId);
            _context.Participants.Remove(RetrievedEvent);
            _context.SaveChanges();
            return Redirect("/home");
        }
        
        [HttpGet("delete/{eventId}/{userId}")]
        public IActionResult Delete(int eventId, int userId)
        {
            Console.WriteLine("**********I am inside the Delete function**********");
            Event RetrievedEvent = _context.Events
                .FirstOrDefault(ev => ev.EventId == eventId);
            if (RetrievedEvent.UserId == userId)
            {
                _context.Events.Remove(RetrievedEvent);
                _context.SaveChanges();
                return Redirect("/home");
            }
            else
            {
                ModelState.AddModelError("Delete", "Can't Delete!");
                return View("Home");
            }
            
        }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            Console.WriteLine("**********I am inside the Logout function**********");
            HttpContext.Session.Clear();
            Console.WriteLine("**********Logged Out**********");
            return Redirect("/");
        }

        public IActionResult Privacy()
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
