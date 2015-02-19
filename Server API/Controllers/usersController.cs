using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Server_API.Models;

namespace Server_API.Controllers
{
    public class usersController : ApiController
    {
        private csci4950s15Entities db = new csci4950s15Entities();

        /// <summary>
        /// A UserResult class to trim down the information and named types that
        /// are exposed to the web. This is better than making our schema
        /// directly available.
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
        public IQueryable<UserResult> Getusers(int id=0, string email="")
        {
            // Create the result set
            var users = from u in db.users
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
            foreach (var usr in users)
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
        [ResponseType(typeof(user))]
        public async Task<IHttpActionResult> Postuser(user user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.users.Add(user);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = user.id }, user);
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