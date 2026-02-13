using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Request
{
    public class MTFWIPInternalRequest
    {
       
        public string FromPCCode { get; set; }
        public string ToPCCode { get; set; }
        public string ReqCode { get; set; }
        public string ProdPartCode { get; set; }
        public int ReqBalQty { get; set; }
        public int MTFQty { get; set; }
        public string MTFDetails { get; set; }
        public string Remark { get; set; }

        public string UserID { get; set; }
        public string CompID { get; set; }

    }
}