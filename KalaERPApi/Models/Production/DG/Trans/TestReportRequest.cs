using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Request.Production.DG.Trans
{
    public class TestReportRequest
    {

        public string PFBCode { get; set; }
        public string DGSrNo { get; set; }
        public string TRCode { get; set; }
        public string TRTime { get; set; }
        public string QA6M { get; set; }
        public string QAStatus { get; set; }
        public string TRPrcChkDts { get; set; }
        public string DieselQty { get; set; }
        public string DieselRate { get; set; }
        public string TRDts { get; set; }
        public string Remark { get; set; }

    }

}