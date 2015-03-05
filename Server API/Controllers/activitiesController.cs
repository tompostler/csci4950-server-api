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
        public async Task<IHttpActionResult> Getactivities(int id = 0, int user_id = 0, string name = "", byte category = 0)
        {
            // If we have an ID to search by, handle it
            if (id != 0)
            {
                activity act = await db.activities.FindAsync(id);
                if (act == null)
                    return StatusCode(HttpStatusCode.NotFound);
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

            // Filter by category
            if (category != 0)
                activities = activities.Where(p => p.category.Equals(category));

            // Convert the activities to more API friendly things
            List<Activity_API> results = new List<Activity_API>();
            List<activity> activitylist = await activities.ToListAsync();
            foreach (var act in activitylist)
                results.Add(ConvertActivityToActivityApi(act));

            if (results.Count == 0)
                return StatusCode(HttpStatusCode.NotFound);
            else
                return Ok(results);
        }

        // PUT: api/activities/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putactivity(int id, Activity_API Activity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify the Activity
            var verification = await VerifyActivityAndID(Activity);
            if (verification != null)
                return verification;

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

            // Verify the Activity
            var verification = await VerifyActivity(Activity);
            if (verification != null)
                return verification;

            // Convert the Activity_API to the EntityModel activity
            activity act = await ConvertActivityApiToActivity(Activity);

            // Add the activity to the DB
            db.activities.Add(act);
            await db.SaveChangesAsync();

            // Update the ID with the one that was auto-assigned
            Activity.SetID(act.id);

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

            return StatusCode(HttpStatusCode.NoContent);
        }

        public HttpResponseMessage Options()
        {
            var response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            return response;
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
            act.user = Activity.user_id;
            act.name = Activity.name;
            act.category = Activity.category;
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
            act.SetID(Activity.id);
            act.user_id = Activity.user;
            act.name = Activity.name;
            act.category = Activity.category;
            // Magic to get just the IDs out of tag objects
            act.tag_ids = Activity.tags.Select(p => p.id).ToList();

            return act;
        }

        /// <summary>
        /// Verifies the activity by checking that the UserID and TagIDs exist.
        /// </summary>
        /// <param name="Activity">The activity to verify.</param>
        /// <returns>
        /// Null for success. The appropriate IHttpActionResult on failure.
        /// </returns>
        private async Task<IHttpActionResult> VerifyActivity(Activity_API Activity)
        {
            // Verify UserID exists
            if (await db.users.FindAsync(Activity.user_id) == null)
                return BadRequest("user_id does not exist");

            // Verify TagIDs exist
            foreach (int id in Activity.tag_ids)
                if (await db.tags.FindAsync(id) == null)
                    return BadRequest("Tag with id " + id.ToString() + " does not exist");

            return null;
        }

        /// <summary>
        /// Verifies the activity and the ID for the activity. This is more useful in PUT requests.
        /// </summary>
        /// <param name="Activity">The activity.</param>
        /// <returns>
        /// 404 if an ID is not found; the appropriate IHttpActionResult on failure; null on success.
        /// </returns>
        private async Task<IHttpActionResult> VerifyActivityAndID(Activity_API Activity)
        {
            // Verify ID. Returns a 404 if not valid
            if (await db.activities.FindAsync(Activity.id) == null)
                return StatusCode(HttpStatusCode.NotFound);

            return await VerifyActivity(Activity);
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