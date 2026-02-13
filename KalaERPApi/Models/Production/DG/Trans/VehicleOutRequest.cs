using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Transport.Trans
{
    public class VehicleOutRequest
    {
        public string VehicleNo { get; set; }
        public string VoutDts { get; set; }
        public string ExpAmt { get; set; }
        public string ExpAmtLoadUnload { get; set; }
        public string ExpAmtToll { get; set; }
        public string ExpAmtVehExp { get; set; }
        public string ExpAmtCarriageOut { get; set; }
        public string ExpAmtStaffwel { get; set; }
        public string NextDestination { get; set; }
        public string FinalDestination { get; set; }
        public string VoutReading { get; set; }
        public string VPrevReading { get; set; }
        public string TransporterName { get; set; }
        public string DriverName { get; set; }
        public string DriverMobileNo { get; set; }
        public string Diesel { get; set; }
        public string PCCode { get; set; }
        public string LRNo { get; set; }
        public string LRDate { get; set; }
        public string TType { get; set; }
        public string VType { get; set; }
        public string image_string { get; set; }
        public string CompID { get; set; }
        public string Remark { get; set; }

    }
}