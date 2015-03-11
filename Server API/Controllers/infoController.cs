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
                version = "1.01.0000",
                version_date = "2015-02-06",
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
                hostname = "ralphie.cloudapp.net/api",
                root_nodes = new List<string>
                {
                    "activities",
                    "activity_units",
                    "locations",
                    "tags",
                    "users"
                },
                comment = "Read the documentation",
                fun_fact = "At the peak of its popularity, the number of Farmville players outnumbered real farmers 60 to 1."
            };

            return Ok(info);
        }
    }
}
