using Binbin.Linq;
using Newtonsoft.Json;
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
    public class activityunitsController : ApiController
    {
        private csci4950s15Model db = new csci4950s15Model();

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
            public long id { get; set; }
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
        public async Task<IHttpActionResult> Putactivityunit(int id, ActivityUnit_API ActivityUnit)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
        public async Task<IHttpActionResult> Postactivityunit(ActivityUnit_API ActivityUnit)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Convert the ActivityUnit_API to the EntityModel activityunit
            activityunit acu = await ConvertActivityUnitApiToActivityUnit(ActivityUnit);

            // Add the activity unit to the DB
            db.activityunits.Add(acu);
            await db.SaveChangesAsync();

            // Update the ID with the one that was auto-assigned
            ActivityUnit.id = acu.id;

            return CreatedAtRoute("DefaultApi", new { id = ActivityUnit.id }, ActivityUnit);
        }

        // DELETE: api/activityunit/5
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
            acu.id = ActivityUnit.id;
            acu.activity_id = ActivityUnit.activity_id;
            acu.location_id = ActivityUnit.location_id;
            acu.stime = ActivityUnit.stime;
            acu.etime = ActivityUnit.etime;
            // Magic to get just the IDs out of tag objects
            acu.tag_ids = ActivityUnit.tags.Select(p => p.id).ToList().ConvertAll(x => (int)x);

            return acu;
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