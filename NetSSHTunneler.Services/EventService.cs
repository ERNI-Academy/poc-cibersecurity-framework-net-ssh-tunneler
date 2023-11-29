using Microsoft.AspNetCore.SignalR;
using NetSSHTunneler.Services.Interfaces;
using System.Threading.Tasks;

namespace NetSSHTunneler.Services
{
    public class EventService : IEventService
    {
        private readonly IHubContext<ChatHub> _hubContext;


        public EventService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }


        public async Task SendMessage(NewMessage message)
        {
            await _hubContext.Clients.All.SendAsync("NewMessage", message);
        }
    }
}
