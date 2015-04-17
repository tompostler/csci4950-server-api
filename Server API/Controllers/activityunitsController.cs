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
                tag_ids = new List<byte>();
            }

            public long id { get; set; }

            [Required]
            public int activity_id { get; set; }

            [Required]
            public int location_id { get; set; }

            [Required, StringLength(50)]
            public string name { get; set; }

            [StringLength(100)]
            public string description { get; set; }

            [Required]
            public DateTime stime { get; set; }

            [Required]
            public DateTime etime { get; set; }

            public DateTime mdate { get; set; }

            public List<byte> tag_ids { get; set; }
        }

        // GET: api/activityunit
        public async Task<IHttpActionResult> Get([FromUri] List<long> id = null, int activity_id = 0, int location_id = 0)
        {
            // Verify token
            int tok_id = AuthorizeHeader.VerifyToken(ActionContext);
            if (tok_id <= 0)
                return BadRequest(AuthorizeHeader.InvalidTokenToMessage(tok_id));

            // If we have IDs to search by, handle it
            if (id != null)
            {
                IQueryable<activityunit> acus = from acu in db.activityunits
                                                where acu.activity.user_id == tok_id
                                                where id.Contains(acu.id)
                                                select acu;

                // Get the results
                var acusResults = (await acus.ToListAsync()).ConvertAll(acu => ConvertActivityUnitToActivityUnitApi(acu));
                if (acusResults == null)
                    return NotFound();
                else if (acusResults.Count == 1)
                    return Ok(acusResults.FirstOrDefault());
                else
                    return Ok(acusResults);
            }

            // Create the result set
            IQueryable<activityunit> activityunits = from au in db.activityunits
                                                    where au.activity.user_id == tok_id
                                                    select au;

            // Filter on activity_id
            if (activity_id != 0)
                activityunits = activityunits.Where(p => p.activity_id.Equals(activity_id));

            // Filter on location_id
            if (location_id != 0)
                activityunits = activityunits.Where(p => p.location.Equals(location_id));

            // Convert the activityunit to more API friendly things
            var results = (await activityunits.ToListAsync()).ConvertAll(acu => ConvertActivityUnitToActivityUnitApi(acu));

            if (results.Count == 0)
                return NotFound();
            else
                return Ok(results);
        }

        // PUT: api/activityunit/5
        [ValidateViewModel]
        public async Task<IHttpActionResult> Put(int id, ActivityUnit_API ActivityUnit)
        {
            // Verify request ID
            if (id != ActivityUnit.id)
                return BadRequest("PUT URL and ID in the activity unit do not match");

            // Verify token
            string msg = AuthorizeHeader.VerifyTokenWithUserId(ActionContext, (await db.activities.FindAsync(ActivityUnit.activity_id)).user_id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

            // Convert the ActivityUnit_API to the EntityModel activityunit
            activityunit acu = ConvertActivityUnitApiToActivityUnit(ActivityUnit);

            // Update the activity unit
            db.Entry(acu).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/activityunit
        [ValidateViewModel]
        public async Task<IHttpActionResult> Post(ActivityUnit_API ActivityUnit)
        {
            // Verify token
            string msg = AuthorizeHeader.VerifyTokenWithUserId(ActionContext, (await db.activities.FindAsync(ActivityUnit.activity_id)).user_id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

            // Convert the ActivityUnit_API to the EntityModel activityunit
            activityunit acu = ConvertActivityUnitApiToActivityUnit(ActivityUnit);

            // Add the activity unit to the DB
            db.activityunits.Add(acu);
            await db.SaveChangesAsync();

            // Update the ID with the one that was auto-assigned
            ActivityUnit.id = acu.id;

            return Ok(ActivityUnit);
        }

        // DELETE: api/activityunit/5
        public async Task<IHttpActionResult> Delete(int id)
        {
            activityunit activityunit = await db.activityunits.FindAsync(id);
            if (activityunit == null)
                return NotFound();

            // Verify token
            string msg = AuthorizeHeader.VerifyTokenWithUserId(ActionContext, (await db.activities.FindAsync(activityunit.activity_id)).user_id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

            db.activityunits.Remove(activityunit);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
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
        private activityunit ConvertActivityUnitApiToActivityUnit(ActivityUnit_API ActivityUnit)
        {
            // Convert our API type into the representing Model
            activityunit acu = new activityunit();
            acu.id = ActivityUnit.id;
            acu.activity_id = ActivityUnit.activity_id;
            acu.location_id = ActivityUnit.location_id;
            acu.name = ActivityUnit.name;
            acu.description = ActivityUnit.description;
            acu.stime = ActivityUnit.stime;
            acu.etime = ActivityUnit.etime;
            acu.mdate = Util.UtcDateTimeInMilliseconds();

            return acu;
        }


        /// <summary>
        /// Converts an EntityModel activityunit to an ActivityUnit_API.
        /// </summary>
        /// <param name="ActivityUnit">The EntityModel activityunit to convert.</param>
        /// <returns>An ActivityUnit_API corresponding to the EntityModel activityunit.</returns>
        internal static ActivityUnit_API ConvertActivityUnitToActivityUnitApi(activityunit ActivityUnit)
        {
            // Convert EntityModel type to our API type
            ActivityUnit_API acu = new ActivityUnit_API();
            acu.id = ActivityUnit.id;
            acu.activity_id = ActivityUnit.activity_id;
            acu.location_id = ActivityUnit.location_id;
            acu.name = ActivityUnit.name;
            acu.description = ActivityUnit.description;
            acu.stime = ActivityUnit.stime;
            acu.etime = ActivityUnit.etime;
            acu.mdate = ActivityUnit.mdate;
            // Magic to get just the IDs out of tag objects
            acu.tag_ids = ActivityUnit.tags.Select(p => p.id).ToList();

            return acu;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}