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
    public class locationsController : ApiController
    {
        private csci4950s15Entities db = new csci4950s15Entities();

        /// <summary>
        /// A Location_API class to trim down the information and named types that are exposed to
        /// the web. This is better than making our schema directly available.
        /// </summary>
        public class Location_API
        {
            public Location_API(int id = 0)
            {
                this.id = id;
            }
            public int id { get; private set; }
            [Required]
            public int user { get; set; }
            [Required, MaxLength(50)]
            public string name { get; set; }
            [Required]
            public byte type { get; set; }
            [Required, MaxLength(100)]
            public string content { get; set; }
        }

        // GET: api/locations
        public async Task<IQueryable<Location_API>> Getlocations(int id = 0, int user = 0, byte type = 0, string content = "")
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
            List<Location_API> results = new List<Location_API>();
            List<location> locationlist = await locations.ToListAsync();
            foreach (var loc in locationlist)
            {
                var locRes = new Location_API(loc.id);
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
        public async Task<HttpResponseMessage> Postlocation(Location_API post)
        {
            if (!ModelState.IsValid)
                return failMsg(JsonConvert.SerializeObject(ModelState));

            // Convert our API type into the representing Model
            location loc = new location();
            loc.user_id = post.user;
            loc.name = post.name;
            loc.type = post.type;
            loc.content = post.content;
            db.locations.Add(loc);
            await db.SaveChangesAsync();

            return goodMsg(loc.id);
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

        protected HttpResponseMessage failMsg(string desc = null)
        {
            string json = "\"success\":false";
            if (!String.IsNullOrEmpty(desc))
                json += String.Format(",\"description\":{0}", desc);
            json = "{" + json + "}";
            var response = this.Request.CreateResponse(HttpStatusCode.Forbidden);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        protected HttpResponseMessage goodMsg(int id = 0)
        {
            string json = "\"success\":true";
            if (id != 0)
                json += String.Format(",\"id\":{0}", id);
            json = "{" + json + "}";
            var response = this.Request.CreateResponse(HttpStatusCode.Created);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
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