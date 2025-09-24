using Application.Service;
using Domain.Model;
using Domain.Model.Request;
using Domain.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("order")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderService _service;
        private readonly ILogger _logger;

        public OrderController(IOrderService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint responsável por criar um pedido
        /// </summary>
        /// <returns>Valida os dados passados para criação de pedido e retorna os dados cadastrados</returns>
        [Authorize]
        [HttpPost("create")]
        [ProducesResponseType(typeof(Response<Order>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Response<Order>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<Order>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Response<Order>>> Create(CreateOrderRequest orderRequest)
        {
            try
            {
                var response = await _service.Create(orderRequest);
                return StatusCode(StatusCodes.Status201Created, new Response<Order>() { Status = 201, Message = $"Pedido gerado com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while creating new Order!");
                switch (ex.Message)
                {                   
                    case "errorCreate":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<Order>() { Status = 400, Message = $"Erro ao cadastrar novo Pedido.", Success = false, Error = ex.Message });
                    case "orderNotCreated":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<Order>() { Status = 400, Message = $"Erro ao cadastrar novo Pedido.", Success = false, Error = ex.Message });
                    case "errorWhileInsertOrderOnDB":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<Order>() { Status = 400, Message = $"Não foi possível registrar pedido. Erro no processo de inserção do pedido na base de dados.", Success = false, Error = ex.Message });
                   case "NonExistentBranch":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<Order>() { Status = 400, Message = $"Erro ao cadastrar novo Pedido. Não foi possivel encontrar filial.", Success = false, Error = ex.Message });
                    case "NonExistentConsumer":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<Order>() { Status = 400, Message = $"Erro ao cadastrar novo Pedido. Não foi possivel encontrar endereço do consumidor.", Success = false, Error = ex.Message });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<Order>() { Status = 500, Message = $"Internal server error!", Success = false, Error = ex.Message });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por listar pedidos
        /// </summary>
        /// <returns>Retorna lista de pedidos</returns>
        [Authorize]
        [HttpGet("")]
        [ProducesResponseType(typeof(Response<ListOrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<ListOrderResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<ListOrderResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<ListOrderResponse>> GetOrders(string filter, int? page, int? itensPerPage, DateTime? start_date = null, DateTime? end_date = null)
        {
            try
            {
                var filters = new Filter
                {
                    Page = page ?? 1,
                    ItensPerPage = itensPerPage ?? 5,
                    Filters = filter,
                    Start_date = start_date != null ? start_date.Value.ToString("yyyy-MM-dd 00:00:00") : "-infinity",
                    End_date = start_date != null ? end_date.Value.ToString("yyyy-MM-dd 23:59:59") : "infinity",
                };
                var response = _service.GetOrders(filters);
                return StatusCode(StatusCodes.Status200OK, new Response<ListOrderResponse>() { Status = 200, Message = $"Pedidos listados com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while listing orders!");
                switch (ex.Message)
                {
                    case "errorListingOrders":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<ListOrderResponse>() { Status = 400, Message = $"Erro ao listar pedidos.", Success = false, Error = ex.Message });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<ListOrderResponse>() { Status = 500, Message = $"Internal server error!", Success = false, Error = ex.Message });
                }
            }
        }


        /// <summary>
        /// Endpoint responsável por listar pedidos do administrador
        /// </summary>
        /// <returns>Retorna lista de pedidos</returns>
        [Authorize]
        [HttpGet("byadmin/{admin_id}")]
        [ProducesResponseType(typeof(Response<ListOrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<ListOrderResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<ListOrderResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<ListOrderResponse>> GetOrdersByAdmin(Guid admin_id,string? order_number, string? status, string? consumer, string? partner, int? page, int? itensPerPage, DateTime? start_date = null, DateTime? end_date = null)
        {
            try
            {
                var filters = new FilterAdmin
                {

                    Order_number = order_number,
                    Page = page ?? 1,
                    ItensPerPage = itensPerPage ?? 5,
                    Status = status,
                    Consumer = consumer,
                    Partner = partner,
                    Start_date = start_date != null ? start_date.Value.ToString("yyyy-MM-dd 00:00:00") : "-infinity",
                    End_date = start_date != null ? end_date.Value.ToString("yyyy-MM-dd 23:59:59") : "infinity",
                };

                var response = _service.GetOrdersByAdminId(admin_id, filters);
                return StatusCode(StatusCodes.Status200OK, new Response<ListOrderResponse>() { Status = 200, Message = $"Pedidos listados com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while listing orders!");
                switch (ex.Message)
                {
                    case "errorListingOrders":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<ListOrderResponse>() { Status = 400, Message = $"Erro ao listar pedidos.", Success = false, Error = ex.Message });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<ListOrderResponse>() { Status = 500, Message = $"Internal server error!", Success = false, Error = ex.Message });
                }
            }
        }
        /// <summary>
        /// Endpoint responsável por listar pedidos realizados pelo consumidor
        /// </summary>
        /// <returns>Retorna lista de pedidos</returns>
        [Authorize]
        [HttpGet("byconsumer/{consumer_id}")]
        [ProducesResponseType(typeof(Response<ListOrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<ListOrderResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<ListOrderResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<ListOrderResponse>> GetOrdersByConsumer(Guid consumer_id, string filter, int? page, int? itensPerPage, DateTime? start_date = null, DateTime? end_date = null)
        {
            try
            {
                var filters = new Filter
                {
                    Page = page ?? 1,
                    ItensPerPage = itensPerPage ?? 5,
                    Filters = filter,
                    Start_date = start_date != null ? start_date.Value.ToString("yyyy-MM-dd 00:00:00") : "-infinity",
                    End_date = start_date != null ? end_date.Value.ToString("yyyy-MM-dd 23:59:59") : "infinity",
                };

                var response = _service.GetOrdersByConsumerId(consumer_id, filters);
                return StatusCode(StatusCodes.Status200OK, new Response<ListOrderResponse>() { Status = 200, Message = $"Pedidos listados com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while listing orders!");
                switch (ex.Message)
                {
                    case "errorListingOrders":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<ListOrderResponse>() { Status = 400, Message = $"Erro ao listar pedidos.", Success = false, Error = ex.Message });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<ListOrderResponse>() { Status = 500, Message = $"Internal server error!", Success = false, Error = ex.Message });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por listar pedidos do parceiro
        /// </summary>
        /// <returns>Retorna lista de pedidos</returns>
        [Authorize]
        [HttpGet("bypartner/{partner_id}")]
        [ProducesResponseType(typeof(Response<ListOrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<ListOrderResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<ListOrderResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<ListOrderResponse>> GetOrdersByPartner(Guid partner_id, string? order_number,  string? status, string? consumer, string? filial, int? page, int? itensPerPage, DateTime? start_date = null, DateTime? end_date = null)
        {
            try
            {
                var filters = new FilterPartner
                {
                    Page = page ?? 1,
                    Order_number = order_number,
                    ItensPerPage = itensPerPage ?? 5,
                    Status = status,
                    Consumer = consumer,
                    Filial = filial,
                    Start_date = start_date != null ? start_date.Value.ToString("yyyy-MM-dd 00:00:00") : "-infinity",
                    End_date = start_date != null ? end_date.Value.ToString("yyyy-MM-dd 23:59:59") : "infinity",
                };

                var response = _service.GetOrdersByPartnerId(partner_id, filters);
                return StatusCode(StatusCodes.Status200OK, new Response<ListOrderResponse>() { Status = 200, Message = $"Pedidos listados com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while listing orders!");
                switch (ex.Message)
                {
                    case "errorListingOrders":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<ListOrderResponse>() { Status = 400, Message = $"Erro ao listar pedidos.", Success = false, Error = ex.Message });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<ListOrderResponse>() { Status = 500, Message = $"Internal server error!", Success = false, Error = ex.Message });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por listar detalhes do pedido realizado pelo consumidor
        /// </summary>
        /// <returns>Retorna pedido informado</returns>
        [Authorize]
        [HttpGet("details/{order_id}")]
        [ProducesResponseType(typeof(Response<OrderDetails>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<OrderDetails>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<OrderDetails>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<ListOrderResponse>> GetOrdersByConsumeretails(Guid order_id)
        {
            try
            {
                var response = _service.GetDetailsOrder(order_id);
                return StatusCode(StatusCodes.Status200OK, new Response<OrderDetails>() { Status = 200, Message = $"Pedidos listados com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while listing order!");
                switch (ex.Message)
                {
                    case "errorListingOrder":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<OrderDetails>() { Status = 400, Message = $"Erro ao listar pedidos. Verifique o numero do pedido informado.", Success = false, Error = ex.Message });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<OrderDetails>() { Status = 500, Message = $"Internal server error!", Success = false, Error = ex.Message });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por listar meios de pagamento e frete da filial
        /// </summary>
        /// <returns>Retorna opções de pagamento e frete</returns>
        [Authorize]
        [HttpGet("payment/{branch_id}")]
        [ProducesResponseType(typeof(Response<PaymentShippingOptions>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<PaymentShippingOptions>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<PaymentShippingOptions>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<PaymentShippingOptions>> GetPaymentAndShippingByBranchID([Required] Guid branch_id, [Required] string latitude, [Required] string longitude)
        {
            try
            {
                var response = _service.GetPaymentAndShippingByBranchID(branch_id, latitude, longitude);
                return StatusCode(StatusCodes.Status200OK, new Response<PaymentShippingOptions>() { Status = 200, Message = $"Opções e pagamento e frete listados com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while listing option payment and shipping!");
                switch (ex.Message)
                {
                    case "errorListingOptions":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<PaymentShippingOptions>() { Status = 400, Message = $"Erro ao listar opções de pagamento e frete. Verifique a filial informada.", Success = false, Error = ex.Message });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<PaymentShippingOptions>() { Status = 500, Message = $"Internal server error!", Success = false, Error = ex.Message });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por listar status
        /// </summary>
        /// <returns>Retorna todos os status</returns>
        [Authorize]
        [HttpGet("status")]
        [ProducesResponseType(typeof(Response<StatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<StatusResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<StatusResponse>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<StatusResponse>> GetStatus()
        {
            try
            {
                var response = _service.GetStatus();
                return StatusCode(StatusCodes.Status200OK, new Response<StatusResponse>() { Status = 200, Message = $"Status listados com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while listing status!");
                switch (ex.Message)
                {
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<StatusResponse>() { Status = 500, Message = $"Internal server error!", Success = false, Error = ex.Message });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por alterar pedido
        /// </summary>
        /// <returns>Valida os dados passados para alteração do pedido e retorna os dados alterados</returns>
        [Authorize]
        [HttpPut("update")]
        [ProducesResponseType(typeof(Response<UpdateOrderRequest>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<UpdateOrderRequest>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<UpdateOrderRequest>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Response<Order>>> Update(UpdateOrderRequest orderRequest)
        {
            try
            {
                var response = await _service.Update(orderRequest);
                return StatusCode(StatusCodes.Status200OK, new Response<Order>() { Status = 200, Message = $"Pedido gerado com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while creating new Order!");
                switch (ex.Message)
                {
                    case "errorCreate":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<Order>() { Status = 400, Message = $"Erro ao alterar Pedido.", Success = false, Error = ex.Message });
                    case "orderNotCreated":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<Order>() { Status = 400, Message = $"Erro ao alterar Pedido.", Success = false, Error = ex.Message });
                    case "errorWhileUpdateOrderOnDB":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<Order>() { Status = 400, Message = $"Não foi possível registrar pedido. Erro no processo de alteração do pedido na base de dados.", Success = false, Error = ex.Message });
                    case "NonExistentBranch":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<Order>() { Status = 400, Message = $"Erro ao alterar Pedido. Não foi possivel encontrar filial.", Success = false, Error = ex.Message });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<Order>() { Status = 500, Message = $"Internal server error!", Success = false, Error = ex.Message });
                }
            }
        }
    }
}
