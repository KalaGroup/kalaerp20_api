using KalaERPApi.Service;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace KalaERPApi.Controllers.Purchase.Trans
{
    public class SuppTOCController : ApiController
    {
        #region
        StringBuilder sb = new StringBuilder();
        CommonCon dc = new CommonCon();
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        public SqlTransaction tran = null;
        SqlCommand cmd = null;
        #endregion

        [HttpGet]
        [Route("SuppTOC/GetSuppInfo")]
        public DataTable GetSuppInfo(string SCode, string LoginType)
        {           
            return dc.procDT("exec GetSuppTOCInfo '"+ SCode + "','"+ LoginType + "'", "tbl_SupplierInfo");
        }

        [HttpGet]
        [Route("SuppTOC/GetPOItemDtls")]
        public DataTable GetPOItemDtls(string SCode, string LoginType)
        {       
            return dc.procDT("exec GetSuppTOCPartItems '" + SCode + "','" + LoginType + "','PO'", "tbl_POItemDetails");
        }

        [HttpGet]
        [Route("SuppTOC/GetBillItemDtls")]
        public DataTable GetBillItemDtls(string SCode, string LoginType)
        {
            return dc.procDT("exec GetSuppTOCPartItems '" + SCode + "','" + LoginType + "','Bill'", "tbl_BilltemDetails");                       
        }

        [HttpPost]
        [Route("SuppTOC/GetPOItemSubmit")]
        public string GetPOItemSubmit([FromBody] RaiseBill raiseBill)
        {
            #region
            string strDispCode = "";
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                strDispCode = dc.GetMaxNo("SuppTOCBill", "VCN", raiseBill.CompCode.Trim(), con, tran);

                sb.Remove(0, sb.Length);
                sb.Append("insert into SuppTOCBill(SChallanNo,Yr,MaxNo,SuppCode,CompanyCode) ");
                sb.Append("values('" + strDispCode.Trim() + "','" + strDispCode.Substring(4, 5) + "',");
                sb.Append("'" + strDispCode.Substring(10, 8) + "','" + raiseBill.SuppCode.Trim() + "','" + raiseBill.CompCode.Trim() + "')");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                //Details  
                #region
                string[] strMain = Regex.Split(raiseBill.POPartDts, "-->");
                int SrNo = 0;
                foreach (string strM in strMain)
                {
                    SrNo += 1;
                    string[] strDtls = Regex.Split(strM, ",");
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into SuppTOCBillDetails(SChallanNo,SrNo,POCode,PartCode,Qty) ");
                    sb.Append("values('" + strDispCode.Trim() + "','" + SrNo + "','" + strDtls[0].Trim() + "',");
                    sb.Append("'" + strDtls[1].Trim() + "','" + strDtls[2].Trim() + "')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("update PurchaseOrderDetails set SuppRaiseBillStatus='C' where POCode='" + strDtls[0].Trim() + "' and PartCode='" + strDtls[1].Trim() + "' and SuppRaiseBillStatus<>'C'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    int totRec = Convert.ToInt16(dc.getTranName("select count(SuppRaiseBillStatus) as Cnt from PurchaseOrderDetails where POCode='" + strDtls[0].Trim() + "'", "PurchaseOrderDetails", "Cnt", con, tran));
                    int cntDtls = Convert.ToInt16(dc.getTranName("select count(SuppRaiseBillStatus) as Cnt from PurchaseOrderDetails where POCode='" + strDtls[0].Trim() + "' and SuppRaiseBillStatus='C'", "PurchaseOrderDetails", "Cnt", con, tran));
                    if (totRec == cntDtls)
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("update PurchaseOrder set SuppRaiseBillStatus='C' where POCode='" + strDtls[0].Trim() + "' and SuppRaiseBillStatus<>'C'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                #endregion

                //****************User Acivity****************
                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@EmpID", raiseBill.SuppCode.Trim());
                cmd.Parameters.AddWithValue("@TransactionType", "S");
                cmd.Parameters.AddWithValue("@TransactionFrom", "SuppTOCBill");
                cmd.Parameters.AddWithValue("@TransactionNo", strDispCode.Trim());
                cmd.Parameters.AddWithValue("@CompanyCode", raiseBill.CompCode.Trim());
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                tran.Commit();
            }
            catch (Exception ex)
            {
                strDispCode = "StackTrace " + ex.StackTrace + ", Message" + ex.Message;
                tran.Rollback();
            }
            return strDispCode;

            #endregion
        }

    }

    public class RaiseBill
    {
        public string SuppCode { get; set; }
        public string CompCode { get; set; }
        public string POPartDts { get; set; }
    }
}
