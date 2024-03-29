﻿using Newtonsoft.Json;
using Server_API.Auth;
using Server_API.Filters;
using Server_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server_API.Controllers
{
    [RequireHttps]
    public class usersController : ApiController
    {
        private csci4950s15Model db = new csci4950s15Model();

        /// <summary>
        /// A User_API class to trim down the information and named types that are exposed to the 
        /// web. This is better than making our schema directly available.
        /// </summary>
        public class User_API
        {
            public User_API()
            {
                activity_ids = new List<int>();
                location_ids = new List<int>();
            }

            public int id { get; set; }

            [Required, StringLength(50)]
            public string fname { get; set; }

            [Required, StringLength(50)]
            public string lname { get; set; }

            [Required, StringLength(50), EmailAddress]
            public string email { get; set; }

            [Required]
            public string password { get; set; }

            public DateTime mdate { get; set; }

            public List<int> activity_ids { get; set; }

            public List<int> location_ids { get; set; }
        }

        // GET: api/users
        public async Task<IHttpActionResult> Get()
        {
            // Verify token
            int tok_id = AuthorizeHeader.VerifyToken(ActionContext);
            if (tok_id <= 0)
                return BadRequest(AuthorizeHeader.InvalidTokenToMessage(tok_id));
            
            // Allow only querying the token's user_id
            user usr = await db.users.FindAsync(tok_id);
            if (usr == null)
                return NotFound();
            else
                return Ok(ConvertUserToUserApi(usr));
        }

        // PUT: api/users/5
        [ValidateViewModel]
        public async Task<IHttpActionResult> Put(int id, User_API User)
        {
            // Verify request ID
            if (id != User.id)
                return BadRequest("PUT URL and ID in the location do not match");

            // Verify token
            string msg = AuthorizeHeader.VerifyTokenWithUserId(ActionContext, id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

            // Convert the User_API to the EntityModel location
            user usr = ConvertUserApiToUser(User);

            // Update the user
            db.Entry(usr).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/users
        [ValidateViewModel]
        public async Task<IHttpActionResult> Post(User_API User)
        {
            // Convert the User_API to the EntityModel user
            user usr = ConvertUserApiToUser(User);

            // Add the user to the DB
            db.users.Add(usr);
            await db.SaveChangesAsync();

            // Update the ID with the one that was auto-assigned
            User = ConvertUserToUserApi(usr);

            return Ok(User);
        }

        // DELETE: api/users/5
        public async Task<IHttpActionResult> Delete(int id)
        {
            user user = await db.users.FindAsync(id);
            if (user == null)
                return NotFound();

            // Verify token
            string msg = AuthorizeHeader.VerifyTokenWithUserId(ActionContext, id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

            db.users.Remove(user);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);

        }

        // OPTIONS: api/users
        public IHttpActionResult Options()
        {
            return Ok();
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
            usr.fname = User.fname;
            usr.lname = User.lname;
            usr.email = User.email;
            usr.password = Hashing.HashPassword(User.password);
            usr.mdate = Util.UtcDateTimeInMilliseconds();

            return usr;
        }

        /// <summary>
        /// Converts an EntityModel user to an User_API.
        /// </summary>
        /// <param name="User">The EntityModel user to convert.</param>
        /// <returns>An User_API corresponding to the EntityModel user.</returns>
        internal static User_API ConvertUserToUserApi(user User)
        {
            // Convert EntityModel type to our API type
            User_API usr = new User_API();
            usr.id = User.id;
            usr.fname = User.fname;
            usr.lname = User.lname;
            usr.email = User.email;
            usr.password = null;
            usr.mdate = User.mdate;

            // Get the lists of ids for the corresponding types
            usr.activity_ids = User.activities.Select(p => p.id).ToList();
            usr.location_ids = User.locations.Select(p => p.id).ToList();

            return usr;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}