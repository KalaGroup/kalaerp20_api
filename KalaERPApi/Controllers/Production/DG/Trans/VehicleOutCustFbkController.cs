
using KalaERPApi.Models.Transport.Trans;
using KalaERPApi.Service.Transport.Trans;
using System.Data;
using System.Web.Http;


namespace KalaERPApi.Controllers.Transport.Trans
{
    public class VehicleOutCustFbkController : ApiController
    {
        VehicleOutCustFbkCon dc = new VehicleOutCustFbkCon();

        [HttpGet]
        [Route("VehicleOutCustFbk/GetInvScanDts")]
        public DataTable GetInvScanDts(string strComp, string strInvNo)
        {
            return dc.GetInvScanDts(strComp, strInvNo);
        }

        [HttpPost]
        [Route("VehicleOutCustFbk/Submit")]
        public string Submit(VehicleOutCustFbkRequest VoutCustFbkReq)
        {
            return dc.Submit(VoutCustFbkReq);
        }
    }
}
