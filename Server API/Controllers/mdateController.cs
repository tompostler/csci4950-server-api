using Server_API.Auth;
using Server_API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server_API.Controllers
{
    [RequireHttps]
    public class mdateController : ApiController
    {
        private csci4950s15Model db = new csci4950s15Model();

        public class Mdate_API
        {
            public List<activitiesController.Activity_API> activities { get; set; }

            public List<activityunitsController.ActivityUnit_API> activityunits { get; set; }

            public List<locationsController.Location_API> locations { get; set; }

            public settingsController.Setting_API setting { get; set; }

            public usersController.User_API user { get; set; }
        }

        // GET: api/mdate
        public async Task<IHttpActionResult> Get(DateTime mdate)
        {
            // Verify token
            int tok_id = AuthorizeHeader.VerifyToken(ActionContext);
            if (tok_id <= 0)
                return BadRequest(AuthorizeHeader.InvalidTokenToMessage(tok_id));

            // Activities
            IQueryable<activity> acts = from act in db.activities
                                        where act.user_id == tok_id
                                        where act.mdate > mdate
                                        select act;
            
            // Activityunits
            IQueryable<activityunit> acus = from acu in db.activityunits
                                            where acu.activity.user_id == tok_id
                                            where acu.mdate > mdate
                                            select acu;

            // Location
            IQueryable<location> locs = from loc in db.locations
                                        where loc.user_id == tok_id
                                        where loc.mdate > mdate
                                        select loc;

            // Setting
            IQueryable<setting> set = from s in db.settings
                                      where s.user_id == tok_id
                                      where s.mdate > mdate
                                      select s;

            // User
            IQueryable<user> usr = from u in db.users
                                   where u.id == tok_id
                                   where u.mdate > mdate
                                   select u;

            // Assemble
            Mdate_API result = new Mdate_API
            {
                activities = (await acts.ToListAsync()).ConvertAll(act => activitiesController.ConvertActivityToActivityApi(act)),
                activityunits = (await acus.ToListAsync()).ConvertAll(acu => activityunitsController.ConvertActivityUnitToActivityUnitApi(acu)),
                locations = (await locs.ToListAsync()).ConvertAll(loc => locationsController.ConvertLocationToLocationApi(loc)),
                setting = (await set.ToListAsync()).ConvertAll(s => settingsController.ConvertSettingToSettingApi(s)).FirstOrDefault(),
                user = (await usr.ToListAsync()).ConvertAll(u => usersController.ConvertUserToUserApi(u)).FirstOrDefault()
            };

            // Check if there isn't anything new
            if ((result.activities.Count == 0) && (result.activityunits.Count == 0)
                && (result.locations.Count == 0) && (result.setting == null)
                && (result.user == null))
                return StatusCode(HttpStatusCode.NotFound);


            return Ok(result);
        }

        // OPTIONS: api/activities
        public IHttpActionResult Options()
        {
            return Ok();
        }
    }
}
