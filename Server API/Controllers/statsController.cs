using Newtonsoft.Json;
using Server_API.Auth;
using Server_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server_API.Controllers
{
    [RequireHttps]
    public class statsController : ApiController
    {

        private csci4950s15Model db = new csci4950s15Model();


        public class Stat_API
        {
            public Stat_API()
            {
                tag_ids = new List<byte>();
                activityunit_ids = new List<long>();
            }

            [Required, StringLength(12)]
            public string course_id { get; set; }

            [Required, StringLength(50)]
            public string name { get; set; }

            [StringLength(100)]
            public string description { get; set; }

            public DateTime? ddate { get; set; }

            public float? eduration { get; set; }

            public float? pduration { get; set; }

            public byte? importance { get; set; }

            public DateTime mdate { get; set; }

            public List<byte> tag_ids { get; set; }

            public List<long> activityunit_ids { get; set; }
        }

        // GET: api/stats
        public async Task<IHttpActionResult> Getstats(string course_id = "", DateTime? ddate = null)
        {
            // Verify token
            int tok_id = AuthorizeHeader.VerifyToken(ActionContext);
            if (tok_id <= 0)
                return BadRequest(AuthorizeHeader.InvalidTokenToMessage(tok_id));

            // Create the result set
            IQueryable<activity> activities = from act in db.activities
                                              select act;

            // Filter by course_id
            if (!String.IsNullOrEmpty(course_id))
                activities = activities.Where(p => p.course_id.Equals(course_id));

            // Convert the activities to more API friendly things
            List<Stat_API> results = new List<Stat_API>();
            List<activity> activitylist = await activities.ToListAsync();
            foreach (var act in activitylist)
                results.Add(ConvertActivityToStatApi(act));

            if (results.Count == 0)
                return NotFound();
            else
                return Ok(results);
            
            
        }

        // GET: api/stats/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/stats
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/stats/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/stats/5
        public void Delete(int id)
        {
        }

        private Stat_API ConvertActivityToStatApi(activity Activity)
        {
            // Convert the EntityModel type to our API type
            Stat_API act = new Stat_API();
            act.course_id = Activity.course_id;
            act.name = Activity.name;
            act.description = Activity.description;
            act.ddate = Activity.ddate;
            act.eduration = Activity.eduration;
            act.pduration = Activity.pduration;
            act.importance = Activity.importance;
            act.mdate = Activity.mdate;

            // Magic to get just the IDs out of objects
            act.tag_ids = Activity.tags.Select(p => p.id).ToList();
            act.activityunit_ids = Activity.activityunits.Select(p => p.id).ToList();

            return act;
        }
    }
}
