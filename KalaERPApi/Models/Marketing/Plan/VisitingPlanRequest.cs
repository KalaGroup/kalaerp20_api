using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace KalaERPApi.Models.Request.Marketing.Plan
{
    public class VisitingPlanRequest
    {
        public string StrType { get; set; }
        public string VPCode { get; set; }
        public string EmpCode { get; set; }
        public string PCCode { get; set; }
        public string CompCode { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ODType { get; set; }
        public string VP_OD_GPNo { get; set; }
        public string ExpCode { get; set; }
        public string PlanDts { get; set; }
        public string Remark { get; set; }
        public string HODRemark { get; set; }
    }
}