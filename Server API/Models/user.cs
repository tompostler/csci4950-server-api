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
    
    public partial class user
    {
        public user()
        {
            this.activities = new HashSet<activity>();
            this.locations = new HashSet<location>();
            this.tags = new HashSet<tag>();
        }
    
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    
        public virtual ICollection<activity> activities { get; set; }
        public virtual ICollection<location> locations { get; set; }
        public virtual ICollection<tag> tags { get; set; }
    }
}
