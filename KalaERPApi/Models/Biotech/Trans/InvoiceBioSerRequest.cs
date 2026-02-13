using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace KalaERPApi.Models.Request.Biotech.Trans
{
    public class InvoiceBioSerRequest
    {
        public string StrType { get; set; }
        public string CCode { get; set; }
        public string EmpCode { get; set; }
        public string PCCode { get; set; }
        public string CompCode { get; set; }
        public string CompNo { get; set;}
        public string PCACode { get; set;}

        public string CustomerCode { get; set; }
        public string IndentorCode { get; set;}
        public string IndentorChk { get; set;}
        public string SiteID { get; set; }

        public string MatAmt { get; set; }
        public string LabAmt { get; set;}
        public string InvoiceTotalAmt { get; set; }
        public string InvoiceTotalInWord { get; set; }
        public string Remark { get; set;}
       
        public string InvDesc { get; set; }
        public string InvQty { get; set; }
        public string InvUOM { get; set; }
        public string INVType { get; set; }
        public string PONo { get; set; }
        public string PODate { get; set; }
        
        public string MtlDts { get; set; }
        public string LblDts { get; set; }
        public string ActDtls { get; set; }

    }
}