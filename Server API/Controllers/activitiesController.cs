using Binbin.Linq;
using Newtonsoft.Json;
using Server_API.Auth;
using Server_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Server_API.Controllers
{
    public class activitiesController : ApiController
    {
        private csci4950s15Model db = new csci4950s15Model();

        /// <summary>
        /// An Activity_API class to trim down the information and named types that are exposed to
        /// the web. This is better than making our schema directly available.
        /// </summary>
        public class Activity_API
        {
            public Activity_API()
            {
                tag_ids = new List<byte>();
                activityunit_ids = new List<long>();
            }

            public int id { get; set; }

            [Required]
            public int user_id { get; set; }

            [Required, StringLength(50)]
            public string name { get; set; }

            [StringLength(100)]
            public string description { get; set; }

            public List<byte> tag_ids { get; set; }

            public List<long> activityunit_ids { get; set; }
        }

        // GET: api/activities
        public async Task<IHttpActionResult> Getactivities(int id = 0, int user_id = 0, string name = "")
        {
            // If we have an ID to search by, handle it
            if (id != 0)
            {
                activity act = await db.activities.FindAsync(id);
                if (act == null)
                    return NotFound();
                else
                    return Ok(ConvertActivityToActivityApi(act));
            }

            // Create the result set
            IQueryable<activity> activities = from act in db.activities
                                              select act;

            // Filter by user_id
            if (user_id != 0)
                activities = activities.Where(p => p.user.Equals(user_id));

            // Filter by name, strict matching
            if (!String.IsNullOrEmpty(name))
                activities = activities.Where(p => p.name.Equals(name));

            // Convert the activities to more API friendly things
            List<Activity_API> results = new List<Activity_API>();
            List<activity> activitylist = await activities.ToListAsync();
            foreach (var act in activitylist)
                results.Add(ConvertActivityToActivityApi(act));

            if (results.Count == 0)
                return NotFound();
            else
                return Ok(results);
        }

        // PUT: api/activities/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putactivity(int id, Activity_API Activity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify request ID
            if (id != Activity.id)
                return BadRequest("PUT URL and ID in the activity do not match");

            // Convert the Activity_API to the EntityModel activity
            activity act = await ConvertActivityApiToActivity(Activity);

            // Update the activity
            db.Entry(act).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(Activity);
        }

        // POST: api/activities
        [ResponseType(typeof(Activity_API))]
        public async Task<IHttpActionResult> Postactivity(Activity_API Activity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Convert the Activity_API to the EntityModel activity
            activity act = await ConvertActivityApiToActivity(Activity);

            // Add the activity to the DB
            db.activities.Add(act);
            await db.SaveChangesAsync();

            // Update the ID with the one that was auto-assigned
            Activity.id = act.id;

            return CreatedAtRoute("DefaultApi", new { id = Activity.id }, Activity);
        }

        // DELETE: api/activities/5
        [ResponseType(typeof(void))]
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

        // OPTIONS: api/activities
        [RequireHttps]
        public IHttpActionResult Options()
        {
            return Ok();
        }

        /// <summary>
        /// Converts an Activity_API to an EntityModel activity.
        /// </summary>
        /// <param name="Activity">The Activity_API to convert.</param>
        /// <returns>An EntityModel activity corresponding to the Activity_API.</returns>
        private async Task<activity> ConvertActivityApiToActivity(Activity_API Activity)
        {
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
            act.id = Activity.id;
            act.user_id = Activity.user_id;
            act.name = Activity.name;
            act.tags = await tags.ToListAsync();

            return act;
        }

        /// <summary>
        /// Converts an EntityModel activity to an Activity_API.
        /// </summary>
        /// <param name="Activity">The EntityModel activity to convert.</param>
        /// <returns>An Activity_API corresponding to the EntityModel activity.</returns>
        private Activity_API ConvertActivityToActivityApi(activity Activity)
        {
            // Convert the EntityModel type to our API type
            Activity_API act = new Activity_API();
            act.id = Activity.id;
            act.user_id = Activity.user_id;
            act.name = Activity.name;
            // Magic to get just the IDs out of tag objects
            act.tag_ids = Activity.tags.Select(p => p.id).ToList();

            return act;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}