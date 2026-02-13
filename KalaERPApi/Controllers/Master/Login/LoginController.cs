using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using KalaERPApi.Service;

namespace KalaERPApi.Controllers.Master.Login
{
    public class LoginController : ApiController
    {
        [HttpGet]
        [Route("LoginAngular/Company")]
        public DataTable GetCompanyInfo(string Code)
        {
            DataTable ds = new DataTable();
            CommonCon dc = new CommonCon();
            ds = dc.GetLoginCompInfo("Company", Code);
            return ds;
        }

        [HttpGet]
        [Route("LoginAngular/UserLogin")]
        public DataTable GetUserPassword(string Id, string Password)
        {
            DataTable ds = new DataTable();
            CommonCon dc = new CommonCon();
            ds = dc.GetUserLoginInfo( Id, Password);
            return ds;
        }

    }
}
