using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.Http;
using KalaERPApi.Models.Request.Accounts.Trans;

namespace KalaERPApi.Service.Accounts.Invoice.Trans
{
    public class InvScanCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        StringBuilder sb = new StringBuilder();       
        public SqlTransaction tran = null;
        SqlCommand cmd = new SqlCommand();
        public object SqlDbTypeChar { get; private set; }

        public DataTable GetScanDtsInv(string strSrNo)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetInvoiceScanDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@InvBarcode", SqlDbType.Char).Value = strSrNo;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string Submit([FromBody] InvoiceScanRequest Invoicereq)
        {            
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();  
                if (Invoicereq.InvId.Trim().Substring(10, 2) == "01" || Invoicereq.InvId.Trim().Substring(10, 2) == "03" || Invoicereq.InvId.Trim().Substring(10, 2) == "08" || Invoicereq.InvId.Trim().Substring(10, 2) == "28")
                {
                    sb.Remove(0, sb.Length);
                    sb.Append("Update InvoiceSales set GateOut='D',GateOutTime=GetDate() where Invid='" + Invoicereq.InvId.Trim() + "'");                 
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                else
                {
                    sb.Remove(0, sb.Length);
                    sb.Append("Update InvoiceDealer set GateOut='D',GateOutTime=GetDate() where  Invid='" + Invoicereq.InvId.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                tran.Commit();             
                return Invoicereq.InvId.Trim();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ("StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString());
            }

            finally
            {
                con.Close();
            }
        }




    }

    }