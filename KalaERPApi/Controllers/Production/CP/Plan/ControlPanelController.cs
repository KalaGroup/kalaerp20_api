using KalaERPApi.Models.Production.CP.Plan;
using KalaERPApi.Models.Request;
using KalaERPApi.Service;
using KalaERPApi.Service.Production.CP.Plan;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KalaERPApi.Controllers.Production.CP.Plan
{
    public class ControlPanelController : ApiController
    {

        ControlPanel_Con dc = new ControlPanel_Con();
        DataTable ds = new DataTable();



        [HttpGet]
        [Route("ControlPanel/GetCpPlan")]
        public DataTable GetCpPlan(string CompId)
        {
            return dc.GetCpPlan("Cpy_Plan", CompId);
        }

        [HttpPost]
        [Route("ControlPanel/SubmitCP")]
        public string SubmitCP([FromBody] ControlPanel_Request job_Cpyreq)
        {
            return dc.SubmitCP(job_Cpyreq);
        }
    }
}
