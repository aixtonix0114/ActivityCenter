using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ActivityCenter.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ActivityCenter.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;     
        public HomeController(MyContext context)
        {
            dbContext = context;
        }


        [HttpGet("signin")]
        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Signin");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("UserInSession", newUser.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Signin");
        }

        [HttpPost("login")]
        public IActionResult Login(LogUser LoginUser)
        {
            if(ModelState.IsValid)
            {
                User userInDB = dbContext.Users.FirstOrDefault(u => u.Email == LoginUser.LEmail);
                if(userInDB == null)
                {
                    ModelState.AddModelError("LEmail", "Invalid Email/Password");
                    return View("Signin");
                }
                var hasher = new PasswordHasher<LogUser>();
                var result = hasher.VerifyHashedPassword(LoginUser, userInDB.Password, LoginUser.LPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LPassword", "Invalid Email/Password");
                    return View("Signin");
                }
                HttpContext.Session.SetInt32("UserInSession", userInDB.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Signin");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Signin");
            }
            ViewBag.AllActivities = dbContext.Activities.OrderBy(c => c.Date).ThenBy(d => d.Time)
                .Include(w => w.ActivitiParticipant)
                .ThenInclude(u => u.User)
                .ToList();

            ViewBag.OneUser = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserInSession"));

            ViewBag.Creator = dbContext.Users.ToList();
            return View();
        }

        [HttpGet("/new")]
        public IActionResult CreateForm()
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Signin");
            }
            return View();
        }

        [HttpPost("createactivity")]
        public IActionResult CreateActivity(Activiti newActiviti)
        {
            if(ModelState.IsValid)
            {
                if(newActiviti.Date < DateTime.Today)
                {
                    ModelState.AddModelError("Date", "Activity Date MUST be a FUTURE date");
                    return View("CreateForm");
                }
                newActiviti.UserId = (int)HttpContext.Session.GetInt32("UserInSession");
                dbContext.Add(newActiviti);
                dbContext.SaveChanges();
                return Redirect("/activityinfo/" + newActiviti.ActivitiId);
            }
            return View("CreateForm");
        }

        [HttpGet("activityinfo/{ActivitiId}")]
        public IActionResult ActivityInfo(int ActivitiId)
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Signin");
            }
            
            Activiti AllActivities = dbContext.Activities.Include(p => p.ActivitiParticipant).ThenInclude(u => u.User).FirstOrDefault(p => p.ActivitiId == ActivitiId);

            ViewBag.Creator = dbContext.Users.ToList();

            ViewBag.AllActivities = dbContext.Activities
                .Include(w => w.ActivitiParticipant)
                .ThenInclude(u => u.User)
                .ToList();
            ViewBag.OneUser = dbContext.Users
                .FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserInSession"));

            return View(AllActivities);
        }

        [HttpPost("deleteactivity/{ActivitiId}")]
        public IActionResult DeleteActivity(int ActivitiId)
        {
            Activiti ActivityToDelete = dbContext.Activities.FirstOrDefault(p => p.ActivitiId == ActivitiId);
            dbContext.Remove(ActivityToDelete);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [HttpPost("dontgotoactivity/{ActivitiId}")]
        public IActionResult DontGoToActivity(int ActivitiId)
        {
            int session = (int)HttpContext.Session.GetInt32("UserInSession");

            Participant ParticipantToDelete = dbContext.Participants.Where(m => m.ActivitiId == ActivitiId).FirstOrDefault(a => a.UserId == session);
            dbContext.Remove(ParticipantToDelete);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [HttpPost("gotoactivity/{ActivitiId}")]
        public IActionResult GoToActivity(int ActivitiId, Participant newParticipant)
        {
            newParticipant.ActivitiId = ActivitiId;
            newParticipant.UserId = (int)HttpContext.Session.GetInt32("UserInSession");
            dbContext.Add(newParticipant);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return View("Signin");
        }
    }
}
