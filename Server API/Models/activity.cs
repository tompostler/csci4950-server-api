namespace Server_API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class activity
    {
        public activity()
        {
            activityunits = new HashSet<activityunit>();
            tags = new HashSet<tag>();
        }

        public int id { get; set; }

        public int user_id { get; set; }

        [Required]
        [StringLength(12)]
        public string course_id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [StringLength(100)]
        public string description { get; set; }

        public DateTime? ddate { get; set; }

        public DateTime mdate { get; set; }

        public virtual course course { get; set; }

        public virtual user user { get; set; }

        public virtual ICollection<activityunit> activityunits { get; set; }

        public virtual ICollection<tag> tags { get; set; }
    }
}
