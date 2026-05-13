using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeAlpha_SocialMedia.Models;
using Microsoft.AspNetCore.Http;

namespace CodeAlpha_SocialMedia.Controllers
{
    public class HistoryController : Controller
    {
        private readonly CodeAlphaSocialMediaContext _context;

        public HistoryController(CodeAlphaSocialMediaContext context)
        {
            _context = context;
        }

        // 1. History Page Display Karna (Updated with Pause Status)
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            // --- NAYA LOGIC: User ka Pause status View mein bhejne ke liye ---
            var user = await _context.Users.FindAsync(userId);
            ViewBag.IsPaused = user?.IsHistoryPaused ?? false;

            var history = await _context.Histories
                .Include(h => h.Post)
                .ThenInclude(p => p.User)
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.ViewedAt)
                .ToListAsync();

            return View(history);
        }

        // 2. Poori History Delete Karna (Clear All) - No Change
        [HttpPost]
        public async Task<IActionResult> ClearHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var userHistory = _context.Histories.Where(h => h.UserId == userId);
            _context.Histories.RemoveRange(userHistory);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // 3. Single Item Delete Karna (X button ke liye) - No Change
        [HttpPost]
        public async Task<IActionResult> RemoveFromHistory(int historyId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var item = await _context.Histories
                .FirstOrDefaultAsync(h => h.HistoryId == historyId && h.UserId == userId);

            if (item != null)
            {
                _context.Histories.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // --- NAYA ACTION: 4. Pause ya Resume karne ke liye ---
        [HttpPost]
        public async Task<IActionResult> TogglePauseHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                // Toggle logic: Agar true hai toh false kar do, aur vice versa
                user.IsHistoryPaused = !(user.IsHistoryPaused ?? false);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // --- NAYA ACTION: 5. Manage History (Abhi Index par hi rakhte hain) ---
        public IActionResult ManageHistory()
        {
            // Future mein aap yahan filters ya naya page add kar sakti hain
            return RedirectToAction("Index");
        }
    }
}