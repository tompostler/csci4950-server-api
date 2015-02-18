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
    public class activitiesController : ApiController
    {
        private csci4950s15Entities db = new csci4950s15Entities();

        // GET: api/activities
        public IQueryable<activity> Getactivities(int user=0, string name="", byte category=0)
        {
            // Create the result set
            var activities = from act in db.activities
                             select act;

            // Filter by user
            if (user != 0)
                activities = activities.Where(p => p.user.Equals(user));

            // Filter by name, strict matching
            if (!String.IsNullOrEmpty(name))
                activities = activities.Where(p => p.name.Equals(name));

            // Filter by category
            if (category != 0)
                activities = activities.Where(p => p.category.Equals(category));

            return db.activities;
        }

        // GET: api/activities/5
        [ResponseType(typeof(activity))]
        public async Task<IHttpActionResult> Getactivity(int id)
        {
            activity activity = await db.activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            return Ok(activity);
        }

        // PUT: api/activities/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putactivity(int id, activity activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != activity.id)
            {
                return BadRequest();
            }

            db.Entry(activity).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!activityExists(id))
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

        // POST: api/activities
        [ResponseType(typeof(activity))]
        public async Task<IHttpActionResult> Postactivity()
        {
            activity act1 = new activity();
            act1.id = 3;
            act1.name = "Testing the API";
            act1.user = 2;
            act1.category = 1;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

<<<<<<< .merge_file_a08912
            db.activities.Add(act1);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (activityExists(act1.id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
=======
            db.activities.Add(activity);
            await db.SaveChangesAsync();
>>>>>>> .merge_file_a10068

            return CreatedAtRoute("DefaultApi", new { id = act1.id }, act1);
        }

        // DELETE: api/activities/5
        [ResponseType(typeof(activity))]
        public async Task<IHttpActionResult> Deleteactivity(int id)
        {
            activity activity = await db.activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            db.activities.Remove(activity);
            await db.SaveChangesAsync();

            return Ok(activity);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool activityExists(int id)
        {
            return db.activities.Count(e => e.id == id) > 0;
        }
    }
}