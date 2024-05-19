using Microsoft.AspNetCore.SignalR;
using OnlineChat.Api.Models;

namespace OnlineChat.Api.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinChat(UserConnection connection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);
        }
    }
}
