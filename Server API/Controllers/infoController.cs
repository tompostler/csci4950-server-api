﻿using System.Collections.Generic;
using System.Web.Http;

namespace Server_API.Controllers
{
    public class infoController : ApiController
    {
        public class Info_API
        {
            public class Contact
            {
                public string name { get; set; }
                public string email { get; set; }
            }

            public string version { get; set; }
            
            public string version_date { get; set; }
            
            public List<Contact> contacts { get; set; }
            
            public string hostname { get; set; }
            
            public List<string> root_nodes { get; set; }
            
            public string fun_fact { get; set; }
        }

        public IHttpActionResult Get()
        {
            Info_API info = new Info_API
            {
                version = "3.05.0000",
                version_date = "2015-04-23",
                contacts = new List<Info_API.Contact>
                {
                    new Info_API.Contact
                    {
                        name = "Tom Postler",
                        email = "tcp@umn.edu"
                    },
                    new Info_API.Contact
                    {
                        name = "Anthony Nixon",
                        email = "nixon069@umn.edu"
                    }
                },
                hostname = "msfrizzle.me/api",
                root_nodes = new List<string>
                {
                    "activities",
                    "activityunits",
                    "auth",
                    "courses",
                    "info",
                    "locations",
                    "settings",
                    "tags",
                    "users"
                },
                fun_fact = "Various studies have shown that coffee makes you live longer, die sooner, prevent cancer, cause cancer, and prevent diabetes."
            };

            return Ok(info);
        }

        // OPTIONS: api/info
        public IHttpActionResult Options()
        {
            return Ok();
        }
    }
}
