using Chat.SignalR.Hubs;
using Chat.SignalR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace Chat.SignalR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDistributedCache _cache;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<ChatHub> _hubContext;

        public HomeController(ILogger<HomeController> logger,IDistributedCache cache ,UserManager<ApplicationUser> userManager, IHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            _cache = cache;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        // HomeController.cs
        public async Task<IActionResult> ChatsPro()
        {
            return View();
        }

        public async Task<IActionResult> GetChatMessages()
        {
            try
            {
                // Retrieve chat messages from Redis cache
                string? existingMessagesJson = await _cache.GetStringAsync("chat_messages");
                if (existingMessagesJson != null)
                {
                    var existingMessages = JsonSerializer.Deserialize<List<ChatMessage>>(existingMessagesJson);
                    return Ok(existingMessages); // Return the deserialized list of ChatMessage objects
                }
                else
                {
                    return Ok(new List<ChatMessage>()); // Return an empty list if no messages found
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                return StatusCode(500, ex.Message);
            }
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
