using System.Collections.Generic;
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
            public string comment { get; set; }
            public string fun_fact { get; set; }
        }

        public IHttpActionResult Get()
        {
            Info_API info = new Info_API
            {
                version = "2.04.0000",
                version_date = "2015-03-25",
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
                    "locations",
                    "tags",
                    "users"
                },
                comment = "Read the documentation. . . Now with HTTPS!",
                fun_fact = "There is a volcano in Indonesia that spews blue lava."
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
