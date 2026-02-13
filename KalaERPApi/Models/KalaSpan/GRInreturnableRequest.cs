using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.KalaSpan
{
    public class GRInreturnableRequest
    {
        public string GRICode { get; set; }
        public string StageNo { get; set; }
        public string VehicleNo { get; set; }
        public string VehPartCode { get; set; }
        public string AudioDts { get; set; }
        public string VideoDts { get; set; }
        public string PhotoDtS { get; set; }
       
    }
}