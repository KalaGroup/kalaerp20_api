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
    public class CustomerComplaintController : ApiController
    {
        //[HttpGet]
        //[Route("VisitingPlan/GetProfitcenter")]
        //public DataTable GetVistingPlanEmpInfo(string Code)
        //{
        //    DataTable ds = new DataTable();
        //    CommonCon dc = new CommonCon();
        //    ds = dc.GetEmpPCINFO("EmpPCINFO", Code);
        //    return ds;
        //}

        [HttpGet]
        [Route("Biotech/CustomerComplaint/GetCompProblemType")]
        public DataTable GetCompProblemType()
        {
            DataTable ds = new DataTable();
            CustomerComplaintCon dc = new CustomerComplaintCon();
            ds = dc.GetCompProblemType();
            return ds;
        }

        [HttpGet]
        [Route("Biotech/CustomerComplaint/GetCustSiteInvCode")]
        public DataTable GetCustSiteInvCode(string Code)
        {
            DataTable ds = new DataTable();
            CustomerComplaintCon dc = new CustomerComplaintCon();
            ds = dc.GetCustSiteInvCode(Code);
            return ds;
        }

        [HttpPost]
        [Route("Biotech/CustomerComplaint/Submit")]
        public string Submit([FromBody] CustomerComplaintRequest CustomerComplaintreq)
        {
            string strMsg = "";
            CustomerComplaintCon dc = new CustomerComplaintCon();
            strMsg = dc.Submit(CustomerComplaintreq);
            return strMsg;
        }
    }
}
