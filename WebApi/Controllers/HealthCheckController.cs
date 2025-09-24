using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Domain.Model.Response;

namespace WebApi.Controllers
{
    [Route("healthcheck")]
  [ApiController]
  public class HealthCheckController : Controller
  {
    /// <summary>
    /// Endpoint responsável por retornar se o Microserviço está ativo.
    /// </summary>
    /// <returns>Retorna "true" caso o microserviço esteja ativo.</returns>
    /// 
    [HttpGet("")]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
    public ActionResult<Response<bool>> HealthCheck()
    {
      return Ok(new Response<bool>()
      {
        Status = 200,
        Message = "Service alive",
        Success = true,
        Data = true
      });
    }


  }
}
