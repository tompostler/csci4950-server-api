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

        public class Tag_API
        {
            public byte id { get; set; }

            [MaxLength(50)]
            public string name { get; set; }

            [MaxLength(6)]
            public string default_color { get; set; }
        }

        // GET: api/tags
        public async Task<IHttpActionResult> Get()
        {
            // Create the result set
            IQueryable<tag> tags = from tg in db.tags
                                   select tg;

            // Convert the tags to more API friendly things
            List<Tag_API> results = new List<Tag_API>();
            List<tag> taglist = await tags.ToListAsync();
            foreach (var tg in taglist)
                results.Add(ConvertTagToTagApi(tg));

            // Hard coded (at some point)
            return Ok(results);
        }

        // OPTIONS: api/tags
        public IHttpActionResult Options()
        {
            return Ok();
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
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}