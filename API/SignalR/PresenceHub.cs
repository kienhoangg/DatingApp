using System;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            var currentUsername = Context.User.GetUsername();
            var isOnline = await _tracker.UserConnected(currentUsername, Context.ConnectionId);
            if (isOnline)
                await Clients.Others.SendAsync("UserIsOnline", currentUsername);

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var currentUsername = Context.User.GetUsername();
            var isOffline = await _tracker.UserDisconnected(currentUsername, Context.ConnectionId);
            if (isOffline)
                await Clients.Others.SendAsync("UserIsOffline", currentUsername);

            await base.OnDisconnectedAsync(exception);
        }
    }
}