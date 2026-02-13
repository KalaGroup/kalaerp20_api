using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using KalaERPApi.Models.Request.Production.Common.Trans;

namespace KalaERPApi.Service.Production.Common.Trans
{
    public class QRScanCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        StringBuilder sb = new StringBuilder();
        SqlTransaction tran = null;
        DataSet dSet = new DataSet();
        SqlDataAdapter dAd = null;
        SqlCommand cmd;

        public DataTable GetQRList(string TransType , string PCCode, string strSrNo, string strPartCode,string strtranscode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getQRScan4SrNoStock", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@TransType", SqlDbType.Char).Value = TransType;
            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
            dAd.SelectCommand.Parameters.Add("@SerialNo", SqlDbType.Char).Value = strSrNo;
            dAd.SelectCommand.Parameters.Add("@PartCode", SqlDbType.Char).Value = strPartCode;
            dAd.SelectCommand.Parameters.Add("@TransCode", SqlDbType.Char).Value = strtranscode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetInvDGDtlsList(string CompCode, string InvNo, string SerialNo)
        {
            dAd = new SqlDataAdapter("VehileLoading_SerialNo_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@CompCode", SqlDbType.Char).Value = CompCode;
            dAd.SelectCommand.Parameters.Add("@InvNo", SqlDbType.Char).Value = InvNo;
            dAd.SelectCommand.Parameters.Add("@SerialNo", SqlDbType.Char).Value = SerialNo;
            dAd.SelectCommand.CommandTimeout = 0;
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string Submit(QRScanRequest QRScanReq)
        {
            string QRScan = "";
           

            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                SqlCommand cmd = new SqlCommand();
                sb.Remove(0, sb.Length);
                sb.Append("INSERT INTO tbl_ScanedQRSrNo(PCCode,TransCode,TransDt,PartCode,SerialNo,BalQty,PhyQty)");
                sb.Append(" VALUES('" + QRScanReq.PCCode.Trim() + "','" + QRScanReq.TransCode.Trim() + "','" + QRScanReq.TransDt.Trim() + "','" + QRScanReq.PartCode.Trim() + "','" + QRScanReq.SerialNo.Trim() + "',");
                sb.Append("'" + double.Parse(QRScanReq.BalQty.Trim()) + "','" + double.Parse(QRScanReq.PhyQty.Trim()) + "')");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                tran.Commit();
                //tran.Rollback();
                return QRScan="Record Saved Sucessfully For Serial No="+ QRScanReq.SerialNo.Trim();
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