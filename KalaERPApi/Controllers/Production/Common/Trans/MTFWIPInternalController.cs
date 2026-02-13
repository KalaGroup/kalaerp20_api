using System.Web.Http;
using System.Data;
using KalaERPApi.Service;
using KalaERPApi.Service.Production.Common.Trans;
using KalaERPApi.Models.Request;


namespace KalaERPApi.Controllers.Common.Trans
{
    public class MTFWIPInternalController : ApiController
    {
        CommonCon cm = new CommonCon();
        MTFWIPInternalCon dc = new MTFWIPInternalCon();

        [HttpGet]
        [Route("MTF/GetProfitcenter")]
        public DataTable GetMTFProfitcenter(string PCCode)
        {  
            return cm.GetPCodeAll(PCCode, "MTF");
        }

        [HttpGet]
        [Route("MTF/GetReqCode")]
        public DataTable GetMTFGetReqCode(string FPCCode, string TPCCode)
        {
            return dc.GetReqCodeAll(FPCCode, TPCCode);
        }

        [HttpGet]
        [Route("MTF/GetReqProductDetails")]
        public DataTable GetMTFReqProductDetails(string ReqCode)
        {             
            return dc.GetReqProdDetails(ReqCode);
        }

        [HttpGet]
        [Route("MTF/GetReqDetails")]
      
        public DataTable GetMTFReqDetails(string PCCode, string strBomCode, string StrReqCode, double StrReqQty, double StrMTFQty)
        {
            return dc.GetReqDetails(PCCode, strBomCode, StrReqCode, StrReqQty, StrMTFQty);
        }

        [HttpPost]
        [Route("MTF/MTFWIPInternal/Submit")]
        public string Submit(MTFWIPInternalRequest MTFIntWIPreq)
        {           
            return dc.Submit(MTFIntWIPreq);
        }

        [HttpGet]
        [Route("MTF/GetSerialNoRpt")]
        public DataTable GetSerialNoRpt(string CompCode,string FromDt, string ToDt)
        {
            return dc.GetSerialNoDateWise(CompCode,FromDt, ToDt);

        }

    }
}
