using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace KalaERPApi.Models.Request.Marketing.Plan
{
    public class VisitingPlanExpRequest
    {
        public string SaveType { get; set; }
        public string ReqCode { get; set; }
        public string EmpCode { get; set; }
        public string PCCode { get; set; }
        public string ExpTypeAdv { get; set; }
        public string ExpSupEmpType { get; set; }
        public string ExpSupEmpcode { get; set; }
        public string VP_Pln_Exp_Dtls { get; set; }
        public string ExpCompanyCode { get; set; }
        public string AuthExpByEcode { get; set; }
        //public string AuthExpBODRemark { get; set; }

    }
}