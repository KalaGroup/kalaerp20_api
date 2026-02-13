using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace KalaERPApi.Models.Request.Corporate.Trans
{
    public class CorporateReqRequest
    {
        public string StrType { get; set; }
      
        public string ReqCode { get; set; }
        public string EmpCode { get; set; }
        public string FromPCCode { get; set; }
        public string ToPCCode { get; set;}
        public string ToEmpCode { get; set; }
        //RB 
        public string Priority { get; set; }
       //end
        public string ReqMsg { get; set;}
        public string CompanyCode { get; set; }
        public string AttachFileDts { get; set; }

        public string AssignHODEmpCode { get; set; }

    }
}