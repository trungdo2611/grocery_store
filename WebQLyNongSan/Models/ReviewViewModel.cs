using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebQLyNongSan.Models
{
    public class ReviewViewModel
    {
        public Review reviews { get; set; }
        public string userName { get; set; }
        public string avatarUser { get; set; }
    }
}