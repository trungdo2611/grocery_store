//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebQLyNongSan.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Blog
    {
        public int id { get; set; }
        public Nullable<int> UserID { get; set; }
        public string Title { get; set; }
        public string Picture { get; set; }
        public string Content { get; set; }
        public Nullable<System.DateTime> Create_at { get; set; }
    
        public virtual UserTable UserTable { get; set; }
    }
}
