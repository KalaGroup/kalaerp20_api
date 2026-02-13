using System.Web.Http;
using System.Data;
using KalaERPApi.Models.Request.Biotech.Trans;
using KalaERPApi.Service.Biotech.Trans;

namespace KalaERPApi.Controllers.Biotech.Trans
{
    // [EnableCors(origins: "http://www.kalapms.com:8282", headers: "*", methods: "*")]
    public class InvoiceBioSerController : ApiController
    {
        [HttpGet]
        [Route("Biotech/InvoiceBioSer/GetExpenditure")]
        public DataTable GetExpenditure()
        {
            DataTable ds = new DataTable();
            InvoiceBioSerCon dc = new InvoiceBioSerCon();
            ds = dc.GetExpenditure();
            return ds;
        }

        [HttpGet]
        [Route("Biotech/InvoiceBioSer/GetMtlLblProcuct")]
        public DataTable GetMtlLblProcuct(string Type)
        {
            DataTable ds = new DataTable();
            InvoiceBioSerCon dc = new InvoiceBioSerCon();
            ds = dc.GetMtlLblProcuct(Type);
            return ds;
        }

        [HttpGet]
        [Route("Biotech/InvoiceBioSer/getCGST")]
        public DataTable getCGST()
        {
            DataTable ds = new DataTable();
            InvoiceBioSerCon dc = new InvoiceBioSerCon();
            ds = dc.getCGST();
            return ds;
        }

        [HttpGet]
        [Route("Biotech/InvoiceBioSer/getSGST")]
        public DataTable getSGST()
        {
            DataTable ds = new DataTable();
            InvoiceBioSerCon dc = new InvoiceBioSerCon();
            ds = dc.getSGST();
            return ds;
        }

        [HttpGet]
        [Route("Biotech/InvoiceBioSer/getIGST")]
        public DataTable getIGST()
        {
            DataTable ds = new DataTable();
            InvoiceBioSerCon dc = new InvoiceBioSerCon();
            ds = dc.getIGST();
            return ds;
        }

        [HttpGet]
        [Route("Biotech/InvoiceBioSer/getTDS")]
        public DataTable getTDS()
        {
            DataTable ds = new DataTable();
            InvoiceBioSerCon dc = new InvoiceBioSerCon();
            ds = dc.getTDS();
            return ds;
        }

        [HttpGet]
        [Route("Biotech/InvoiceBioSer/GetProjectHead_Tech_Dealer")]
        public DataTable GetProjectHead(string Code, string Type)
        {
            DataTable ds = new DataTable();
            InvoiceBioSerCon dc = new InvoiceBioSerCon();
            ds = dc.GetProjectHead_Tech_Dealer(Code, Type);
            return ds;
        }

        [HttpGet]
        [Route("Biotech/InvoiceBioSer/GetInvoicePendingComp")]
        public DataTable GetActionPendingComp(string Code)
        {
            DataTable ds = new DataTable();
            InvoiceBioSerCon dc = new InvoiceBioSerCon();
            ds = dc.GetInvoicePendingComp(Code);
            return ds;
        }

        [HttpGet]
        [Route("Biotech/InvoiceBioSer/GetSelectedCompType")]
        public DataTable GetSelectedCompType(string Code)
        {
            DataTable ds = new DataTable();
            InvoiceBioSerCon dc = new InvoiceBioSerCon();
            ds = dc.GetSelectedCompType(Code);
            return ds;
        }

        [HttpGet]
        [Route("Biotech/InvoiceBioSer/GetPrevActionDetails")]
        public DataTable GetPrevActionDetails(string Code)
        {
            DataTable ds = new DataTable();
            InvoiceBioSerCon dc = new InvoiceBioSerCon();
            ds = dc.GetPrevActionDetails(Code);
            return ds;
        }

        [HttpPost]
        [Route("Biotech/InvoiceBioSer/Submit")]
        public string Submit([FromBody] InvoiceBioSerRequest InvoiceBioSerreq)
        {
            string strMsg = "";
            InvoiceBioSerCon dc = new InvoiceBioSerCon();
            strMsg = dc.Submit(InvoiceBioSerreq);
            return strMsg;
        }
    }

}
