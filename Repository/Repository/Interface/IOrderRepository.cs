using Domain.Model.Request;
using Domain.Model.Response;
using System;
using Domain.Model;
using Domain.Model.PagSeguro;

namespace Infrastructure.Repository
{
  public interface IOrderRepository
    {
        Order Create(CreateOrderRequest order, Branch branch,RateSettings rateSettings);
        Order Update(UpdateOrderRequest order, Branch branch, RateSettings rateSettings);
        ListOrderResponse GetOrders(Filter filter);
        ListOrderResponse GetOrdersByConsumerId(Guid consumer_id, Filter filter);
        ListOrderResponse GetOrdersByPartnerId(Guid partner_id, FilterPartner filter);
        ListOrderResponse GetOrdersByAdminId(Guid admin_id, FilterAdmin filter);
        OrderDetails GetDetailsOrder(Guid order_id);
        ListOrder UpdateStatusOrder(Guid order_id, Guid order_status_id, Guid updated_by);
        Branch GetBranchById(Guid branch_id);
        ConsumerDetails GetConsumerAddress(Guid address_id);
        PaymentShippingOptions GetPaymentAndShippingByBranchID(Guid branch_id, string latitude, string longitude);
        StatusResponse GetStatus();
        Guid? GetChatIdByOrderId(Guid order_id);
        RateSettings GetRateSettings(Guid partner_id);
        TaxAndAccount_idPartner GetTaxAndAccount_id(Guid partner_id);
        bool CreatePaymentHistory(Guid order_id, string pagseguroresponse, int satus, string pagsegurorequest);
        Card GetCard(Guid card_id);
        Consumer GetConsumer(Guid consumer_id);
        string GetIdPayment(Guid order_id);
        Partner GetTaxPartner(Guid partner_id);
    }
}
