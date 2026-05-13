using Microsoft.AspNetCore.Mvc;
using CodeAlpha_SocialMedia.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CodeAlpha_SocialMedia.Controllers
{
    public class PostsController : Controller
    {
        private readonly CodeAlphaSocialMediaContext _context;

        public PostsController(CodeAlphaSocialMediaContext context)
        {
            _context = context;
        }

        // 1. Post Details (Play Page) - Updated with Pause History Logic
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(m => m.PostId == id);

            if (post == null) return NotFound();

            // --- HISTORY LOGIC START (Updated) ---
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                // Pehle check karein ke user ne history pause toh nahi ki
                var user = await _context.Users.FindAsync(userId);

                if (user != null && !(user.IsHistoryPaused ?? false))
                {
                    var existingHistory = await _context.Histories
                        .FirstOrDefaultAsync(h => h.UserId == userId && h.PostId == id);

                    if (existingHistory == null)
                    {
                        var newHistory = new History
                        {
                            UserId = userId.Value,
                            PostId = id.Value,
                            ViewedAt = DateTime.Now
                        };
                        _context.Histories.Add(newHistory);
                    }
                    else
                    {
                        existingHistory.ViewedAt = DateTime.Now;
                        _context.Entry(existingHistory).State = EntityState.Modified;
                    }

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        // Log error if needed
                    }
                }
            }
            // --- HISTORY LOGIC END ---

            ViewBag.SuggestedPosts = await _context.Posts
                .Where(p => p.PostId != id)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View(post);
        }

        // 2. Toggle Like (Add or Remove Like) - As is
        [HttpPost]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId.Value);

            if (existingLike != null)
            {
                _context.Likes.Remove(existingLike);
            }
            else
            {
                var like = new Like
                {
                    PostId = postId,
                    UserId = userId.Value,
                    CreatedAt = DateTime.Now
                };
                _context.Likes.Add(like);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = postId });
        }

        // 3. Add Comment - As is
        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, string commentText)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            if (!string.IsNullOrEmpty(commentText))
            {
                var comment = new Comment
                {
                    PostId = postId,
                    UserId = userId.Value,
                    CommentText = commentText,
                    CreatedAt = DateTime.Now
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = postId });
        }

        // 4. Create Post - GET - As is
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // 5. Create Post - POST - Updated validation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, IFormFile postImage)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            if (postImage != null && postImage.Length > 0)
            {
                string folder = "uploads/posts/";
                string serverFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);

                if (!Directory.Exists(serverFolder))
                {
                    Directory.CreateDirectory(serverFolder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(postImage.FileName);
                string filePath = Path.Combine(serverFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await postImage.CopyToAsync(stream);
                }

                post.ImagePath = "/" + folder + fileName;
            }

            post.UserId = userId.Value;
            post.CreatedAt = DateTime.Now;

            ModelState.Remove("User");
            ModelState.Remove("Comments");
            ModelState.Remove("Likes");
            ModelState.Remove("Histories");

            if (ModelState.IsValid)
            {
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Post Published Successfully!" });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(new { success = false, errors = errors });
        }

        // 6. My Posts - As is
        public async Task<IActionResult> MyPosts()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var userPosts = await _context.Posts
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(userPosts);
        }

        // 7. Delete Post - As is
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                if (!string.IsNullOrEmpty(post.ImagePath))
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", post.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MyPosts));
        }
    }
}