using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.Request.Production.DG.Trans
{
    public class PackingslipRequest
    {
        public string PSTime { get; set; }
        public string TRCode { get; set; }
        public string PSCode { get; set; }
        public string DiNo { get; set; }
        public string MOFCode { get; set; }
        public string KVA { get; set; }
        public string CPType { get; set; }
        public string PDICode { get; set; }
        public string DGSrNo { get; set; }
        public string BatTer { get; set; }
        public string BatLead { get; set; }
        public string ExhPipe { get; set; }
        public string DCBulb { get; set; }
        public string CanopyKey { get; set; }
        public string FuelCapKey { get; set; }
        public string RubberPad { get; set; }
        public string FunnelPad { get; set; }
        public string PrdManual { get; set; }
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
        public string MOFAddParts { get; set; }
        public string Remark { get; set; }
    }
}