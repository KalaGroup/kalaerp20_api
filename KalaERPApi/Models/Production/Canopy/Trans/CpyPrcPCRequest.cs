using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Production.Canopy.Trans
{
    public class CpyPrcPCRequest
    {
        public string EmpCode { get; set; }
        public string PCCode { get; set; }
        public string SupplierCode { get; set; }
        public string MachineCodeSrNo { get; set; }
        public int StdSqft { get; set; }
        public string PrcDts { get; set; }  //item.PlanCode + "-->" +item.ProductCode + "-->"  +  item.BOMCode+ "-->" + item.KitCode+ "-->" +  item.BatchQty+ "-->" + item.Sqft+ "-->" + item.PrcQty + "-->" + item.PFBCode+ "-->" + item.EDT;
        public string Remark { get; set; }
        public string CatID { get; set; }
        public string AttachFileDts { get; set; }
    }
}