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

namespace Server_API.Controllers
{
    [RequireHttps]
    public class locationsController : ApiController
    {
        private csci4950s15Model db = new csci4950s15Model();

        /// <summary>
        /// A Location_API class to trim down the information and named types that are exposed to
        /// the web. This is better than making our schema directly available.
        /// </summary>
        public class Location_API
        {
            public Location_API()
            {
                tag_ids = new List<byte>();
                activityunit_ids = new List<long>();
            }

            public int id { get; set; }

            [Required]
            public int user_id { get; set; }

            [Required, MaxLength(50)]
            public string name { get; set; }

            [Required, MaxLength(100)]
            public string content { get; set; }

            public List<byte> tag_ids { get; set; }

            public List<long> activityunit_ids { get; set; }
        }

        // GET: api/locations
        public async Task<IHttpActionResult> Getlocations(int id = 0, int user = 0)
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
        public async Task<IHttpActionResult> Putlocation(int id, Location_API Location)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify request ID
            if (id != Location.id)
                return BadRequest("PUT URL and ID in the location do not match");

            // Convert the Location_API to the EntityModel location
            location loc = ConvertLocationApiToLocation(Location);

            // Update the location
            db.Entry(loc).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/locations
        public async Task<IHttpActionResult> Postlocation(Location_API Location)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Convert the Location_API to the EntityModel location
            location loc = ConvertLocationApiToLocation(Location);

            // Add the location to the DB
            db.locations.Add(loc);
            await db.SaveChangesAsync();

            // Update the ID with the one that was auto-assigned
            Location.id = loc.id;

            return Ok(Location);
        }

        // DELETE: api/locations/5
        public async Task<IHttpActionResult> Deletelocation(int id)
        {
            location location = await db.locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            db.locations.Remove(location);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // OPTIONS: api/locations
        [RequireHttps]
        public IHttpActionResult Options()
        {
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
            loc.id = Location.id;
            loc.user_id = Location.user_id;
            loc.name = Location.name;
            loc.content = Location.content;

            // Magic to get just the IDs out of objects
            loc.tag_ids = Location.tags.Select(p => p.id).ToList();
            loc.activityunit_ids = Location.activityunits.Select(p => p.id).ToList();

            return loc;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}