using KalaERPApi.Models.Request;
using KalaERPApi.Service;
using System.Data;
using System.Web.Http;

namespace KalaERPApi.Controllers
{
    public class JobCardController : ApiController
    {
        JobCardCon dc = new JobCardCon();
        DataTable ds = new DataTable();

        [HttpGet]
        [Route("JobCard/DGJobCard1/GetDG")]
        public DataTable GetDG(string CompId)
        {            
            return dc.GetDG("DGWOP", CompId);
        }

        [HttpGet]
        [Route("JobCard/DGJobCard1/GetJabCard1Rpt")]
        public DataTable GetJOBCard1Report(string Type, string JobCardNo, string P_FromDt, string P_ToDt)
        {           
            ds = dc.GetJOBCard1Report(Type, JobCardNo, P_FromDt, P_ToDt);
            return ds;
        }

        [HttpGet]
        [Route("JobCard/DGJobCard2/GetJabCard2Rpt")]
        public DataTable GetJOBCard2Report(string Type, string JobCardNo, string P_FromDt, string P_ToDt)
        {            
            ds = dc.GetJOBCard2Report(Type, JobCardNo, P_FromDt, P_ToDt);
            return ds;
        }

        [HttpGet]
        [Route("JobCard/DGJobCardMTTR/GetJabCardMttrRpt")]
        public DataTable GetJabCardMttrRpt(string Type, string JobCardNo, string P_FromDt, string P_ToDt)
        {            
            ds = dc.GetJabCardMttrRpt(Type, JobCardNo, P_FromDt, P_ToDt);
            return ds;
        }

        [HttpGet]
        [Route("JobCard/DGJobCardMTTR/GetJabCardMttrRptNew")]
        public DataTable GetJabCardMttrRptNew(string Type, string CompanyCode, string JobCardNo, string P_FromDt, string P_ToDt)
        {
            ds = dc.GetJabCardMttrRptNew(Type, CompanyCode, JobCardNo, P_FromDt, P_ToDt);
            return ds;
        }

        [HttpGet]
        [Route("JobCard/DGJobCardMTTR/GetJabCardMttrRptNewQACkeckList")]
        public DataTable GetJabCardMttrRptNewQACkeckList(string StageType, string JobCardNo, string SerialNo)
        {
            ds = dc.GetJabCardMttrRptNewQACkeckList(StageType, JobCardNo, SerialNo);
            return ds;
        }

        [HttpGet]
        [Route("JobCard/DGJobCardMTTR/GetJabCardMttrChart")]
        public DataTable GetJabCardMttrChart(string StageType, string FromDt, string ToDt)
        {           
            ds = dc.GetJabCardMttrChart(StageType, FromDt, ToDt);
            return ds;
        }

        [HttpPost]
        [Route("JobCard/DGJobCard1/Submit")]
        public string Submit([FromBody] JobCardRequest jobreq)
        {
            return dc.Submit(jobreq);
        }
        
        [HttpGet]
        [Route("JobCard/DGJobCard2/GetDG")]
        public DataTable GetJobCard2DG(string CompId)
        {
            return dc.GetDG("DGWIP", CompId);
        }

        [HttpGet]
        [Route("JobCard/DGJobCard2/GetCP")]
        public DataTable GetJobCard2CP()
        {
            return dc.GetCP();
        }

        [HttpGet]
        [Route("JobCard/DGJobCard2/GetCPStk")]
        public string GetJobCard2CP(string StrKVA, string Ph, string PanelType, string LoginCompCode)
        {
            
            return dc.GetCPStk(StrKVA,Ph,PanelType, LoginCompCode);
        }

        [HttpPost]
        [Route("JobCard/DGJobCard2/Submit2")]
        public string Submit2([FromBody] JobCard2Request jobreq2)
        {
            return dc.Submit2(jobreq2);
        }

        //New
        [HttpGet]
        [Route("JobCard/DGAllocation")]
        public DataTable GetDGAllocation(string Type, string P_FromDt, string P_ToDt)
        {
            ds = dc.GetDGAllocationDts(Type, P_FromDt, P_ToDt);
            return ds;
        }

        [HttpGet]
        [Route("JobCard/TRQtyAllocation")]
        public DataTable GetTRAllocate(string MofCode, string PartCode, int DiQty)
        {
            ds = dc.GetTRAllocate(MofCode, PartCode, DiQty);
            return ds;
        }

        [HttpPost]
        [Route("JobCard/SubmitDiTRAllocate")]
        public string Submit([FromBody] DITRAllocationRequest DiTrAlloReq)
        {
            return dc.SubmitDiTRAllocate(DiTrAlloReq);
        }
    }
}
