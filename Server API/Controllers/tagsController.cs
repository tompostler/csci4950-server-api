﻿using Server_API.Models;
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

        public class Tags_API
        {
            public Tags_API(int id = 0)
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
        public IQueryable<tag> Gettags()
        {
            return db.tags;
        }

        // GET: api/tags/5
        [ResponseType(typeof(tag))]
        public async Task<IHttpActionResult> Gettag(int id)
        {
            tag tag = await db.tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            return Ok(tag);
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
        [ResponseType(typeof(tag))]
        public async Task<IHttpActionResult> Posttag(tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.tags.Add(tag);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = tag.id }, tag);
        }

        // DELETE: api/tags/5
        [ResponseType(typeof(tag))]
        public async Task<IHttpActionResult> Deletetag(int id)
        {
            tag tag = await db.tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            db.tags.Remove(tag);
            await db.SaveChangesAsync();

            return Ok(tag);
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