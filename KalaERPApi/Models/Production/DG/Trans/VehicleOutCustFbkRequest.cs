using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Transport.Trans
{
    public class VehicleOutCustFbkRequest
    {
        public string InvDts { get; set; }
        public string CustName { get; set; }
        public string CustSign { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
}