using Server_API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
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
            public ActivityUnit_API(int id = 0)
            {
                this.id = id;
            }
            public int id { get; private set; }
            public int activity { get; set; }
            public int location { get; set; }
            public DateTime stime { get; set; }
            public DateTime etime { get; set; }
        }

        // GET: api/activity_units
        public async Task<IQueryable<ActivityUnit_API>> Getactivity_units(int id = 0, int activity = 0, int location = 0, DateTime? startTime = null, DateTime? endTime = null)
        {
            // Create the result set
            var activity_units = from au in db.activity_units
                                 select au;

            // Filter on id
            if (id != 0)
                activity_units = activity_units.Where(p => p.id.Equals(id));

            // Filter on activity
            if (activity != 0)
                activity_units = activity_units.Where(p => p.activity_id.Equals(activity));

            // Filter on location
            if (location != 0)
                activity_units = activity_units.Where(p => p.location.Equals(location));

            // Filter on startTime
            if (startTime != null)
                activity_units = activity_units.Where(p => p.start_time.Equals(startTime.Value.ToUniversalTime()));

            // Filter on endTime
            if (endTime != null)
                activity_units = activity_units.Where(p => p.end_time.Equals(endTime.Value.ToUniversalTime()));

            // Convert the activity_units to more API friendly things
            List<ActivityUnit_API> results = new List<ActivityUnit_API>();
            List<activity_units> activity_unitslist = await activity_units.ToListAsync();
            foreach (var acu in activity_unitslist)
            {
                var acuRes = new ActivityUnit_API(acu.id);
                acuRes.activity = acu.activity_id;
                acuRes.location = acu.location_id;
                acuRes.stime = acu.start_time;
                acuRes.etime = acu.end_time;
                results.Add(acuRes);
            }

            return results.AsQueryable();
        }

        // GET: api/activity_units
        public IQueryable<ActivityUnit_API> Getactivity_units(DateTime startTimeBeg, DateTime startTimeEnd, DateTime endTimeBeg, DateTime endTimeEnd, int activity = 0, int location = 0)
        {
            // Create the result set
            var activity_units = from au in db.activity_units
                                 select au;

            // Filter on activity
            if (activity != 0)
                activity_units = activity_units.Where(p => p.activity_id.Equals(activity));

            // Filter on location
            if (location != 0)
                activity_units = activity_units.Where(p => p.location.Equals(location));

            // Filter on startTimeBeg
            activity_units = activity_units.Where(p => p.start_time > startTimeBeg.ToUniversalTime());

            // Filter on startTimeEnd
            activity_units = activity_units.Where(p => p.start_time < startTimeEnd.ToUniversalTime());

            // Filter on endTimeBeg
            activity_units = activity_units.Where(p => p.end_time > endTimeBeg.ToUniversalTime());

            // Filter on endTimeEnd
            activity_units = activity_units.Where(p => p.end_time < endTimeEnd.ToUniversalTime());

            // Convert the activity_units to more API friendly things
            List<ActivityUnit_API> results = new List<ActivityUnit_API>();
            foreach (var acu in activity_units)
            {
                var acuRes = new ActivityUnit_API(acu.id);
                acuRes.activity = acu.activity_id;
                acuRes.location = acu.location_id;
                acuRes.stime = acu.start_time;
                acuRes.etime = acu.end_time;
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

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/activity_units
        [ResponseType(typeof(activity_units))]
        public async Task<IHttpActionResult> Postactivity_units(activity_units activity_units)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.activity_units.Add(activity_units);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = activity_units.id }, activity_units);
        }

        // DELETE: api/activity_units/5
        [ResponseType(typeof(activity_units))]
        public async Task<IHttpActionResult> Deleteactivity_units(int id)
        {
            activity_units activity_units = await db.activity_units.FindAsync(id);
            if (activity_units == null)
            {
                return NotFound();
            }

            db.activity_units.Remove(activity_units);
            await db.SaveChangesAsync();

            return Ok(activity_units);
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