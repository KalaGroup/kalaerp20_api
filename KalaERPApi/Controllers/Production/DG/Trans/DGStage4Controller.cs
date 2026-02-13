using System.Data;
using System.Web.Http;
using KalaERPApi.Service.Production.DG.Trans;
using DGStage4Request = KalaERPApi.Models.Request.Production.DG.Trans.DGStage4Request;

namespace KalaERPApi.Controllers.Production.DG.Trans
{
    public class DGStage4Controller : ApiController
    {
        DGStageCon dc = new DGStageCon();
        [HttpGet]
        [Route("DGStage4/GetScanDts")]
        public DataTable GetScanDts4(string strSrNo, string strPartCode, string strCat, string strStage , string strPCCode)
        {
            return dc.GetScanDts(strSrNo, strPartCode, strCat, strStage, strPCCode);
        }
       
        [HttpGet]
        [Route("DGStage4/GetProdDts")]
        public DataTable GetPodprcDts(string strPartCode,string strPCCode)
        {           
            return dc.GetPodprcDts(strPartCode, strPCCode);
        }

        [HttpPost]
        [Route("DGStage4/Stage/Submit")]
        public string SubmitStage4(DGStage4Request DGStageReq)
        {
            return dc.SubmitStage4(DGStageReq);
        }
    }
}
