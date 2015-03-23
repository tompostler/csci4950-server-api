using Newtonsoft.Json;
using Server_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public class User_API
        {

            public void SetID(int id)
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
        public async Task<IHttpActionResult> Getusers(int id = 0, string email = "")
        {
            // If we have an ID to search by, handle it
            if (id != 0)
            {
                user usr = await db.users.FindAsync(id);
                if (usr == null)
                    return NotFound();
                else
                    return Ok(ConvertUserToUserApi(usr));
            }

            // Create the result set
            IQueryable<user> users = from u in db.users
                                     select u;

            // Filter by email
            if (!String.IsNullOrEmpty(email))
                users = users.Where(p => p.email.Equals(email));

            // Convert the users to more API friendly things
            List<User_API> results = new List<User_API>();
            List<user> userlist = await users.ToListAsync();
            foreach (user usr in userlist)
                results.Add(ConvertUserToUserApi(usr));

            if (results.Count == 0)
                return NotFound();
            else
                return Ok(results);
        }

        // PUT: api/users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putuser(int id, User_API User)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify the User
            var verification = await VerifyUserAndID(User);
            if (verification != null)
                return verification;

            // Verify request ID
            if (id != User.id)
                return BadRequest("PUT URL and ID in the location do not match");

            // Convert the User_API to the EntityModel location
            user usr = ConvertUserApiToUser(User);

            // Update the user
            db.Entry(usr).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(User);
        }

        // POST: api/users
        [ResponseType(typeof(User_API))]
        public async Task<IHttpActionResult> Postuser(User_API User)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Convert the User_API to the EntityModel user
            user usr = ConvertUserApiToUser(User);

            // Add the user to the DB
            db.users.Add(usr);
            await db.SaveChangesAsync();

            // Update the ID with the one that was auto-assigned
            User.SetID(usr.id);

            return CreatedAtRoute("DefaultApi", new { id = User.id }, User);
        }

        // DELETE: api/users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Deleteuser(int id)
        {
            user user = await db.users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.users.Remove(user);
            await db.SaveChangesAsync();

            return Ok();

        }

        public HttpResponseMessage Options()
        {
            var response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        /// <summary>
        /// Converts an User_API to an EntityModel user.
        /// </summary>
        /// <param name="User">The User_API to convert.</param>
        /// <returns>An EntityModel user corresponding to the User_API.</returns>
        private user ConvertUserApiToUser(User_API User)
        {
            // Convert our API type into the representing Model
            user usr = new user();
            usr.id = User.id;
            usr.first_name = User.fname;
            usr.last_name = User.lname;
            usr.email = User.email;
            usr.password = User.password;

            return usr;
        }

        /// <summary>
        /// Converts an EntityModel user to an User_API.
        /// </summary>
        /// <param name="User">The EntityModel user to convert.</param>
        /// <returns>An User_API corresponding to the EntityModel user.</returns>
        private User_API ConvertUserToUserApi(user User)
        {
            // Convert EntityModel type to our API type
            User_API usr = new User_API();
            usr.SetID(User.id);
            usr.fname = User.first_name;
            usr.lname = User.last_name;
            usr.email = User.email;
            usr.password = User.password;

            return usr;
        }

        /// <summary>
        /// Verifies the user and the ID for the user. This is more useful in PUT requests.
        /// </summary>
        /// <param name="User">The user.</param>
        /// <returns>
        /// 404 if an ID is not found; the appropriate IHttpActionResult on failure; null on success.
        /// </returns>
        private async Task<IHttpActionResult> VerifyUserAndID(User_API User)
        {
            // Verify ID. Returns a 404 if not valid
            if (await db.users.FindAsync(User.id) == null)
                return NotFound();

            return null;
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