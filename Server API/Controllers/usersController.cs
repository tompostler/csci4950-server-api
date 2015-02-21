using Server_API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Server_API.Controllers
{
    public class usersController : ApiController
    {
        private csci4950s15Entities db = new csci4950s15Entities();

        /// <summary>
        /// A UserResult class to trim down the information and named types that are exposed to the 
        /// web. This is better than making our schema directly available.
        /// </summary>
        public class UserResult
        {
            public int id { get; set; }
            public string fname { get; set; }
            public string lname { get; set; }
            public string email { get; set; }
            public string password { get; set; }
        }

        // GET: api/users
        public async Task<IQueryable<UserResult>> Getusers(int id=0, string email="")
        {
            // Create the result set
            IQueryable<user> users = from u in db.users
                                     select u;

            // Filter by id
            if (id != 0)
                users = users.Where(p => p.id.Equals(id));

            // Filter by email
            if (!String.IsNullOrEmpty(email))
                users = users.Where(p => p.email.Equals(email));

            // Convert the users to more API friendly things
            // By default, the web API we have generates a list of items
            //  connected by the FKs which fails horribly for some reason.
            // Ideally, we'd have a way to prevent that from being queried in
            //  the first place, but oh well for now.
            List<UserResult> results = new List<UserResult>();
            List<user> userlist = await users.ToListAsync();
            foreach (user usr in userlist)
            {
                var usrRes = new UserResult();
                usrRes.id = usr.id;
                usrRes.fname = usr.first_name;
                usrRes.lname = usr.last_name;
                usrRes.email = usr.email;
                usrRes.password = usr.password;
                results.Add(usrRes);
            }

            return results.AsQueryable();
        }

        // PUT: api/users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putuser(int id, user user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.id)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!userExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/users
        public async Task<HttpResponseMessage> Postuser([FromBody]dynamic body)
        {
            if (!ModelState.IsValid)
                return failMsg("Bad Model State");

            if (body == null)
                return failMsg("must have POST body");

            // Check to make sure they're not providing an ID
            int? id = body.Value<int?>("id");
            if (id.HasValue)
                return failMsg("cannot supply id in POST");

            // Grab all values out of POST
            string fname = body.Value<string>("fname");
            string lname = body.Value<string>("lname");
            string email = body.Value<string>("email");
            string password = body.Value<string>("password");

            // Validate fname
            if (String.IsNullOrEmpty(fname))
                return failMsg("must have fname");
            else if (fname.Length > 50)
                return failMsg("fname must be <50 characters");

            // Validate lname
            if (String.IsNullOrEmpty(lname))
                return failMsg("must have lname");
            else if (lname.Length > 50)
                return failMsg("lname must be <50 characters");

            // Validate email
            if (String.IsNullOrEmpty(email))
                return failMsg("must have email");
            else if (email.Length > 50)
                return failMsg("email must be <50 characters");

            // Validate password
            if (String.IsNullOrEmpty(password))
                return failMsg("must have password");
            else if (password.Length > 50)
                return failMsg("password must be <50 characters");

            user usr = new user();
            usr.first_name = fname;
            usr.last_name = lname;
            usr.email = email;
            usr.password = password;
            db.users.Add(usr);
            await db.SaveChangesAsync();

            return goodMsg(usr.id);
        }

        // DELETE: api/users/5
        [ResponseType(typeof(user))]
        public async Task<IHttpActionResult> Deleteuser(int id)
        {
            user user = await db.users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        protected HttpResponseMessage failMsg(string msg = null)
        {
            string json = "\"success\":false";
            if (!String.IsNullOrEmpty(msg))
                json += String.Format(",\"message\":\"{0}\"", msg.Replace("\"", "\"\""));
            json = "{" + json + "}";
            var response = this.Request.CreateResponse(HttpStatusCode.Forbidden);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        protected HttpResponseMessage goodMsg(int id)
        {
            string json = "{\"success\":true,\"id\":" + id.ToString() + "}";
            var response = this.Request.CreateResponse(HttpStatusCode.Created);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool userExists(int id)
        {
            return db.users.Count(e => e.id == id) > 0;
        }
    }
}