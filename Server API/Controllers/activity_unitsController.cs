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
    public class activity_unitsController : ApiController
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
            public void SetID(int id)
            {
                this.id = id;
            }
            public int id { get; private set; }
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

        // GET: api/activity_units
        public async Task<IQueryable<ActivityUnit_API>> Getactivity_units(int id = 0, int activity_id = 0, int location_id = 0, DateTime? stime = null, DateTime? etime = null)
        {
            // Create the result set
            var activity_units = from au in db.activity_units
                                 select au;

            // Filter on id
            if (id != 0)
                activity_units = activity_units.Where(p => p.id.Equals(id));

            // Filter on activity_id
            if (activity_id != 0)
                activity_units = activity_units.Where(p => p.activity_id.Equals(activity_id));

            // Filter on location_id
            if (location_id != 0)
                activity_units = activity_units.Where(p => p.location.Equals(location_id));

            // Filter on stime
            if (stime != null)
                activity_units = activity_units.Where(p => p.start_time.Equals(stime.Value.ToUniversalTime()));

            // Filter on etime
            if (etime != null)
                activity_units = activity_units.Where(p => p.end_time.Equals(etime.Value.ToUniversalTime()));

            // Convert the activity_units to more API friendly things
            List<ActivityUnit_API> results = new List<ActivityUnit_API>();
            List<activity_units> activity_unitslist = await activity_units.ToListAsync();
            foreach (var acu in activity_unitslist)
            {
                var acuRes = new ActivityUnit_API();
                acuRes.SetID(acu.id);
                acuRes.activity_id = acu.activity_id;
                acuRes.location_id = acu.location_id;
                acuRes.stime = acu.start_time;
                acuRes.etime = acu.end_time;
                // Magic to get just the IDs out of tag objects
                acuRes.tag_ids = acu.tags.Select(p => p.id).ToList();
                results.Add(acuRes);
            }

            return results.AsQueryable();
        }

        // GET: api/activity_units
        public async Task<IQueryable<ActivityUnit_API>> Getactivity_units(DateTime btime, DateTime etime, int activity = 0, int location = 0)
        {
            // Create the result set
            var activity_units = from au in db.activity_units
                                 select au;

            // Filter on activity_id
            if (activity != 0)
                activity_units = activity_units.Where(p => p.activity_id.Equals(activity));

            // Filter on location_id
            if (location != 0)
                activity_units = activity_units.Where(p => p.location.Equals(location));

            // Filter on btime
            activity_units = activity_units.Where(p => p.start_time > btime.ToUniversalTime());
            activity_units = activity_units.Where(p => p.end_time > btime.ToUniversalTime());

            // Filter on etime
            activity_units = activity_units.Where(p => p.end_time < etime.ToUniversalTime());
            activity_units = activity_units.Where(p => p.end_time < etime.ToUniversalTime());

            // Convert the activity_units to more API friendly things
            List<ActivityUnit_API> results = new List<ActivityUnit_API>();
            List<activity_units> activity_unitslist = await activity_units.ToListAsync();
            foreach (var acu in activity_unitslist)
            {
                var acuRes = new ActivityUnit_API();
                acuRes.SetID(acu.id);
                acuRes.activity_id = acu.activity_id;
                acuRes.location_id = acu.location_id;
                acuRes.stime = acu.start_time;
                acuRes.etime = acu.end_time;
                // Magic to get just the IDs out of tag objects
                acuRes.tag_ids = acu.tags.Select(p => p.id).ToList();
                results.Add(acuRes);
            }

            return results.AsQueryable();
        }

        // PUT: api/activity_units/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putactivity_units(int id, activity_units activity_units)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != activity_units.id)
            {
                return BadRequest();
            }

            db.Entry(activity_units).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!activity_unitsExists(id))
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

        // POST: api/activity_units
        [ResponseType(typeof(ActivityUnit_API))]
        public async Task<IHttpActionResult> Postactivity_units(ActivityUnit_API ActivityUnit)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Start times have to be after end times
            if (ActivityUnit.stime > ActivityUnit.etime)
                return BadRequest("etime cannot be before stime");

            // Verify ActivityID exists
            if (await db.activities.CountAsync(p => p.id.Equals(ActivityUnit.activity_id)) != 1)
                return BadRequest("activity_id does not exist");

            // Verify LocationID exists
            if (await db.locations.CountAsync(p => p.id.Equals(ActivityUnit.location_id)) != 1)
                return BadRequest("location_id does not exist");

            // Verify TagIDs exist
            foreach (int id in ActivityUnit.tag_ids)
                if (await db.tags.CountAsync(p => p.id.Equals(id)) != 1)
                    return BadRequest("Tag with id " + id.ToString() + " does not exist");

            // Get the tags referenced by this activity to do a proper insertion with the WebAPI
            // http://stackoverflow.com/a/2101561
            var tags = from tg in db.tags
                       select tg;
            var tagsPredicate = PredicateBuilder.False<tag>();
            foreach (int id in ActivityUnit.tag_ids)
                tagsPredicate = tagsPredicate.Or(p => p.id.Equals(id));
            tags = tags.Where(tagsPredicate);

            // Convert our API type into the representing Model
            activity_units acu = new activity_units();
            acu.activity_id = ActivityUnit.activity_id;
            acu.location_id = ActivityUnit.location_id;
            acu.start_time = ActivityUnit.stime;
            acu.end_time = ActivityUnit.etime;
            acu.tags = await tags.ToListAsync();

            db.activity_units.Add(acu);
            await db.SaveChangesAsync();

            ActivityUnit.SetID(acu.id);

            return CreatedAtRoute("DefaultApi", new { id = ActivityUnit.id }, ActivityUnit);
        }

        // DELETE: api/activity_units/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Deleteactivity_units(int id)
        {
            activity_units activity_units = await db.activity_units.FindAsync(id);
            if (activity_units == null)
            {
                return NotFound();
            }

            db.activity_units.Remove(activity_units);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool activity_unitsExists(int id)
        {
            return db.activity_units.Count(e => e.id == id) > 0;
        }
    }
}