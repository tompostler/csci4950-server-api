using Newtonsoft.Json;
using Server_API.Auth;
using Server_API.Filters;
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
                activityunit_ids = new List<long>();
            }

            public int id { get; set; }

            [Required]
            public int user_id { get; set; }

            [Required, MaxLength(50)]
            public string name { get; set; }

            [Required, MaxLength(100)]
            public string content { get; set; }

            public DateTime mdate { get; set; }

            public List<long> activityunit_ids { get; set; }
        }

        // GET: api/locations
        public async Task<IHttpActionResult> Getlocations([FromUri] List<int> id = null)
        {
            // Verify token
            int tok_id = AuthorizeHeader.VerifyToken(ActionContext);
            if (tok_id <= 0)
                return BadRequest(AuthorizeHeader.InvalidTokenToMessage(tok_id));

            // If we have IDs to search by, handle it
            if ((id != null) && (id.Count > 0))
            {
                IQueryable<location> locs = from loc in db.locations
                                            where loc.user_id == tok_id
                                            where id.Contains(loc.id)
                                            select loc;

                // Get the results
                var locsResults = (await locs.ToListAsync()).ConvertAll(loc => ConvertLocationToLocationApi(loc));
                if (locsResults == null)
                    return NotFound();
                else if (locsResults.Count == 1)
                    return Ok(locsResults.FirstOrDefault());
                else
                    return Ok(locsResults);
            }

            // Create the result set
            IQueryable<location> locations = from loc in db.locations
                                             select loc;

            // Filter on user_id
            locations = locations.Where(p => p.user_id.Equals(tok_id));

            // Convert the locations to more API friendly things
            var results = (await locations.ToListAsync()).ConvertAll(loc => ConvertLocationToLocationApi(loc));

            if (results.Count == 0)
                return NotFound();
            else
                return Ok(results);
        }

        // PUT: api/locations/5
        [ValidateViewModel]
        public async Task<IHttpActionResult> Putlocation(int id, Location_API Location)
        {
            // Verify request ID
            if (id != Location.id)
                return BadRequest("PUT URL and ID in the location do not match");

            // Verify token
            string msg = AuthorizeHeader.VerifyTokenWithUserId(ActionContext, Location.user_id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

            // Convert the Location_API to the EntityModel location
            location loc = ConvertLocationApiToLocation(Location);

            // Update the location
            db.Entry(loc).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/locations
        [ValidateViewModel]
        public async Task<IHttpActionResult> Postlocation(Location_API Location)
        {
            // Verify token
            string msg = AuthorizeHeader.VerifyTokenWithUserId(ActionContext, Location.user_id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

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
                return NotFound();

            // Verify token
            string msg = AuthorizeHeader.VerifyTokenWithUserId(ActionContext, location.user_id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

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
            loc.mdate = Util.UtcDateTimeInMilliseconds();

            return loc;
        }

        /// <summary>
        /// Converts an EntityModel location to a Location_API.
        /// </summary>
        /// <param name="Location">The EntityModel location to convert.</param>
        /// <returns>A Location_API corresponding to the EntityModel location.</returns>
        internal static Location_API ConvertLocationToLocationApi(location Location)
        {
            // Convert the EntityModel location to the Location_API
            Location_API loc = new Location_API();
            loc.id = Location.id;
            loc.user_id = Location.user_id;
            loc.name = Location.name;
            loc.content = Location.content;
            loc.mdate = Location.mdate;

            // Magic to get just the IDs out of objects
            loc.activityunit_ids = Location.activityunits.Select(p => p.id).ToList();

            return loc;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}