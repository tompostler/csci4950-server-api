using Binbin.Linq;
using Newtonsoft.Json;
using Server_API.Auth;
using Server_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Server_API.Filters;

namespace Server_API.Controllers
{
    [RequireHttps]
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

            [StringLength(12)]
            public string course_id { get; set; }

            [Required, StringLength(50)]
            public string name { get; set; }

            [StringLength(100)]
            public string description { get; set; }

            public DateTime? ddate { get; set; }

            public float? eduration { get; set; }

            public float? pduration { get; set; }

            public byte? importance { get; set; }

            public DateTime mdate { get; set; }

            public List<byte> tag_ids { get; set; }

            public List<long> activityunit_ids { get; set; }
        }

        // GET: api/activities
        public async Task<IHttpActionResult> Get([FromUri] List<int> id = null, string course_id = "", string name = "", DateTime? ddate = null)
        {
            // Verify token
            int tok_id = AuthorizeHeader.VerifyToken(ActionContext);
            if (tok_id <= 0)
                return BadRequest(AuthorizeHeader.InvalidTokenToMessage(tok_id));

            // If we have IDs to search by, handle it
            if ((id != null) && (id.Count > 0))
            {
                IQueryable<activity> acts = from act in db.activities
                                            where act.user_id == tok_id
                                            where id.Contains(act.id)
                                            select act;

                // Get the results
                var actsResults = (await acts.ToListAsync()).ConvertAll(act => ConvertActivityToActivityApi(act));
                if (actsResults == null)
                    return NotFound();
                else if (actsResults.Count == 1)
                    return Ok(actsResults.FirstOrDefault());
                else
                    return Ok(actsResults);
            }

            // Create the result set
            IQueryable<activity> activities = from act in db.activities
                                              select act;

            // Filter by user_id
            activities = activities.Where(p => p.user_id.Equals(tok_id));

            // Filter by course_id
            if (!String.IsNullOrEmpty(course_id))
                activities = activities.Where(p => p.course_id.Equals(course_id));

            // Filter by name, strict matching
            if (!String.IsNullOrEmpty(name))
                activities = activities.Where(p => p.name.Equals(name));

            // Filter by ddate
            if (ddate != null)
                activities = activities.Where(p => p.ddate.Equals(ddate.Value));

            // Convert the activities to more API friendly things
            var results = (await activities.ToListAsync()).ConvertAll(act => ConvertActivityToActivityApi(act));

            if (results.Count == 0)
                return NotFound();
            else
                return Ok(results);
        }

        // PUT: api/activities/5
        [ValidateViewModel]
        public async Task<IHttpActionResult> Put(int id, Activity_API Activity)
        {
            // Verify request ID
            if (id != Activity.id)
                return BadRequest("PUT URL and ID in the activity do not match");

            // Verify token
            string msg = AuthorizeHeader.VerifyTokenWithUserId(ActionContext, Activity.user_id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

            // Convert the Activity_API to the EntityModel activity
            activity act = await ConvertActivityApiToActivity(Activity);

            // Update the activity
            db.Entry(act).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/activities
        [ValidateViewModel]
        public async Task<IHttpActionResult> Post(Activity_API Activity)
        {
            // Verify token
            string msg = AuthorizeHeader.VerifyTokenWithUserId(ActionContext, Activity.user_id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

            // Convert the Activity_API to the EntityModel activity
            activity act = await ConvertActivityApiToActivity(Activity);

            // Add the activity to the DB
            db.activities.Add(act);
            await db.SaveChangesAsync();

            // Update the ID and mdate with the one that was auto-assigned
            Activity.id = act.id;
            Activity.mdate = act.mdate;

            return Ok(Activity);
        }

        // DELETE: api/activities/5
        public async Task<IHttpActionResult> Delete(int id)
        {
            activity activity = await db.activities.FindAsync(id);
            if (activity == null)
                return NotFound();

            // Verify token
            string msg = AuthorizeHeader.VerifyTokenWithUserId(ActionContext, activity.user_id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

            db.activities.Remove(activity);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
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
            act.course_id = Activity.course_id;
            act.name = Activity.name;
            act.description = Activity.description;
            act.ddate = Activity.ddate;
            act.eduration = Activity.eduration;
            act.pduration = Activity.pduration;
            act.importance = Activity.importance;
            act.mdate = Util.UtcDateTimeInMilliseconds();
            act.tags = await tags.ToListAsync();

            return act;
        }

        /// <summary>
        /// Converts an EntityModel activity to an Activity_API.
        /// </summary>
        /// <param name="Activity">The EntityModel activity to convert.</param>
        /// <returns>An Activity_API corresponding to the EntityModel activity.</returns>
        internal static Activity_API ConvertActivityToActivityApi(activity Activity)
        {
            // Convert the EntityModel type to our API type
            Activity_API act = new Activity_API();
            act.id = Activity.id;
            act.user_id = Activity.user_id;
            act.course_id = Activity.course_id;
            act.name = Activity.name;
            act.description = Activity.description;
            act.ddate = Activity.ddate;
            act.eduration = Activity.eduration;
            act.pduration = Activity.pduration;
            act.importance = Activity.importance;
            act.mdate = Activity.mdate;

            // Magic to get just the IDs out of objects
            act.tag_ids = Activity.tags.Select(p => p.id).ToList();
            act.activityunit_ids = Activity.activityunits.Select(p => p.id).ToList();

            return act;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}