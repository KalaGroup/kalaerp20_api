
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Request
{
    public class JobCardRequest
    {           
        public string PCCode { get; set; }
        public string JobCardDts { get; set; }
        public string Remark { get; set; }
        
    }
}