namespace Server_API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tag
    {
        public tag()
        {
            activities = new HashSet<activity>();
            activityunits = new HashSet<activityunit>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [StringLength(6)]
        public string default_color { get; set; }

        public virtual ICollection<activity> activities { get; set; }

        public virtual ICollection<activityunit> activityunits { get; set; }
    }
}
