using KalaERPApi.Models.Request;
using KalaERPApi.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KalaERPApi.Controllers
{
    public class JobCardController : ApiController
    {
        [HttpGet]
        [Route("JobCard/DGJobCard1/GetDG")]
        public DataTable GetDG()
        {
            DataTable ds = new DataTable();
           JobCardCon dc = new JobCardCon();
            ds = dc.GetDG();
            return ds;
        }
        [HttpPost]
        [Route("JobCard/DGJobCard1/Submit")]
        public string Submit(JobCardRequest jobreq)
        {
             //ds = new DataTable();
            string strMaxNo= "";
            JobCardCon dc = new JobCardCon();
            strMaxNo = dc.Submit(jobreq);
            return strMaxNo;
        }
        //[HttpPost]
        //[Route("JobCard/DGJobCard1/Save")]
        //public string Save(string TableName, string Prefix, string CompCode,string PCCode, string Remark)
        //{
        //    //ds = new DataTable();
        //    string strMaxNo = "";
        //    JobCardCon dc = new JobCardCon();
        //    strMaxNo = dc.GetMax(TableName, Prefix, CompCode);
        //    return strMaxNo;
        //}
    }
}
