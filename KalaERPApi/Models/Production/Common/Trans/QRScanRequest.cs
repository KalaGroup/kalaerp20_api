using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Request.Production.Common.Trans
{
    public class QRScanRequest
    {
        public string PCCode { get; set; }
        public string TransCode { get; set; }
        public string TransDt { get; set; }
        public string PartCode { get; set; }
        public string SerialNo { get; set; }
        public string BalQty { get; set; }
        public string PhyQty { get; set; }
    }
}