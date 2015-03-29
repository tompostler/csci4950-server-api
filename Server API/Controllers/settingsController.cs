using Server_API.Auth;
using Server_API.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Server_API.Controllers
{
    [RequireHttps]
    public class settingsController : ApiController
    {
        private csci4950s15Model db = new csci4950s15Model();

        /// <summary>
        /// A Setting_API class to trim down the information and named types that are exposed to the
        /// web. This is better than making our schema directly available.
        /// </summary>
        public class Setting_API
        {
            public int user_id { get; set; }

            [Required]
            public string value { get; set; }

            public DateTime mdate { get; set; }
        }

        // GET: api/settings
        public async Task<IHttpActionResult> Getsetting()
        {
            // Verify token
            int tok_id = AuthorizeHeader.VerifyToken(ActionContext);
            if (tok_id <= 0)
                return BadRequest(AuthorizeHeader.InvalidTokenToMessage(tok_id));

            // Get the setting
            setting set = await db.settings.FindAsync(tok_id);
            if (set == null)
                return NotFound();
            else
                return Ok(ConvertSettingToSettingApi(set));
        }

        // PUT: api/settings/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putsetting(int id, Setting_API Setting)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify request ID
            if (id != Setting.user_id)
                return BadRequest("PUT URL and user_id in the setting do not match");

            // Verify token
            string msg = AuthorizeHeader.VerifyTokenWithUserId(ActionContext, id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

            // Convert the Setting_API to the EntityModel setting
            setting set = ConvertSettingApiToSetting(Setting);

            // Update the setting
            db.Entry(set).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/settings
        public async Task<IHttpActionResult> Postsetting(Setting_API Setting)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify token
            string msg = AuthorizeHeader.VerifyTokenWithUserId(ActionContext, Setting.user_id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

            // Convert the Setting_API to the EntityModel setting
            setting set = ConvertSettingApiToSetting(Setting);

            // Add the setting to the DB
            db.settings.Add(set);
            await db.SaveChangesAsync();

            return Ok(Setting);
        }

        // DELETE: api/settings/5
        [ResponseType(typeof(setting))]
        public async Task<IHttpActionResult> Deletesetting(int id)
        {
            setting setting = await db.settings.FindAsync(id);
            if (setting == null)
            {
                return NotFound();
            }

            // Verify token
            string msg = AuthorizeHeader.VerifyTokenWithUserId(ActionContext, setting.user_id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

            // Remove the setting from the DB
            db.settings.Remove(setting);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // OPTIONS: api/info
        public IHttpActionResult Options()
        {
            return Ok();
        }

        /// <summary>
        /// Converts a Setting_API to an EntityModel setting.
        /// </summary>
        /// <param name="Setting">The Setting_API to convert.</param>
        /// <returns>An EntityModel setting correspond to the Setting_API.</returns>
        private setting ConvertSettingApiToSetting(Setting_API Setting)
        {
            // Convert our API type to the EntityModel type
            setting set = new setting();
            set.user_id = Setting.user_id;
            set.value = Setting.value;
            set.mdate = Setting.mdate;

            return set;
        }

        /// <summary>
        /// Converts an EntityModel setting to a Setting_API.
        /// </summary>
        /// <param name="Setting">The EntityModel setting to convert.</param>
        /// <returns>A Setting_API correspond to the EntityModel setting.</returns>
        private Setting_API ConvertSettingToSettingApi(setting Setting)
        {
            // Convert the EntityModel type to our API type
            Setting_API set = new Setting_API();
            set.user_id = Setting.user_id;
            set.value = Setting.value;
            set.mdate = Setting.mdate;

            return set;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool settingExists(int id)
        {
            return db.settings.Count(e => e.user_id == id) > 0;
        }
    }
}