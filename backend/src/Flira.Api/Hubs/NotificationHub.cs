using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Flira.Api.Hubs;

[Authorize]
public class NotificationHub : Hub
{
}
