using System.Data;
using System.Web.Http;
using KalaERPApi.Service.Production.DG.Trans;
using KalaERPApi.Service;
using TestReportRequest = KalaERPApi.Models.Request.Production.DG.Trans.TestReportRequest;

namespace KalaERPApi.Controllers.Production.DG.Trans
{
    public class TestReportController : ApiController
    {
        TestReportCon dc = new TestReportCon();
        CommonCon dc1 = new CommonCon();

        [HttpGet]
        [Route("TestReport/GetScanDts")]
        public DataTable GetScanDts(string strSrNo, string StrDGSrNo, string strCat, string StrPCCode)
        {
            return dc.GetScanDts(strSrNo, StrDGSrNo, strCat, StrPCCode);
        }

        [HttpGet]
        [Route("TestReport/GetPrcChkDts")]
        public DataTable GetPrcChkDts(string strStageNo, string PrcStatusName)
        {
            return dc1.GetPrcChkDts(strStageNo, PrcStatusName);
        }

        [HttpGet]
        [Route("TestReport/GetProdDts")]
        public DataTable GetProdDts(string strPartCode, string StrDGSrNo, string strPfbCode)
        {
            return dc.GetProdDts(strPartCode, StrDGSrNo, strPfbCode);
        }

        [HttpGet]
        [Route("TestReport/Get6M")]
        public DataTable Get6M()
        {
            return dc.Get6M();
        }

        [HttpGet]
        [Route("TestReport/GetPrcStatus")]
        public DataTable GetPrcStatus()
        {
            return dc1.GetPrcStatus();
        }

        [HttpPost]
        [Route("TestReport/Submit")]
        public string Submit(TestReportRequest TestReportReq)
        {
            return dc.Submit(TestReportReq);
        }
    }
}
