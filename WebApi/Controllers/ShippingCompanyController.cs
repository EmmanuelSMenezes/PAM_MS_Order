using Application.Service;
using Domain.Model;
using Domain.Model.Request;
using Domain.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Route("shippingcompany")]
    [ApiController]
    public class ShippingCompanyController : Controller
    {
        private readonly IShippingCompanyService _service;
        private readonly ILogger _logger;

        public ShippingCompanyController(IShippingCompanyService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint responsável por criar uma transportadora
        /// </summary>
        /// <returns>Valida os dados passados para criação da transportadora e retorna os dados cadastrados</returns>
        //[Authorize]
        [AllowAnonymous]
        [HttpPost("create")]
        [ProducesResponseType(typeof(Response<ShippingCompany>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<ShippingCompany>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<ShippingCompany>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<ShippingCompany>> Create(ShippingCompanyRequest shippingCompanyRequest)
        {
            try
            {
                var response = _service.Create(shippingCompanyRequest);
                return StatusCode(StatusCodes.Status201Created, new Response<ShippingCompanyResponse>() { Status = 201, Message = $"Transportadora cadastrada com sucesso.", Data = response, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while creating new partner!");
                switch (ex.Message)
                {
                    case "errorCreate":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<ShippingCompanyResponse>() { Status = 403, Message = $"Erro ao cadastrar nova Transportadora.", Success = false });
                    case "shippingCompanyNotCreated":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<ShippingCompanyResponse>() { Status = 403, Message = $"Erro ao cadastrar nova Transportadora.", Success = false });
                    case "errorWhileInsertShippingCompanyOnDB":
                        return StatusCode(StatusCodes.Status304NotModified, new Response<ShippingCompanyResponse>() { Status = 304, Message = $"Não foi possível registrar nova Transportadora. Erro no processo de inserção da nova Transportadora na base de dados.", Success = false });
                    case "errorWhileDeleteShippingCompanyOnDB":
                        return StatusCode(StatusCodes.Status304NotModified, new Response<ShippingCompanyResponse>() { Status = 304, Message = $"Não foi possível deletar o nova Transportadora. Erro no processo de deleção da nova Transportadora na base de dados.", Success = false });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<ShippingCompanyResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por atualizar o cadastro de uma transportadora
        /// </summary>
        /// <returns>Retorna o objeto que representa a transportadora em caso de sucesso</returns>
        //[Authorize]
        [AllowAnonymous]
        [HttpPut("update")]
        [ProducesResponseType(typeof(Response<ShippingCompany>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<ShippingCompany>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<ShippingCompany>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<ShippingCompany>> Update(ShippingCompany company)
        {
            if (company.Shipping_company_id == Guid.Empty)
                return BadRequest(new Response<ShippingCompany>() { Status = 400, Message = "Id não informado", Success = false });

            try
            {
                var response = _service.Update(company);

                if (response != null && !response.created)
                {
                    _logger.Information($"Transportadora atualizado com sucesso.");
                    return StatusCode(StatusCodes.Status200OK, new Response<ShippingCompany>() { Status = 200, Message = $"Parceiro atualizado com sucesso", Success = true });
                }
                else
                {
                    _logger.Warning($"Não foi possível atualizar a Transportadora.");
                    return BadRequest(new Response<ShippingCompany>() { Status = 400, Message = $"Não foi possível atualizar a Transportadora.", Success = false });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while updateing shipping company!");
                switch (ex.Message)
                {
                    case "shippingCompanyNotCreated":
                        return StatusCode(StatusCodes.Status204NoContent, new Response<ShippingCompanyResponse>() { Status = 304, Message = $"Não foi possível localizar a Tarnsportadora na base de dados.", Success = false });
                    case "errorWhileUpdateShippingCompanyOnDB":
                        return StatusCode(StatusCodes.Status304NotModified, new Response<ShippingCompanyResponse>() { Status = 304, Message = $"Não foi possível atualizar a Tarnsportadora na base de dados.", Success = false });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<ShippingCompanyResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por excluir uma ou mais transportadoras
        /// </summary>
        /// <returns>Valida os dados passados para deleção das transportadoras</returns>
        //[Authorize]
        [AllowAnonymous]
        [HttpDelete("delete")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<bool>> Delete(List<Guid> id)
        {
            try
            {
                var response = _service.Delete(id);
                return StatusCode(StatusCodes.Status200OK, new Response<ShippingCompanyResponse>() { Status = 200, Message = $"Transportadora excluída com sucesso.", Success = true });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while deleting shipping company!");
                switch (ex.Message)
                {
                    case "errorCreate":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<ShippingCompanyResponse>() { Status = 403, Message = $"Erro ao cadastrar nova Transportadora.", Success = false });
                    case "shippingCompanyNotCreated":
                        return StatusCode(StatusCodes.Status400BadRequest, new Response<ShippingCompanyResponse>() { Status = 403, Message = $"Erro ao cadastrar nova Transportadora.", Success = false });
                    case "errorWhileInsertShippingCompanyOnDB":
                        return StatusCode(StatusCodes.Status304NotModified, new Response<ShippingCompanyResponse>() { Status = 304, Message = $"Não foi possível cadastrar Transportadora. Erro no processo de inserção da transportadora na base de dados.", Success = false });
                    case "errorWhileDeleteShippingCompanyOnDB":
                        return StatusCode(StatusCodes.Status304NotModified, new Response<ShippingCompanyResponse>() { Status = 304, Message = $"Não foi possível deletar a Transportadora. Erro no processo de deleção da transportadora na base de dados.", Success = false });
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<ShippingCompanyResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false });
                }
            }
        }

        /// <summary>
        /// Endpoint responsável por retornar lista de transportadora cadastradas
        /// </summary>
        /// <returns>Em caso de sucesso, irá listar os transportadora cadastradas</returns>
        //[Authorize]
        [AllowAnonymous]
        [HttpGet("")]
        [ProducesResponseType(typeof(Response<ShippingCompany>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<>), StatusCodes.Status500InternalServerError)]
        public ActionResult<Response<List<ShippingCompany>>> GetShippingCompanies()
        {
            try
            {
                var response = _service.GetShippingCompanies();
                if (response.Count > 0)
                {
                    _logger.Information("Transportadoras listadas com sucesso!");
                    return Ok(new Response<List<ShippingCompany>>() { Status = 200, Message = "Transportadoras listadas com sucesso!", Data = response, Success = true });
                }
                else
                {
                    _logger.Information("Lista de Transportadoras vazia!");
                    return Ok(new Response<List<ShippingCompany>>() { Status = 401, Message = "Transportadora não encontrada no sistema!", Success = false });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception white listing shipping companies!");
                switch (ex.Message)
                {
                    case "errorWhileListingShippingCompaniesOnDB":
                        return StatusCode(StatusCodes.Status304NotModified, new Response<ShippingCompanyResponse>() { Status = 304, Message = $"Erro ao listar Transportadoras na base de dados.", Success = false });

                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response<ShippingCompanyResponse>() { Status = 500, Message = $"Internal server error! Exception Detail: {ex.Message}", Success = false });
                }
                throw;
            }
        }
    }
}
