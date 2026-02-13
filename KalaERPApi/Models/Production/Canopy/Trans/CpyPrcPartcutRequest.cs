using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Production.Canopy.Trans
{
    public class CpyPrcPartcutRequest
    {
        public string EmpCode { get; set; }
        public string PCCode { get; set; }
        public string PlanCode { get; set; }
        public string ProductCode { get; set; }

        public string TkitId { get; set; }
        public string SheetPartcode { get; set; }
        public int BatchQty { get; set; }
        public string MachineCodeSrNo { get; set; }

        public int SerialNo { get; set; }
        public double ShQtyPerset { get; set; }
        public double ShWtperUts { get; set; }
        public double ShWtperSet { get; set; }
        public double ShWtperBatch { get; set; }


        public string PrcDts { get; set; }  //Partcode,KitQty,TotQty,PfbRate,Plen,Pwidth,PThk,PLossWt,WtPeruts,SqftPerUts,catCode
        public string Remark { get; set; }
        public string AttachFileDts { get; set; }
        public string CatID { get; set; }
    }
}