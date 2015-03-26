using Server_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace Server_API.Auth
{
    public static class AuthorizeHeader
    {
        /// <summary>
        /// Verifies the token with user identifier.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <param name="user_id">The user_id.</param>
        /// <returns>The string appropriate error message on failure, null on success.</returns>
        public static string VerifyTokenWithUserId(HttpActionContext actionContext, int user_id)
        {
            int tok = VerifyToken(actionContext);
            if (tok < 0)
                return "token not found";
            else if (tok == 0)
                return "Expired token";
            else if (tok != user_id)
                return "Permission denied by token";

            return null;
        }

        /// <summary>
        /// Verifies the token.
        /// </summary>
        /// <returns>-1 if not found, 0 if expired, else associated user_id.</returns>
        public static int VerifyToken(HttpActionContext actionContext)
        {
            // Get provided token
            string token = GetToken(actionContext);
            if (token == null)
                return -1;

            // Get matching auths
            List<auth> auths = GetAuthsFromToken(token);

            // Handle not found
            if (auths.Count == 0)
                return -1;

            // Found
            auth aut = auths.First();

            // Check if expired
            if (aut.expire < DateTime.UtcNow)
                return 0;

            return aut.user_id;
        }

        /// <summary>
        /// Gets the auths from token. Encapsulates the process.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        private static List<auth> GetAuthsFromToken(string token)
        {
            csci4950s15Model db = new csci4950s15Model();
            var aut = from a in db.auths
                      select a;
            aut = aut.Where(p => p.token.Equals(token));
            List<auth> auths = aut.ToList();
            return auths;
        }

        /// <summary>
        /// Gets the token from the HTTP request (pulled from the Authorization header).
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>null on error, token on success.</returns>
        public static string GetToken(HttpActionContext actionContext)
        {
            string[] header = GetAuthorizationHeader(actionContext);
            if (header == null)
                return null;
            return header[0];
        }

        /// <summary>
        /// Gets the authorization header.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>The string[] of values from the Authorization HTTP header. null on error.</returns>
        public static string[] GetAuthorizationHeader(HttpActionContext actionContext)
        {
            IEnumerable<string> authorize;
            bool exists = actionContext.Request.Headers.TryGetValues("Authorization", out authorize);

            // Check if the auth header exists
            if (!exists)
                return null;

            // Return it
            return (string[])authorize;
        }
    }
}