using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Request
{
    public class MTFSave
    {
        public string FromPCCode { get; set; }
        public string ToPCCode { get; set; }
        public string ReqCode { get; set; }
        public string ProdPartCode { get; set; }
        public int PlanIssueQty { get; set; }
        public string Remark { get; set; }
    }
}