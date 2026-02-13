using System.Data;
using System.Web.Http;
using KalaERPApi.Service;
using KalaERPApi.Service.Transport.Trans;
using VehicleOutRequest = KalaERPApi.Models.Transport.Trans.VehicleOutRequest;


namespace KalaERPApi.Controllers.Transport.Trans
{
    public class VehicleOutController : ApiController
    {
        CommonCon ComCon = new CommonCon();
        VehicleOutCon dc = new VehicleOutCon();
        [HttpGet]
        [Route("VehicleOut/GetDGScanDts")]
        public DataTable GetDGScanDts(string strComp, string strInvNo, string strDGNo)
        {
            return dc.GetDGScanDts(strComp, strInvNo, strDGNo);
        }
                
        [HttpGet]
        [Route("VehicleOut/GetVehicleNo")]
        public DataTable GetVehicleNo(string CompCode)
        {
            return dc.GetVehicleNo(CompCode);
        }

        [HttpGet]
        [Route("VehicleOut/GetVehicleDtls")]
        public DataTable GetVehicleDtls(string VNo)
        {
            return dc.GetVehicleDtls(VNo);
        }

        [HttpGet]
        [Route("VehicleOut/GetVehicleNextDestination")]
        public DataTable GetVehicleNextDestination(string CompCode)
        {
            return dc.GetNextDestination(CompCode);
        }

        [HttpGet]
        [Route("VehicleOut/GetVehicleTransporterNameComp")]
        public DataTable GetVehicleTransporterNameComp()
        {
            return dc.GetVehicleTransporterNameComp();
        }

        [HttpGet]
        [Route("VehicleOut/GetVehicleTransporterName")]
        public DataTable GetVehicleTransporterName()
        {
            return dc.GetVehicleTransporterName();
        }

        [HttpPost]
        [Route("VehicleOut/Submit")]
        public string Submit(VehicleOutRequest VoutReq)
        {
            return dc.Submit(VoutReq);
        }
    }
}
