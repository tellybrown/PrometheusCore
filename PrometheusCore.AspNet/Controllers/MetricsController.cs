using Microsoft.AspNetCore.Mvc;
using PrometheusCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PrometheusCore.AspNet.Controllers
{
    [Route("")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private readonly ICollectorFactory _factory;
        public MetricsController(ICollectorFactory factory)
        {
            _factory = factory;
        }

        [HttpGet("metrics")]
        public async Task<IActionResult> GetMetricsAsync()
        {
            try
            {
                await _factory.CollectAsync(HttpContext.Response.Body);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(503);
            }
           
        }
    }
}