//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Server_API.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tag
    {
        public tag()
        {
            this.activities = new HashSet<activity>();
            this.activity_units = new HashSet<activity_units>();
        }
    
        public int id { get; set; }
        public int user_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string color { get; set; }
    
        public virtual user user { get; set; }
        public virtual ICollection<activity> activities { get; set; }
        public virtual ICollection<activity_units> activity_units { get; set; }
    }
}
