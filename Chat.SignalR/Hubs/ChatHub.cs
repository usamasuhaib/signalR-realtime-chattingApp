using Chat.SignalR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chat.SignalR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDistributedCache _cache;

        private readonly TimeSpan _expiration = TimeSpan.FromMinutes(1); // Expiration time of 2 minutes


        public ChatHub(UserManager<ApplicationUser> userManager, IDistributedCache cache)
        {
            _userManager = userManager;
            _cache = cache;
        }

        public async Task SendMessage(string message)
        {
            var user = await _userManager.GetUserAsync(Context.User);
            var userName = user.FirstName;

            // Store message in Redis cache
            await StoreMessageAsync(userName, message);

            // Broadcast the message to all clients
            await Clients.All.SendAsync("ReceiveMessage", userName, message);
        }

        private async Task StoreMessageAsync(string userName, string message)
        {
            try
            {
                // Retrieve existing messages from cache
                var existingMessagesJson = await _cache.GetStringAsync("chat_messages");
                List<ChatMessage> messages = existingMessagesJson != null ? JsonSerializer.Deserialize<List<ChatMessage>>(existingMessagesJson) : new List<ChatMessage>();

                // Add new message to the list
                messages.Add(new ChatMessage
                {
                    UserName = userName,
                    Message = message,
                    Timestamp = DateTime.UtcNow
                });

                // Serialize messages and store back to cache with expiration time
                var updatedMessagesJson = JsonSerializer.Serialize(messages);
                await _cache.SetStringAsync("chat_messages", updatedMessagesJson, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _expiration
                });
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }

    }
}
