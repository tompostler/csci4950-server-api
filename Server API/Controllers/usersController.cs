using Newtonsoft.Json;
using Server_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        /// A User_API class to trim down the information and named types that are exposed to the 
        /// web. This is better than making our schema directly available.
        /// </summary>
        /// <remarks>
        /// The data annotations allow the ApiController to use its built-in validation techniques
        /// to validate POST and PUT data. Sweet.
        /// </remarks>
        public class User_API
        {
            public User_API(int id=0)
            {
                this.id = id;
            }
            public int id { get; private set; }
            [Required, MaxLength(50)]
            public string fname { get; set; }
            [Required, MaxLength(50)]
            public string lname { get; set; }
            [Required, MaxLength(50), EmailAddress]
            public string email { get; set; }
            [Required, MaxLength(50)]
            public string password { get; set; }
        }

        // GET: api/users
        public async Task<IQueryable<User_API>> Getusers(int id=0, string email="")
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
            List<User_API> results = new List<User_API>();
            List<user> userlist = await users.ToListAsync();
            foreach (user usr in userlist)
            {
                var usrRes = new User_API(usr.id);
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
        public async Task<HttpResponseMessage> Postuser(User_API post)
        {
            if (!ModelState.IsValid)
                return failMsg(JsonConvert.SerializeObject(ModelState));

            // Convert our API type into the representing Model
            user usr = new user();
            usr.first_name = post.fname;
            usr.last_name = post.lname;
            usr.email = post.email;
            usr.password = post.password;
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

        protected HttpResponseMessage failMsg(string desc = null)
        {
            string json = "\"success\":false";
            if (!String.IsNullOrEmpty(desc))
                json += String.Format(",\"description\":{0}", desc);
            json = "{" + json + "}";
            var response = this.Request.CreateResponse(HttpStatusCode.Forbidden);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        protected HttpResponseMessage goodMsg(int id = 0)
        {
            string json = "\"success\":true";
            if (id != 0)
                json += String.Format(",\"id\":{0}", id);
            json = "{" + json + "}";
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