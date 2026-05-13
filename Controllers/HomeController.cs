using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeAlpha_SocialMedia.Models;
using System.Diagnostics;

namespace CodeAlpha_SocialMedia.Controllers
{
    public class HomeController : Controller
    {
        private readonly CodeAlphaSocialMediaContext _context;

        public HomeController(CodeAlphaSocialMediaContext context)
        {
            _context = context;
        }

        // UPDATE: Ab ye sirf database se posts fetch karega
        public async Task<IActionResult> Index()
        {
            try
            {
                // Database se saari posts fetch karein aur newest ko pehle dikhayein
                var posts = await _context.Posts
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                // Agar posts null hain toh khali list bhej dein taake error na aaye
                return View(posts ?? new List<Post>());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error fetching posts: " + ex.Message);
                return View(new List<Post>());
            }
        }

        // REMOVED: GetMoreVideos aur YouTube API method hata diye gaye hain 
        // kyunke ab humein infinite scroll aur API errors nahi chahiye.

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}