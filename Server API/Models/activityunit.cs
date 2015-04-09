namespace Server_API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class activityunit
    {
        public activityunit()
        {
            tags = new HashSet<tag>();
        }

        public long id { get; set; }

        public int activity_id { get; set; }

        public int location_id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [StringLength(100)]
        public string description { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime stime { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime etime { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime mdate { get; set; }

        public virtual activity activity { get; set; }

        public virtual location location { get; set; }

        public virtual ICollection<tag> tags { get; set; }
    }
}
