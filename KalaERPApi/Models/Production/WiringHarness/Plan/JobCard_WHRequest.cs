
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Request
{
    public class JobCard_WHRequest
    {
   

        public string Code { get; set; }
        public string EmpCode { get; set; }
        public string PCCode { get; set; }
        public string CompCode { get; set; }
        public string JobCard_CpyDts { get; set; }
        
        public string Remark { get; set; }
        
    }
}