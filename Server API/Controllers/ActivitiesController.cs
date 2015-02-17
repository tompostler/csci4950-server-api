using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Server_API.Controllers
{
    public class ActivitiesController : ApiController
    {
        // GET: api/Activities
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Activities/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Activities
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Activities/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Activities/5
        public void Delete(int id)
        {
        }
    }
}
