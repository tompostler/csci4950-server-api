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

            public float eduration_average { get; set; }

            public float pduration_average { get; set; }

            public float total_eduration { get; set; }

            public float total_pduration { get; set; }

            public int total_activities { get; set; }

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

            bool includeDate = false;
            if (ddate.HasValue)
            {
                activities = activities.Where(p => p.ddate.HasValue);
                includeDate = true;
                //activities = activities.Where(p => ddate.Value.Date.CompareTo(p.ddate.Value.Date) == 0);
            }

            // Convert the activities to more API friendly things
            List<activity> activitylist = await activities.ToListAsync();

            float totalPduration = 0;
            float totalEduration = 0;

            List<float> pdurations = new List<float>();
            List<float> edurations = new List<float>();


            foreach (var act in activitylist)
                if (includeDate)
                {
                    if (act.ddate.Value.Date.CompareTo(ddate.Value.Date) == 0)
                    {
                        if (act.pduration.HasValue)
                        {
                            totalPduration += act.pduration.Value;
                            pdurations.Add(act.pduration.Value);
                        }
                        if (act.eduration.HasValue)
                        {
                            totalEduration += act.eduration.Value;
                            edurations.Add(act.eduration.Value);
                        }
                    }
                }
                else
                {
                    if (act.pduration.HasValue)
                    {
                        totalPduration += act.pduration.Value;
                        pdurations.Add(act.pduration.Value);
                    }
                    if (act.eduration.HasValue)
                    {
                        totalEduration += act.eduration.Value;
                        edurations.Add(act.eduration.Value);
                    }
                }

            int totalActivities = pdurations.Count;
            float eduration_average = edurations.Average();
            float pduration_average = pdurations.Average();
            float total_eduration = edurations.Sum();
            float total_pduration = pdurations.Sum();

            Stat_API stats = ConvertDataToStatApi(totalActivities, eduration_average, total_eduration, pduration_average, total_pduration);


                

           return Ok(stats);
            
            
        }

        // OPTIONS: api/info
        public IHttpActionResult Options()
        {
            return Ok();
        }

        private Stat_API ConvertDataToStatApi(int total, float eduration_average, float total_eduration, float pduration_average, float total_pduration)
        {
            // Convert the EntityModel type to our API type
            Stat_API stats = new Stat_API();
            stats.total_activities = total;
            stats.eduration_average = eduration_average;
            stats.total_eduration = total_eduration;
            stats.pduration_average = pduration_average;
            stats.total_pduration = total_pduration;

            return stats;
        }
    }
}
