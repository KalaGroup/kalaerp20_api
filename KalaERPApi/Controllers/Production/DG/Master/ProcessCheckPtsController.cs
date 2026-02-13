using KalaERPApi.Models.Request;
using KalaERPApi.Service;
using System.Data;
using System.Web.Http;

namespace KalaERPApi.Controllers
{
    public class ProcessCheckPtsController : ApiController
    {
        ProcessCheckPtsCon dc = new ProcessCheckPtsCon();

        [HttpGet]
        [Route("ProcessCheckPts/ProcessCheckListViewrpt")]
        public DataTable GetProcessCheckListViewrpt(string Type, string ID, string ProcessName, string ProcessStatus, string FromDt, string ToDt)
        {
            DataTable ds = new DataTable();
            ds = dc.GetProcessCheckListViewrpt(Type, ID, ProcessName, ProcessStatus, FromDt, ToDt);
            return ds;
        }

        [HttpPost]
        [Route("ProcessCheckPts/Submit")]
        public string Submit([FromBody] ProcessCheckPtsRequest ProcessCheckPtsReq)
        {
            return dc.Submit(ProcessCheckPtsReq);
        }

   }
}
