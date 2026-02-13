using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Production.Canopy.Trans
{
    public class CpyPrcBendRequest
    {
        public string EmpCode { get; set; }
        public string PCCode { get; set; }
        public string PlanCode { get; set; }
        public string ProductCode { get; set; }
        public string PFBCode { get; set; }
        public string CpyKitcode { get; set; }
        public int BatchQty { get; set; }
        public int PrcQty { get; set; }
        public string MachineCodeSrNo { get; set; }
        public string BOMcode { get; set; }
        public double PFBRate { get; set; }
        public double Strokes { get; set; }
        public string PrcDts { get; set; }  //Partcode,KitQty,TotQty,PfbRate,WtPeruts,
        public string Remark { get; set; }
        public double Rate { get; set; }
        public string AttachFileDts { get; set; }
        public double PWt { get; set; }
        public double PSqft { get; set; }
        public string CatID { get; set; }
    }
}
