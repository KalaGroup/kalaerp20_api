using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace KalaERPApi.Models.Request.Corporate.Trans
{
    public class CorporateReqFeedbackRequest
    {
        public string StrType { get; set; }
        public string PrvActionID { get; set; }
        public string ReqCode { get; set; }
         public string EmpCode { get; set; }
        public string PCCode { get; set; }
             
        public string FeedbackYOrNStatus { get; set; }
        public string FeedbackRating { get; set; }
        public string Feedback { get; set;}

        public string AttachFileDts { get; set; }
        public string CompanyCode { get; set;}
       
    }
}