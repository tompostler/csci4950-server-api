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
    public class activitiesController : ApiController
    {
        private csci4950s15Entities db = new csci4950s15Entities();

        /// <summary>
        /// An Activity_API class to trim down the information and named types that are exposed to
        /// the web. This is better than making our schema directly available.
        /// </summary>
        public class Activity_API
        {
            public Activity_API(int id = 0)
            {
                this.id = id;
            }
            public int id { get; private set; }
            [Required]
            public int user { get; set; }
            [Required, MaxLength(50)]
            public string name { get; set; }
            [Required]
            public byte category { get; set; }
        }

        // GET: api/activities
        public async Task<IQueryable<Activity_API>> Getactivities(int id = 0, int user = 0, string name = "", byte category = 0)
        {
            // Create the result set
            var activities = from act in db.activities
                             select act;

            // Filter by id
            if (id != 0)
                activities = activities.Where(p => p.id.Equals(id));

            // Filter by user
            if (user != 0)
                activities = activities.Where(p => p.user.Equals(user));

            // Filter by name, strict matching
            if (!String.IsNullOrEmpty(name))
                activities = activities.Where(p => p.name.Equals(name));

            // Filter by category
            if (category != 0)
                activities = activities.Where(p => p.category.Equals(category));

            // Convert the activities to more API friendly things
            List<Activity_API> results = new List<Activity_API>();
            List<activity> activitylist = await activities.ToListAsync();
            foreach (var act in activitylist)
            {
                var actRes = new Activity_API(act.id);
                actRes.user = act.user;
                actRes.name = act.name;
                actRes.category = act.category;
                results.Add(actRes);
            }

            return results.AsQueryable();
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
        public async Task<HttpResponseMessage> Postactivity(Activity_API post)
        {
            if (!ModelState.IsValid)
                return failMsg(desc: JsonConvert.SerializeObject(ModelState));

            // Convert our API type into the representing Model
            activity act = new activity();
            act.user = post.user;
            act.name = post.name;
            act.category = post.category;
            db.activities.Add(act);
            await db.SaveChangesAsync();

            return goodMsg(act.id);
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

        private bool activityExists(int id)
        {
            return db.activities.Count(e => e.id == id) > 0;
        }
    }
}