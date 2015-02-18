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

        // GET: api/activity_units
        public IQueryable<activity_units> Getactivity_units()
        {
            return db.activity_units;
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