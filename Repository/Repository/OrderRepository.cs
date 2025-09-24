using Dapper;
using Domain.Model;
using Domain.Model.PagSeguro;
using Domain.Model.Request;
using Domain.Model.Response;
using Newtonsoft.Json;
using Npgsql;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public OrderRepository(string connectionString, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public Order Create(CreateOrderRequest order, Branch branch, RateSettings rateSettings)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    var transaction = connection.BeginTransaction();

                    var orderbranch = Guid.Empty.ToString().Equals(order.Branch_id.ToString()) ? "NULL" : $"'{order.Branch_id}'";
                    var consumeraddress = Guid.Empty.ToString().Equals(order.Address_id.ToString()) ? "NULL" : $"'{order.Address_id}'";
                    var shipping = Guid.Empty.ToString().Equals(order.Shipping_company_id.ToString()) ? "NULL" : $"'{order.Shipping_company_id}'";
                    // INSERINDO NOVO PEDIDO
                    string sqlInsertOrder = $@"INSERT INTO orders.orders
                            (
                                order_status_id
                                , freight
                                , amount
                                , address_id
                                , observation
                                , shipping_company_id
                                , created_by 
                                , branch_id
                                , consumer_id
                                , change
                                ,service_fee
                                ,card_fee
                            )
                            VALUES
                            (   '{order.Order_status_id}'
                              , {order.Shipping_options.Value}
                              , '{order.Amount.ToString().Replace(",", ".")}'
                              , {consumeraddress}
                              , '{order.Observation}'
                              , {shipping}
                              , '{order.Created_by}'
                              , {orderbranch}
                              , '{order.Consumer_id}'
                              , {order.Change}
                              , '{rateSettings.Service_fee.ToString().Replace(",", ".")}'
                              , '{rateSettings.Card_fee.ToString().Replace(",", ".")}'
                            ) RETURNING *;";

                    var insertOrder = connection.Query<Order>(sqlInsertOrder).FirstOrDefault();

                    var sqlShipping = $@"INSERT INTO orders.order_shipping(
                                            order_id, 
                                            delivery_option_id, 
                                            name, 
                                            value, 
                                            shipping_free,
                                            created_by
                                            )
                                 VALUES(
                                            '{insertOrder.Order_id}',
                                            '{order.Shipping_options.Delivery_option_id}',
                                            '{order.Shipping_options.Name}',
                                            '{order.Shipping_options.Value.ToString().Replace(",", ".")}',
                                            {order.Shipping_options.Shipping_free},
                                            '{order.Created_by}'
                                        ) RETURNING *;";

                    var insertShipping = connection.Query<ShippingOptions>(sqlShipping).ToList();

                    var sql = $@"INSERT INTO orders.order_branch(
                                            order_id, 
                                            branch_id, 
                                            branch_name, 
                                            document, 
                                            partner_id, 
                                            phone)
                                 VALUES(
                                            '{insertOrder.Order_id}',
                                            '{branch.Branch_id}',
                                            '{branch.Branch_name}',
                                            '{branch.Document}',
                                            '{branch.Partner_id}',
                                            '{branch.Phone}'
                                        ) RETURNING *;";

                    var insertOrderBranch = connection.Query<Branch>(sql).ToList();

                    sql = $@"INSERT INTO orders.order_consumer(
                                            order_id, 
                                            address_id, 
                                            street, 
                                            number, 
                                            complement, 
                                            district, 
                                            city, 
                                            state, 
                                            zip_code, 
                                            latitude, 
                                            longitude, 
                                            consumer_id,
                                            legal_name,
                                            fantasy_name,
                                            document,
                                            email,
                                            phone_number
                                            )
                                 VALUES(
                                            '{insertOrder.Order_id}',
                                            {consumeraddress},
                                            '{order.Address.Street}',
                                            '{order.Address.Number}',
                                            '{order.Address.Complement}',
                                            '{order.Address.District}',
                                            '{order.Address.City}',
                                            '{order.Address.Street}',
                                            '{order.Address.Zip_code}',
                                            '{order.Address.Latitude}',
                                            '{order.Address.Longitude}',
                                            '{order.Consumer_id}',
                                            '{order.Address.Legal_name}',
                                            '{order.Address.Fantasy_name}',
                                            '{order.Address.Document}',
                                            '{order.Address.Email}',
                                            '{order.Address.Phone_number}'
                                        ) RETURNING *;";

                    var insertOrderAddress = connection.Query<ConsumerDetails>(sql).ToList();

                    var sqlOrderStatus = $@"INSERT INTO orders.orders_status_history
                                            (
                                            order_id
                                            , order_status_id
                                            , created_by
                                            )
                                            VALUES(
                                            '{insertOrder.Order_id}'
                                            , '{insertOrder.Order_status_id}'
                                            , '{order.Created_by}'
                                            ) RETURNING *;";

                    var insertStatus = connection.Query<Order>(sqlOrderStatus).FirstOrDefault();

                    var insertedOrderItens = new List<OrderItens>();

                    foreach (var item in order.Order_itens)
                    {

                        // INSERINDO PRODUTO NA TABELA DE ITENS DO PEDIDO
                        string sqlInsertItem = $@"INSERT INTO orders.orders_itens
                            (
                                product_name
                              , quantity
                              , product_value
                              , product_id
                              , order_id
                            )
                            VALUES
                            (
                                '{item.Product_name}'
                              , {item.Quantity}
                              , {item.Product_value.ToString().Replace(",", ".")}
                              , '{item.Product_id}'
                              , '{insertOrder.Order_id}'
                            ) RETURNING *;";

                        var insertedItem = connection.Query<OrderItens>(sqlInsertItem).FirstOrDefault();

                        insertedOrderItens.Add(insertedItem);
                    }


                    var insertedPayment = new List<Payment>();

                    foreach (var item in order.Payments)
                    {

                        string sqlInsertItem = $@"INSERT INTO billing.payment
                            (
                                payment_options_id
                              , order_id
                              , amount_paid
                              , installments
                              , created_by
                            )
                            VALUES
                            (
                                '{item.Payment_options_id}'
                              , '{insertOrder.Order_id}'
                              , '{item.Amount_paid.ToString().Replace(",", ".")}'
                              , {item.Installments}
                              , '{order.Created_by}'
                            ) RETURNING *;";

                        var insertedItem = connection.Query<Payment>(sqlInsertItem).FirstOrDefault();

                        insertedPayment.Add(insertedItem);
                    }

                    if (insertOrder is null || order.Order_itens.Count != insertedOrderItens.Count
                        || insertStatus is null || order.Payments.Count != insertedPayment.Count
                        || insertOrderBranch is null || insertOrderAddress is null)
                    {
                        transaction.Dispose();
                        connection.Close();
                        throw new Exception("errorWhileInsertOrderOnDB");
                    }

                    transaction.Commit();
                    connection.Close();

                    insertOrder.Order_itens = insertedOrderItens;
                    insertOrder.Payments = insertedPayment;
                    insertOrder.Partner_id = branch.Partner_id;
                    return insertOrder;
                }
            }
            catch (Exception)
            {
                throw new Exception("errorWhileInsertOrderOnDB");
            }
        }
        public Order Update(UpdateOrderRequest order, Branch branch, RateSettings rateSettings)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    var transaction = connection.BeginTransaction();

                    var orderbranch = Guid.Empty.ToString().Equals(order.Branch_id.ToString()) ? "NULL" : $"'{order.Branch_id}'";
                    var consumeraddress = Guid.Empty.ToString().Equals(order.Address_id.ToString()) ? "NULL" : $"'{order.Address_id}'";
                    var shipping = Guid.Empty.ToString().Equals(order.Shipping_company_id.ToString()) ? "NULL" : $"'{order.Shipping_company_id}'";

                    string sqlUpdateOrder = $@"UPDATE orders.orders SET
                            
                                updated_at=CURRENT_TIMESTAMP
                                , updated_by='{order.Updated_by}'
                            WHERE order_id = '{order.Order_id}'
                            RETURNING *;";

                    var updateOrder = connection.Query<Order>(sqlUpdateOrder).FirstOrDefault();

                    var sqlShipping = $@"UPDATE orders.order_shipping SET

                                         updated_at=CURRENT_TIMESTAMP
                                        , updated_by='{order.Updated_by}'
                            WHERE order_id = '{order.Order_id}'
                            RETURNING *;";

                    var updateShipping = connection.Query<ShippingOptions>(sqlShipping).ToList();


                    var sqlOrderStatus = $@"INSERT INTO orders.orders_status_history
                                            (
                                            order_id
                                            , order_status_id
                                            , created_by
                                            )
                                            VALUES(
                                            '{order.Order_id}'
                                            , '{order.Order_status_id}'
                                            , '{order.Updated_by}'
                                            ) RETURNING *;";

                    var updateStatus = connection.Query<Order>(sqlOrderStatus).FirstOrDefault();



                    var updatedPayment = new List<Payment>();

                    foreach (var item in order.Payments)
                    {

                        string sqlInsertItem = $@"UPDATE billing.payment SET

                                         payment_options_id='{item.Payment_options_id}'
                                        , installments = {item.Installments}
                                        , updated_at=CURRENT_TIMESTAMP
                                        , updated_by='{order.Updated_by}'
                            WHERE payment_id = '{item.Payment_id}'
                            RETURNING *;";

                        var updatedItem = connection.Query<Payment>(sqlInsertItem).FirstOrDefault();

                        updatedPayment.Add(updatedItem);
                    }

                    if (updateStatus is null || order.Payments.Count != updatedPayment.Count)
                    {
                        transaction.Dispose();
                        connection.Close();
                        throw new Exception("errorWhileInsertOrderOnDB");
                    }

                    transaction.Commit();
                    connection.Close();
                    updateOrder.Order_itens = order.Order_itens;
                    updateOrder.Payments = updatedPayment;
                    updateOrder.Partner_id = branch.Partner_id;
                    return updateOrder;
                }
            }
            catch (Exception)
            {
                throw new Exception("errorWhileUpdateOrderOnDB");
            }
        }
        public RateSettings GetRateSettings(Guid partner_id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"SELECT * FROM partner.partner WHERE partner_id = '{partner_id}';";
                    var response = connection.Query<RateSettings>(sql).FirstOrDefault();

                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ListOrderResponse GetOrders(Filter filter)
        {
            try
            {
                // Pegar todos os pedidos, sem filtro
                string sql = $@"SELECT 
                                    o.order_id,
                                    o.order_number,
                                    o.amount,
                                (SELECT json_build_object('delivery_option_id',s.delivery_option_id, 'value', s.value, 'Shipping_free',s.shipping_free,'Name',s.name)
                                     FROM orders.order_shipping s 
                                    WHERE s.order_id = o.order_id) AS shipping_options,
                                    os.order_status_id,
                                    os.name as status_name,
                                   (SELECT json_build_object('consumer_id',c.consumer_id, 'legal_name', c.legal_name, 'user_id',c.user_id)
                                         FROM consumer.consumer c
                                    where c.consumer_id = a.consumer_id) AS consumer,
    
                                    (SELECT json_build_object('partner_id',p.partner_id, 'fantasy_name', p.fantasy_name,
    						                                'user_id',p.user_id, 'identifier',p.identifier, 'branch_id',branch_id, 'branch_name', branch_name)
                                         FROM partner.branch b
                                         inner join partner.partner p
                                    on p.partner_id = b.partner_id
                                    where b.branch_id = o.branch_id 
                                    ) AS partner,
    
                                    (SELECT json_agg(itens) from
                                    (SELECT oi.order_item_id, oi.product_name, oi.quantity, oi.product_value, oi.product_id, product.image_default, product_image.url 
                                    FROM orders.orders_itens oi
                                    inner join catalog.product
                                    on oi.product_id  = product.product_id
                                    inner join catalog.product_image
                                    on product_image.product_image_id = product.image_default
                                    WHERE oi.order_id = o.order_id) itens ) AS order_itens,
                                    o.created_at,
                                    o.updated_at,
                                    po.payment_options_id,
                                    po.description,
                                    pl.payment_local_id, 
                                    pl.payment_local_name
                                    FROM orders.orders o
                                     inner join orders.orders_status os
                                    on o.order_status_id = os.order_status_id
                                    inner join consumer.address a
                                    on a.address_id = o.address_id
                                    inner join consumer.consumer c 
                                    on c.consumer_id = a.consumer_id
                                    inner join partner.branch b 
                                    on b.branch_id = o.branch_id
                                    inner join partner.partner p 
                                    on p.partner_id = b.partner_id
                                    join billing.payment pa
                                    on pa.order_id = o.order_id
                                    inner join billing.payment_options po
                                    on po.payment_options_id = pa.payment_options_id
                                    JOIN billing.payment_options_local pol 
                                    ON pol.payment_options_id = po.payment_options_id
                                    JOIN billing.payment_local pl  
                                    ON pl.payment_local_id = pol.payment_local_id
                                    where (upper(p.fantasy_name) like upper('%{filter.Filters}%') or upper(p.legal_name) like upper('%{filter.Filters}%') or CAST(p.identifier as TEXT) LIKE '%{filter.Filters}%' or 
                                    upper(c.fantasy_name) like upper('%{filter.Filters}%') or upper(c.legal_name) like upper('%{filter.Filters}%') or upper(os.name) like upper('%{filter.Filters}%')) and
                                    o.created_at between '{filter.Start_date}' and '{filter.End_date}'
                                    order by o.created_at asc
                                ";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var response = connection.Query(sql).Select(x => new ListOrder()
                    {
                        Order_id = x.order_id,
                        Order_number = x.order_number,
                        Shipping_options = !string.IsNullOrEmpty(x.shipping_options) ? JsonConvert.DeserializeObject<ShippingOptions>(x.shipping_options) : new ShippingOptions(),
                        Amount = x.amount,
                        Consumer = !string.IsNullOrEmpty(x.consumer) ? JsonConvert.DeserializeObject<Consumer>(x.consumer) : new Consumer(),
                        Partner = !string.IsNullOrEmpty(x.partner) ? JsonConvert.DeserializeObject<Partner>(x.partner) : new Partner(),
                        Order_itens = !string.IsNullOrEmpty(x.order_itens) ? JsonConvert.DeserializeObject<List<ListOrderItens>>(x.order_itens) : new List<ListOrderItens>(),
                        Order_status_id = x.order_status_id,
                        Status_name = x.status_name,
                        Created_at = x.created_at,
                        Updated_at = x.updated_at,
                        Payment_options_id = x.payment_options_id,
                        Description = x.description,
                        Payment_local_id = x.payment_local_id,
                        Payment_local_name = x.payment_local_name
                    }).ToList();

                    if (response != null)
                    {
                        int totalRows = response.Count();
                        float totalPages = (float)totalRows / (float)filter.ItensPerPage;

                        totalPages = (float)Math.Ceiling(totalPages);
                        response = response.Skip((int)((filter.Page - 1) * filter.ItensPerPage)).Take((int)filter.ItensPerPage).ToList();
                        return new ListOrderResponse()
                        {
                            Orders = response,
                            Pagination = new Pagination()
                            {
                                totalRows = totalRows,
                                totalPages = (int)totalPages
                            }
                        };
                    };

                    return new ListOrderResponse();
                }
            }
            catch (Exception)
            {
                throw new Exception("errorListingOrders");
            }
        }
        public ListOrderResponse GetOrdersByConsumerId(Guid consumer_id, Filter filter)
        {
            try
            {
                // Pegar todos os pedidos, sem filtro
                string sql = $@"SELECT 
                                    o.order_id,
                                    o.order_number,
                                    o.amount,
                                (SELECT json_build_object('delivery_option_id',s.delivery_option_id, 'value', s.value, 'Shipping_free',s.shipping_free,'Name',s.name)
                                     FROM orders.order_shipping s 
                                    WHERE s.order_id = o.order_id) AS shipping_options,
                                    os.order_status_id,
                                    os.name as status_name,
                                   (SELECT json_build_object('consumer_id',c.consumer_id, 'legal_name', c.legal_name, 'user_id',c.user_id)
                                         FROM consumer.consumer c
                                    where c.consumer_id = o.consumer_id) AS consumer,
                                    
                                    (SELECT json_build_object('partner_id',p.partner_id, 'fantasy_name', p.fantasy_name,
                                    						'user_id',p.user_id,'identifier',p.identifier, 'branch_id',branch_id, 'branch_name', branch_name,
                                    						'avatar',pf.avatar)
                                         FROM partner.branch b
                                         inner join partner.partner p
                                    on p.partner_id = b.partner_id
                                     inner join authentication.profile pf
                               		 on p.user_id  = pf.user_id
                                    where b.branch_id = o.branch_id) AS partner,
                                    
                                    (SELECT json_agg(itens) from
                                    (SELECT oi.order_item_id, oi.product_name, oi.quantity, oi.product_value, oi.product_id, product.image_default, product_image.url 
                                    FROM orders.orders_itens oi
                                    inner join catalog.product
                                    on oi.product_id  = product.product_id
                                    left join catalog.product_image
                                    on product_image.product_image_id = product.image_default
                                    WHERE oi.order_id = o.order_id) itens ) AS order_itens,
                                    o.created_at,
                                    o.updated_at,
                                    po.payment_options_id,
                                    po.description,
                                    pl.payment_local_id, 
                                    pl.payment_local_name
                                    FROM orders.orders o
                                     inner join orders.orders_status os
                                    on o.order_status_id = os.order_status_id
                                    inner join consumer.consumer c 
                                    on c.consumer_id = o.consumer_id
                                    inner join partner.branch b 
                                    on b.branch_id = o.branch_id
                                    inner join partner.partner p 
                                    on p.partner_id = b.partner_id 
                                    join billing.payment pa
                                    on pa.order_id = o.order_id
                                    inner join billing.payment_options po
                                    on po.payment_options_id = pa.payment_options_id
                                    JOIN billing.payment_options_local pol 
                                    ON pol.payment_options_id = po.payment_options_id
                                    JOIN billing.payment_local pl  
                                    ON pl.payment_local_id = pol.payment_local_id
                                    where o.consumer_id = '{consumer_id}' and 
                                    (upper(p.fantasy_name) like upper('%{filter.Filters}%') or upper(p.legal_name) like upper('%{filter.Filters}%') or CAST(p.identifier as TEXT) LIKE '%{filter.Filters}%' or 
                                    upper(c.fantasy_name) like upper('%{filter.Filters}%') or upper(c.legal_name) like upper('%{filter.Filters}%') or upper(os.name) like upper('%{filter.Filters}%')) and
                                    o.created_at between '{filter.Start_date}' and '{filter.End_date}'
                                    order by o.created_at desc
                                ";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var response = connection.Query(sql).Select(x => new ListOrder()
                    {
                        Order_id = x.order_id,
                        Order_number = x.order_number,
                        Shipping_options = !string.IsNullOrEmpty(x.shipping_options) ? JsonConvert.DeserializeObject<ShippingOptions>(x.shipping_options) : new ShippingOptions(),
                        Amount = x.amount,
                        Consumer = !string.IsNullOrEmpty(x.consumer) ? JsonConvert.DeserializeObject<Consumer>(x.consumer) : new Consumer(),
                        Partner = !string.IsNullOrEmpty(x.partner) ? JsonConvert.DeserializeObject<Partner>(x.partner) : new Partner(),
                        Order_itens = !string.IsNullOrEmpty(x.order_itens) ? JsonConvert.DeserializeObject<List<ListOrderItens>>(x.order_itens) : new List<ListOrderItens>(),
                        Order_status_id = x.order_status_id,
                        Status_name = x.status_name,
                        Created_at = x.created_at,
                        Updated_at = x.updated_at,
                        Payment_options_id = x.payment_options_id,
                        Description = x.description,
                        Payment_local_id = x.payment_local_id,
                        Payment_local_name = x.payment_local_name
                    }).ToList();

                    if (response != null)
                    {
                        int totalRows = response.Count();
                        float totalPages = (float)totalRows / (float)filter.ItensPerPage;

                        totalPages = (float)Math.Ceiling(totalPages);
                        response = response.Skip((int)((filter.Page - 1) * filter.ItensPerPage)).Take((int)filter.ItensPerPage).ToList();
                        return new ListOrderResponse()
                        {
                            Orders = response,
                            Pagination = new Pagination()
                            {
                                totalRows = totalRows,
                                totalPages = (int)totalPages
                            }
                        };
                    };

                    return new ListOrderResponse();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ListOrderResponse GetOrdersByPartnerId(Guid partner_id, FilterPartner filter)
        {
            try
            {
                // Pegar todos os pedidos, sem filtro
                string sql = $@"SELECT 
                                    o.order_id,
                                    o.order_number,
                                    o.amount,
                                (SELECT json_build_object('delivery_option_id',s.delivery_option_id, 'value', s.value, 'Shipping_free',s.shipping_free,'Name',s.name)
                                     FROM orders.order_shipping s 
                                    WHERE s.order_id = o.order_id) AS shipping_options,
                                    os.order_status_id,
                                    os.name as status_name,
                                   (SELECT json_build_object('consumer_id',c.consumer_id, 'legal_name', c.legal_name, 'user_id',c.user_id)
                                         FROM consumer.consumer c
                                    where c.consumer_id = o.consumer_id) AS consumer,
                                    
                                    (SELECT json_build_object('partner_id',p.partner_id, 'fantasy_name', p.fantasy_name,
                                    						'user_id',p.user_id,'identifier',p.identifier, 'branch_id',branch_id, 'branch_name', branch_name,
                                    						'avatar',pf.avatar)
                                         FROM partner.branch b
                                         inner join partner.partner p
                                    on p.partner_id = b.partner_id
                                     inner join authentication.profile pf
                               		 on p.user_id  = pf.user_id
                                    where b.branch_id = o.branch_id) AS partner,
                                    
                                    (SELECT json_agg(itens) from
                                    (SELECT oi.order_item_id, oi.product_name, oi.quantity, oi.product_value, oi.product_id, product.image_default, product_image.url 
                                    FROM orders.orders_itens oi
                                    inner join catalog.product
                                    on oi.product_id  = product.product_id
                                    left join catalog.product_image
                                    on product_image.product_image_id = product.image_default
                                    WHERE oi.order_id = o.order_id) itens ) AS order_itens,
                                    o.created_at,
                                    o.updated_at,
                                    po.payment_options_id,
                                    po.description,
                                    pl.payment_local_id, 
                                    pl.payment_local_name
                                    FROM orders.orders o
                                     inner join orders.orders_status os
                                    on o.order_status_id = os.order_status_id
                                    inner join consumer.consumer c 
                                    on c.consumer_id = o.consumer_id
                                    inner join partner.branch b 
                                    on b.branch_id = o.branch_id
                                    inner join partner.partner p 
                                    on p.partner_id = b.partner_id
                                    join billing.payment pa
                                    on pa.order_id = o.order_id
                                    inner join billing.payment_options po
                                    on po.payment_options_id = pa.payment_options_id
                                    JOIN billing.payment_options_local pol 
                                    ON pol.payment_options_id = po.payment_options_id
                                    JOIN billing.payment_local pl  
                                    ON pl.payment_local_id = pol.payment_local_id
                                    where b.partner_id = '{partner_id}' and upper(b.branch_name) like upper('%{filter.Filial}%') and cast(o.order_number as text) like '%{filter.Order_number}%' and
                                    (upper(c.fantasy_name) like upper('%{filter.Consumer}%') or upper(c.legal_name) like upper('%{filter.Consumer}%')) and upper(os.name) like upper('%{filter.Status}%') and
                                    o.created_at between '{filter.Start_date}' and '{filter.End_date}'
                                    order by o.created_at desc
                                ";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var response = connection.Query(sql).Select(x => new ListOrder()
                    {
                        Order_id = x.order_id,
                        Order_number = x.order_number,
                        Shipping_options = !string.IsNullOrEmpty(x.shipping_options) ? JsonConvert.DeserializeObject<ShippingOptions>(x.shipping_options) : new ShippingOptions(),
                        Amount = x.amount,
                        Consumer = !string.IsNullOrEmpty(x.consumer) ? JsonConvert.DeserializeObject<Consumer>(x.consumer) : new Consumer(),
                        Partner = !string.IsNullOrEmpty(x.partner) ? JsonConvert.DeserializeObject<Partner>(x.partner) : new Partner(),
                        Order_itens = !string.IsNullOrEmpty(x.order_itens) ? JsonConvert.DeserializeObject<List<ListOrderItens>>(x.order_itens) : new List<ListOrderItens>(),
                        Order_status_id = x.order_status_id,
                        Status_name = x.status_name,
                        Created_at = x.created_at,
                        Updated_at = x.updated_at,
                        Payment_options_id = x.payment_options_id,
                        Description = x.description,
                        Payment_local_id = x.payment_local_id,
                        Payment_local_name = x.payment_local_name
                    }).ToList();

                    if (response != null)
                    {
                        int totalRows = response.Count();
                        float totalPages = (float)totalRows / (float)filter.ItensPerPage;

                        totalPages = (float)Math.Ceiling(totalPages);
                        response = response.Skip((int)((filter.Page - 1) * filter.ItensPerPage)).Take((int)filter.ItensPerPage).ToList();
                        return new ListOrderResponse()
                        {
                            Orders = response,
                            Pagination = new Pagination()
                            {
                                totalRows = totalRows,
                                totalPages = (int)totalPages
                            }
                        };
                    };

                    return new ListOrderResponse();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ListOrderResponse GetOrdersByAdminId(Guid admin_id, FilterAdmin filter)
        {
            try
            {
                // Pegar todos os pedidos, sem filtro
                string sql = $@"SELECT 
                                    o.order_id,
                                    o.order_number,
                                    o.amount,
                                    o.change,
                                (SELECT json_build_object('delivery_option_id',s.delivery_option_id, 'value', s.value, 'Shipping_free',s.shipping_free,'Name',s.name)
                                     FROM orders.order_shipping s 
                                    WHERE s.order_id = o.order_id) AS shipping_options,
                                    os.order_status_id,
                                    os.name as status_name,
                                   (SELECT json_build_object('consumer_id',c.consumer_id, 'legal_name', c.legal_name, 'user_id',c.user_id)
                                         FROM consumer.consumer c
                                    where c.consumer_id = o.consumer_id) AS consumer,
                                    
                                    (SELECT json_build_object('partner_id',p.partner_id, 'fantasy_name', p.fantasy_name, 'legal_name', p.legal_name,
                                    						'user_id',p.user_id,'identifier',p.identifier, 'branch_id',branch_id, 'branch_name', branch_name,
                                    						'avatar',pf.avatar)
                                         FROM partner.branch b
                                         inner join partner.partner p
                                    on p.partner_id = b.partner_id
                                     inner join authentication.profile pf
                               		 on p.user_id  = pf.user_id
                                    where b.branch_id = o.branch_id) AS partner,
                                    
                                    (SELECT json_agg(itens) from
                                    (SELECT oi.order_item_id, oi.product_name, oi.quantity, oi.product_value, oi.product_id, product.image_default, product_image.url 
                                    FROM orders.orders_itens oi
                                    inner join catalog.product
                                    on oi.product_id  = product.product_id
                                    left join catalog.product_image
                                    on product_image.product_image_id = product.image_default
                                    WHERE oi.order_id = o.order_id) itens ) AS order_itens,
                                    o.created_at,
                                    o.updated_at,
                                    po.payment_options_id,
                                    po.description,
                                    pl.payment_local_id, 
                                    pl.payment_local_name
                                    FROM orders.orders o
                                     inner join orders.orders_status os
                                    on o.order_status_id = os.order_status_id
                                    inner join consumer.consumer c 
                                    on c.consumer_id = o.consumer_id
                                    inner join partner.branch b 
                                    on b.branch_id = o.branch_id
                                    inner join partner.partner p 
                                    on p.partner_id = b.partner_id
                                    join billing.payment pa
                                    on pa.order_id = o.order_id
                                    inner join billing.payment_options po
                                    on po.payment_options_id = pa.payment_options_id
                                    JOIN billing.payment_options_local pol 
                                    ON pol.payment_options_id = po.payment_options_id
                                    JOIN billing.payment_local pl  
                                    ON pl.payment_local_id = pol.payment_local_id
                                    where p.admin_id = '{admin_id}' and cast(o.order_number as text) like '%{filter.Order_number}%'  and 
                                    (upper(p.fantasy_name) like upper('%{filter.Partner}%') or upper(p.legal_name) like upper('%{filter.Partner}%')) and
                                    (upper(c.fantasy_name) like upper('%{filter.Consumer}%') or upper(c.legal_name) like upper('%{filter.Consumer}%')) and upper(os.name) like upper('%{filter.Status}%') and
                                    o.created_at between '{filter.Start_date}' and '{filter.End_date}'
                                    order by o.created_at desc
                                ";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var response = connection.Query(sql).Select(x => new ListOrder()
                    {
                        Order_id = x.order_id,
                        Order_number = x.order_number,
                        Shipping_options = !string.IsNullOrEmpty(x.shipping_options) ? JsonConvert.DeserializeObject<ShippingOptions>(x.shipping_options) : new ShippingOptions(),
                        Amount = x.amount,
                        Change = x.change,
                        Consumer = !string.IsNullOrEmpty(x.consumer) ? JsonConvert.DeserializeObject<Consumer>(x.consumer) : new Consumer(),
                        Partner = !string.IsNullOrEmpty(x.partner) ? JsonConvert.DeserializeObject<Partner>(x.partner) : new Partner(),
                        Order_itens = !string.IsNullOrEmpty(x.order_itens) ? JsonConvert.DeserializeObject<List<ListOrderItens>>(x.order_itens) : new List<ListOrderItens>(),
                        Order_status_id = x.order_status_id,
                        Status_name = x.status_name,
                        Created_at = x.created_at,
                        Updated_at = x.updated_at,
                        Payment_options_id = x.payment_options_id,
                        Description = x.description,
                        Payment_local_id = x.payment_local_id,
                        Payment_local_name = x.payment_local_name
                    }).ToList();

                    if (response != null)
                    {
                        int totalRows = response.Count();
                        float totalPages = (float)totalRows / (float)filter.ItensPerPage;

                        totalPages = (float)Math.Ceiling(totalPages);
                        response = response.Skip((int)((filter.Page - 1) * filter.ItensPerPage)).Take((int)filter.ItensPerPage).ToList();
                        return new ListOrderResponse()
                        {
                            Orders = response,
                            Pagination = new Pagination()
                            {
                                totalRows = totalRows,
                                totalPages = (int)totalPages
                            }
                        };
                    };

                    return new ListOrderResponse();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public OrderDetails GetDetailsOrder(Guid order_id)
        {
            try
            {
                // Pegar todos os pedidos, sem filtro
                string sql = $@"SELECT 
                                    o.order_id,
                                    o.order_number,
                                    o.amount,
                                    o.change,
                                    o.service_fee,
                                    o.card_fee,
                                (SELECT json_build_object('delivery_option_id',s.delivery_option_id, 'value', s.value, 'Shipping_free',s.shipping_free,'Name',s.name)
                                     FROM orders.order_shipping s 
                                    WHERE s.order_id = o.order_id) AS shipping_options,
                                    o.observation,
                                    os.order_status_id,
                                    os.name as status_name,
                                   (SELECT row_to_json(cons)
                                         FROM (select c.*, a.* FROM consumer.consumer c
                                         join orders.order_consumer a on a.order_id = o.order_id 
                                    where c.consumer_id = o.consumer_id) cons) as consumer,
                                    
                                    (SELECT row_to_json(part)
                                         FROM (SELECT P.*, b.* FROM partner.branch b
                                         inner join partner.partner p
                                    on p.partner_id = b.partner_id
                                    WHERE b.branch_id = o.branch_id) part)AS partner,
                                    
                                    (SELECT json_agg(itens) FROM
                                    (SELECT oi.order_item_id, oi.product_name, oi.quantity, oi.product_value, oi.product_id, product.image_default, product_image.url 
                                    FROM orders.orders_itens oi
                                    inner join catalog.product
                                    on oi.product_id  = product.product_id
                                    left join catalog.product_image
                                    on product_image.product_image_id = product.image_default
                                    WHERE oi.order_id = o.order_id) itens ) AS order_itens,
                                    
                                    (SELECT row_to_json(shipping)
                                        FROM (
                                          SELECT sc.*,
                                          to_jsonb(aa.*) address
                                         FROM orders.shipping_company sc 
                                    left join orders.address aa
                                    on aa.address_id = sc.address_id
                                    where sc.shipping_company_id = o.shipping_company_id 
                                        ) shipping
                                      ) AS shipping,
                                      
                                      (SELECT json_agg(payments) from
                                    (SELECT p.*, ps.description, po.identifier, po.description,pl.payment_local_id, pl.payment_local_name
                                    FROM billing.payment p
                                    JOIN billing.payment_situation ps
                                    on p.payment_situation_id  = ps.payment_situation_id
                                    JOIN billing.payment_options po
                                    on po.payment_options_id = p.payment_options_id
                                    JOIN billing.payment_options_local pol 
                                    ON pol.payment_options_id = po.payment_options_id
                                    JOIN billing.payment_local pl  
                                    ON pl.payment_local_id = pol.payment_local_id
                                    WHERE p.order_id = o.order_id) payments ) AS payments,
                                    o.created_by,
                                    o.created_at,
                                    o.updated_by,
                                    o.updated_at,
                                    (SELECT json_agg(payment) from
                                    (SELECT ph.response->>'id' as id,
										    ph.response->>'created_at' as created_at_payment,
										    ph.status as status_payment,
										    CASE
                                                 WHEN ph.status = -1  THEN 'PENDING'
											     WHEN ph.status = 0  THEN 'CANCELED'
											     WHEN ph.status = 1  THEN 'PAID'
											     WHEN ph.status = 2  THEN 'AUTHORIZED'
											     WHEN ph.status = 3  THEN 'IN_ANALYSIS'
											     WHEN ph.status = 4  THEN 'DECLINED'
											     WHEN ph.status = 5  THEN 'WAITING_PIX'
											end as status_payment_name
                                    FROM billing.payment_history ph                                   
                                    WHERE ph.order_id = o.order_id
                                    order by ph.response->>'created_at' desc) payment ) AS payment_history
                                    FROM orders.orders o
                                     inner join orders.orders_status os
                                    on o.order_status_id = os.order_status_id
                                    where o.order_id = '{order_id}'
                                ";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var response = connection.Query(sql).Select(x => new OrderDetails()
                    {
                        Order_id = x.order_id,
                        Order_number = x.order_number,
                        Shipping_options = !string.IsNullOrEmpty(x.shipping_options) ? JsonConvert.DeserializeObject<ShippingOptions>(x.shipping_options) : new ShippingOptions(),
                        Amount = x.amount,
                        Change = x.change,
                        Service_fee = x.service_fee,
                        Card_fee = x.card_fee,
                        Order_status_id = x.order_status_id,
                        Status_name = x.status_name,
                        Observation = x.observation,
                        Consumer = !string.IsNullOrEmpty(x.consumer) ? JsonConvert.DeserializeObject<ConsumerDetails>(x.consumer) : new ConsumerDetails(),
                        Partner = !string.IsNullOrEmpty(x.partner) ? JsonConvert.DeserializeObject<PartnerDetails>(x.partner) : new PartnerDetails(),
                        Shipping = !string.IsNullOrEmpty(x.shipping) ? JsonConvert.DeserializeObject<ShippingCompany>(x.shipping) : new ShippingCompany(),
                        Order_itens = !string.IsNullOrEmpty(x.order_itens) ? JsonConvert.DeserializeObject<List<ListOrderItens>>(x.order_itens) : new List<ListOrderItens>(),
                        Payments = !string.IsNullOrEmpty(x.payments) ? JsonConvert.DeserializeObject<List<ListPayment>>(x.payments) : new List<ListPayment>(),
                        Payment_history = !string.IsNullOrEmpty(x.payment_history) ? JsonConvert.DeserializeObject<List<PaymentHistory>>(x.payment_history) : new List<PaymentHistory>(),
                        Created_by = x.created_by,
                        Created_at = x.created_at,
                        Updated_by = x.updated_by,
                        Updated_at = x.updated_at,

                    }).ToList();

                    return response.FirstOrDefault();


                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ListOrder UpdateStatusOrder(Guid order_id, Guid order_status_id, Guid updated_by)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    var transaction = connection.BeginTransaction();

                    string sql = $@"UPDATE orders.orders SET
                           
                                order_status_id = '{order_status_id}'
                                , updated_by = '{updated_by}'
                                , updated_at = now()
                            WHERE order_id = '{order_id}' RETURNING *;";

                    var updatedOrder = connection.Query<ListOrder>(sql).FirstOrDefault();

                    var sqlOrderStatus = $@"INSERT INTO orders.orders_status_history
                                            (
                                            order_id
                                            , order_status_id
                                            , created_by
                                            )
                                            VALUES(
                                            '{updatedOrder.Order_id}'
                                            , '{updatedOrder.Order_status_id}'
                                            , '{updated_by}'
                                            ) RETURNING *;";

                    var insertStatus = connection.Query<Order>(sqlOrderStatus).FirstOrDefault();

                    if (updatedOrder is null || insertStatus is null)
                    {
                        transaction.Dispose();
                        connection.Close();
                        throw new Exception("errorWhileUpdatedOrderOnDB");
                    }


                    transaction.Commit();


                    sql = $"SELECT name status_name FROM orders.orders_status WHERE order_status_id = '{order_status_id}'";

                    updatedOrder.Status_name = connection.Query<string>(sql).FirstOrDefault();
                    connection.Close();


                    return updatedOrder;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Branch GetBranchById(Guid branch_id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = $@"SELECT * FROM partner.branch WHERE branch_id = '{branch_id}'";

                    var response = connection.Query<Branch>(sql).FirstOrDefault();

                    return response;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ConsumerDetails GetConsumerAddress(Guid address_id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = $@"SELECT * FROM consumer.address WHERE address_id = '{address_id}'";

                    var response = connection.Query<ConsumerDetails>(sql).FirstOrDefault();

                    return response;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public PaymentShippingOptions GetPaymentAndShippingByBranchID(Guid branch_id, string latitude, string longitude)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sqlshipment = $@"   select s.delivery_option_id,
	                                        ad.name,
	                                        min(s.value) value
                                        from logistics.actuation_area_shipping s 
	                                        JOIN logistics.actuation_area_delivery_option ad 
	                                        on	ad.delivery_option_id = s.delivery_option_id 
	                                        join logistics.actuation_area_config c
	                                        on s.actuation_area_config_id = c.actuation_area_config_id 
	                                        join logistics.actuation_area aa 
	                                        on c.actuation_area_id = aa.actuation_area_id 
	                                        where aa.branch_id = '{branch_id}' and logistics.ST_Intersects(logistics.ST_GeomFromText('POINT({longitude} {latitude})', 4326), aa.geometry)
                                        GROUP BY s.delivery_option_id, ad.name
                                    ";

                    var sqlpayment = $@"SELECT distinct p.payment_options_id, po.description, pl.payment_local_id, pl.payment_local_name 
                                        FROM logistics.actuation_area_payments p 
                                        JOIN billing.payment_options po 
                                        ON po.payment_options_id = p.payment_options_id
                                        JOIN logistics.actuation_area_config aac 
                                        ON aac.actuation_area_config_id = p.actuation_area_config_id 
                                        JOIN logistics.actuation_area aa 
                                        ON aa.actuation_area_id = aac.actuation_area_id
                                        JOIN billing.payment_options_local pol 
                                        ON pol.payment_options_id = po.payment_options_id
                                        JOIN billing.payment_local pl  
                                        ON pl.payment_local_id = pol.payment_local_id
                                        WHERE aa.branch_id = '{branch_id}' and logistics.ST_Intersects(logistics.ST_GeomFromText('POINT({longitude} {latitude})', 4326), aa.geometry)";

                    var payments = connection.Query<PaymentOptions>(sqlpayment).ToList();
                    var shipments = connection.Query<ShippingOptions>(sqlshipment).ToList();

                    var response = new PaymentShippingOptions()
                    {
                        Payment_options = payments,
                        Shipping_options = shipments
                    };

                    return response;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public StatusResponse GetStatus()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = "SELECT * FROM orders.orders_status where active = true";

                    var response = connection.Query<Status>(sql).ToList();
                    return new StatusResponse() { Status = response };
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Guid? GetChatIdByOrderId(Guid order_id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = $"SELECT chat_id FROM communication.chat where order_id = '{order_id}' and closed IS NULL and closed_by IS NULL";

                    var response = connection.Query<Guid>(sql).FirstOrDefault();
                    if (response == null || response == Guid.Empty)
                    {
                        return null;
                    }
                    return response;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public TaxAndAccount_idPartner GetTaxAndAccount_id(Guid partner_id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"SELECT bd.account_id, SUM(p.service_fee + p.card_fee) * 100 tax_admin,SUM(1-(p.service_fee + p.card_fee)) * 100 tax_partner 
                                FROM partner.partner p
                                inner join partner.bank_details bd
                                on bd.partner_id = p.partner_id 
                                where p.partner_id = '{partner_id}'
                                group by bd.account_id";

                    var response = connection.Query<TaxAndAccount_idPartner>(sql).FirstOrDefault();
                    if (response == null)
                    {
                        return null;
                    }
                    return response;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CreatePaymentHistory(Guid order_id, string pagseguroresponse, int satus, string pagsegurorequest)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"INSERT INTO billing.payment_history
                                (order_id, response, status, request)
                                VALUES('{order_id}', '{pagseguroresponse}', {satus}, '{pagsegurorequest}') RETURNING *";

                    var response = connection.Query<dynamic>(sql).FirstOrDefault();
                    if (response == null)
                    {
                        throw new Exception("");
                    }
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }


        }
        public Card GetCard(Guid card_id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"SELECT * FROM consumer.card WHERE card_id = '{card_id}'";

                    var response = connection.Query<Card>(sql).FirstOrDefault();
                    if (response == null)
                    {
                        throw new Exception("");
                    }
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Consumer GetConsumer(Guid consumer_id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"SELECT * FROM consumer.consumer WHERE consumer_id = '{consumer_id}'";

                    var response = connection.Query<Consumer>(sql).FirstOrDefault();
                    if (response == null)
                    {
                        throw new Exception("");
                    }
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string GetIdPayment(Guid order_id)
        {

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"SELECT response->'charges'->0->>'id' as id 
                                 FROM billing.payment_history
                                 WHERE order_id ='{order_id}'";

                    var response = connection.Query<string>(sql).FirstOrDefault();
                    if (response == null)
                    {
                        throw new Exception("");
                    }
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Partner GetTaxPartner(Guid partner_id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"SELECT * FROM partner.partner WHERE partner_id = '{partner_id}'";

                    var response = connection.Query<Partner>(sql).FirstOrDefault();
                    if (response == null)
                    {
                        throw new Exception("");
                    }
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
        } 
    }
}
