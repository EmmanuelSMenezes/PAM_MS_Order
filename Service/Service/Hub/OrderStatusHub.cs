using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Application.Service
{
  public class OrderStatusHub : Hub
  {
    private readonly IOrderService _orderService;
    private readonly ILogger _logger;

    public OrderStatusHub(IOrderService orderService, ILogger logger)
    {
      _orderService = orderService;
      _logger = logger;
    }

    public async Task MoveOrderStatus(Guid order_id, Guid order_status_id, Guid updated_by)
    {

      var order = await _orderService.UpdateStatusOrder(order_id, order_status_id, updated_by);

      await Clients.All.SendAsync("OrderStatusChanged", order.Order_id, order.Order_status_id, order.Status_name);
    }

    public async Task JoinCommunicationOrder(Guid partner_id)
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, partner_id.ToString());
    }

    public async Task RefreshOrders(Guid partner_id, string order)
    {
      await Clients.Group(partner_id.ToString()).SendAsync("RefreshOrderList", order);
    }

  }
}