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
    
    public partial class WishList
    {
        public int id { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> ProductID { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual UserTable UserTable { get; set; }
    }
}
