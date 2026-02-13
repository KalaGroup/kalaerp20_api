using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Marketing.Plan
{
    public class ILocation
    {
        public string vpcode { get; set; }      
        public string srno { get; set; }        
        public string savetype { get; set; }        
        public string customer { get; set; }
        public string feedback { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string accuracy { get; set; }             
        public string filenames { get; set; }             
    }
}