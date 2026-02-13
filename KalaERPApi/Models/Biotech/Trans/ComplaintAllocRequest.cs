using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace KalaERPApi.Models.Request.Biotech.Trans
{
    public class ComplaintAllocRequest
    {
        public string StrType { get; set; }
        public string PCACode { get; set; }

        public string EmpCode { get; set; }
        public string PCCode { get; set; }
        public string CompCode { get; set; }

        public string CompNo { get; set;}


        public string Priority { get; set;}
        public string WarrantyStatus { get; set;}
        public string ProductCode { get; set;}
        public string CompType { get; set; }
        public string AssignProjectHead { get; set;}
        public string TechnicianDtls { get; set; }
        public string DealerDtls { get; set;}
        public string Remark { get; set; }
        public string EmailDts { get; set; }
       
    }
}