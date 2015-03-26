namespace Server_API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class setting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int user_id { get; set; }

        [Column(TypeName = "ntext")]
        [Required]
        public string value { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime mdate { get; set; }

        public virtual user user { get; set; }
    }
}
