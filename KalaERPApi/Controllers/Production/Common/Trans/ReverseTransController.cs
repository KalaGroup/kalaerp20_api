using KalaERPApi.Models.Request;
using KalaERPApi.Service;
using KalaERPApi.Service.Production.Common.Trans;
using KalaERPApi.Models.Production.Common.Trans;
using System.Data;
using System.Web.Http;

namespace KalaERPApi.Controllers.Production.Common.Trans
{
    public class ReverseTransController : ApiController
    {
        ReverseTransCon dc = new ReverseTransCon();
        DataTable ds = new DataTable();

        [HttpGet]
        [Route("ReverseTrans/GetRevTransMst")]
        public DataTable GetReverseMst(string PCCode)
        {
            return dc.GetReverseMst(PCCode);
        }

        [HttpGet]
        [Route("ReverseTrans/GetRevTransddl")]
        public DataTable GetRevTransKVA(int TransType, string ddlType, string PCCode, string KVA)
        {
            return dc.GetReverseddl( TransType,  ddlType,  PCCode, KVA);
        }

        [HttpGet]
        [Route("ReverseTrans/GetRevTransDts")]
        public DataTable GetRevTransDts(int TransType,  string PCCode, string KVA, string Model)
        {
            return dc.GetRevTransDts(TransType, PCCode, KVA, Model);
        }

        [HttpPost]
        [Route("ReverseTrans/Submit")]
        public string Submit([FromBody] ReverseTransRequest revtranreq)
        {
            return dc.Submit(revtranreq);
        }
    }
}
