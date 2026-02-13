using System.Web.Http;
using System.Data;
using KalaERPApi.Service;
using KalaERPApi.Service.Production.Common.Trans;
using KalaERPApi.Models.Request.Production.Common.Trans;

namespace KalaERPApi.Controllers.Production.Common.Trans
{
    public class QRScanController : ApiController
    {
        QRScanCon dc = new QRScanCon();
     
        [HttpGet]
        [Route("QRScan/GetQRlist")]
        public DataTable GetQRlist(string TransType, string PCCode, string srNo,string partcode,string transcode)
        {

            return dc.GetQRList(TransType, PCCode, srNo, partcode, transcode);
        }

        [HttpGet]
        [Route("QRScan/GetInvDGDtls")]
        public DataTable GetInvDGDtls(string CompCode, string InvNo, string SerialNo)
        {
            return dc.GetInvDGDtlsList(CompCode, InvNo, SerialNo);
        }

        [HttpPost]
        [Route("QRScan/Submit")]
        public string Submit(QRScanRequest QRScanReq)
        {
            return dc.Submit(QRScanReq);
        }
    }
}
