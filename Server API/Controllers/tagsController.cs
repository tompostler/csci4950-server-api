using Server_API.Models;
using System.Collections.Generic;
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
    public class tagsController : ApiController
    {
        private csci4950s15Entities db = new csci4950s15Entities();

        public class Tag_API
        {

            public void SetID(int id)
            {
                this.id = id;
            }
            public int id { get; private set; }
            [Required]
            public int user_id { get; set; }
            [MaxLength(20)]
            public string name { get; set; }
            [MaxLength(100)]
            public string description { get; set; }
            [MaxLength(6)]
            public string color { get; set; }
        }

        // GET: api/tags
        public async Task<IQueryable<Tag_API>> Gettags(int id = 0, int user_id = 0)
        {
            // Create the result set
            var tags = from tg in db.tags
                       select tg;

            // Filter by id
            if (id != 0)
                tags = tags.Where(p => p.id.Equals(id));

            // Filter by user_id
            if (user_id != 0)
                tags = tags.Where(p => p.user_id.Equals(user_id));
            
            // Convert the tags to more API friendly things
            List<Tag_API> results = new List<Tag_API>();
            List<tag> taglist = await tags.ToListAsync();
            foreach (var tg in taglist)
            {
                var tgRes = new Tag_API();
                tgRes.SetID(tg.id);
                tgRes.user_id = tg.user_id;
                tgRes.name = tg.name;
                tgRes.description = tg.description;
                tgRes.color = tg.color;
                results.Add(tgRes);
            }

            return results.AsQueryable();
        }

        // PUT: api/tags/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Puttag(int id, tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tag.id)
            {
                return BadRequest();
            }

            db.Entry(tag).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tagExists(id))
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

        // POST: api/tags
        [ResponseType(typeof(Tag_API))]
        public async Task<IHttpActionResult> Posttag(Tag_API Tag)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify UserID exists
            if (await db.users.CountAsync(p => p.id.Equals(Tag.user_id)) != 1)
                return BadRequest("user_id does not exist");

            // Convert the Tag_API to the EntityModel tag
            tag tg = new tag();
            tg.user_id = Tag.user_id;
            tg.name = Tag.name;
            tg.description = Tag.description;
            tg.color = Tag.color;

            db.tags.Add(tg);
            await db.SaveChangesAsync();

            Tag.SetID(tg.id);

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

            return StatusCode(HttpStatusCode.NoContent);
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