using Server_API.Auth;
using Server_API.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Server_API.Controllers
{
    public class tagsController : ApiController
    {
        private csci4950s15Entities db = new csci4950s15Entities();

        public class Tag_API
        {
            public byte id { get; set; }
            [MaxLength(20)]
            public string name { get; set; }
            [MaxLength(6)]
            public string color { get; set; }
        }

        // GET: api/tags
        public async Task<IHttpActionResult> Gettags(int id = 0)
        {
            // If we have an ID to search by, handle it
            if (id != 0)
            {
                tag tg = await db.tags.FindAsync(id);
                if (tg == null)
                    return NotFound();
                else
                    return Ok(ConvertTagToTagApi(tg));
            }

            // Create the result set
            IQueryable<tag> tags = from tg in db.tags
                                   select tg;

            // Convert the tags to more API friendly things
            List<Tag_API> results = new List<Tag_API>();
            List<tag> taglist = await tags.ToListAsync();
            foreach (var tg in taglist)
                results.Add(ConvertTagToTagApi(tg));

            if (results.Count == 0)
                return NotFound();
            else
                return Ok(results);
        }

        // PUT: api/tags/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Puttag(int id, Tag_API Tag)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify request ID
            if (id != Tag.id)
                return BadRequest("PUT URL and ID in the tag do not match");

            // Convert the Tag_API to the EntityModel tag
            tag tg = ConvertTagApiToTag(Tag);

            // Update the location
            db.Entry(tg).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(Tag);
        }

        // POST: api/tags
        [ResponseType(typeof(Tag_API))]
        public async Task<IHttpActionResult> Posttag(Tag_API Tag)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify the Tag
            var verification = await VerifyTag(Tag);
            if (verification != null)
                return verification;

            // Convert the Tag_API to the EntityModel tag
            tag tg = ConvertTagApiToTag(Tag);

            // Add the tag to the DB
            db.tags.Add(tg);
            await db.SaveChangesAsync();

            // Update the ID with the one that was auto-assigned
            Tag.id = tg.id;

            return CreatedAtRoute("DefaultApi", new { id = Tag.id }, Tag);
        }

        // DELETE: api/tags/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Deletetag(int id)
        {
            tag tag = await db.tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            db.tags.Remove(tag);
            await db.SaveChangesAsync();

            return Ok();
        }

        // OPTIONS: api/tags
        [RequireHttps]
        public IHttpActionResult Options()
        {
            return Ok();
        }

        /// <summary>
        /// Converts a Tag_API to an EntitiyModel tag.
        /// </summary>
        /// <param name="Tag">The Tag_API to convert.</param>
        /// <returns>An EntityModel tag corresponding to the Tag_API.</returns>
        private static tag ConvertTagApiToTag(Tag_API Tag)
        {
            // Convert the Tag_API to the EntityModel tag
            tag tg = new tag();
            tg.id = Tag.id;
            tg.name = Tag.name;
            tg.default_color = Tag.color;

            return tg;
        }

        /// <summary>
        /// Converts an EntitiyModel tag to a Tag_API.
        /// </summary>
        /// <param name="Tag">The EntitiyModel tag to convert.</param>
        /// <returns>A Tag_API corresponding to the EntitiyModel tag.</returns>
        private static Tag_API ConvertTagToTagApi(tag Tag)
        {
            // Convert the EntityModel tag to the Tag_API
            Tag_API tg = new Tag_API();
            tg.id = Tag.id;
            tg.name = Tag.name;
            tg.color = Tag.default_color;

            return tg;
        }

        /// <summary>
        /// Verifies the tag by checking that the UserID exists.
        /// </summary>
        /// <param name="Tag">The tag to verify.</param>
        /// <returns>
        /// Null for success. The appropriate IHttpActionResult on failure.
        /// </returns>
        private async Task<IHttpActionResult> VerifyTag(Tag_API Tag)
        {
            // Verify the UserID exists
            //if (await db.users.FindAsync(Tag.user_id) == null)
            //    return BadRequest("user_id does not exist");

            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tagExists(int id)
        {
            return db.tags.Count(e => e.id == id) > 0;
        }
    }
}