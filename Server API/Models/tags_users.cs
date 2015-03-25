namespace Server_API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tags_users
    {
        [Key]
        [Column(Order = 0)]
        public byte tag_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int user_id { get; set; }

        [Required]
        [StringLength(6)]
        public string color { get; set; }

        public virtual tag tag { get; set; }

        public virtual user user { get; set; }
    }
}
