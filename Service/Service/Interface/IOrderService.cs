using Domain.Model;
using Domain.Model.Request;
using Domain.Model.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Service
{
  public interface IOrderService
    {
        Task<Order> Create(CreateOrderRequest order);
        Task<Order> Update(UpdateOrderRequest order);
        ListOrderResponse GetOrders(Filter filter);
        OrderDetails GetDetailsOrder(Guid order_id);
        ListOrderResponse GetOrdersByConsumerId(Guid consumer_id, Filter filter);
        ListOrderResponse GetOrdersByPartnerId(Guid partner_id, FilterPartner filter);
        ListOrderResponse GetOrdersByAdminId(Guid admin_id, FilterAdmin filter);
        Task<ListOrder> UpdateStatusOrder(Guid order_id, Guid order_status_id, Guid updated_by);
        PaymentShippingOptions GetPaymentAndShippingByBranchID(Guid branch_id, string latitude, string longitude);
        StatusResponse GetStatus();
    }
}
