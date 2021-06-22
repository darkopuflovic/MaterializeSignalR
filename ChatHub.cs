using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace MaterializeSignalR
{
    public class ChatHub : Hub
    {
        static readonly Dictionary<string, string> clients;

        static ChatHub()
        {
            clients = new Dictionary<string, string>();
        }

        public List<ClientData> GetClients()
        {
            List<ClientData> allClients = new List<ClientData>();

            foreach (var client in clients)
            {
                var user = Database.GetFromDatabase(client.Value);

                if (user != null)
                {
                    allClients.Add(new ClientData { ID = client.Key, Avatar = user.Avatar, Color = user.Color, TextColor = user.TextColor, UserName = user.UserName });
                }
            }

            return allClients;
        }

        public async Task SendMessage(string idReciever, string idSender, string message)
        {
            await Clients.Client(idReciever).SendAsync("ReceiveMessage", idSender, message);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("YourConnectionID", Context.ConnectionId);

            string email = Context.GetHttpContext().Session.GetString("Email");

            clients.Add(Context.ConnectionId, email);
            var user = Database.GetFromDatabase(email);

            if (user != null)
            {
                await Clients.All.SendAsync("ClientConnected", new ClientData { ID = Context.ConnectionId, Avatar = user.Avatar, Color = user.Color, TextColor = user.TextColor, UserName = user.UserName });
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("ClientDisconnected", Context.ConnectionId);
            clients.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
