using System.Web.Http;
using System.Data;
using KalaERPApi.Service;
using KalaERPApi.Models.Request.Marketing.Plan;
using KalaERPApi.Service.Marketing.Auth;
using System.Text;
using KalaERPApi.Models.Marketing.Auth;

namespace KalaERPApi.Controllers.Marketing.Plan
{
    public class MOFController : ApiController
    {
        StringBuilder sb = new StringBuilder();
        CommonCon dc = new CommonCon();
        MOF_Service ser = new MOF_Service();

        [HttpGet]
        [Route("NFAEntry/GetPndNFAEntry")]
        public DataTable GetPendingNFAEntry(string ID)
        {
            sb.Remove(0, sb.Length);
            sb.Append("select n.ID,convert(varchar(10),n.SysDt,103) as Dt,n.NFAStatus,n.kVA,n.Phase,n.Model,n.Panel ");
            sb.Append(",DealerOrKala=case substring(n.Dealer,1,3) when '04.' then o.GroupName else n.Dealer end ");
            sb.Append(",n.CustomerName,n.Qty,n.KalaPerSetAmt,KalaShare as ApprovedKalaShare,(n.Qty*n.KalaPerSetAmt) as KalaAmt ");
            sb.Append(",n.KoelApprQty,n.KoelPerSetAmt,KoelShare as ApprovedKoelShare,(n.Qty*n.KoelPerSetAmt) as KoelAmt ");
            sb.Append(",NFANo,SubmitTo,convert(varchar(10),n.SubmitDate,103) as SubmitDate,n.Remark,n.CIARemark ");
            sb.Append("from nfaentry n left outer join onaccountof o on n.Dealer=o.groupcode ");
            sb.Append("where n.active='1' and n.auth='0' and n.NFAStatus in('Cleared') order by n.sysdt desc");
            return dc.procDT(sb.ToString(), "tbl_PendingNFAEntry");
        }

        [HttpGet]
        [Route("MOFNFALevel/GetPndMOFNFA")]
        public DataTable GetPendingMOFNFA(string PCCode)
        {
            return dc.procDT("EXEC getAuthMOFApps_sp '0','1','1','1','0'", "tbl_PendingMOFNFA");
        }

        [HttpPost]
        [Route("NFAEntry/Submit")]
        public string SubmitNFAEntry([FromBody]NFAEntryAuth NFAEntryAuth)
        {
            return ser.SubmitNFAEntry(NFAEntryAuth);
        }

        [HttpPost]
        [Route("MOFNFALevel/Submit")]
        public string SubmitMOFNFALevel([FromBody]MOFNFALevelAuth MOFNFALevelAuth)
        {
            return ser.SubmitMOFNFALevel(MOFNFALevelAuth);
        }

    }
}
