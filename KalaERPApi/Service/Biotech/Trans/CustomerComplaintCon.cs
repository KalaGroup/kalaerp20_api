using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using KalaERPApi.Models.Request.Biotech.Trans;

namespace KalaERPApi.Service.Biotech.Trans
{
    public class CustomerComplaintCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        SqlTransaction tran = null;
        SqlCommand cmd = null;
        
        public DataTable GetCustSiteInvCode(string Type)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("LoadBiotechCustSiteInfo_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetCompProblemType()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("LoadBiotechCompType_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string Submit(CustomerComplaintRequest CustomerComplaintreq)
        {
            string[] strPlanDts;
            int SrNo;
            string[] Dts;
            string StrDispCode = "";
            string StrDisplayMsg = "";         
            CommonCon ComCon = new CommonCon();
                    
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                //SaveComplaint
                if (CustomerComplaintreq.StrType.Trim() == "Save")
                {
                    #region
                    StrDispCode = ComCon.GetMaxNo("CustomerComplaintBio", "CCB", CustomerComplaintreq.CompCode.Trim(), con, tran);
                    cmd = new SqlCommand("InsertCustomerComplaintBio", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CompNo", StrDispCode.Trim());
                    cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Yr", (StrDispCode.Substring(4, 5)));
                    cmd.Parameters.AddWithValue("@MaxSrNo", (StrDispCode.Substring(10, 8)));
                    cmd.Parameters.AddWithValue("@PCCode", CustomerComplaintreq.PCCode.Trim());
                    cmd.Parameters.AddWithValue("@EmpCode", CustomerComplaintreq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@CustomerCode", CustomerComplaintreq.CustomerCode.Trim());
                    cmd.Parameters.AddWithValue("@InvoiceNo", CustomerComplaintreq.InvNo.Trim());
                    cmd.Parameters.AddWithValue("@PartCode", CustomerComplaintreq.ProductCode.Trim());
                    cmd.Parameters.AddWithValue("@SiteID", CustomerComplaintreq.SiteID.Trim());
                    cmd.Parameters.AddWithValue("@NatureOfComplaint", CustomerComplaintreq.NatureOfComplaint.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", CustomerComplaintreq.CompCode.Trim());
                    cmd.Parameters.AddWithValue("@Remark", CustomerComplaintreq.Remark.Trim());
                    cmd.Parameters.AddWithValue("@MailStatus", "P");
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Details                    
                    strPlanDts = Regex.Split(CustomerComplaintreq.CompTypeDtls, "@#@");
                    SrNo = 0;
                    foreach (String StrSub in strPlanDts)
                    {
                        SrNo += 1;
                        Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                        cmd = new SqlCommand("InsertCustomerComplaintDetailsBio", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CompNo", StrDispCode.Trim());
                        cmd.Parameters.AddWithValue("@SrNo", SrNo);
                        cmd.Parameters.AddWithValue("@CompTypeCode", Dts[0].ToString().Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();            
                    }
                                      
                    //****************User Acivity****************
                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;                  
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", CustomerComplaintreq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "CustomerComplaintBio");
                    cmd.Parameters.AddWithValue("@TransactionNo", StrDispCode.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", CustomerComplaintreq.CompCode.Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                                       
                    StrDisplayMsg = "";
                    StrDisplayMsg = "Complaint Saved Successfully with Code : " + StrDispCode.Trim() + " ";
                                       
                    #endregion
                }               
                else if (CustomerComplaintreq.StrType.Trim() == "Update")
                {
                 
                }
                 tran.Commit();
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