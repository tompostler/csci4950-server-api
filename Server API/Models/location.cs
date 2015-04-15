namespace Server_API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class location
    {
        public location()
        {
            activityunits = new HashSet<activityunit>();
        }

        public int id { get; set; }

        public int user_id { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [StringLength(100)]
        public string content { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime mdate { get; set; }

        public virtual ICollection<activityunit> activityunits { get; set; }

        public virtual user user { get; set; }
    }
}
