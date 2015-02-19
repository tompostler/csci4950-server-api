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
    public class locationsController : ApiController
    {
        private csci4950s15Entities db = new csci4950s15Entities();

        public class LocationResult
        {
            public int id { get; set; }
            public int user { get; set; }
            public string name { get; set; }
            public byte type { get; set; }
            public string content { get; set; }
        }

        // GET: api/locations
        public IQueryable<LocationResult> Getlocations(int id=0, int user=0, byte type=0, string content="")
        {
            // Create the result set
            var locations = from loc in db.locations
                            select loc;

            // Filter on id
            if (id != 0)
                locations = locations.Where(p => p.id.Equals(id));

            // Filter on user
            if (user != 0)
                locations = locations.Where(p => p.user.Equals(user));

            // Filter on type
            if (type != 0)
                locations = locations.Where(p => p.type.Equals(type));

            // Filter on content, strict matching
            if (!String.IsNullOrEmpty(content))
                locations = locations.Where(p => p.content.Equals(content));

            // Convert the locations to more API friendly things
            List<LocationResult> results = new List<LocationResult>();
            foreach (var loc in locations)
            {
                var locRes = new LocationResult();
                locRes.id = loc.id;
                locRes.user = loc.user_id;
                locRes.name = loc.name;
                locRes.type = loc.type;
                locRes.content = loc.content;
                results.Add(locRes);
            }

            return results.AsQueryable();
        }

        // PUT: api/locations/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putlocation(int id, location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != location.id)
            {
                return BadRequest();
            }

            db.Entry(location).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!locationExists(id))
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

        // POST: api/locations
        [ResponseType(typeof(location))]
        public async Task<IHttpActionResult> Postlocation(location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.locations.Add(location);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = location.id }, location);
        }

        // DELETE: api/locations/5
        [ResponseType(typeof(location))]
        public async Task<IHttpActionResult> Deletelocation(int id)
        {
            location location = await db.locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            db.locations.Remove(location);
            await db.SaveChangesAsync();

            return Ok(location);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool locationExists(int id)
        {
            return db.locations.Count(e => e.id == id) > 0;
        }
    }
}