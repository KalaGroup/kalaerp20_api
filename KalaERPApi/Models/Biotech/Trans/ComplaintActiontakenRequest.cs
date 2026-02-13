using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace KalaERPApi.Models.Request.Biotech.Trans
{
    public class ComplaintActiontakenRequest
    {
        public string StrType { get; set; }
        public string ACTNo { get; set; }
        public string EmpCode { get; set; }
        public string PCCode { get; set; }
        public string CompCode { get; set; }
        public string PCACode { get; set;}
        public string ActionStatus { get; set;}
        public string NextFlwDt { get; set;}
        public string ActRemark { get; set;}
        public string EmailDts { get; set; }

        public string ActCorrServAnalysisDts { get; set; }
        public string ActMaterialDts { get; set;}
        public string ExpTypeAdv { get; set; }
        public string ExpSupEmpType { get; set;}
        public string ExpSupEmpcode { get; set; }
        public string SiteName { get; set; }
        public string SiteAddress { get; set; }
        public string CompProduct { get; set; }
        public string ExpDts { get; set; }
        public string ExpAttachFileDts { get; set; }
        public string ActionAttachFileDts { get; set; }
        
    }
}