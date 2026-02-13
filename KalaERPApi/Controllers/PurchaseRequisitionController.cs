using KalaERPApi.Models.Response;
using KalaERPApi.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace KalaERPApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PurchaseRequisitionController : ApiController
    {
        [HttpGet]
        [Route("PurchaseRequisition/GetPurchaseReqPCName/companyCode")]
        public DataTable GetPurchaseReqPCName([FromUri]string compCode)
        {
            DataTable ds = new DataTable();
            DBConnection dc = new DBConnection();
            ds=dc.PurchaseRequsitionPCName(compCode);
            return ds;
        }

        [HttpGet]
        [Route("PurchaseRequisition/GetPartDescReqByClassCode/ClassCode")]
        public DataTable GetPartDescReqByClassCode([FromUri]string ClassCode)
        {
            DataTable ds = new DataTable();
            DBConnection dc = new DBConnection();
            ds = dc.GetPartDescReqByClassCode(ClassCode);
            return ds;
        }

        [HttpGet]
        [Route("PurchaseRequisition/GetPurchaseReqSuppName/PartCode")]
        public DataSet GetPurchaseReqSuppName([FromUri]string PartCode)
        {
            DataSet ds = new DataSet();
            DBConnection dc = new DBConnection();
            ds = dc.PurchaseRequsitionSuppName(PartCode);
            return ds;
        }

    }
}
