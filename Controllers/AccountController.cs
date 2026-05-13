using Microsoft.AspNetCore.Mvc;
using CodeAlpha_SocialMedia.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace CodeAlpha_SocialMedia.Controllers
{
    public class AccountController : Controller
    {
        private readonly CodeAlphaSocialMediaContext _context;

        // Dependency Injection ke zariye database context access karein
        public AccountController(CodeAlphaSocialMediaContext context)
        {
            _context = context;
        }

        // GET: Register Page
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register Logic
        [HttpPost]
        public async Task<IActionResult> Register(User user, IFormFile profilePic)
        {
            if (ModelState.IsValid)
            {
                // Profile Picture upload handle karein
                if (profilePic != null && profilePic.Length > 0)
                {
                    string folder = "uploads/profiles/";
                    string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profilePic.FileName);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder, fileName);

                    // Folder check karein agar nahi bana hua toh bana dein
                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder)))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder));
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePic.CopyToAsync(stream);
                    }
                    user.ProfilePicture = "/" + folder + fileName;
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // GET: Login Page
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login Logic
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Database se user check karein
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Session mein user ka data save karein taake poori app mein use ho sakay
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserPic", user.ProfilePicture ?? "/images/default-user.png");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid Email or Password!";
            return View();
        }

        // Logout Logic
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}