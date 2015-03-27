using Server_API.Auth;
using Server_API.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server_API.Controllers
{
    [RequireHttps]
    public class authController : ApiController
    {
        private csci4950s15Model db = new csci4950s15Model();

        /// <summary>
        /// The information that accompanies a request to generate an auth token.
        /// </summary>
        public class AuthReq_API
        {
            [Required]
            public int user_id { get; set; }

            [Required]
            public string password { get; set; }
        }

        /// <summary>
        /// The information that is returned upon generation of an auth token.
        /// </summary>
        public class AuthRet_API
        {
            [Required]
            public int user_id { get; set; }

            [Required, StringLength(64)]
            public string token { get; set; }

            public DateTime expire { get; set; }
        }

        // POST: api/auth
        public async Task<IHttpActionResult> Postauth(AuthReq_API AuthRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get the corresponding user
            user usr = await db.users.FindAsync(AuthRequest.user_id);
            if (usr == null)
                return BadRequest("user_id not found");

            // Verify the password
            if (!Hashing.ValidatePassword(AuthRequest.password, usr.password))
                return BadRequest("password does not match user_id");

            // Generate the token
            string token = Hashing.GenerateToken();

            // Assemble and insert
            auth aur = new auth
            {
                user_id = AuthRequest.user_id,
                token = token,
                expire = DateTime.UtcNow.AddDays(1)
            };
            db.auths.Add(aur);
            await db.SaveChangesAsync();

            // Convert to friendly
            AuthRet_API aut = new AuthRet_API
            {
                user_id = aur.user_id,
                token = aur.token,
                expire = aur.expire
            };

            return Ok(aut);
        }

        // DELETE: api/auth/5
        public async Task<IHttpActionResult> Deleteauth(int id)
        {
            auth aut = await db.auths.FindAsync(id);
            if (aut == null)
                return NotFound();

            db.auths.Remove(aut);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // OPTIONS: api/auth
        public IHttpActionResult Options()
        {
            return Ok();
        }
    }
}