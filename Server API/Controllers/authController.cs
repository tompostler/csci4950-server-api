using Server_API.Auth;
using Server_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Server_API.Controllers
{
    [RequireHttps]
    public class authController : ApiController
    {
        private csci4950s15Model db = new csci4950s15Model();

        /// <summary>
        /// An Auth_API class to trim down the information and named types that are exposed to the
        /// web. This is better than making our schema directly available.
        /// </summary>
        public class Auth_API
        {
            public int id { get; set; }

            [Required, StringLength(64)]
            public string token { get; set; }

            public DateTime expire { get; set; }
        }
    }
}