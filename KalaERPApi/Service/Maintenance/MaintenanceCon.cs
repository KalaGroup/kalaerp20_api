using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.IO;

namespace KalaERPApi.Service.Maintenance.Trans
{
    public class MaintenanceCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        StringBuilder sb = new StringBuilder();
        DataSet dsChkStage = new DataSet();
        CommonCon ComCon = new CommonCon();
        string strSql = "";
        SqlTransaction tran = null;
        SqlCommand cmd;

        public DataTable GetMachineDtls(string MNo)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getMachineScanDtls", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@MachineNo", SqlDbType.Char).Value = MNo;
            //dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }


        public string Submit(string AMCCode, string PartCode, string MachineSrNo, string ForPCCode, string FromPCCode, string Reason, string EmpCode, string Remark)
        {
            string StrDisplayMsg = "";
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                SqlCommand cmd = new SqlCommand();
                string PrcNo = "";

                // Process & TR Start
                #region

                PrcNo = AMCCode.Trim();

                sb.Remove(0, sb.Length);
                sb.Append("INSERT INTO MaintenanceComplaint ");
                sb.Append(" VALUES('" + PrcNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                sb.Append("'" + (PrcNo.Substring(10, 8)) + "','" + ComCon.yearEnd(con, tran) + "',");
                sb.Append("'" + FromPCCode.Trim() + "','" + PartCode.Trim() + "','" + ForPCCode.Trim() + "',");
                sb.Append("'" + FromPCCode.Trim().Substring(0, 2) + "','" + EmpCode.Trim() + "',");
                sb.Append("'" + Remark.Trim() + "','P','M','1','1','0','0','0')");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();


                sb.Remove(0, sb.Length);
                sb.Append("Insert into MaintenanceComplaintDetails");
                sb.Append(" VALUES('" + PrcNo.Trim() + "',");
                sb.Append("'" + MachineSrNo.Trim() + "',");
                sb.Append("'" + Reason.ToString().Trim() + "',");
                sb.Append("'P','P',Null,'','NIL','0')");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();


                //****************User Acivity****************
                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@EmpID", EmpCode.Trim());
                cmd.Parameters.AddWithValue("@TransactionType", "S");
                cmd.Parameters.AddWithValue("@TransactionFrom", "MaintenanceComplaint");
                cmd.Parameters.AddWithValue("@TransactionNo", PrcNo.Trim());
                cmd.Parameters.AddWithValue("@CompanyCode", FromPCCode.Trim().Substring(0, 2));
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                StrDisplayMsg = "";
                StrDisplayMsg = "Complaint Saved Successfully with Code : " + PrcNo.Trim() + " ";
                #endregion
                // Process & TR End

                tran.Commit();
                return StrDisplayMsg.Trim();
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream("C:/Error/ApiError.txt", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter m_streamWriter = new StreamWriter(fs);
                m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                m_streamWriter.WriteLine();
                m_streamWriter.WriteLine("***************** " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:tt") + "*****************");
                m_streamWriter.WriteLine("StackTrace " + ex.StackTrace.ToString());
                m_streamWriter.WriteLine("Message " + ex.Message.ToString());
                m_streamWriter.Flush();
                m_streamWriter.Close();

                tran.Rollback();
                return ("StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString());
            }

            finally
            {
                con.Close();
            }
        }

        public string GetmaxPrc(string tablename, string fieldname, string Yr, string CompCode, SqlConnection con, SqlTransaction tran)
        {

            string PrcCode = "";
            string Max = "0";
            SqlCommand cmd = new SqlCommand("select max(substring(" + fieldname + ",13,7)) as MX from " + tablename.Trim() + " where yr='" + Yr.Trim() + "' and CompanyCode='" + CompCode.Trim() + "'", con, tran);
            cmd.CommandTimeout = 0;
            cmd.Transaction = tran;
            if ((cmd.ExecuteScalar().ToString() == System.DBNull.Value.ToString()))
            {
                Max = (CompCode + "000001");
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 9))
            {
                Max = (CompCode + ("00000" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 99))
            {
                Max = (CompCode + ("0000" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 999))
            {
                Max = (CompCode + ("000" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 9999))
            {
                Max = (CompCode + ("00" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 99999))
            {
                Max = (CompCode + ("0" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else
            {
                Max = (CompCode + (Convert.ToInt32(cmd.ExecuteScalar()) + 1));
            }
            PrcCode = "MCF" + "/" + Yr + "/" + Max;

            cmd.Dispose();
            return PrcCode;
        }


    }
}