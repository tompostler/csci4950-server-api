namespace Server_API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class auth
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int user_id { get; set; }

        [Required]
        [StringLength(64)]
        public string token { get; set; }

        public DateTime expire { get; set; }

        public virtual user user { get; set; }
    }
}
