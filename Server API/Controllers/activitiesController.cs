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
    public class activitiesController : ApiController
    {
        private csci4950s15Entities db = new csci4950s15Entities();

        // GET: api/activities
        public IQueryable<activity> Getactivities()
        {
            return db.activities;
        }

        // GET: api/activities/5
        [ResponseType(typeof(activity))]
        public async Task<IHttpActionResult> Getactivity(int id, string test="")
        {
            System.Console.WriteLine(test);
            activity activity = await db.activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            return Ok(activity);
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

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/activities
        [ResponseType(typeof(activity))]
        public async Task<IHttpActionResult> Postactivity()
        {
            activity act1 = new activity();
            act1.id = 3;
            act1.name = "Testing the API";
            act1.user = 2;
            act1.category = 1;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.activities.Add(act1);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (activityExists(act1.id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = act1.id }, act1);
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

            return Ok(activity);
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