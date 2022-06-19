﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace AutoLot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet("error")]
        public IActionResult Error()
        {
            return NotFound();
        }


        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(400, "The request was invalid")]
        public IActionResult Get()
        {
            return Ok(new string[] { "value1", "value2" });
        }

        /// <summary>
        /// This is an example Get method returning JSON
        /// </summary>
        /// <remarks>This is one of several examples for returning JSON:
        /// <pre>
        /// [
        /// "value1",
        /// "value2"
        /// ]
        /// </pre>
        /// </remarks>
        /// <returns>List of strings</returns>
        [HttpGet("one")]
        public IEnumerable<string> Get1()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("two")]
        public ActionResult<IEnumerable<string>> Get2()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("three")]
        public string[] Get3()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("four")]
        public IActionResult Get4()
        {
            return new JsonResult(new string[] { "value1", "value2" });
        }
    }
}
