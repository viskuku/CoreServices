using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoreServices.Configuration;
using CoreServices.Filter;
using CoreServices.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreServices.Controllers
{
    [Route("api/[controller]")]
    //[Route("api/[controller]/[action]")] uncomment to run two get method from same project
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IClientConfiguration clientConfiguration;

        public ValuesController(IClientConfiguration clientConfiguration)
        {
            this.clientConfiguration = clientConfiguration;
        }

        //// GET api/values
        //[ActionName("ClientConfig")]
        //[HttpGet]
        //public ActionResult<ClientConfiguration> GetClientConfig()
        //{
        //    //throw new NullReferenceException();
        //    return new ClientConfiguration { ClientName = clientConfiguration.ClientName, InvokedDateTime = clientConfiguration.InvokedDateTime };
        //}

        // GET api/values
        [ActionName("Value")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetValue()
        {
           // return new string[] { "value1", "value2" };
            throw new HttpResponseException() { Status = (int)HttpStatusCode.BadRequest, Value = "Testing" };

        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
