using Domain.Model;
using Domain.Model.PagSeguro;
using Domain.Model.Request;
using Domain.Model.Response;
using Infrastructure.Repository;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Domain.Model.Payment_Options;

namespace Application.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly ILogger _logger;
        private readonly string _privateSecretKey;
        private readonly string _tokenValidationMinutes;
        private readonly IHubContext<OrderStatusHub> _hubContext;
        private readonly PagSeguroAccess _pagSeguroAccess;



        public OrderService(IOrderRepository repository, ILogger logger, string privateSecretKey, string tokenValidationMinutes, IHubContext<OrderStatusHub> hubContext, PagSeguroAccess pagSeguroAccess)
        {
            _repository = repository;
            _logger = logger;
            _privateSecretKey = privateSecretKey;
            _tokenValidationMinutes = tokenValidationMinutes;
            _hubContext = hubContext;
            _pagSeguroAccess = pagSeguroAccess;
        }

        public async Task<Order> Create(CreateOrderRequest order)
        {
            try
            {
                var getBranch = _repository.GetBranchById(order.Branch_id);
                if (getBranch is null) throw new Exception("NonExistentBranch");

                // var getConsumerAddress = _repository.GetConsumerAddress(order.Address_id);
                // if (getBranch is null) throw new Exception("NonExistentConsumer");
                var partner = _repository.GetTaxPartner(getBranch.Partner_id);

                order.Service_fee = partner.Service_fee;
                order.Card_fee = partner.Card_fee;

                var rateSettings = _repository.GetRateSettings(getBranch.Partner_id);
                bool isPagSeguro = false;

                Order postOrderResponse = new Order();
                Payment_PagSeguro payment = new Payment_PagSeguro();

                order.Order_status_id = Guid.Parse("d71cb62a-28dd-44a8-a008-9d7d7d1af810");
                if (order.Payments.First().Payment_options_id == Guid.Parse("68e05062-eb22-42b1-bdba-b0de058de52e") ||
                      order.Payments.First().Payment_options_id == Guid.Parse("c336dc68-88ba-49c9-a9ca-dcc89952acb6") ||
                      order.Payments.First().Payment_options_id == Guid.Parse("ec50fa62-d353-4cd9-8fad-b55ed491c2a5"))
                {
                    payment = GetValuePaymentPagSeguro(order.Payments.First().Payment_options_id);
                    order.Order_status_id = Guid.Parse("0cff5cdc-6253-4e59-9753-6cde54a33e58");
                    isPagSeguro = true;
                }

                var tax = _repository.GetTaxAndAccount_id(getBranch.Partner_id);

                postOrderResponse = _repository.Create(order, getBranch, rateSettings) ?? throw new Exception("");
                order.Order_id = postOrderResponse.Order_id;

                postOrderResponse.Chat_id = _repository.GetChatIdByOrderId(postOrderResponse.Order_id);

                await _hubContext.Clients.Group(postOrderResponse.Partner_id.ToString()).SendAsync("RefreshOrderList", JsonConvert.SerializeObject(postOrderResponse));

                string[] response;

                postOrderResponse.Pagseguro = new Pagseguro
                {
                    ErrorPayment = new ErrorPagSeguro()

                };

                if (isPagSeguro && (payment == Payment_PagSeguro.CREDIT_CARD || payment == Payment_PagSeguro.DEBIT_CARD))
                {
                    Card card = _repository.GetCard(order.Payments.First().Card_id);
                    card.Security_code = order.Payments.First().Security_code;
                    card.Encrypted = order.Encrypted;

                    Consumer consumer = _repository.GetConsumer(order.Consumer_id);
                    response = payment == Payment_PagSeguro.CREDIT_CARD ? await PaymentCardPagseguroAsync(postOrderResponse, getBranch, card, payment, consumer, order.Address) : await PaymentCardDebitPagseguroAsync(postOrderResponse, getBranch, card, payment, consumer,order.Address,order.AuthenticationMethod);

                    if (response.Contains("error_messages"))
                    {

                        postOrderResponse.Pagseguro.ErrorPayment = JsonConvert.DeserializeObject<ErrorPagSeguro>(response[1]);
                        _logger.Information($"Erro Pagseguro: {response[1]}");
                    }
                    else
                    {
                        int status = -1;
                        string statusname = response[1];
                        if (statusname.Contains(Payment_PagSeguro_Status.PENDING.ToString())) status = (int)Payment_PagSeguro_Status.PENDING;
                        if (statusname.Contains(Payment_PagSeguro_Status.CANCELED.ToString())) status = (int)Payment_PagSeguro_Status.CANCELED;
                        if (statusname.Contains(Payment_PagSeguro_Status.PAID.ToString())) status = (int)Payment_PagSeguro_Status.PAID;
                        if (statusname.Contains(Payment_PagSeguro_Status.AUTHORIZED.ToString())) status = (int)Payment_PagSeguro_Status.AUTHORIZED;
                        if (statusname.Contains(Payment_PagSeguro_Status.IN_ANALYSIS.ToString())) status = (int)Payment_PagSeguro_Status.IN_ANALYSIS;
                        if (statusname.Contains(Payment_PagSeguro_Status.DECLINED.ToString())) status = (int)Payment_PagSeguro_Status.DECLINED;

                        _repository.CreatePaymentHistory(postOrderResponse.Order_id, response[1], status, response[0]);
                        postOrderResponse.Pagseguro.SucessPayment = response[1];

                        if (status == 1 || status == 2)
                        {
                            var ordesstatus = Guid.Parse("36eebcfb-9758-4fdc-ad95-bcdf70703c4a");
                            var updatestatus = UpdateStatusOrder(order.Order_id, ordesstatus, order.Created_by);

                            ordesstatus = Guid.Parse("d71cb62a-28dd-44a8-a008-9d7d7d1af810");
                            updatestatus = UpdateStatusOrder(order.Order_id, ordesstatus, order.Created_by);

                            await _hubContext.Clients.Group(postOrderResponse.Partner_id.ToString()).SendAsync("RefreshOrderList", JsonConvert.SerializeObject(postOrderResponse));
                        }

                    }
                }
                else if (isPagSeguro && payment == Payment_PagSeguro.PIX)
                {
                    var consumer = _repository.GetConsumer(order.Consumer_id);
                    response = await PaymentPixAsync(postOrderResponse, consumer);
                    if (response.Contains("error_messages"))
                    {
                        postOrderResponse.Pagseguro.ErrorPayment = JsonConvert.DeserializeObject<ErrorPagSeguro>(response[1]);
                        _logger.Information($"Erro Pagseguro: {response[1]}");
                    }
                    else
                    {
                        _repository.CreatePaymentHistory(postOrderResponse.Order_id, response[1], 5, response[0]);
                        postOrderResponse.Pagseguro.SucessPayment = response[1];
                    }
                }
                return postOrderResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Order> Update(UpdateOrderRequest order)
        {
            try
            {
                var getBranch = _repository.GetBranchById(order.Branch_id);
                if (getBranch is null) throw new Exception("NonExistentBranch");

                // var getConsumerAddress = _repository.GetConsumerAddress(order.Address_id);
                // if (getBranch is null) throw new Exception("NonExistentConsumer");

                var rateSettings = _repository.GetRateSettings(getBranch.Partner_id);
                bool isPagSeguro = false;

                Order orderResponse = new Order();
                Payment_PagSeguro payment = new Payment_PagSeguro();

                order.Order_status_id = Guid.Parse("d71cb62a-28dd-44a8-a008-9d7d7d1af810");
                if (order.Payments.First().Payment_options_id == Guid.Parse("68e05062-eb22-42b1-bdba-b0de058de52e") ||
                      order.Payments.First().Payment_options_id == Guid.Parse("c336dc68-88ba-49c9-a9ca-dcc89952acb6") ||
                      order.Payments.First().Payment_options_id == Guid.Parse("ec50fa62-d353-4cd9-8fad-b55ed491c2a5"))
                {
                    payment = GetValuePaymentPagSeguro(order.Payments.First().Payment_options_id);
                    order.Order_status_id = Guid.Parse("0cff5cdc-6253-4e59-9753-6cde54a33e58");
                    isPagSeguro = true;
                }

                orderResponse = _repository.Update(order, getBranch, rateSettings) ?? throw new Exception("");
                order.Order_id = orderResponse.Order_id;

                orderResponse.Chat_id = _repository.GetChatIdByOrderId(orderResponse.Order_id);

                await _hubContext.Clients.Group(orderResponse.Partner_id.ToString()).SendAsync("RefreshOrderList", JsonConvert.SerializeObject(orderResponse));

                string[] response;

                orderResponse.Pagseguro = new Pagseguro
                {
                    ErrorPayment = new ErrorPagSeguro()

                };

                if (isPagSeguro && (payment == Payment_PagSeguro.CREDIT_CARD || payment == Payment_PagSeguro.DEBIT_CARD))
                {
                    Card card = _repository.GetCard(order.Payments.First().Card_id);
                    card.Security_code = order.Payments.First().Security_code;
                    card.Encrypted = order.Encrypted;
                    Consumer consumer = _repository.GetConsumer(order.Consumer_id);
                    response = payment == Payment_PagSeguro.CREDIT_CARD ? await PaymentCardPagseguroAsync(orderResponse, getBranch, card, payment, consumer, order.Address) : await PaymentCardDebitPagseguroAsync(orderResponse, getBranch, card, payment, consumer, order.Address, order.AuthenticationMethod);

                    if (response.Contains("error_messages"))
                    {

                        orderResponse.Pagseguro.ErrorPayment = JsonConvert.DeserializeObject<ErrorPagSeguro>(response[1]);
                    }
                    else
                    {
                        int status = -1;
                        string statusname = response[1];

                        if (statusname.Contains(Payment_PagSeguro_Status.PENDING.ToString())) status = (int)Payment_PagSeguro_Status.PENDING;
                        if (statusname.Contains(Payment_PagSeguro_Status.CANCELED.ToString())) status = (int)Payment_PagSeguro_Status.CANCELED;
                        if (statusname.Contains(Payment_PagSeguro_Status.PAID.ToString())) status = (int)Payment_PagSeguro_Status.PAID;
                        if (statusname.Contains(Payment_PagSeguro_Status.AUTHORIZED.ToString())) status = (int)Payment_PagSeguro_Status.AUTHORIZED;
                        if (statusname.Contains(Payment_PagSeguro_Status.IN_ANALYSIS.ToString())) status = (int)Payment_PagSeguro_Status.IN_ANALYSIS;
                        if (statusname.Contains(Payment_PagSeguro_Status.DECLINED.ToString())) status = (int)Payment_PagSeguro_Status.DECLINED;

                        _repository.CreatePaymentHistory(orderResponse.Order_id, response[1], status, response[0]);
                        orderResponse.Pagseguro.SucessPayment = response[1];

                        if (status == 1 || status == 2)
                        {
                            var ordesstatus = Guid.Parse("36eebcfb-9758-4fdc-ad95-bcdf70703c4a");
                            var updatestatus = UpdateStatusOrder(order.Order_id, ordesstatus, order.Updated_by);

                            ordesstatus = Guid.Parse("d71cb62a-28dd-44a8-a008-9d7d7d1af810");
                            updatestatus = UpdateStatusOrder(order.Order_id, ordesstatus, order.Updated_by);
                            await _hubContext.Clients.Group(orderResponse.Partner_id.ToString()).SendAsync("RefreshOrderList", JsonConvert.SerializeObject(orderResponse));
                        }

                    }
                }
                else if (isPagSeguro && payment == Payment_PagSeguro.PIX)
                {
                    var consumer = _repository.GetConsumer(order.Consumer_id);
                    response = await PaymentPixAsync(orderResponse, consumer);
                    if (response.Contains("error_messages"))
                    {
                        orderResponse.Pagseguro.ErrorPayment = JsonConvert.DeserializeObject<ErrorPagSeguro>(response[1]);
                    }
                    else
                    {
                        _repository.CreatePaymentHistory(orderResponse.Order_id, response[1], 5, response[0]);
                        orderResponse.Pagseguro.SucessPayment = response[1];
                    }
                }
                return orderResponse;
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
                var ordersResponse = _repository.GetOrders(filter);
                foreach (var order in ordersResponse.Orders)
                {
                    order.Chat_id = _repository.GetChatIdByOrderId(order.Order_id);
                }
                return ordersResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ListOrderResponse GetOrdersByConsumerId(Guid consumer_id, Filter filter)
        {
            try
            {
                var ordersResponse = _repository.GetOrdersByConsumerId(consumer_id, filter);
                foreach (var order in ordersResponse.Orders)
                {
                    order.Chat_id = _repository.GetChatIdByOrderId(order.Order_id);
                }
                return ordersResponse;
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
                var ordersResponse = _repository.GetOrdersByPartnerId(partner_id, filter);
                foreach (var order in ordersResponse.Orders)
                {
                    order.Chat_id = _repository.GetChatIdByOrderId(order.Order_id);
                }
                return ordersResponse;
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
                var ordersResponse = _repository.GetOrdersByAdminId(admin_id, filter);
                foreach (var order in ordersResponse.Orders)
                {
                    order.Chat_id = _repository.GetChatIdByOrderId(order.Order_id);
                }
                return ordersResponse;
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
                var order = _repository.GetDetailsOrder(order_id);
                if (order == null) throw new Exception("errorListingOrder");
                order.Chat_id = _repository.GetChatIdByOrderId(order.Order_id);
                return order;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public DecodedToken GetDecodeToken(string token, string secret)
        {
            DecodedToken decodedToken = new DecodedToken();
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken = jwtSecurityTokenHandler.ReadToken(token) as JwtSecurityToken;
            foreach (Claim claim in jwtSecurityToken.Claims)
            {
                if (claim.Type == "email")
                {
                    decodedToken.email = claim.Value;
                }
                else if (claim.Type == "name")
                {
                    decodedToken.name = claim.Value;
                }
                else if (claim.Type == "userId")
                {
                    decodedToken.UserId = new Guid(claim.Value);
                }
                else if (claim.Type == "roleId")
                {
                    decodedToken.RoleId = new Guid(claim.Value);
                }

            }
            return decodedToken;

            throw new Exception("invalidToken");
        }
        public async Task<ListOrder> UpdateStatusOrder(Guid order_id, Guid order_status_id, Guid updated_by)
        {
            try
            {

                ListOrder listOrder = new ListOrder();
                if (order_status_id == Guid.Parse("c1a38ac0-37d8-450a-aa91-d297e5c97be3") || order_status_id == Guid.Parse("4840f990-fc9e-4048-97f3-19e105b9aec5"))
                {
                    var detailsOrder = _repository.GetDetailsOrder(order_id);
                    var idPayment = string.Empty;

                    if (detailsOrder.Status_name != "Aguardando pagamento")
                    {

                        if (detailsOrder.Payments.First().Payment_options_id == "68e05062-eb22-42b1-bdba-b0de058de52e" ||
                       detailsOrder.Payments.First().Payment_options_id == "c336dc68-88ba-49c9-a9ca-dcc89952acb6")
                        {
                            idPayment = _repository.GetIdPayment(order_id);
                            var response = await CancelPaymentCardPagseguro(idPayment, (int)(detailsOrder.Amount * 100));
                            listOrder.Pagseguro = new Pagseguro()
                            {
                                ErrorPayment = new ErrorPagSeguro()
                            };
                            if (response.Contains("error_messages") || string.IsNullOrEmpty(response[1]))
                            {
                                listOrder.Pagseguro.ErrorPayment = JsonConvert.DeserializeObject<ErrorPagSeguro>(response[1]);
                                return listOrder;
                            }
                            else
                            {
                                _repository.CreatePaymentHistory(order_id, response[1], (int)Payment_PagSeguro_Status.CANCELED, response[0]);
                                listOrder.Pagseguro.SucessPayment = response[1];
                            }
                        }
                    }

                }
                var order = _repository.UpdateStatusOrder(order_id, order_status_id, updated_by);
                order.Chat_id = _repository.GetChatIdByOrderId(order.Order_id);
                listOrder = order;
                return listOrder;

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
                var get = _repository.GetPaymentAndShippingByBranchID(branch_id, latitude, longitude);
                return get == null ? throw new Exception("errorListingOptions") : get;
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
                return _repository.GetStatus();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private async Task<string[]> PaymentCardPagseguroAsync(Order order, Branch branch, Card card, Payment_PagSeguro payment_PagSeguro, Consumer consumer, ConsumerDetails shipping)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_pagSeguroAccess.Token}");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                //var response = await httpClient.PostAsync($"{_pagSeguroAccess.Url}checkout-sdk/sessions", null);
                //string responseContent = await response.Content.ReadAsStringAsync();

                //Authenticated3ds session = new Authenticated3ds();
                //if (response.StatusCode == System.Net.HttpStatusCode.Created)
                //{
                //    session = JsonConvert.DeserializeObject<Authenticated3ds>(responseContent);
                //}
                var settings = _repository.GetTaxAndAccount_id(branch.Partner_id);

                var splitpartner = ((order.Amount * settings.Tax_partner) + order.Freight);
                var splitadmin = order.Amount * settings.Tax_admin;
                ReceiverRequest accountPartner = new ReceiverRequest
                {
                    account = new AccountRequest() { id = settings.Account_id },
                    amount = new AmountSplit() { value = (int)splitpartner }
                };

                ReceiverRequest accountAdmin = new ReceiverRequest
                {
                    account = new AccountRequest() { id = _pagSeguroAccess.Account_Id },
                    amount = new AmountSplit() { value = (int)splitadmin }
                };

                PagSeguroCardCreditRequest pagseguroRequest = new PagSeguroCardCreditRequest()
                {
                    reference_id = order.Order_id.ToString(),
                    customer = new Customer()
                    {
                        name = consumer.Legal_name,
                        tax_id = consumer.Document,
                        email = consumer.Email
                    },
                    shipping = new ShippingRequest() { 
                    address = new AddressRequestPagseguro()
                    {
                               street = shipping.Street,
                               number = shipping.Number,
                               complement = shipping.Complement,
                               locality = shipping.District,
                               city = shipping.City,
                               region_code = shipping.State,
                               country = "BRA",
                               postal_code = shipping.Zip_code
                    }
                    },
                    charges = new List<Charges>
                    { new Charges()
                    {
                    reference_id = order.Order_id.ToString(),
                    description = $"PAM Plataform - {branch.Branch_name}",
                    amount = new AmountRequest()
                    {
                        value = (int)((order.Amount + order.Freight) * 100),
                        currency = "BRL"
                    },
                    payment_method = new PaymentMethodRequest()
                    {
                        type = payment_PagSeguro.ToString(),
                        installments = order.Payments.First().Installments,
                        capture = true,
                        soft_descriptor = $"PAM_Plataform",
                        card = new CardRequest()
                        {
                           encrypted = card.Encrypted,
                           // security_code = card.Security_code,
                            holder = new HolderRequest()
                            {
                                name = card.Name,
                               // tax_id = card.Document.Replace("-", "").Replace("/", "").Replace(".", "")
                            },
                            store = false
                        }
                    },
                    splits = new SplitsRequest()
                    {
                        method = _pagSeguroAccess.Method_Split,
                        receivers = new List<ReceiverRequest>(),

                    }
                    } },
                    items = new List<Item>()
                };

                pagseguroRequest.charges.First().splits.receivers.Add(accountAdmin);
                pagseguroRequest.charges.First().splits.receivers.Add(accountPartner);

                foreach (var item in order.Order_itens)
                {
                    Item product = new Item()
                    {
                        name = item.Product_name,
                        quantity = item.Quantity,
                        reference_id = item.Product_id.ToString(),
                        unit_amount = (int)(item.Product_value * 100)
                    };
                    pagseguroRequest.items.Add(product);
                }

                var body = JsonConvert.SerializeObject(pagseguroRequest);

                _logger.Information($"Request Credit Card: {body}");


                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{_pagSeguroAccess.Url}orders", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                string[] result = new string[2] { body, responseContent};
                    
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<string[]> PaymentCardDebitPagseguroAsync(Order order, Branch branch, Card card, Payment_PagSeguro payment_PagSeguro, Consumer consumer, ConsumerDetails shipping, AuthenticationMethod authenticationMethod)
        {
            try
            {
                var httpClient = new HttpClient();
                //httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_pagSeguroAccess.Token}");
                //httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                //var response = await httpClient.PostAsync($"{_pagSeguroAccess.Url}checkout-sdk/sessions", null);
                //string responseContent = await response.Content.ReadAsStringAsync();

                //Authenticated3ds session = new Authenticated3ds();
                //if (response.StatusCode == System.Net.HttpStatusCode.Created)
                //{
                //    session = JsonConvert.DeserializeObject<Authenticated3ds>(responseContent);
                //}


                PagSeguroCardDebitRequest pagseguroRequest = new PagSeguroCardDebitRequest()
                {
                    reference_id = order.Order_id.ToString(),
                    customer = new Customer()
                    {
                        name = consumer.Legal_name,
                        tax_id = consumer.Document,
                        email = consumer.Email
                    },
                    shipping = new ShippingRequest()
                    {
                        address = new AddressRequestPagseguro()
                        {
                            street = shipping.Street,
                            number = shipping.Number,
                            complement = shipping.Complement,
                            locality = shipping.District,
                            city = shipping.City,
                            region_code = shipping.State,
                            country = "BRA",
                            postal_code = shipping.Zip_code
                        }
                    },
                    charges = new List<ChargesDebit>
                    { new ChargesDebit()
                    {
                    reference_id = order.Order_id.ToString(),
                    description = $"PAM Plataform - {branch.Branch_name}",
                    amount = new AmountRequest()
                    {
                        value = (int)((order.Amount + order.Freight) * 100),
                        currency = "BRL"
                    },
                    payment_method = new PaymentMethodDebitCardRequest()
                    {
                        type = payment_PagSeguro.ToString(),
                        installments = order.Payments.First().Installments,
                        capture = true,
                        soft_descriptor = $"PAM_Plataform",
                        card = new CardRequest()
                        {
                           encrypted = card.Encrypted,
                           // security_code = card.Security_code,
                            holder = new HolderRequest()
                            {
                                name = card.Name,
                                //tax_id = card.Document.Replace("-", "").Replace("/", "").Replace(".", "")
                            },
                            store = false
                        },
                        authentication_method = new AuthenticationMethod()
                        {
                            type = authenticationMethod.type,
                            id = authenticationMethod.id,
                        }
                    }
                    } },
                    items = new List<Item>()
                };

                foreach (var item in order.Order_itens)
                {
                    Item product = new Item()
                    {
                        name = item.Product_name,
                        quantity = item.Quantity,
                        reference_id = item.Product_id.ToString(),
                        unit_amount = (int)(item.Product_value * 100)
                    };
                    pagseguroRequest.items.Add(product);
                }

                var body = JsonConvert.SerializeObject(pagseguroRequest);
                _logger.Information($"Request Debit Card: {body}");



                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{_pagSeguroAccess.Url}orders", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                string[] result = new string[2] {body, responseContent }; 
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<string[]> PaymentPixAsync(Order order, Consumer consumer)
        {
            try
            {
                PagSeguroPixRequest pagSeguroPixRequest = new PagSeguroPixRequest()
                {
                    customer = new Customer()
                    {
                        name = consumer.Legal_name,
                        email = consumer.Email,
                        tax_id = consumer.Document
                    },
                    reference_id = order.Order_id.ToString(),
                    qr_codes = new List<QrCode>()
                    {
                        new QrCode()
                        {
                            amount = new AmountPix()
                            {
                                value = ((order.Amount + order.Freight) * 100).ToString()
                            },
                            expiration_date = DateTime.Now.AddMinutes(15).ToString("yyyy-MM-ddTHH:mm:sszzz")
                        }
                    }
                };

                var body = JsonConvert.SerializeObject(pagSeguroPixRequest);
                _logger.Information($"Request Pix: {body}");
                var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_pagSeguroAccess.Token}");
                var content = new StringContent(body, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{_pagSeguroAccess.Url}orders", content);

                string responseContent = await response.Content.ReadAsStringAsync();
                string[] result = new string[2] { body, responseContent };
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<string[]> CancelPaymentCardPagseguro(string id, int amount)
        {
            var pagseguro = new PagSeguroReversal()
            {
                amount = new AmountReversal()
                {
                    value = amount.ToString()
                }
            };
            var body = JsonConvert.SerializeObject(pagseguro);

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_pagSeguroAccess.Token}");
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{_pagSeguroAccess.Url}charges/{id}/cancel", content);

            string responseContent = await response.Content.ReadAsStringAsync();

            string[] result = new string[2] {body, responseContent };
            return result;
        }
    }
}
