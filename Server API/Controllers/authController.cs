using Server_API.Auth;
using Server_API.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server_API.Controllers
{
    /// <summary>
    /// The auth controller breaks the normal API style, per discussion with Greyson.
    /// </summary>
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

        // GET: api/auth
        public async Task<IHttpActionResult> Getauth()
        {
            // Verify token
            int tok_id = AuthorizeHeader.VerifyToken(ActionContext);
            string msg = AuthorizeHeader.InvalidTokenToMessage(tok_id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

            auth aut = await db.auths.FindAsync(tok_id);
            return Ok(ConvertAuthToAuthRetApi(aut));
        }

        // PUT: api/auth
        public async Task<IHttpActionResult> Putauth()
        {
            // Get the token
            int tok_id = AuthorizeHeader.VerifyToken(ActionContext);

            // Check the token
            string tok_msg = AuthorizeHeader.InvalidTokenToMessage(tok_id);
            if (!String.IsNullOrEmpty(tok_msg))
                return BadRequest(tok_msg);

            // If we made it this far, then update the auth
            auth authExisting = await db.auths.FindAsync(tok_id);
            if (authExisting == null)
                return NotFound();
            authExisting.expire = DateTime.UtcNow.AddDays(21);
            db.Entry(authExisting).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(ConvertAuthToAuthRetApi(authExisting));
        }

        // POST: api/auth
        public async Task<IHttpActionResult> Postauth(AuthReq_API AuthRequest)
        {
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
            auth authRequest = new auth
            {
                user_id = AuthRequest.user_id,
                token = token,
                expire = DateTime.UtcNow.AddDays(21)
            };
            db.auths.Add(authRequest);
            await db.SaveChangesAsync();

            return Ok(ConvertAuthToAuthRetApi(authRequest));
        }

        // DELETE: api/auth/5
        public async Task<IHttpActionResult> Deleteauth()
        {
            int tok_id = AuthorizeHeader.VerifyToken(ActionContext);
            auth aut = await db.auths.FindAsync(tok_id);
            if (aut == null)
                return NotFound();

            // Verify token
            string msg = AuthorizeHeader.InvalidTokenToMessage(tok_id, aut.user_id);
            if (!String.IsNullOrEmpty(msg))
                return BadRequest(msg);

            db.auths.Remove(aut);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // OPTIONS: api/auth
        public IHttpActionResult Options()
        {
            return Ok();
        }

        /// <summary>
        /// Converts an EntityModel auth to an AuthRet_API.
        /// </summary>
        /// <param name="Auth">The EntityModel auth to convert.</param>
        /// <returns>An AuthRet_API corresponding to the EntityModel auth.</returns>
        private static AuthRet_API ConvertAuthToAuthRetApi(auth Auth)
        {
            return new AuthRet_API
            {
                user_id = Auth.user_id,
                token = Auth.token,
                expire = Auth.expire
            };
        }
    }
}