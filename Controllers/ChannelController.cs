using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeAlpha_SocialMedia.Models;

namespace CodeAlpha_SocialMedia.Controllers
{
    public class ChannelController : Controller
    {
        private readonly CodeAlphaSocialMediaContext _context;

        public ChannelController(CodeAlphaSocialMediaContext context)
        {
            _context = context;
        }

        // 1. Channel Profile - Dynamic Loading
        public async Task<IActionResult> Index(int id)
        {
            // Login user ki ID session se lein
            int currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var channel = await _context.Users
                .Include(u => u.Posts) // Real posts include ho rhi hain
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (channel == null) return NotFound();

            // Channel Views Update (Logic check: User apni profile khud view kare to count na barhe)
            if (currentUserId != id)
            {
                channel.ChannelViews = (channel.ChannelViews ?? 0) + 1;
                await _context.SaveChangesAsync();
            }

            // Subscriber count nikalna
            ViewBag.SubscriberCount = await _context.Subscriptions
                .CountAsync(s => s.ChannelId == id);

            // Check krain ke kya current user ne subscribe kiya hua hai (for button state)
            ViewBag.IsSubscribed = await _context.Subscriptions
                .AnyAsync(s => s.SubscriberId == currentUserId && s.ChannelId == id);

            return View(channel);
        }

        // 2. Subscribe/Unsubscribe Logic (Fully Dynamic AJAX)
        [HttpPost]
        public async Task<IActionResult> Subscribe(int channelId)
        {
            int currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (currentUserId == 0) return Json(new { success = false, message = "Please Login" });
            if (currentUserId == channelId) return Json(new { success = false, message = "Cannot subscribe to yourself" });

            bool isSubscribedNow = false;

            var existingSub = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.SubscriberId == currentUserId && s.ChannelId == channelId);

            if (existingSub == null)
            {
                var sub = new Subscription
                {
                    SubscriberId = currentUserId,
                    ChannelId = channelId,
                    SubscribedAt = DateTime.Now
                };
                _context.Subscriptions.Add(sub);
                isSubscribedNow = true;
            }
            else
            {
                _context.Subscriptions.Remove(existingSub);
                isSubscribedNow = false;
            }

            await _context.SaveChangesAsync();

            // Naya count nikal kar wapas bhejna taake UI update ho sake
            var newCount = await _context.Subscriptions.CountAsync(s => s.ChannelId == channelId);

            return Json(new
            {
                success = true,
                isSubscribed = isSubscribedNow,
                newCount = newCount
            });
        }

        // 3. Explore More / Shorts (Dynamic Categories)
        public async Task<IActionResult> Explore(string category = "Shorts")
        {
            var items = await _context.Posts
                .Where(p => p.Category == category)
                .Include(p => p.User) // Post karne wale ki details ke liye
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            ViewBag.CurrentCategory = category;
            return View(items);
        }
    }
}