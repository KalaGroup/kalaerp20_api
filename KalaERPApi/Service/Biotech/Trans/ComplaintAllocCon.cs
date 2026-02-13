using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using Swashbuckle.Swagger;
using System.Text.RegularExpressions;
using KalaERPApi.Models.Request.Biotech.Trans;
using System.IO;

namespace KalaERPApi.Service.Biotech.Trans
{

    public class ComplaintAllocCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        SqlTransaction tran = null;
        SqlCommand cmd = null;
      
        public DataTable GetProjectHead_Tech_Dealer(string Code, string Type)
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("LoadAssignEmpTec_Bio_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = Code;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        
        public DataTable GetAllocPendingComp(string Type)
        {
           // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("LoadBiotechCompAssignDetail_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;

            //if (Type != "0")
            //{
            //    Dts = Regex.Split(Type.ToString().Trim(), "-->");

            //    dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = Dts[1].ToString().Trim();
            //    dAd.SelectCommand.Parameters.Add("@PartCode", SqlDbType.Char).Value = Dts[2].ToString().Trim();

            //}

           // dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = "0";
          //  dAd.SelectCommand.Parameters.Add("@PartCode", SqlDbType.Char).Value = "0";
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetSelectedCompType(string Type)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("[LoadSelectedCompTypeBio_Sp]", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string Submit(ComplaintAllocRequest ComplaintAllocreq)
        {
            string[] strPlanDts;
            int SrNo;
            string[] Dts;
            string StrDispCode = "";
            string StrDisplayMsg = "";
            CommonCon ComCon = new CommonCon();
            DateTime sysdate = new DateTime(Convert.ToInt16(DateTime.Now.Year), Convert.ToInt16(DateTime.Now.Month), Convert.ToInt16(DateTime.Now.Day));
                
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                //SaveComplaint
                if (ComplaintAllocreq.StrType.Trim() == "Save")
                {
                    #region
                    StrDispCode = ComCon.GetMaxNo("PrimaryCompAssignBio", "CPB", ComplaintAllocreq.CompCode.Trim(), con, tran);
                    //string Max =MTFCode.Substring(10,7).ToString();
                    // SqlCommand cmd = new SqlCommand();
                    cmd = new SqlCommand("InsertPrimaryCompAssignBio", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PCACode", StrDispCode.Trim());
                    cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Yr", (StrDispCode.Substring(4, 5)));
                    cmd.Parameters.AddWithValue("@MaxSrNo", (StrDispCode.Substring(10, 8)));
                    cmd.Parameters.AddWithValue("@CompNo", ComplaintAllocreq.CompNo.Trim());
                    cmd.Parameters.AddWithValue("@Priority", ComplaintAllocreq.Priority.Trim());
                    cmd.Parameters.AddWithValue("@WarrantyStatus", ComplaintAllocreq.WarrantyStatus.Trim());
                    cmd.Parameters.AddWithValue("@ProductCode", ComplaintAllocreq.ProductCode.Trim());
                    cmd.Parameters.AddWithValue("@CompType", ComplaintAllocreq.CompType.Trim());
                    cmd.Parameters.AddWithValue("@AssignProjectHead", ComplaintAllocreq.AssignProjectHead.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", ComplaintAllocreq.CompCode.Trim());
                    cmd.Parameters.AddWithValue("@Remark", ComplaintAllocreq.Remark.Trim());
                   // cmd.Parameters.AddWithValue("@MailStatus", "P");
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Details                    
                    strPlanDts = Regex.Split(ComplaintAllocreq.TechnicianDtls, "@#@");
                    SrNo = 0;
                    foreach (String StrSub in strPlanDts)
                    {
                        SrNo += 1;
                        Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                        cmd = new SqlCommand("InsertPrimaryCompAssignDetailsBio", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PCACode", StrDispCode.Trim());
                        cmd.Parameters.AddWithValue("@SrNo", SrNo);
                        cmd.Parameters.AddWithValue("@SupEmpCode", Dts[1].ToString().Trim());
                        cmd.Parameters.AddWithValue("@Type", "E");
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    strPlanDts = null;
                    strPlanDts = Regex.Split(ComplaintAllocreq.DealerDtls, "@#@");
                    SrNo = 0;
                    foreach (String StrSub in strPlanDts)
                    {
                        SrNo += 1;
                        Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                        cmd = new SqlCommand("InsertPrimaryCompAssignDetailsBio", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PCACode", StrDispCode.Trim());
                        cmd.Parameters.AddWithValue("@SrNo", SrNo);
                        cmd.Parameters.AddWithValue("@SupEmpCode", Dts[1].ToString().Trim());
                        cmd.Parameters.AddWithValue("@Type", "D");
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }


                 
                    //Update Status
                    //Update Customer Complaint Master
                    sb.Remove(0, sb.Length);
                    sb.Append("Update CustomerComplaintBio SET CompStatus='C' WHERE CompNo='" + ComplaintAllocreq.CompNo.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    // END Update Customer Compplaint Master


                    //DateTime.Now.ToString("yyyy-MM-dd")
                    //****************User Acivity****************
                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", ComplaintAllocreq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "PrimaryCompAssignBio");
                    cmd.Parameters.AddWithValue("@TransactionNo", StrDispCode.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", ComplaintAllocreq.CompCode.Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    
                                        
                    tran.Commit();

                    StrDisplayMsg = "";
                    StrDisplayMsg = "Complaint allocation save successfully with code : " + StrDispCode.Trim() + " ";
                                       
                    #endregion
                }
                //Update 
                else if (ComplaintAllocreq.StrType.Trim() == "Update")
                {
                 
                }
                
                return StrDisplayMsg;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                //   return null;
                return ("StackTrace" + ex.StackTrace.ToString() + "\n" + "Message" + ex.Message.ToString());
            }
            finally
            {
                con.Close();
            }
        }
    }
}