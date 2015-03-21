using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Server_API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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
                version = "2.01.0000",
                version_date = "2015-03-21",
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
                hostname = "frizzle.cloudapp.net/api",
                root_nodes = new List<string>
                {
                    "activities",
                    "activityunits",
                    "locations",
                    "tags",
                    "users"
                },
                comment = "Read the documentation",
                fun_fact = "'Pentheraphobia' is a fear of a mother-in-law."
            };

            return Ok(info);
        }
    }
}
