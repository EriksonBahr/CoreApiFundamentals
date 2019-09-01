using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class OperationsController: ControllerBase
    {
        private readonly IConfiguration config;

        public OperationsController(IConfiguration config)
        {
            this.config = config;
        }

        [HttpOptions("reloadconfiguration")]
        public IActionResult ReloadConfig()
        {
            try
            {
                var rootConfig = (IConfigurationRoot)config;
                rootConfig.Reload();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}
