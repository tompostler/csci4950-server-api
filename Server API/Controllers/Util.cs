using System;

namespace Server_API.Controllers
{
    /// <summary>
    /// Utility class to contain miscellaneous functions that don't seem to have any other home.
    /// </summary>
    static class Util
    {
        /// <summary>
        /// The current DateTime.UtcNow truncated to the nearest millisecond instead of 100ns. Done
        /// to account for Java's truncation of DateTime objects.
        /// </summary>
        /// <returns></returns>
        public static DateTime UtcDateTimeInMilliseconds()
        {
            DateTime date = DateTime.UtcNow;
            return new DateTime(date.Ticks - date.Ticks % TimeSpan.TicksPerMillisecond, date.Kind);
        }
    }
}