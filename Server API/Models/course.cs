namespace Server_API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("courses")]
    public partial class course
    {
        public course()
        {
            activities = new HashSet<activity>();
        }

        [StringLength(12)]
        public string id { get; set; }

        [Required]
        [StringLength(32)]
        public string name { get; set; }

        public virtual ICollection<activity> activities { get; set; }
    }
}
