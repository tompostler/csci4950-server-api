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
    public class activity_unitsController : ApiController
    {
        private csci4950s15Entities db = new csci4950s15Entities();

        public class ActivityUnitResult
        {
            public int id { get; set; }
            public int activity { get; set; }
            public int location { get; set; }
            public DateTime stime { get; set; }
            public DateTime etime { get; set; }
        }

        // GET: api/activity_units
        public IQueryable<ActivityUnitResult> Getactivity_units(int activity=0, int location=0, DateTime? startTime=null, DateTime? endTime=null)
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

            // Filter on startTime
            if (startTime != null)
                activity_units = activity_units.Where(p => p.start_time.Equals(startTime.Value.ToUniversalTime()));

            // Filter on endTime
            if (endTime != null)
                activity_units = activity_units.Where(p => p.end_time.Equals(endTime.Value.ToUniversalTime()));

            // Convert the activity_units to more API friendly things
            List<ActivityUnitResult> results = new List<ActivityUnitResult>();
            foreach (var acu in activity_units)
            {
                var acuRes = new ActivityUnitResult();
                acuRes.id = acu.id;
                acuRes.activity = acu.activity_id;
                acuRes.location = acu.location_id;
                acuRes.stime = acu.start_time;
                acuRes.etime = acu.end_time;
                results.Add(acuRes);
            }

            return results.AsQueryable();
        }

        // GET: api/activity_units
        public IQueryable<activity_units> Getactivity_units(DateTime startTimeBeg, DateTime startTimeEnd, DateTime endTimeBeg, DateTime endTimeEnd, int activity = 0, int location = 0)
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

            return activity_units;
        }

        // GET: api/activity_units/5
        [ResponseType(typeof(activity_units))]
        public async Task<IHttpActionResult> Getactivity_units(int id)
        {
            activity_units activity_units = await db.activity_units.FindAsync(id);
            if (activity_units == null)
            {
                return NotFound();
            }

            return Ok(activity_units);
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