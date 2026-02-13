using System.Web.Http;
using System.Data;
using KalaERPApi.Service;
using KalaERPApi.Service.Production.Common.Trans;
using MTFScanRequest = KalaERPApi.Models.Request.Production.Common.Trans.MTFScanRequest;

namespace KalaERPApi.Controllers.Production.Common.Trans
{
    public class MTFScanController : ApiController
    {
        CommonCon cm = new CommonCon();
        MTFScanCon dc = new MTFScanCon();
        MTFWIPInternalCon mr = new MTFWIPInternalCon();

        [HttpGet]
        [Route("MTFScan/GetProfitcenter")]
        public DataTable GetMTFScanProfitcenter(string PCCode)
        {           
            return cm.GetPCodeAll(PCCode, "MTFScan");

        }
        [HttpGet]
        [Route("MTFScan/GetMTFCode")]
        public DataTable GetMTFScanMTFCode(string FPCCode,string TPCCode)
        {
            return dc.GetMTFCode(FPCCode, TPCCode);
        }

        [HttpGet]
        [Route("MTFScan/GetMTFProdDts")]
        public DataTable GetMTFProdDtls(string MTFCode)
        {           
            return mr.GetReqProdDetails(MTFCode);
        }

        [HttpGet]
        [Route("MTFScan/GetMTFDts")]
        public DataTable GetMTFDts(string MTFCode)
        {            
            return dc.GetMTFDts(MTFCode);
        }
        
        [HttpPost]
        [Route("MTFScan/Submit")]
        public string Submit(MTFScanRequest MTFscanReq)
        {           
            return dc.Submit(MTFscanReq);
        }
    }
}
