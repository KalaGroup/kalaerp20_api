using System.Web.Http;
using System.Data;
using KalaERPApi.Service.UserActivity;
using KalaERPApi.Models.Request.UserActivity;
using KalaERPApi.Service;

namespace KalaERPApi.Controllers.UserActivity
{
    public class LoginAppController : ApiController
    {
        LoginAppCon dc = new LoginAppCon();
        DataTable ds = new DataTable();
        CommonCon cf = new CommonCon();

        [HttpGet]
        [Route("Login/UserDetails")]
        public DataTable UserDetails(string IMEINo)
        {          
            return dc.UserDetails(IMEINo);
        }

        [HttpGet]
        [Route("Login/KVendorUserDetails")]
        public DataTable KVendorUserDetails(string IMEINo)
        {
            return dc.KVendorUserDetails(IMEINo);
        }

        [HttpPost]
        [Route("Login/UserActivation")]
        public string UserActivation(string UserId, string Password, string mPin, string IMEINo)
        {           
            return dc.UserActivation(UserId, Password, mPin, IMEINo);
        }

        [HttpGet]
        [Route("Login/LoginCredintial")]
        public string LoginCredintial(string IMEINo, string mPin)
        {            
            return dc.LoginCredintial(IMEINo, mPin);
        }

        [HttpGet]
        [Route("Login/CheckLogin")]
        public DataTable CheckLogin(string UserName, string Psw)
        {            
            return dc.CheckLogin(UserName, Psw);
        }

        [HttpGet]
        [Route("Login/GetCompany")]
        public DataTable GetLoginCompany()
        { 
            ds = dc.GetCompanyName();
            return ds;
        }

        [HttpGet]
        [Route("Login/GetPageRights")]
        public DataTable GetPageRights(string userid, int pagetype, string comp_id, string pccode)
        {           
            ds = dc.GetPageRights(userid, pagetype, comp_id, pccode);
            return ds;
        }

        [HttpGet]
        [Route("Login/GetMainMenuApp")]
        public DataTable MainMenuApp(string userid, string comp_id)
        {           
            ds = dc.MainMenuApp(userid, comp_id);
            return ds;
        }

        [HttpPost]
        [Route("Login/DeviceLocation")]
        public string DeviceLocation(DeviceLocation deviceLocation)
        {           
            return dc.InsertDeviceLocation(deviceLocation);
        }

        [HttpGet]
        [Route("Login/Company")]
        public DataTable GetCompanyInfo(string Code)
        {  
            ds = cf.GetLoginCompInfo("Company", Code);
            return ds;
        }

        [HttpGet]
        [Route("Login/UserLogin")]
        public DataTable GetUserPassword(string Id, string Password)
        { 
            ds = cf.GetUserLoginInfo(Id, Password);
            return ds;
        }

    }
}
