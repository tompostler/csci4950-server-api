using Server_API.Auth;
using Server_API.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server_API.Controllers
{
    [RequireHttps]
    public class tagsController : ApiController
    {
        private csci4950s15Model db = new csci4950s15Model();

        public class DefaultTag_API
        {
            public byte id { get; set; }
            [MaxLength(50)]
            public string name { get; set; }
            [MaxLength(6)]
            public string default_color { get; set; }
        }

        public class Tag_API
        {
            [Required]
            public byte tag_id { get; set; }
            [Required]
            public int user_id { get; set; }
            [Required, StringLength(6)]
            public string color { get; set; }
        }

        // GET: api/tags
        public async Task<IHttpActionResult> Gettags()
        {
            // Create the result set
            IQueryable<tag> tags = from tg in db.tags
                                   select tg;

            // Convert the tags to more API friendly things
            List<DefaultTag_API> results = new List<DefaultTag_API>();
            List<tag> taglist = await tags.ToListAsync();
            foreach (var tg in taglist)
                results.Add(ConvertDefaultTagToDefaultTagApi(tg));

            if (results.Count == 0)
                return NotFound();
            else
                return Ok(results);
        }

        // GET: api/tags?user_id=##
        public async Task<IHttpActionResult> Gettags(int user_id, int tag_id = 0)
        {
            // Create the result set
            IQueryable<tags_users> tag_users = from tgu in db.tags_users
                                               select tgu;

            // Filter on user_id
            tag_users = tag_users.Where(p => p.user_id.Equals(user_id));

            // Filter on tag_id
            if (tag_id != 0)
                tag_users = tag_users.Where(p => p.tag_id.Equals(tag_id));

            // Convert the tags_users to more API friendly things
            List<Tag_API> results = new List<Tag_API>();
            List<tags_users> tgulist = await tag_users.ToListAsync();
            foreach (var tgu in tgulist)
                results.Add(ConvertTagToTagApi(tgu));

            if (results.Count == 0)
                return NotFound();
            else
                return Ok(results);
        }

        // PUT: api/tags
        public async Task<IHttpActionResult> Puttag(Tag_API UserTag)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Convert the Tag_API to the EntityModel tags_users
            tags_users tgu = ConvertTagApiToTag(UserTag);

            // Update the tags_users
            db.Entry(tgu).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(UserTag);
        }

        // POST: api/tags
        public async Task<IHttpActionResult> Posttag(Tag_API UserTag)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Convert the DefaultTag_API to the EntityModel tag
            tags_users tg = ConvertTagApiToTag(UserTag);

            // Add the tag to the DB
            db.tags_users.Add(tg);
            await db.SaveChangesAsync();

            return Ok(UserTag);
        }

        // DELETE: api/tags?tag_id=##&user_id=##
        public async Task<IHttpActionResult> Deletetag(int tag_id, int user_id)
        {
            // Create the result set
            IQueryable<tags_users> tag_users = from tgu in db.tags_users
                                               select tgu;

            // Filter on user_id
            tag_users = tag_users.Where(p => p.user_id.Equals(user_id));
            tag_users = tag_users.Where(p => p.tag_id.Equals(tag_id));

            // Get the one tags_users
            tags_users tu = await tag_users.FirstOrDefaultAsync();
            if (tu == null)
                return NotFound();

            // Remove it
            db.tags_users.Remove(tu);
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
        /// Converts a DefaultTag_API to an EntitiyModel tag.
        /// </summary>
        /// <param name="Tag">The DefaultTag_API to convert.</param>
        /// <returns>An EntityModel tag corresponding to the DefaultTag_API.</returns>
        private static tag ConvertDefaultTagApiToDefaultTag(DefaultTag_API Tag)
        {
            // Convert the DefaultTag_API to the EntityModel tag
            tag tg = new tag();
            tg.id = Tag.id;
            tg.name = Tag.name;
            tg.default_color = Tag.default_color;

            return tg;
        }

        /// <summary>
        /// Converts an EntitiyModel tag to a DefaultTag_API.
        /// </summary>
        /// <param name="Tag">The EntitiyModel tag to convert.</param>
        /// <returns>A DefaultTag_API corresponding to the EntitiyModel tag.</returns>
        private static DefaultTag_API ConvertDefaultTagToDefaultTagApi(tag Tag)
        {
            // Convert the EntityModel tag to the DefaultTag_API
            DefaultTag_API tg = new DefaultTag_API();
            tg.id = Tag.id;
            tg.name = Tag.name;
            tg.default_color = Tag.default_color;

            return tg;
        }

        /// <summary>
        /// Converts an Tag_API to an EntitiyModel tags_users.
        /// </summary>
        /// <param name="Tag">The Tag_API to convert.</param>
        /// <returns>An EntitiyModel tags_users corresponding to the Tag_API.</returns>
        private static tags_users ConvertTagApiToTag(Tag_API Tag)
        {
            // Convert the Tag_API to the EntityModel tags_users
            tags_users tgu = new tags_users();
            tgu.tag_id = Tag.tag_id;
            tgu.user_id = Tag.user_id;
            tgu.color = Tag.color;

            return tgu;
        }

        /// <summary>
        /// Converts an EntitiyModel tags_users to an Tag_API.
        /// </summary>
        /// <param name="Tag">The EntitiyModel tags_users to convert.</param>
        /// <returns>An Tag_API corresponding to the EntitiyModel tags_users.</returns>
        private static Tag_API ConvertTagToTagApi(tags_users Tag)
        {
            // Convert the EntityModel tags_users to the Tag_API
            Tag_API tgu = new Tag_API();
            tgu.tag_id = Tag.tag_id;
            tgu.user_id = Tag.user_id;
            tgu.color = Tag.color;

            return tgu;
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