using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Request.Production.DG.Trans
{
    public class DGStage4Request
    {
        public string PCCode { get; set; }
        public string JobCardCode { get; set; }
        public string ProductCode { get; set; }
        public string CPType { get; set; }
        public string EngPartCode { get; set; }
        public string EngSrNo { get; set; }
        public string AltPartcode { get; set; }
        public string AltSrno { get; set; }
        public string CpyPartcode { get; set; }
        public string CpySrno { get; set; }
        public string BatPartcode { get; set; }
        public string BatSrno { get; set; }
        public string Bat2Partcode { get; set; }
        public string Bat2Srno { get; set; }
        public string CPPartcode { get; set; }
        public string CPSrno { get; set; }
        public string CP2Partcode { get; set; }
        public string CP2Srno { get; set; }
        public string KRMPartcode { get; set; }
        public string KRMSrno { get; set; }
        public string PrcDts { get; set; }
        public string Remark { get; set; }
        public string QA6M { get; set; }
        public string PrcStatus { get; set; }
        public string PrcChkDts { get; set; }
        public string PfbCode { get; set; }
    }
}