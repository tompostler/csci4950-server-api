namespace Server_API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class user
    {
        public user()
        {
            activities = new HashSet<activity>();
            locations = new HashSet<location>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string fname { get; set; }

        [Required]
        [StringLength(50)]
        public string lname { get; set; }

        [Required]
        [StringLength(50)]
        public string email { get; set; }

        [Required]
        [StringLength(60)]
        public string password { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime mdate { get; set; }

        public virtual ICollection<activity> activities { get; set; }

        public virtual auth auth { get; set; }

        public virtual ICollection<location> locations { get; set; }

        public virtual setting setting { get; set; }
    }
}
