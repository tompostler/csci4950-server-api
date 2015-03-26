using Server_API.Auth;
using Server_API.Models;
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
    public class tagsController : ApiController
    {
        private csci4950s15Model db = new csci4950s15Model();

        public class Tag_API
        {
            public byte id { get; set; }

            [MaxLength(50)]
            public string name { get; set; }

            [MaxLength(6)]
            public string default_color { get; set; }
        }

        // GET: api/tags
        public async Task<IHttpActionResult> Gettags()
        {
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

        // PUT: api/tags
        public async Task<IHttpActionResult> Puttag(Tag_API Tag)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Convert the Tag_API to the EntityModel tags_users
            tag tg = ConvertTagApiToTag(Tag);

            // Update the tags_users
            db.Entry(tg).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/tags
        public async Task<IHttpActionResult> Posttag(Tag_API Tag)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Convert the Tag_API to the EntityModel tag
            tag tg = ConvertTagApiToTag(Tag);

            // Add the tag to the DB
            db.tags.Add(tg);
            await db.SaveChangesAsync();

            // Update the ID with the one that was auto-assigned
            Tag.id = tg.id;

            return Ok(Tag);
        }

        // DELETE: api/tags/5
        public async Task<IHttpActionResult> Deletetag(int id)
        {
            tag tg = await db.tags.FindAsync(id);
            if (tg == null)
            {
                return NotFound();
            }

            db.tags.Remove(tg);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // OPTIONS: api/tags
        [RequireHttps]
        public IHttpActionResult Options()
        {
            return Ok();
        }

        /// <summary>
        /// Converts an Tag_API to an EntitiyModel tag.
        /// </summary>
        /// <param name="Tag">The Tag_API to convert.</param>
        /// <returns>An EntitiyModel tag corresponding to the Tag_API.</returns>
        private static tag ConvertTagApiToTag(Tag_API Tag)
        {
            // Convert the Tag_API to the EntityModel tag
            tag tg = new tag();
            tg.id = Tag.id;
            tg.default_color = Tag.default_color;

            return tg;
        }

        /// <summary>
        /// Converts an EntitiyModel tag to an Tag_API.
        /// </summary>
        /// <param name="Tag">The EntitiyModel tag to convert.</param>
        /// <returns>An Tag_API corresponding to the EntitiyModel tag.</returns>
        private static Tag_API ConvertTagToTagApi(tag Tag)
        {
            // Convert the EntityModel tag to the Tag_API
            Tag_API tg = new Tag_API();
            tg.id = Tag.id;
            tg.default_color = Tag.default_color;

            return tg;
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