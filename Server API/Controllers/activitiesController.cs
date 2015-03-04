using Binbin.Linq;
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
    public class activitiesController : ApiController
    {
        private csci4950s15Entities db = new csci4950s15Entities();

        /// <summary>
        /// An Activity_API class to trim down the information and named types that are exposed to
        /// the web. This is better than making our schema directly available.
        /// </summary>
        public class Activity_API
        {
            public Activity_API()
            {
                tag_ids = new List<int>();
            }
            public void SetID(int id)
            {
                this.id = id;
            }
            public int id { get; private set; }
            [Required]
            public int user_id { get; set; }
            [Required, MaxLength(50)]
            public string name { get; set; }
            [Required]
            public byte category { get; set; }
            public List<int> tag_ids { get; set; }
        }

        // GET: api/activities
        public async Task<IQueryable<Activity_API>> Getactivities(int id = 0, int user_id = 0, string name = "", byte category = 0)
        {
            // Create the result set
            var activities = from act in db.activities
                             select act;

            // Filter by id
            if (id != 0)
                activities = activities.Where(p => p.id.Equals(id));

            // Filter by user_id
            if (user_id != 0)
                activities = activities.Where(p => p.user.Equals(user_id));

            // Filter by name, strict matching
            if (!String.IsNullOrEmpty(name))
                activities = activities.Where(p => p.name.Equals(name));

            // Filter by category
            if (category != 0)
                activities = activities.Where(p => p.category.Equals(category));

            // Convert the activities to more API friendly things
            List<Activity_API> results = new List<Activity_API>();
            List<activity> activitylist = await activities.ToListAsync();
            foreach (var act in activitylist)
            {
                var actRes = new Activity_API();
                actRes.SetID(act.id);
                actRes.user_id = act.user;
                actRes.name = act.name;
                actRes.category = act.category;
                // Magic to get just the IDs out of tag objects
                actRes.tag_ids = act.tags.Select(p => p.id).ToList();
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

            return StatusCode(HttpStatusCode.OK);
        }

        // POST: api/activities
        [ResponseType(typeof(Activity_API))]
        public async Task<IHttpActionResult> Postactivity(Activity_API Activity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify UserID exists
            if (await db.users.CountAsync(p => p.id.Equals(Activity.user_id)) != 1)
                return BadRequest("user_id does not exist");

            // Verify TagIDs exist
            foreach (int id in Activity.tag_ids)
                if (await db.tags.CountAsync(p => p.id.Equals(id)) != 1)
                    return BadRequest("Tag with id " + id.ToString() + " does not exist");

            // Get the tags referenced by this activity
            // http://stackoverflow.com/a/2101561
            var tags = from tg in db.tags
                       select tg;
            var tagsPredicate = PredicateBuilder.False<tag>();
            foreach (int id in Activity.tag_ids)
                tagsPredicate = tagsPredicate.Or(p => p.id.Equals(id));
            tags = tags.Where(tagsPredicate);

            // Convert the Activity_API to the EntityModel activity
            activity act = new activity();
            act.user = Activity.user_id;
            act.name = Activity.name;
            act.category = Activity.category;
            act.tags = await tags.ToListAsync();

            db.activities.Add(act);
            await db.SaveChangesAsync();

            Activity.SetID(act.id);

            return CreatedAtRoute("DefaultApi", new { id = Activity.id }, Activity);
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

            return Ok();
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