using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using KalaERPApi.Service;

namespace KalaERPApi.Controllers
{
    public class ProcessController : ApiController
    {
        [HttpGet]
        [Route("Process/GetDGProcessStkMOFwithPart")]
        public DataTable GetDGProcessStkMOFwithPart(string compCode, string PCName)
        {
            DataTable ds = new DataTable();
            ProcessCon dc = new ProcessCon();
            ds = dc.DGPrcStockMOFWithPart(compCode, PCName);
            return ds;
        }
    }
}
