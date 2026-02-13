using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KalaERPApi.Models.KalaSpan.Trans
{
    public class KalaSpanStageRequest
    {
        public string JBCode { get; set; }
        public int StageNo { get; set; }
        public string ProductCode { get; set; }
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
        public string QA6M { get; set; }
        public string PrcStatus { get; set; }
        public string PrcChkDts { get; set; }
        public string EngPlay { get; set; }
    }
}