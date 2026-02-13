using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Production.Common.Trans
{
    public class ReverseTransRequest
    {
        public string PCCode { get; set; }
        public int RevTransFor { get; set; }
        public string RevDGDts { get; set; }
        public string Remark { get; set; }
    }
}