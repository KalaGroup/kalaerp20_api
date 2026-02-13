using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using KalaERPApi.Models.Request;

namespace KalaERPApi.Service
{
    public class ProcessCheckPtsCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        SqlTransaction tran = null;
        SqlCommand cmd = null;
        public object SqlDbTypeChar { get; private set; }
               
        public DataTable GetProcessCheckListViewrpt(string Type, string ID, string ProcessName, string ProcessStatus, string FromDt, string ToDt)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("ProcessCheckListViewrpt_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.Parameters.Add("@ID", SqlDbType.Char).Value = ID;
            dAd.SelectCommand.Parameters.Add("@ProcessName", SqlDbType.Char).Value = ProcessName;
            dAd.SelectCommand.Parameters.Add("@ProcessStatus", SqlDbType.Char).Value = ProcessStatus;
            dAd.SelectCommand.Parameters.Add("@FromDt", SqlDbType.Char).Value = FromDt;
            dAd.SelectCommand.Parameters.Add("@ToDt", SqlDbType.Char).Value = ToDt;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string Submit(ProcessCheckPtsRequest ProcessCheckPtsRequest)
        {

            string[] strDts;
            int SrNo;
            string[] Dts;        
            string StrDisplayMsg = "";
            
            CommonCon ComCon = new CommonCon();
            DateTime sysdate = new DateTime(Convert.ToInt16(DateTime.Now.Year), Convert.ToInt16(DateTime.Now.Month), Convert.ToInt16(DateTime.Now.Day));
            try
            {

                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                //Save
                if (ProcessCheckPtsRequest.StrType.Trim() == "Save")
                {
                    // 
                    #region
                    if (!string.IsNullOrEmpty(ProcessCheckPtsRequest.CheckPointDesc.ToString().Trim()))
                    {
                        strDts = null;
                        strDts = Regex.Split(ProcessCheckPtsRequest.CheckPointDesc, "@#@");
                        SrNo = 0;
                        foreach (String StrSub in strDts)
                        {
                            SrNo += 1;
                            Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProcessCheckPoints ");
                            sb.Append("(Dt,PrcStatusName,ProcessName,CheckPointDesc,Remark,Active)");
                            sb.Append(" VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                            sb.Append("'" + ProcessCheckPtsRequest.PrcStatusName.Trim() + "',");
                            sb.Append("'" + ProcessCheckPtsRequest.ProcessName.Trim() + "',");
                            sb.Append("'" + Dts[1].ToString().Trim() + "',");
                            sb.Append("'" + ProcessCheckPtsRequest.Remark.Trim() + "',");
                            sb.Append(" '1');SELECT @@Identity;");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            object lblDispID = cmd.ExecuteScalar();
                            cmd.Dispose();

                            //**Save IN  PRV  tbl 
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProcessCheckPointsPrevious Select * from ProcessCheckPoints WHERE ID='" + lblDispID + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            //**************************

                            //**Update IN  PRV  tbl
                            sb.Remove(0, sb.Length);
                            sb.Append("Update ProcessCheckPointsPrevious SET ChangeDt= '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "' WHERE ID='" + lblDispID + "' and ChangeDt is null  ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            //**************************

                          
                            //DateTime.Now.ToString("yyyy-MM-dd")
                            //****************User Acivity****************
                            cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                            cmd.CommandType = CommandType.StoredProcedure;                          
                            cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@EmpID", ProcessCheckPtsRequest.EmpCode.Trim());
                            cmd.Parameters.AddWithValue("@TransactionType", "S");
                            cmd.Parameters.AddWithValue("@TransactionFrom", "ProcessCheckPoints");
                            cmd.Parameters.AddWithValue("@TransactionNo", Convert.ToInt16(lblDispID));
                            cmd.Parameters.AddWithValue("@CompanyCode", ProcessCheckPtsRequest.CompanyCode.Trim());
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }
                    }
                    #endregion
                    tran.Commit();
                 
                    StrDisplayMsg = "";
                    StrDisplayMsg = "Record Saved Successfully";
                }
                //Update 
                else if (ProcessCheckPtsRequest.StrType.Trim() == "Update")
                {
                    // 
                    #region
                    if (!string.IsNullOrEmpty(ProcessCheckPtsRequest.CheckPointDesc.ToString().Trim()))
                    {
                        strDts = null;
                        strDts = Regex.Split(ProcessCheckPtsRequest.CheckPointDesc, "@#@");
                        SrNo = 0;
                        foreach (String StrSub in strDts)
                        {
                            SrNo += 1;
                            Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            sb.Remove(0, sb.Length);
                            sb.Append("Update ProcessCheckPoints SET CheckPointDesc = '" + Dts[1].ToString().Trim()  + "' WHERE ID='" + ProcessCheckPtsRequest.ProcessID.Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con); 
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //**Save IN  PRV  tbl 
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO ProcessCheckPointsPrevious Select * from ProcessCheckPoints  WHERE ID='" + ProcessCheckPtsRequest.ProcessID.Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            //**************************

                            //**Update IN  PRV  tbl
                            sb.Remove(0, sb.Length);
                            sb.Append("Update ProcessCheckPointsPrevious SET ChangeDt= '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "' WHERE ID='" + ProcessCheckPtsRequest.ProcessID.Trim() + "' and ChangeDt is null  ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            //**************************

                          
                            //DateTime.Now.ToString("yyyy-MM-dd")
                            //****************User Acivity****************
                            cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            //cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                            cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@EmpID", ProcessCheckPtsRequest.EmpCode.Trim());
                            cmd.Parameters.AddWithValue("@TransactionType", "U");
                            cmd.Parameters.AddWithValue("@TransactionFrom", "ProcessCheckPoints");
                            cmd.Parameters.AddWithValue("@TransactionNo",  ProcessCheckPtsRequest.ProcessID.Trim() );
                            cmd.Parameters.AddWithValue("@CompanyCode", ProcessCheckPtsRequest.CompanyCode.Trim());
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }
                    }
                    #endregion
                    tran.Commit();

                    StrDisplayMsg = "";
                    StrDisplayMsg = "Update Saved Successfully";
                }

                return StrDisplayMsg;
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