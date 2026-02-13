
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Request
{
    public class JobCardRequest
    {
        public string TableName { get; set; }
        public string Prefix { get; set; }
        public string CompCode { get; set; }
        public string Remark { get; set; }
        public string JCDDts { get; set; }
    }
}