using Newtonsoft.Json;
using Server_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Server_API.Controllers
{
    public class locationsController : ApiController
    {
        private csci4950s15Entities db = new csci4950s15Entities();

        /// <summary>
        /// A Location_API class to trim down the information and named types that are exposed to
        /// the web. This is better than making our schema directly available.
        /// </summary>
        public class Location_API
        {
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
            public byte type { get; set; }
            [Required, MaxLength(100)]
            public string content { get; set; }
        }

        // GET: api/locations
        public async Task<IHttpActionResult> Getlocations(int id = 0, int user = 0, byte type = 0, string content = "")
        {
            // If we have an ID to search by, handle it
            if (id != 0)
            {
                location loc = await db.locations.FindAsync(id);
                if (loc == null)
                    return NotFound();
                else
                    return Ok(ConvertLocationToLocationApi(loc));
            }

            // Create the result set
            IQueryable<location> locations = from loc in db.locations
                                             select loc;

            // Filter on user_id
            if (user != 0)
                locations = locations.Where(p => p.user.Equals(user));

            // Filter on type
            if (type != 0)
                locations = locations.Where(p => p.type.Equals(type));

            // Filter on content, strict matching
            if (!String.IsNullOrEmpty(content))
                locations = locations.Where(p => p.content.Equals(content));

            // Convert the locations to more API friendly things
            List<Location_API> results = new List<Location_API>();
            List<location> locationlist = await locations.ToListAsync();
            foreach (var loc in locationlist)
                results.Add(ConvertLocationToLocationApi(loc));

            if (results.Count == 0)
                return NotFound();
            else
                return Ok(results);
        }

        // PUT: api/locations/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putlocation(int id, Location_API Location)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify the Location
            var verification = await VerifyLocationAndID(Location);
            if (verification != null)
                return verification;

            // Verify request ID
            if (id != Location.id)
                return BadRequest("PUT URL and ID in the location do not match");

            // Convert the Location_API to the EntityModel location
            location loc = ConvertLocationApiToLocation(Location);

            // Update the location
            db.Entry(loc).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(Location);
        }

        // POST: api/locations
        [ResponseType(typeof(Location_API))]
        public async Task<IHttpActionResult> Postlocation(Location_API Location)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify the Location
            var verification = await VerifyLocation(Location);
            if (verification != null)
                return verification;

            // Convert the Location_API to the EntityModel location
            location loc = ConvertLocationApiToLocation(Location);

            // Add the location to the DB
            db.locations.Add(loc);
            await db.SaveChangesAsync();

            // Update the ID with the one that was auto-assigned
            Location.SetID(loc.id);

            return CreatedAtRoute("DefaultApi", new { id = Location.id }, Location);
        }

        // DELETE: api/locations/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Deletelocation(int id)
        {
            location location = await db.locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            db.locations.Remove(location);
            await db.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Converts a Location_API to an EntityModel location.
        /// </summary>
        /// <param name="Location">The Location_API to convert.</param>
        /// <returns>An EntityModel location corresponding to the Location_API.</returns>
        private location ConvertLocationApiToLocation(Location_API Location)
        {
            // Convert the Location_API to the EntityModel location
            location loc = new location();
            loc.id = Location.id;
            loc.user_id = Location.user_id;
            loc.name = Location.name;
            loc.type = Location.type;
            loc.content = Location.content;

            return loc;
        }

        /// <summary>
        /// Converts an EntityModel location to a Location_API.
        /// </summary>
        /// <param name="Location">The EntityModel location to convert.</param>
        /// <returns>A Location_API corresponding to the EntityModel location.</returns>
        private Location_API ConvertLocationToLocationApi(location Location)
        {
            // Convert the EntityModel location to the Location_API
            Location_API loc = new Location_API();
            loc.SetID(Location.id);
            loc.user_id = Location.user_id;
            loc.name = Location.name;
            loc.type = Location.type;
            loc.content = Location.content;

            return loc;
        }

        /// <summary>
        /// Verifies the location by checking that the UserID exists.
        /// </summary>
        /// <param name="Location">The location to verify.</param>
        /// <returns>
        /// Null for success. The appropriate IHttpActionResult on failure.
        /// </returns>
        private async Task<IHttpActionResult> VerifyLocation(Location_API Location)
        {
            // Verify the UserID exists
            if (await db.users.FindAsync(Location.user_id) == null)
                return BadRequest("user_id does not exist");

            return null;
        }

        /// <summary>
        /// Verifies the location and the ID for the location. This is more useful in PUT requests.
        /// </summary>
        /// <param name="Location">The location.</param>
        /// <returns>
        /// 404 if an ID is not found; the appropriate IHttpActionResult on failure; null on success.
        /// </returns>
        private async Task<IHttpActionResult> VerifyLocationAndID(Location_API Location)
        {
            // Verify ID. Returns a 404 if not valid
            if (await db.locations.FindAsync(Location.id) == null)
                return NotFound();

            return await VerifyLocation(Location);
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