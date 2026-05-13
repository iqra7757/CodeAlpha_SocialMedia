using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeAlpha_SocialMedia.Models;
using Newtonsoft.Json;
using System.Net;

namespace CodeAlpha_SocialMedia.Controllers
{
    public class SearchController : Controller
    {
        private readonly CodeAlphaSocialMediaContext _context;
        // Key: AIzaSyBnl... (Bnl mein small 'l' hai)
        private readonly string _youtubeApiKey = "AIzaSyBnl7d6YpL5lxLvGOZ0l9G88lDBqg4g_zY";

        public SearchController(CodeAlphaSocialMediaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string query)
        {
            if (string.IsNullOrEmpty(query))
                return RedirectToAction("Index", "Home");

            ViewBag.SearchQuery = query;

            // 1. Local Database Search (People Section)
            var users = await _context.Users
                .Where(u => u.FullName.Contains(query) || u.Email.Contains(query))
                .Take(10)
                .ToListAsync();

            // 2. YouTube Search
            var youtubeVideos = await GetYouTubeVideosFromAPI(query);

            // DATA ASSIGNMENT: Ye names View ke loops se match hone chahiye
            ViewBag.Users = users;
            ViewBag.YouTubeVideos = youtubeVideos;

            return View();
        }

        private async Task<List<YouTubeVideoModel>> GetYouTubeVideosFromAPI(string query)
        {
            var videos = new List<YouTubeVideoModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    string encodedQuery = WebUtility.UrlEncode(query);
                    string url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=12&q={encodedQuery}&type=video&key={_youtubeApiKey}";

                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject(content);

                        foreach (var item in data.items)
                        {
                            videos.Add(new YouTubeVideoModel
                            {
                                Title = WebUtility.HtmlDecode((string)item.snippet.title),
                                VideoId = (string)item.id.videoId,
                                Thumbnail = (string)item.snippet.thumbnails.high.url,
                                ChannelTitle = (string)item.snippet.channelTitle,
                                PublishedAt = (DateTime)item.snippet.publishedAt
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("YouTube API Error: " + ex.Message);
            }
            return videos;
        }
    }

    // Model class for YouTube results
    public class YouTubeVideoModel
    {
        public string Title { get; set; }
        public string VideoId { get; set; }
        public string Thumbnail { get; set; }
        public string ChannelTitle { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}