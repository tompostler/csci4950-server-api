using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Server_API.Controllers
{
    public class infoController : ApiController
    {
        private string json = @"{
    ""version"": 0.5,
    ""version_date"": ""2015-02-28"",
    ""contacts"": [
        {
            ""name"": ""Tom Postler"",
            ""email"": ""tcp@umn.edu""
        },
        {
            ""name"": ""Anthony Nixon"",
            ""email"": ""nixon069@umn.edu""
        }
    ],
    ""hostname"": ""ralphie.cloudapp.net/api"",
    ""root_nodes"": [
        ""activities"",
        ""activity_units"",
        ""locations"",
        ""tags"",
        ""users""
    ],
    ""comment"": ""GET, POST, DELETE for all""
}";

        public HttpResponseMessage Get()
        {
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }
    }
}
