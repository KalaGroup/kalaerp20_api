using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Production.Canopy.Trans
{
    public class CpyRevRequest
    {
        public string EmpCode { get; set; }
        public string PCCode { get; set; }
        public string TransType { get; set; }
        public string RevPrcDts { get; set; }
        public string Remark { get; set; }
        public string AttachFileDts { get; set; }

    }
}