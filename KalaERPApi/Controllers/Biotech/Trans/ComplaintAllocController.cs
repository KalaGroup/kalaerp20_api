using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using KalaERPApi.Service;
using KalaERPApi.Models.Request.Biotech.Trans;
using KalaERPApi.Service.Biotech.Trans;
using System.Web.Http.Cors;
using System.IO;
namespace KalaERPApi.Controllers.Biotech.Trans
{

    // [EnableCors(origins: "http://www.kalapms.com:8282", headers: "*", methods: "*")]
    public class ComplaintAllocController : ApiController
    {

        [HttpGet]
        [Route("Biotech/ComplaintAlloc/GetProjectHead_Tech_Dealer")]
        public DataTable GetProjectHead(string Code, string Type)
        {
            DataTable ds = new DataTable();
            ComplaintAllocCon dc = new ComplaintAllocCon();
            ds = dc.GetProjectHead_Tech_Dealer(Code, Type);
            return ds;
        }

        [HttpGet]
        [Route("Biotech/ComplaintAlloc/GetAllocPendingComp")]
        public DataTable GetAllocPendingComp(string Code)
        {
            DataTable ds = new DataTable();
            ComplaintAllocCon dc = new ComplaintAllocCon();
            ds = dc.GetAllocPendingComp(Code);
            return ds;
        }


        [HttpGet]
        [Route("Biotech/ComplaintAlloc/GetSelectedCompType")]
        public DataTable GetSelectedCompType(string Code)
        {
            DataTable ds = new DataTable();
            ComplaintAllocCon dc = new ComplaintAllocCon();
            ds = dc.GetSelectedCompType(Code);
            return ds;
        }


        [HttpPost]
        [Route("Biotech/ComplaintAlloc/Submit")]
        public string Submit([FromBody] ComplaintAllocRequest ComplaintAllocreq)
        {
            string strMsg = "";
            ComplaintAllocCon dc = new ComplaintAllocCon();
            strMsg = dc.Submit(ComplaintAllocreq);
            return strMsg;
        }
    }
}
