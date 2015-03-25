using Server_API.Auth;
using Server_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Server_API.Controllers
{
    [RequireHttps]
    public class coursesController : ApiController
    {
        private csci4950s15Model db = new csci4950s15Model();

        /// <summary>
        /// A Course_API class to trim down the information and named types that are exposed to the
        /// web. This is better than making our schema directly available.
        /// </summary>
        public class Course_API
        {
            [Required, StringLength(12)]
            public string id { get; set; }

            [Required, StringLength(32)]
            public string name { get; set; }
        }

        // GET: api/courses
        public async Task<IHttpActionResult> Getcourses(string id = "")
        {
            // If we have an ID to search by, handle it
            if (!String.IsNullOrEmpty(id))
            {
                course cou = await db.courses.FindAsync(id);
                if (cou == null)
                    return NotFound();
                else
                    return Ok(ConvertCourseToCourseApi(cou));
            }

            // Create the result set
            IQueryable<course> courses = from cou in db.courses
                                         select cou;

            // Convert the courses to more API friendly things
            List<Course_API> results = new List<Course_API>();
            List<course> courselist = courses.ToList();
            foreach (var cou in courselist)
                results.Add(ConvertCourseToCourseApi(cou));

            // Courses is hardcoded
            return Ok(results);
        }

        // OPTIONS: api/courses
        public IHttpActionResult Options()
        {
            return Ok();
        }

        /// <summary>
        /// Converts an EntityModel course to a Course_API.
        /// </summary>
        /// <param name="cou">The course to convert.</param>
        /// <returns>A Course_API corresponding to the EntityModel course.</returns>
        private static Course_API ConvertCourseToCourseApi(course Course)
        {
            // Convert the EntityModel type to our API type
            Course_API cou = new Course_API();
            cou.id = Course.id;
            cou.name = Course.name;

            return cou;
        }
    }
}