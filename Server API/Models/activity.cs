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
    
    public partial class activity
    {
        public activity()
        {
            this.activity_units = new HashSet<activity_units>();
            this.tags = new HashSet<tag>();
        }
    
        public int id { get; set; }
        public int user { get; set; }
        public string name { get; set; }
        public byte category { get; set; }
    
        public virtual user user1 { get; set; }
        public virtual ICollection<activity_units> activity_units { get; set; }
        public virtual ICollection<tag> tags { get; set; }
    }
}
