using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using CodeAlpha_SocialMedia.Models; // Namespace sahi kar diya

namespace CodeAlpha_SocialMedia.Controllers
{
    public class InteractionController : Controller
    {
        // Context ka naam aapke Models folder ke mutabiq
        private readonly CodeAlphaSocialMediaContext _context;

        public InteractionController(CodeAlphaSocialMediaContext context)
        {
            _context = context;
        }

        // Liked Content Action
        public async Task<IActionResult> LikedContent()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Data fetch karna
            var likedPosts = await _context.Posts
                .Include(p => p.User)
                .Where(p => p.Likes.Any(l => l.UserId == userId))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(likedPosts);
        }
    }
}