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
    public class activityunitsController : ApiController
    {
        private csci4950s15Entities db = new csci4950s15Entities();

        /// <summary>
        /// An ActivityUnit_API class to trim down the information and named types that are
        /// exposed to the web. This is better than making our schema directly available.
        /// </summary>
        public class ActivityUnit_API
        {
            public ActivityUnit_API()
            {
                tag_ids = new List<int>();
            }
            public void SetID(long id)
            {
                this.id = id;
            }
            public long id { get; private set; }
            [Required]
            public int activity_id { get; set; }
            [Required]
            public int location_id { get; set; }
            [Required]
            public DateTime stime { get; set; }
            [Required]
            public DateTime etime { get; set; }
            public List<int> tag_ids { get; set; }
        }

        // GET: api/activityunit
        public async Task<IHttpActionResult> Getactivityunit(int id = 0, int activity_id = 0, int location_id = 0, DateTime? stime = null, DateTime? etime = null)
        {
            // If we have an ID to search by, handle it
            if (id != 0)
            {
                activityunit acu = await db.activityunits.FindAsync(id);
                if (acu == null)
                    return NotFound();
                else
                    return Ok(ConvertActivityUnitToActivityUnitApi(acu));
            }

            // Create the result set
            IQueryable<activityunit> activityunit = from au in db.activityunits
                                                        select au;

            // Filter on activity_id
            if (activity_id != 0)
                activityunit = activityunit.Where(p => p.activity_id.Equals(activity_id));

            // Filter on location_id
            if (location_id != 0)
                activityunit = activityunit.Where(p => p.location.Equals(location_id));

            // Filter on stime
            if (stime != null)
                activityunit = activityunit.Where(p => p.stime.Equals(stime.Value.ToUniversalTime()));

            // Filter on etime
            if (etime != null)
                activityunit = activityunit.Where(p => p.etime.Equals(etime.Value.ToUniversalTime()));

            // Convert the activityunit to more API friendly things
            List<ActivityUnit_API> results = new List<ActivityUnit_API>();
            List<activityunit> activityunitlist = await activityunit.ToListAsync();
            foreach (var acu in activityunitlist)
                results.Add(ConvertActivityUnitToActivityUnitApi(acu));

            if (results.Count == 0)
                return NotFound();
            else
                return Ok(results);
        }

        // PUT: api/activityunit/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putactivityunit(int id, ActivityUnit_API ActivityUnit)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify the Activity Unit
            var verification = await VerifyActivityUnitAndID(ActivityUnit);
            if (verification != null)
                return verification;

            // Verify request ID
            if (id != ActivityUnit.id)
                return BadRequest("PUT URL and ID in the activity unit do not match");

            // Convert the ActivityUnit_API to the EntityModel activityunit
            activityunit acu = await ConvertActivityUnitApiToActivityUnit(ActivityUnit);

            // Update the activity unit
            db.Entry(acu).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(ActivityUnit);
        }

        // POST: api/activityunit
        [ResponseType(typeof(ActivityUnit_API))]
        public async Task<IHttpActionResult> Postactivityunit(ActivityUnit_API ActivityUnit)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify the ActivityUnit
            var verification = await VerifyActivityUnit(ActivityUnit);
            if (verification != null)
                return verification;

            // Convert the ActivityUnit_API to the EntityModel activityunit
            activityunit acu = await ConvertActivityUnitApiToActivityUnit(ActivityUnit);

            // Add the activity unit to the DB
            db.activityunits.Add(acu);
            await db.SaveChangesAsync();

            // Update the ID with the one that was auto-assigned
            ActivityUnit.SetID(acu.id);

            return CreatedAtRoute("DefaultApi", new { id = ActivityUnit.id }, ActivityUnit);
        }

        // DELETE: api/activityunit/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Deleteactivityunit(int id)
        {
            activityunit activityunit = await db.activityunits.FindAsync(id);
            if (activityunit == null)
            {
                return NotFound();
            }

            db.activityunits.Remove(activityunit);
            await db.SaveChangesAsync();

            return Ok();
        }

        // OPTIONS: api/activityunits
        [RequireHttps]
        public IHttpActionResult Options()
        {
            return Ok();
        }


        /// <summary>
        /// Converts an ActivityUnit_API to an EntityModel activityunit.
        /// </summary>
        /// <param name="ActivityUnit">The ActivityUnit_API to convert.</param>
        /// <returns>An EntityModel activityunit corresponding to the ActivityUnit_API.</returns>
        private async Task<activityunit> ConvertActivityUnitApiToActivityUnit(ActivityUnit_API ActivityUnit)
        {
            // Get the tags referenced by this activity to do a proper insertion with the WebAPI
            // http://stackoverflow.com/a/2101561
            var tags = from tg in db.tags
                       select tg;
            var tagsPredicate = PredicateBuilder.False<tag>();
            foreach (int id in ActivityUnit.tag_ids)
                tagsPredicate = tagsPredicate.Or(p => p.id.Equals(id));
            tags = tags.Where(tagsPredicate);

            // Convert our API type into the representing Model
            activityunit acu = new activityunit();
            acu.id = ActivityUnit.id;
            acu.activity_id = ActivityUnit.activity_id;
            acu.location_id = ActivityUnit.location_id;
            acu.stime = ActivityUnit.stime;
            acu.etime = ActivityUnit.etime;
            acu.tags = await tags.ToListAsync();

            return acu;
        }


        /// <summary>
        /// Converts an EntityModel activityunit to an ActivityUnit_API.
        /// </summary>
        /// <param name="ActivityUnit">The EntityModel activityunit to convert.</param>
        /// <returns>An ActivityUnit_API corresponding to the EntityModel activityunit.</returns>
        private ActivityUnit_API ConvertActivityUnitToActivityUnitApi(activityunit ActivityUnit)
        {
            // Convert EntityModel type to our API type
            ActivityUnit_API acu = new ActivityUnit_API();
            acu.SetID(ActivityUnit.id);
            acu.activity_id = ActivityUnit.activity_id;
            acu.location_id = ActivityUnit.location_id;
            acu.stime = ActivityUnit.stime;
            acu.etime = ActivityUnit.etime;
            // Magic to get just the IDs out of tag objects
            acu.tag_ids = ActivityUnit.tags.Select(p => p.id).ToList().ConvertAll(x => (int)x);

            return acu;
        }

        /// <summary>
        /// Verifies the activity unit by checking that the ActivityID, LocationID, and TagIDs exist
        /// in addition to making sure stime <= etime.
        /// </summary>
        /// <param name="ActivityUnit">The activity unit to verify.</param>
        /// <returns>
        /// Null for success. The appropriate IHttpActionResult on failure.
        /// </returns>
        private async Task<IHttpActionResult> VerifyActivityUnit(ActivityUnit_API ActivityUnit)
        {
            // Verify ActivityID exists
            if (await db.activities.FindAsync(ActivityUnit.activity_id) == null)
                return BadRequest("activity_id does not exist");

            // Verify LocationID exists
            if (await db.locations.FindAsync(ActivityUnit.location_id) == null)
                return BadRequest("location_id does not exist");

            // Verify time range
            if (ActivityUnit.stime >= ActivityUnit.etime)
                return BadRequest("etime cannot be before stime");

            // Verify TagIDs exist
            foreach (int id in ActivityUnit.tag_ids)
                if (await db.tags.FindAsync(id) == null)
                    return BadRequest("Tag with id " + id.ToString() + " does not exist");

            return null;
        }

        /// <summary>
        /// Verifies the activity unit and the ID for the activity unit. This is more useful in PUT
        /// requests.
        /// </summary>
        /// <param name="ActivityUnit">The activity unit.</param>
        /// <returns>
        /// 404 if an ID is not found; the appropriate IHttpActionResult on failure; null on success.
        /// </returns>
        private async Task<IHttpActionResult> VerifyActivityUnitAndID(ActivityUnit_API ActivityUnit)
        {
            // Verify ID. Returns a 404 if not valid
            if (await db.activityunits.FindAsync(ActivityUnit.id) == null)
                return NotFound();

            return await VerifyActivityUnit(ActivityUnit);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool activityunitExists(int id)
        {
            return db.activityunits.Count(e => e.id == id) > 0;
        }
    }
}