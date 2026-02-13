using KalaERPApi.Service;
using System.Data;
using System.Text;
using System.Web.Http;

namespace KalaERPApi.Controllers.Biotech.Reports
{
    public class ServiceMttrBioController : ApiController
    {
        StringBuilder sb = new StringBuilder();
        CommonCon dc = new CommonCon();

        [HttpGet]
        [Route("Biotech/GetBioServiceInvoice")]
        public DataTable GetBioServiceInvoice(string CompNo)
        {
            sb.Remove(0, sb.Length);
            sb.Append("exec getBioServiceInvoiceList_Rpt '" + CompNo.Trim() + "' ");
            return dc.procDT(sb.ToString(), "tbl_BioServiceInvoice");
        }


        [HttpGet]
        [Route("Biotech/GetBioServiceMTTR")]
        public DataTable GetBioServiceMTTR(string fromdate, string todate, string type)
        {
            sb.Remove(0, sb.Length);
            sb.Append("exec getBioServiceMTTR_Rpt '" + fromdate.Trim() + "','" + todate.Trim() + "','" + type.Trim() + "' ");
            return dc.procDT(sb.ToString(), "tbl_BioServiceMTTR");
        }
    }
}
