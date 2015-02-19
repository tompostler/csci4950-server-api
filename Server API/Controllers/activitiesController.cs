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

        public class ActivityResult
        {
            public int id { get; set; }
            public int user { get; set; }
            public string name { get; set; }
            public byte category { get; set; }
        }

        // GET: api/activities
        public IQueryable<ActivityResult> Getactivities(int id=0, int user=0, string name="", byte category=0)
        {
            // Create the result set
            var activities = from act in db.activities
                             select act;

            // Filter by id
            if (id != 0)
                activities = activities.Where(p => p.id.Equals(id));

            // Filter by user
            if (user != 0)
                activities = activities.Where(p => p.user.Equals(user));

            // Filter by name, strict matching
            if (!String.IsNullOrEmpty(name))
                activities = activities.Where(p => p.name.Equals(name));

            // Filter by category
            if (category != 0)
                activities = activities.Where(p => p.category.Equals(category));

            // Convert the activities to more API friendly things
            List<ActivityResult> results = new List<ActivityResult>();
            foreach (var act in activities)
            {
                var actRes = new ActivityResult();
                actRes.id = act.id;
                actRes.user = act.user;
                actRes.name = act.name;
                actRes.category = act.category;
                results.Add(actRes);
            }

            return results.AsQueryable();
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
        public async Task<IHttpActionResult> Postactivity(activity activity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.activities.Add(activity);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = activity.id }, activity);
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