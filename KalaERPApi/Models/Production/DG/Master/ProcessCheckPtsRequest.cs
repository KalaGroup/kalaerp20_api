
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Request
{
    public class ProcessCheckPtsRequest
    {
           
        public string StrType { get; set; }
        public string ProcessID { get; set; }
        public string EmpCode { get; set; }
        public string CompanyCode { get; set; }
        public string PrcStatusName { get; set; }
        public string ProcessName { get; set; }
        public string CheckPointDesc { get; set; }
        public string Remark { get; set; }
    }
}