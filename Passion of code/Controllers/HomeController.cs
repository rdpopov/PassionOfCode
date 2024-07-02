using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Passion_of_code.Controllers.Passion_of_code.Controllers;
using Passion_of_code.Models;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Security.Cryptography;

namespace Passion_of_code.Controllers
{
    static class  MsgStyle {
        public const string ErrorStyle = "color:orangered";  
        public const string ValidStyle = "color:darkgreen";  
    }
    public class Message {
        public string color { get; set; } = "";
        public string msg { get; set; } = "";
    }


    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpPost] 
        public IActionResult ResetPass(string uname, string email, string pass)
        {
            if (uname == null || pass == null|| email== null) { 
                    return Login(new Message{color = MsgStyle.ErrorStyle ,msg = "Wrong credentials"});
            }
            string pass_hash = Pass.CreateMD5(pass);
            using (var db_context = new PassionOfCodeContext())
            {
                var users = db_context.Users
                               .Where(u => u.Username == uname && u.Eail == email)
                               .ToList();
                if (users.Count > 0)
                {
                    var usr= users.First();
                    usr.Passwd_hash = pass_hash;
                    db_context.Update(usr);
                    db_context.SaveChanges();
                    return Login(new Message{color = MsgStyle.ValidStyle ,msg = "Password reset. Now log in"});
                }
                else { 
                    return Login(new Message{color = MsgStyle.ErrorStyle ,msg = "Wrong credentials"});
                }
                
            }
            return View(new Message { });
        }
        public IActionResult ResetPass() { 
            return View(new Message { });
        }

        [HttpPost]
        public IActionResult Login(string uname, string pass)
        {
            if (uname == null || pass == null) { 
                    return Login(new Message{color = MsgStyle.ErrorStyle ,msg = "Wrong credentials"});
            }
            string pass_hash = Pass.CreateMD5(pass);
            using (var db_context = new PassionOfCodeContext())
            {

                var users = db_context.Users
                               .Where(u => u.Username == uname && u.Passwd_hash == pass_hash)
                               .ToList();
                if (users.Count>0)
                {
                    HttpContext.Session.SetString("uname", uname);

                    return RedirectToAction("Archive","Event");
                }
                else { 
                    return Login(new Message{color = MsgStyle.ErrorStyle ,msg = "Wrong credentials"});
                }
                
            }


            return View(new Message { });
        }
        // Message has a default init
        public IActionResult Login(Message ms)
        {
            return View(ms);
        }
        // user dashboard
        public IActionResult UserProfile()
        {
            // todo implemnet this
            return View();
        }

        // logout control
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Login(new Message { color = MsgStyle.ValidStyle, msg = "User logged oout, Now you can log in again" });
        }

        [HttpPost]
        public IActionResult Register(string uname, string mail, string pass, string rpass)
        {
            if (rpass != pass) { 
                return View(new Message { color = MsgStyle.ErrorStyle , msg = "Passwords don't match" });
            }
            using (var db_context = new PassionOfCodeContext())
            {
                var unames = db_context.Users
                               .Where(u => u.Username == uname)
                               .ToList();
                               
                if (unames.Count > 0)
                {
                    return View(new Message { color = MsgStyle.ErrorStyle, msg = "Username Taken" });
                }

                var mails = db_context.Users
                               .Where(u => u.Eail == mail)
                               .ToList();

                if (mails.Count > 0)
                {
                    return View(new Message { color = MsgStyle.ErrorStyle, msg = "Email already used" });
                }

                var NewRegisteredUser = new User
                {
                    Username = uname,
                    Passwd_hash = Pass.CreateMD5(pass),
                    Eail = mail,
                };
                try
                {
                    db_context.Add(NewRegisteredUser);
                    db_context.SaveChanges();
                    return Login(new Message { color = MsgStyle.ValidStyle, msg = "User Registered, Now log in" });
                }
                catch (Exception e)
                {
                    return View(new Message { color = MsgStyle.ErrorStyle, msg = "Couldn't register user" });
                }
            }
            return View(new Message { });
        }
        public IActionResult Register()
        {
            return View(new Message { });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
