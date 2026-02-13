using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using KalaERPApi.Models.Request.Marketing.Plan;
using KalaERPApi.Models.Marketing.Auth;

namespace KalaERPApi.Service.Marketing.Auth
{
    public class MOF_Service
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        SqlTransaction tran = null;
        SqlCommand cmd = null;

        public string SubmitMOFNFALevel(MOFNFALevelAuth MOFNFALevelAuth)
        {
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                if (MOFNFALevelAuth.SaveType.Trim() == "Auth")
                {
                    #region

                    sb.Remove(0, sb.Length);
                    sb.Append("UPDATE MOF SET AuthNFA='1',AuthNFARemark='" + MOFNFALevelAuth.AuthRemark.Trim() + "',AuthRemark='record auth from apps'");
                    sb.Append(" WHERE MOFCode='" + MOFNFALevelAuth.MOFNo.Trim() + "' AND MOFType='N' AND Auth2='1' and AuthNFA='0'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO AuthorizationDetails");
                    sb.Append("(AuthDateTime,EmpID,AuthForm,TransactionNo,CompanyCode)");
                    sb.Append(" VALUES('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ,'" + MOFNFALevelAuth.UserID.Trim() + "','MOF NFA Level Auth','" + MOFNFALevelAuth.MOFNo.Trim() + "','07')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    #endregion
                }
                else if (MOFNFALevelAuth.SaveType.Trim() == "Hold")
                {
                    #region

                    sb.Remove(0, sb.Length);
                    sb.Append("UPDATE MOF SET Auth2='0',AuthNFARemark='" + MOFNFALevelAuth.AuthRemark.Trim() + "' WHERE MOFCode='" + MOFNFALevelAuth.MOFNo.Trim() + "' AND MOFType='N' AND Auth2='1'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO AuthorizationDetails");
                    sb.Append("(AuthDateTime,EmpID,AuthForm,TransactionNo,CompanyCode)");
                    sb.Append(" VALUES('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ,'" + MOFNFALevelAuth.UserID.Trim() + "','MOF Hold Level Auth','" + MOFNFALevelAuth.MOFNo.Trim() + "','07')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    #endregion
                }

                tran.Commit();
                return "Success";
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ("StackTrace" + ex.StackTrace.ToString() + "\n" + "Message" + ex.Message.ToString());
            }
            finally
            {
                con.Close();
            }
        }

        public string SubmitNFAEntry(NFAEntryAuth NFAEntryAuth)
        {
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                sb.Remove(0, sb.Length);
                sb.Append("UPDATE nfaentry SET auth='1',AuthRemark='"+ NFAEntryAuth.AuthRemark.Trim() + "' WHERE id='" + NFAEntryAuth.NFAID.Trim() + "'");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                sb.Remove(0, sb.Length);
                sb.Append("INSERT INTO AuthorizationDetails");
                sb.Append("(AuthDateTime,EmpID,AuthForm,TransactionNo,CompanyCode)");
                sb.Append(" VALUES('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ,'" + NFAEntryAuth.UserID.Trim() + "','NFA Auth','" + NFAEntryAuth.NFAID.Trim() + "','07')");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                tran.Commit();
                return "Success";
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ("StackTrace" + ex.StackTrace.ToString() + "\n" + "Message" + ex.Message.ToString());
            }
            finally
            {
                con.Close();
            }
        }

    }
}