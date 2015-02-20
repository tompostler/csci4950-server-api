using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Server_API.Controllers
{
    public class infoController : ApiController
    {
        private string json = @"{
    ""version"": 0.1,
    ""version_date"": ""2014-02-19"",
    ""contacts"": [
        {
            ""name"": ""Tom Postler"",
            ""email"": ""tcp@umn.edu""
        }
    ],
    ""hostname"": ""tenunreals-t03.cloudapp.net/api"",
    ""root_nodes"": [
        ""activities"",
        ""activity_units"",
        ""locations"",
        ""users""
    ],
    ""comment"": ""Currently only supporting GET requests""
}";

        public HttpResponseMessage Get()
        {
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }
    }
}
