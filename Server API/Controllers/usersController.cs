﻿using Newtonsoft.Json;
using Server_API.Auth;
using Server_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
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
            public int id { get; set; }
            [Required, StringLength(50)]
            public string fname { get; set; }
            [Required, StringLength(50)]
            public string lname { get; set; }
            [Required, StringLength(50), EmailAddress]
            public string email { get; set; }
            [Required, StringLength(128)]
            public string password { get; set; }

            public List<int> activity_ids { get; set; }
            public List<int> location_ids { get; set; }
            public List<byte> custom_tags { get; set; }
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
        public async Task<IHttpActionResult> Putuser(int id, User_API User)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify request ID
            if (id != User.id)
                return BadRequest("PUT URL and ID in the location do not match");

            // Convert the User_API to the EntityModel location
            user usr = await ConvertUserApiToUser(User);

            // Update the user
            db.Entry(usr).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(User);
        }

        // POST: api/users
        public async Task<IHttpActionResult> Postuser(User_API User)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Convert the User_API to the EntityModel user
            user usr = await ConvertUserApiToUser(User);

            // Add the user to the DB
            db.users.Add(usr);
            await db.SaveChangesAsync();

            // Update the ID with the one that was auto-assigned
            User.id = usr.id;

            return CreatedAtRoute("DefaultApi", new { id = User.id }, User);
        }

        // DELETE: api/users/5
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
        private async Task<user> ConvertUserApiToUser(User_API User)
        {
            // Convert our API type into the representing Model
            user usr = new user();
            usr.id = User.id;
            usr.fname = User.fname;
            usr.lname = User.lname;
            usr.email = User.email;
            usr.password = User.password;

            // Get the activities referenced by this user
            var activities = from act in db.activities
                             select act;
            activities = activities.Where(p => p.user_id.Equals(User.id));

            // Get the locations referenced by this user
            var locations = from loc in db.locations
                            select loc;
            locations = locations.Where(p => p.user_id.Equals(User.id));

            // Get the custom tags referenced by this user
            var custom_tags = from tags in db.tags_users
                              select tags;
            custom_tags = custom_tags.Where(p => p.user_id.Equals(User.id));

            // Assemble the lists
            usr.activities = await activities.ToListAsync();
            usr.locations = await locations.ToListAsync();
            usr.tags_users = await custom_tags.ToListAsync();

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
            usr.id = User.id;
            usr.fname = User.fname;
            usr.lname = User.lname;
            usr.email = User.email;
            usr.password = User.password;

            // Get the lists of ids for the corresponding types
            usr.activity_ids = User.activities.Select(p => p.id).ToList();
            usr.location_ids = User.locations.Select(p => p.id).ToList();
            usr.custom_tags = User.tags_users.Select(p => p.tag_id).ToList();

            return usr;
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