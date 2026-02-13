using KalaERPApi.Service;
using System.Data;
using System.Text;
using System.Web.Http;

namespace KalaERPApi.Controllers.Corporate.Reports
{
    public class CorporateRequisitionController : ApiController
    {
        StringBuilder sb = new StringBuilder();
        CommonCon dc = new CommonCon();

        [HttpGet]
        [Route("CorpReqMTTR/GetCorpReqMTTR")]
        //public DataTable GetProductRange(string Type, string Code, string PCCode, string EmpCode, string FromDt, string ToDt)
        //{
        //    sb.Remove(0, sb.Length);
        //    // sb.Append("exec getCorpReqMTTR_Rpt '" + Type.Trim() + "','" + Code.Trim() + "','" + PCCode.Trim() + "','" + EmpCode.Trim() + "','" + FromDt.Trim() + "','" + ToDt.Trim() + "' ");\
        //    sb.Append("exec getCorpReqMTTR_Rpt '" + Type.Trim() + "','" + Code.Trim() + "','" + PCCode.Trim() + "','" + EmpCode.Trim() + "','" + FromDt.Trim() + "','" + ToDt.Trim() + "' ");
        //    return dc.procDT(sb.ToString(), "tbl_CorpReqMTTR");
        //}

        public DataTable GetProductRange(string Type, string Code, string PCCode, string EmpCode, string FromDt, string ToDt, string PositionID)
        {
            sb.Remove(0, sb.Length);
            // sb.Append("exec getCorpReqMTTR_Rpt '" + Type.Trim() + "','" + Code.Trim() + "','" + PCCode.Trim() + "','" + EmpCode.Trim() + "','" + FromDt.Trim() + "','" + ToDt.Trim() + "' ");\
            sb.Append("exec getCorpReqMTTR_Rpt '" + Type.Trim() + "','" + Code.Trim() + "','" + PCCode.Trim() + "','" + EmpCode.Trim() + "','" + FromDt.Trim() + "','" + ToDt.Trim() + "' ,'" + PositionID.Trim() + "'  ");
            return dc.procDT(sb.ToString(), "tbl_CorpReqMTTR");
        }

    }
}
