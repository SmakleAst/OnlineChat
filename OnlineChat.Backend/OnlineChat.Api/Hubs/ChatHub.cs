using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using OnlineChat.Api.Models;
using System.Text.Json;

namespace OnlineChat.Api.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IDistributedCache _cache;

        public ChatHub(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task JoinChat(UserConnection connection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);

            var connectionString = JsonSerializer.Serialize(connection);

            await _cache.SetStringAsync(Context.ConnectionId, connectionString);

            await Clients
                .Group(connection.ChatRoom)
                .ReceiveMessage("Admin", $"{connection.UserName} присоединился к чату");
        }

        public async Task SendMessage(string message)
        {
            var connectionString = await _cache.GetAsync(Context.ConnectionId);

            var connection = JsonSerializer.Deserialize<UserConnection>(connectionString);

            if (connection is not null)
            {
                await Clients
                    .Group(connection.ChatRoom)
                    .ReceiveMessage(connection.UserName, message);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionString = await _cache.GetAsync(Context.ConnectionId);

            var connection = JsonSerializer.Deserialize<UserConnection>(connectionString);

            if (connection is not null)
            {
                await _cache.RemoveAsync(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.ChatRoom);

                await Clients
                    .Group(connection.ChatRoom)
                    .ReceiveMessage("Admin", $"{connection.UserName} вышел из чата");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }

    public interface IChatClient
    {
        public Task ReceiveMessage(string userName, string message);
    }
}
