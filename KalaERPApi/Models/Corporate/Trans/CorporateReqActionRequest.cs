using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace KalaERPApi.Models.Request.Corporate.Trans
{
    public class CorporateReqActionRequest
    {
        public string StrType { get; set; }
        public string ReqCode { get; set; }
        public string PrvActionID { get; set; }
        public string EmpCode { get; set; }
        public string PCCode { get; set; }


        public string AssignByCode { get; set; }
        public string AssignToCode { get; set; }
        public string ProblemCode { get; set; }

        public string ActionTaken { get; set; }
        public string ActionStatus { get; set; }
        public string AssignToNext { get; set; }

        public string AttachFileDts { get; set; }

        public string ExpTypeAdv { get; set; }
        public string ExpSupEmpType { get; set; }
        public string ExpSupEmpcode { get; set; }
        public string ExpDts { get; set; }
        public string CompanyCode { get; set; }
        // Add RB
        public string Priority { get; set; }

    }
}