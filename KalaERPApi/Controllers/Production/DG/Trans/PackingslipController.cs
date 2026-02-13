using System.Data;
using System.Web.Http;
using KalaERPApi.Service.Production.DG.Trans;
using PackingslipRequest = KalaERPApi.Models.Request.Production.DG.Trans.PackingslipRequest;

namespace KalaERPApi.Controllers.Production.DG.Trans
{
    public class PackingslipController : ApiController
    {
        PackingslipCon dc = new PackingslipCon();

        [HttpGet]
        [Route("Packingslip/GetScanDts")]
        public DataTable GetScanDts(string strSrNo, string StrDGSrNo, string strCat, string strCPBatCnt, string StrPCCode)
        {
            return dc.GetScanDts(strSrNo, StrDGSrNo, strCat, strCPBatCnt, StrPCCode);
        }

        [HttpGet]
        [Route("Packingslip/GetMOFAddDts")]
        public DataTable GetMOFAddPartDts(string strMOFCode)
        {
            return dc.GetMOFAddPartDts(strMOFCode);
        }

        [HttpPost]
        [Route("Packingslip/Submit")]
        public string Submit(PackingslipRequest PSReq)
        {
            return dc.Submit(PSReq);
        }
    }
}
