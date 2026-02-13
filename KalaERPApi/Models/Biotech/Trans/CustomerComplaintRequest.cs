using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace KalaERPApi.Models.Request.Biotech.Trans
{
    public class CustomerComplaintRequest
    {
        public string StrType { get; set; }
        public string CompNo { get; set; }
        public string EmpCode { get; set;}
        public string PCCode { get; set;}
        public string CompCode {get; set;}
        public string CustomerCode { get; set;}
        public string InvNo { get; set;}
        public string ProductCode { get; set; }
        public string SiteID { get; set;}
        public string CompTypeDtls { get; set; }
        public string NatureOfComplaint { get; set; }
        public string Remark { get; set; }
       public string EmailDts { get; set; }
    }
}