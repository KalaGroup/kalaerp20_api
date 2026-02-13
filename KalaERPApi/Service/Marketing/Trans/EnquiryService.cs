using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using KalaERPApi.Models.Marketing.Trans;
using Microsoft.AspNetCore.Hosting.Server;
using Swashbuckle.Swagger;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace KalaERPApi.Service.Marketing.Trans
{
    public class EnquiryService
    {
        #region
        public SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].Trim());
        public StringBuilder sb = new StringBuilder();
        public SqlCommand cmd;
        CommonCon cls = new CommonCon();
        string emailStatus = "";
        #endregion

        public string Submit(EnquirySaveModel enquirySaveModel)
        {
            SqlTransaction tran = null;
            string EnqNo = "";
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); }
                ;
                tran = con.BeginTransaction();

                cmd = new SqlCommand("InsertEnquiryApps", con);
                cmd.CommandType = CommandType.StoredProcedure;

                //Plan
                cmd.Parameters.AddWithValue("@EmpCode", enquirySaveModel.EmpNo.Trim());
                cmd.Parameters.AddWithValue("@CompanyCode", enquirySaveModel.CompCode.Trim());
                cmd.Parameters.AddWithValue("@PCCode", enquirySaveModel.PCCode.Trim());
                cmd.Parameters.AddWithValue("@DomainID", enquirySaveModel.DomainID.Trim());
                cmd.Parameters.AddWithValue("@AreaID", enquirySaveModel.AreaID.Trim());

                //Enq 
                cmd.Parameters.AddWithValue("@CustName", enquirySaveModel.CustName.Trim());
                cmd.Parameters.AddWithValue("@RefFrom", enquirySaveModel.RefFrom.Trim());
                cmd.Parameters.AddWithValue("@MktExName", enquirySaveModel.MktExName.Trim());
                cmd.Parameters.AddWithValue("@Address", enquirySaveModel.Address.Trim());
                cmd.Parameters.AddWithValue("@ContactPerson", enquirySaveModel.ContactPerson.Trim());
                cmd.Parameters.AddWithValue("@MobileNo", enquirySaveModel.MobileNo.Trim());
                cmd.Parameters.AddWithValue("@EMailID", enquirySaveModel.EMailID.Trim());
                cmd.Parameters.AddWithValue("@CityID", enquirySaveModel.CityID.Trim());
                cmd.Parameters.AddWithValue("@EnqRemark", "Record saved from mobile apps");
                cmd.Parameters.AddWithValue("@ActualReqQty", "1");
                //Details 
                cmd.Parameters.AddWithValue("@PartCode_items", enquirySaveModel.PartCode_items.Trim());
                cmd.Parameters.AddWithValue("@Qty_items", enquirySaveModel.Qty_items.Trim());
                cmd.Parameters.AddWithValue("@FStatusID", enquirySaveModel.FStatusID.Trim());
                cmd.Parameters.AddWithValue("@NextFDate", enquirySaveModel.NextFDate.Trim());
                cmd.Parameters.AddWithValue("@FUpRemark", enquirySaveModel.FUpRemark.Trim());
                // chk enq login
                cmd.Parameters.AddWithValue("@ConfEMail", enquirySaveModel.ConfEMail.Trim());
                cmd.Parameters.AddWithValue("@ConfCustName", enquirySaveModel.ConfCustName.Trim());
                cmd.Parameters.AddWithValue("@ConfMobile", enquirySaveModel.ConfMobile.Trim());
                cmd.Parameters.AddWithValue("@AssignToEmpID", enquirySaveModel.AssignToEmpID.Trim());

                // Out Param
                cmd.Parameters.Add("@ENQNo_New", SqlDbType.NVarChar, 50);
                cmd.Parameters["@ENQNo_New"].Direction = ParameterDirection.Output;
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                EnqNo = (string)cmd.Parameters["@ENQNo_New"].Value.ToString();
                cmd.Dispose();

                tran.Commit();
                return EnqNo;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ("StackTrace" + ex.StackTrace.ToString() + "\n Message" + ex.Message.ToString());
            }
            finally
            {
                con.Close();
            }
        }

        public string UpdateEnquiry(EnquirySaveModel enquirySaveModel)
        {
            string CRGCode = cls.getName("select CRGCode from Enquiry where EnqNo='" + enquirySaveModel.EnqNo.Trim() + "' and active='1'", "tbl_Enquiry", "CRGCode");
            string stateid = cls.getName("select stateid from city where CID='" + enquirySaveModel.CityID.Trim() + "'", "tbl_city", "stateid");

            SqlTransaction tran = null;
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); }
                ;
                tran = con.BeginTransaction();

                //**Save IN PRV Enquiry 
                sb.Remove(0, sb.Length);
                sb.Append("INSERT INTO EnquiryPrevious Select * from Enquiry WHERE EnqNo='" + enquirySaveModel.EnqNo.Trim() + "'");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                //**Save IN PRV EnquiryDetails 
                sb.Remove(0, sb.Length);
                sb.Append("INSERT INTO EnquiryPreviousDetails Select * from EnquiryDetails WHERE EnqNo='" + enquirySaveModel.EnqNo.Trim() + "' order by srno");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                //**Save IN PRV EnquiryDetails 
                sb.Remove(0, sb.Length);
                sb.Append("INSERT INTO EnquiryPreviousFollowupDetails Select top 1 * from EnquiryFollowupDetails WHERE EnqNo='" + enquirySaveModel.EnqNo.Trim() + "' order by dt desc");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                //**************************

                //**Update Enquiry Last Followp Actual Qty
                sb.Remove(0, sb.Length);
                sb.Append("UPDATE Enquiry SET CustName='" + enquirySaveModel.CustName.Trim() + "',Address='" + enquirySaveModel.Address.Trim() + "'");
                sb.Append(",ContactPerson='" + enquirySaveModel.ContactPerson.Trim() + "',MobileNo='" + enquirySaveModel.MobileNo.Trim() + "'");
                sb.Append(",EMailID='" + enquirySaveModel.EMailID.Trim() + "',RefFrom='" + enquirySaveModel.RefFrom.Trim() + "'");
                sb.Append(",City='" + enquirySaveModel.CityID.Trim() + "',State ='" + stateid.Trim() + "',ConfEMail ='" + enquirySaveModel.ConfEMail.Trim() + "'");
                sb.Append(",ConfCustName='" + enquirySaveModel.ConfCustName.Trim() + "',ConfMobile='" + enquirySaveModel.ConfMobile.Trim() + "'");
                sb.Append(",Remark ='Record updated from mobile apps',AssignToEmpID='" + enquirySaveModel.AssignToEmpID + "' WHERE EnqNo='" + enquirySaveModel.EnqNo.Trim() + "' ");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                string[] PartCodeCount = Regex.Split(enquirySaveModel.PartCode_items.Trim(), ",");
                string[] QtyCount = Regex.Split(enquirySaveModel.Qty_items.Trim(), ",");
                if (PartCodeCount.Length > 0)
                {
                    //**Delete PRV EnquiryDetails 
                    sb.Remove(0, sb.Length);
                    sb.Append("delete from EnquiryDetails WHERE EnqNo='" + enquirySaveModel.EnqNo.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    int FID = 0;
                    int cntNoDtl = 0;
                    for (int i = 0; i < PartCodeCount.Length; i++)
                    {
                        cntNoDtl = (cntNoDtl + 1);
                        //******************************Code to insert Followup details
                        cmd = new SqlCommand("InsertUpdateEnquiryFollowupDetailsD", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EnqNo", enquirySaveModel.EnqNo.Trim());
                        cmd.Parameters.AddWithValue("@PartCode", PartCodeCount[i].Trim());
                        cmd.Parameters.AddWithValue("@EmpID", enquirySaveModel.EmpNo.Trim());
                        cmd.Parameters.AddWithValue("@NextFDate", enquirySaveModel.NextFDate.Trim());
                        cmd.Parameters.AddWithValue("@QtnStatus", "P");
                        cmd.Parameters.AddWithValue("@FStatusID", enquirySaveModel.FStatusID.Trim());
                        cmd.Parameters.AddWithValue("@Remark", enquirySaveModel.FUpRemark.Trim());
                        if (enquirySaveModel.FStatusID.Trim() == "03") // Enquiry Lost 
                        {
                            cmd.Parameters.AddWithValue("@ToWhom", enquirySaveModel.CompititorName.Trim());
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ToWhom", "Record updated from mobile apps");
                        }
                        cmd.Parameters.AddWithValue("@OrderStatus", "PQ");
                        cmd.Parameters.AddWithValue("@ActualReqQty", "1");
                        cmd.Parameters.AddWithValue("@UpdateBy", enquirySaveModel.EmpNo.Trim());
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        FID = (int)cmd.Parameters["@ID"].Value;
                        cmd.Dispose();

                        //Update CallRegister Followup Status
                        sb.Remove(0, sb.Length);
                        sb.Append("Update CallRegister Set FStatusID='" + enquirySaveModel.FStatusID.Trim() + "' Where CRGCode='" + CRGCode.Trim() + "' and CRGCode<>'0'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //*******************************
                        cmd = new SqlCommand("InsertUpdateEnquiryDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@EnqNo", enquirySaveModel.EnqNo.Trim());
                        cmd.Parameters.AddWithValue("@SrNo", cntNoDtl);
                        cmd.Parameters.AddWithValue("@PartCode", PartCodeCount[i].Trim());
                        cmd.Parameters.AddWithValue("@Qty", QtyCount[i].Trim());
                        cmd.Parameters.AddWithValue("@FID", FID.ToString().Trim());
                        cmd.Parameters.AddWithValue("@Status", 0);
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }

                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@EmpID", enquirySaveModel.EmpNo.Trim());
                cmd.Parameters.AddWithValue("@TransactionType", "U");
                cmd.Parameters.AddWithValue("@TransactionFrom", "Enquiry");
                cmd.Parameters.AddWithValue("@TransactionNo", enquirySaveModel.EnqNo.Trim());
                cmd.Parameters.AddWithValue("@CompanyCode", enquirySaveModel.CompCode.Trim());
                cmd.Transaction = tran;
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                tran.Commit();
                return enquirySaveModel.EnqNo.Trim();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ("StackTrace" + ex.StackTrace.ToString() + "\n Message" + ex.Message.ToString());
            }
            finally
            {
                con.Close();
            }
        }

        public string UpdateFollowUp(FollowUpUpdate followUpUpdate)
        {
            string CRGCode = cls.getName("select CRGCode from Enquiry where EnqNo='" + followUpUpdate.EnqNo.Trim() + "' and active='1'", "tbl_Enquiry", "CRGCode");
            SqlTransaction tran = null;
            string FID = "";
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); }
                ;
                tran = con.BeginTransaction();

                cmd = new SqlCommand("InsertFollowupApps", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@EnqNo", followUpUpdate.EnqNo.Trim());
                cmd.Parameters.AddWithValue("@PartCode", followUpUpdate.PartCode.Trim());
                cmd.Parameters.AddWithValue("@EmpCode", followUpUpdate.EmpCode.Trim());
                cmd.Parameters.AddWithValue("@FStatusID", followUpUpdate.FStatusID.Trim());
                cmd.Parameters.AddWithValue("@NextFDate", followUpUpdate.NextFDate.Trim());
                cmd.Parameters.AddWithValue("@FUpRemark", followUpUpdate.FUpRemark.Trim());
                if (followUpUpdate.FStatusID.Trim() == "03") // Enquiry Lost 
                {
                    cmd.Parameters.AddWithValue("@ToWhom", followUpUpdate.CompititorName.Trim());
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ToWhom", "record saved from apps");
                }
                cmd.Parameters.AddWithValue("@AssignToEmpID", followUpUpdate.AssignToEmpID.Trim());
                // Out Param
                cmd.Parameters.Add("@FID_New", SqlDbType.NVarChar, 50);
                cmd.Parameters["@FID_New"].Direction = ParameterDirection.Output;
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                FID = (string)cmd.Parameters["@FID_New"].Value.ToString();
                cmd.Dispose();

                //Update CallRegister Followup Status
                sb.Remove(0, sb.Length);
                sb.Append("Update CallRegister Set FStatusID='" + followUpUpdate.FStatusID.Trim() + "' Where CRGCode='" + CRGCode.Trim() + "' and CRGCode<>'0'");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                tran.Commit();
                return "Success:" + FID;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ("StackTrace" + ex.StackTrace.ToString() + "\n Message" + ex.Message.ToString());
            }
            finally
            {
                con.Close();
            }
        }

        public string SubmitAngular(EnquirySaveModelAngular enquirySaveModel)
        {
            if (enquirySaveModel.SaveType.Trim() == "NewEnquiry")
            {
                //skk
                string alreadySave = cls.getName("select EnqNo from Enquiry where VPCode='" + enquirySaveModel.VPCode.Trim() + "' and VPSrNo='" + enquirySaveModel.VPSrNo.Trim() + "' and Active='1'", "Enquiry", "EnqNo");
                if (alreadySave != "0")
                {
                    return enquirySaveModel.VPCode + " Enquiry Already Saved for Customer " + enquirySaveModel.CustName;
                }
            }
            string CRGCode = cls.getName("select CRGCode from Enquiry where EnqNo='" + enquirySaveModel.EnqNo.Trim() + "' and active='1'", "tbl_Enquiry", "CRGCode");

            SqlTransaction tran = null;
            int SrNo = 0;
            string EPGNo, EnqNo = "", strProc = "", QtnNo = "", kva_items = "", emailStatus = "";
            string[] enqPartDts, quotPartDts, addPartDts, instOfferDts, enqDts, quotDts, addDts, instDts;
            string fromDt = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            string toDt = DateTime.Now.AddDays(60).ToString("yyyy-MM-dd") + " 23:59:59";

            string AssToEmpPCCode = "";
            try
            {
                AssToEmpPCCode = cls.getName("select ProfitCenter from Employee where Ecode='" + enquirySaveModel.AssToEmpCode.Trim() + "'", "tbl_Emp", "ProfitCenter");

                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); }
                ;
                tran = con.BeginTransaction();

                if (enquirySaveModel.SaveType.Trim() == "NewEnquiry")
                {
                    //Plan
                    #region
                    EPGNo = cls.getName("select top 1 ep.EGPCode from EnquiryGenerationPlan ep " +
                        " inner join EnquiryGenerationPlanDetails epd on ep.EGPCode=epd.EGPCode where ep.EmpCode='" + enquirySaveModel.AssToEmpCode.Trim() + "'" +
                       " and epd.EGPStatus='P' and ep.EGPStatus='P' and ep.ProfitCenterCode='" + AssToEmpPCCode.Trim() + "'" +
                        " and ep.FromDate between '" + fromDt.Trim() + "' and '" + toDt.Trim() + "' and ep.Active='1' order by dt desc", "EnquiryGenerationPlan", "EGPCode");
                    if (EPGNo == "0")
                    {
                        EPGNo = cls.GetMaxNo("EnquiryGenerationPlan", "EGP", enquirySaveModel.CompID.Trim(), con, tran);
                    }

                    cmd = new SqlCommand("InsertEnquiryGenerationPlan", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SearchType", "S");
                    cmd.Parameters.AddWithValue("@EGPCode", EPGNo.Trim());
                    cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@MaxSrNo", EPGNo.Substring(10, 8));
                    cmd.Parameters.AddWithValue("@Yr", EPGNo.Substring(4, 5));
                    cmd.Parameters.AddWithValue("@FromDate", fromDt.Trim());
                    cmd.Parameters.AddWithValue("@ToDate", toDt.Trim());
                    cmd.Parameters.AddWithValue("@EmpCode", enquirySaveModel.AssToEmpCode.Trim());
                    cmd.Parameters.AddWithValue("@remark", enquirySaveModel.EnqRemark.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", enquirySaveModel.CompID.Trim());
                    cmd.Parameters.AddWithValue("@ProfitCenterCode", AssToEmpPCCode.Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    cmd = new SqlCommand("InsertEnquiryGenerationPlanDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SearchType", "S");
                    cmd.Parameters.AddWithValue("@EGPCode", EPGNo.Trim());
                    cmd.Parameters.AddWithValue("@SrNo", '1');
                    cmd.Parameters.AddWithValue("@DomainID ", enquirySaveModel.DomainID.Trim());
                    cmd.Parameters.AddWithValue("@AreaID", enquirySaveModel.CityID.Trim());
                    cmd.Parameters.AddWithValue("@KVA", "0");
                    cmd.Parameters.AddWithValue("@Qty", "50");
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", enquirySaveModel.AssToEmpName.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "EnquiryGenerationPlan");
                    cmd.Parameters.AddWithValue("@TransactionNo", EPGNo.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", enquirySaveModel.CompID.Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    #endregion

                    //Enquiry
                    #region
                    EnqNo = cls.GetMaxNo("Enquiry", "ENQ", enquirySaveModel.CompID.Trim(), con, tran);

                    cmd = new SqlCommand("InsertUpdateEnquiry", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SearchType", "S");
                    cmd.Parameters.AddWithValue("@EnqNo", EnqNo);
                    cmd.Parameters.AddWithValue("@MaxSrNo", EnqNo.Substring(10, 8));
                    cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Yr", EnqNo.Substring(4, 5));
                    cmd.Parameters.AddWithValue("@PrvEnqNo", "0");
                    cmd.Parameters.AddWithValue("@EGPCode", EPGNo.Trim());
                    cmd.Parameters.AddWithValue("@CRGCode", "0");
                    cmd.Parameters.AddWithValue("@ENQCODEStatus", "N");
                    cmd.Parameters.AddWithValue("@EnqCODCode", enquirySaveModel.UserID.Trim());
                    cmd.Parameters.AddWithValue("@DomainID", enquirySaveModel.DomainID.Trim());
                    cmd.Parameters.AddWithValue("@CustName", enquirySaveModel.CustName.Trim());
                    cmd.Parameters.AddWithValue("@RefFrom", enquirySaveModel.RefFrom.Trim());
                    cmd.Parameters.AddWithValue("@MktExName", enquirySaveModel.AssToEmpName.Trim());
                    cmd.Parameters.AddWithValue("@Address", enquirySaveModel.Address.Trim());
                    cmd.Parameters.AddWithValue("@ContactPerson", enquirySaveModel.ContactPerson.Trim());
                    cmd.Parameters.AddWithValue("@OfficePhNo", enquirySaveModel.OfficePhNo.Trim());
                    cmd.Parameters.AddWithValue("@ResPhNo", "");
                    cmd.Parameters.AddWithValue("@MobileNo", enquirySaveModel.MobileNo.Trim());
                    cmd.Parameters.AddWithValue("@EMailID", enquirySaveModel.EMailID.Trim());
                    cmd.Parameters.AddWithValue("@FaxNo", "");
                    cmd.Parameters.AddWithValue("@Country", "01");
                    cmd.Parameters.AddWithValue("@State", cls.getName("select StateID from City where CID='" + enquirySaveModel.CityID.Trim() + "'", "tbl_City", "StateID"));
                    cmd.Parameters.AddWithValue("@City", enquirySaveModel.CityID.Trim());
                    cmd.Parameters.AddWithValue("@Prodtype", "01");
                    cmd.Parameters.AddWithValue("@CompanyCode", enquirySaveModel.CompID.Trim());

                    cmd.Parameters.AddWithValue("@BranchCode", AssToEmpPCCode.Trim());
                    cmd.Parameters.AddWithValue("@Remark", enquirySaveModel.EnqRemark.Trim());
                    cmd.Parameters.AddWithValue("@AssignToEmpID", enquirySaveModel.AssToEmpCode.Trim());
                    cmd.Parameters.AddWithValue("@ActualReqQty", enquirySaveModel.ActualReqQty.Trim());
                    cmd.Parameters.AddWithValue("@ConfEMail", enquirySaveModel.ConfEMail.Trim());
                    cmd.Parameters.AddWithValue("@ConfCustAssign", "N");
                    cmd.Parameters.AddWithValue("@ConfCustName", enquirySaveModel.ConfCustName.Trim());
                    cmd.Parameters.AddWithValue("@ConfMobile", enquirySaveModel.ConfMobile.Trim());
                    cmd.Parameters.AddWithValue("@CPMktHead", 0);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    if (enquirySaveModel.SendQuot.Trim() == "true")
                    {
                        sb.Append("UPDATE Enquiry SET VPCode='" + enquirySaveModel.VPCode.Trim() + "',VPSrNo='" + enquirySaveModel.VPSrNo.Trim() + "',QuotSendStatus='Y' WHERE enqno='" + EnqNo.Trim() + "'");
                    }
                    else
                    {
                        sb.Append("UPDATE Enquiry SET VPCode='" + enquirySaveModel.VPCode.Trim() + "',VPSrNo='" + enquirySaveModel.VPSrNo.Trim() + "' WHERE enqno='" + EnqNo.Trim() + "'");
                    }
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Details                    
                    enqPartDts = Regex.Split(enquirySaveModel.EnqPartCode_items, "@#@");
                    int FID = 0; SrNo = 0;
                    foreach (string enqSub in enqPartDts)
                    {
                        enqDts = Regex.Split(enqSub.Trim(), "-->");
                        SrNo += 1;
                        cmd = new SqlCommand("InsertUpdateEnquiryFollowupDetailsD", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EnqNo", EnqNo.Trim());
                        cmd.Parameters.AddWithValue("@PartCode", enqDts[0].Trim());
                        cmd.Parameters.AddWithValue("@EmpID", enquirySaveModel.AssToEmpCode.Trim());
                        cmd.Parameters.AddWithValue("@NextFDate", enquirySaveModel.NextFDate.Trim());
                        if (enquirySaveModel.SendQuot.Trim() == "true")
                        {
                            cmd.Parameters.AddWithValue("@QtnStatus", "C");
                        }
                        else if (enquirySaveModel.SendQuot.Trim() == "false")
                        {
                            cmd.Parameters.AddWithValue("@QtnStatus", "P");
                        }
                        cmd.Parameters.AddWithValue("@FStatusID", enquirySaveModel.FStatusID.Trim());
                        cmd.Parameters.AddWithValue("@Remark", enquirySaveModel.FUpRemark.Trim());
                        cmd.Parameters.AddWithValue("@ToWhom", "");
                        cmd.Parameters.AddWithValue("@OrderStatus", "PQ");
                        cmd.Parameters.AddWithValue("@ActualReqQty", enquirySaveModel.ActualReqQty.Trim());
                        cmd.Parameters.AddWithValue("@UpdateBy", enquirySaveModel.UserID.Trim());
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        FID = (int)cmd.Parameters["@ID"].Value;
                        cmd.Dispose();

                        //Update CallRegister Followup Status
                        sb.Remove(0, sb.Length);
                        sb.Append("Update CallRegister Set FStatusID='" + enquirySaveModel.FStatusID.Trim() + "' Where CRGCode='" + CRGCode.Trim() + "' and CRGCode<>'0'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        cmd = new SqlCommand("InsertUpdateEnquiryDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@EnqNo", EnqNo.Trim());
                        cmd.Parameters.AddWithValue("@SrNo", SrNo);
                        cmd.Parameters.AddWithValue("@PartCode", enqDts[0].Trim());
                        cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(enqDts[1].Trim()));
                        cmd.Parameters.AddWithValue("@FID", FID.ToString().Trim());
                        cmd.Parameters.AddWithValue("@Status", 0);
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", enquirySaveModel.UserID.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "Enquiry");
                    cmd.Parameters.AddWithValue("@TransactionNo", EnqNo.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", enquirySaveModel.CompID.Trim());
                    cmd.Transaction = tran;
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    #endregion

                    //Quat        
                    if (enquirySaveModel.SendQuot.Trim() == "true")
                    {
                        #region
                        //Master
                        #region
                        QtnNo = cls.GetMaxNo("Quotation", "QTN", enquirySaveModel.CompID.Trim(), con, tran);

                        sb.Remove(0, sb.Length);
                        cmd = new SqlCommand("InsertUpdateQuotationD", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@QtnNo", QtnNo);
                        cmd.Parameters.AddWithValue("@MaxSrNo", QtnNo.Substring(10, 8));
                        cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Yr", QtnNo.Substring(4, 5));
                        cmd.Parameters.AddWithValue("@EnqNo", EnqNo.Trim());
                        cmd.Parameters.AddWithValue("@AssignToEmpID", enquirySaveModel.AssToEmpCode.Trim());
                        cmd.Parameters.AddWithValue("@AdditionStand", "NIL");
                        cmd.Parameters.AddWithValue("@PriceCommercial", "NIL");
                        cmd.Parameters.AddWithValue("@Taxation", "NIL");
                        cmd.Parameters.AddWithValue("@TermsCondition", "NIL");
                        cmd.Parameters.AddWithValue("@QtnStatus", "P");
                        cmd.Parameters.AddWithValue("@QtnShow", "Y");
                        cmd.Parameters.AddWithValue("@ExciseDuty", "0");
                        cmd.Parameters.AddWithValue("@CESS", "0");
                        cmd.Parameters.AddWithValue("@HEdCESS", "0");
                        cmd.Parameters.AddWithValue("@VAT", "0");
                        cmd.Parameters.AddWithValue("@CST", "0");
                        cmd.Parameters.AddWithValue("@ServiceTax", "0");
                        cmd.Parameters.AddWithValue("@EntryTax", "0");
                        cmd.Parameters.AddWithValue("@Other", "0");
                        cmd.Parameters.AddWithValue("@SurCharge", "0");
                        cmd.Parameters.AddWithValue("@Octri", "0");
                        cmd.Parameters.AddWithValue("@OctriBy", "C");
                        cmd.Parameters.AddWithValue("@Transport", "0");
                        cmd.Parameters.AddWithValue("@TransportBy", "C");
                        cmd.Parameters.AddWithValue("@Unloading", "0");
                        cmd.Parameters.AddWithValue("@UnloadingBy", "C");
                        cmd.Parameters.AddWithValue("@Insurance", "0");
                        cmd.Parameters.AddWithValue("@InsuranceBy", "C");
                        cmd.Parameters.AddWithValue("@Packing", "0");
                        cmd.Parameters.AddWithValue("@PackingBy", "C");
                        cmd.Parameters.AddWithValue("@Specification", "");
                        cmd.Parameters.AddWithValue("@CompanyCode", enquirySaveModel.CompID.Trim());
                        //ASK pc CODE tO sVAE
                        // cmd.Parameters.AddWithValue("@BranchCode", enquirySaveModel.PCCode.Trim());
                        cmd.Parameters.AddWithValue("@BranchCode", AssToEmpPCCode.Trim());
                        cmd.Parameters.AddWithValue("@Remark", "record saved from angular app");
                        cmd.Parameters.AddWithValue("@IntsGrade", "N");
                        cmd.Parameters.AddWithValue("@IntsTaxType", "0");
                        cmd.Parameters.AddWithValue("@IntsVATPerc", "0");
                        cmd.Parameters.AddWithValue("@IntsCST", "0");
                        cmd.Parameters.AddWithValue("@IntsSrvTax", "0");
                        cmd.Parameters.AddWithValue("@IntsSurcharges", "0");
                        cmd.Parameters.AddWithValue("@IntsFrieght", "0");
                        cmd.Parameters.AddWithValue("@IntsOther", "0");
                        cmd.Parameters.AddWithValue("@IntsMaterialAmount", "0");
                        cmd.Parameters.AddWithValue("@IntsLabourAmount", "0");
                        cmd.Parameters.AddWithValue("@Auth", 1);
                        cmd.Parameters.AddWithValue("@Active", 1);
                        cmd.Parameters.AddWithValue("@SendMailType", "Q");
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        #endregion

                        //Details                    
                        quotPartDts = Regex.Split(enquirySaveModel.QuotPartCode_items, "@#@");
                        SrNo = 0;
                        if (enquirySaveModel.QuotPartCode_items != "")
                        {
                            foreach (string qoutSub in quotPartDts)
                            {
                                //Q Dtls
                                #region
                                double QouPartAmt = 0;
                                SrNo += 1;
                                quotDts = Regex.Split(qoutSub.Trim(), "-->");
                                cmd = new SqlCommand("InsertUpdateQuotationDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@SearchType", "S");
                                cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                cmd.Parameters.AddWithValue("@PartCode", quotDts[1].Trim());
                                cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(quotDts[2].Trim()));
                                cmd.Parameters.AddWithValue("@BasicPrice", Convert.ToDouble(quotDts[3].Trim()));
                                QouPartAmt = Math.Round(Convert.ToDouble(quotDts[2].Trim()) * Convert.ToDouble(quotDts[3].Trim()));
                                cmd.Parameters.AddWithValue("@QtRemark", quotDts[5].Trim());
                                cmd.Parameters.AddWithValue("@QtnStatus", "P");
                                cmd.Parameters.AddWithValue("@MailStatus", "1");
                                cmd.Parameters.AddWithValue("@PanelStatus", "N");
                                cmd.Parameters.AddWithValue("@PanelPart", "");
                                cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                cmd.Transaction = tran;
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                if (kva_items == "")
                                {
                                    kva_items = quotDts[0].Trim();
                                }
                                else
                                {
                                    string[] duplicate = Regex.Split(kva_items, ",");
                                    if (!duplicate.Contains(quotDts[0].Trim()))
                                    {
                                        kva_items = kva_items + "," + quotDts[0].Trim();
                                    }
                                }

                                #endregion

                                //For Addtinal Part 
                                addPartDts = Regex.Split(enquirySaveModel.AdditionalPart_items, "@#@");
                                double AddPartCodeAmt = 0;
                                if (enquirySaveModel.AdditionalPart_items != "")
                                {
                                    foreach (string strSub in addPartDts)
                                    {
                                        #region                               
                                        addDts = Regex.Split(strSub.Trim(), "-->");
                                        //  QtnPart = Add Part Parent Part
                                        if (quotDts[1].Trim() == addDts[0].Trim())
                                        {
                                            SrNo += 1;
                                            cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("@SearchType", "S");
                                            cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                            cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                            cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                            cmd.Parameters.AddWithValue("@RecType", "Part");
                                            cmd.Parameters.AddWithValue("@PanelDesc", addDts[1].Trim());
                                            cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(addDts[2].Trim()));
                                            cmd.Parameters.AddWithValue("@PanelPrice", Convert.ToDouble(addDts[3].Trim()));
                                            cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                            cmd.Transaction = tran;
                                            cmd.CommandTimeout = 0;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();
                                            AddPartCodeAmt = AddPartCodeAmt + Math.Round(Convert.ToDouble(addDts[2].Trim()) * Convert.ToDouble(addDts[3].Trim()), 0, MidpointRounding.AwayFromZero);
                                        }
                                        #endregion
                                    }
                                }

                                // Transaport & GST Individual Part.                                                                   
                                #region
                                //Transaport 
                                double TranspotAmt = 0;
                                SrNo += 1;
                                cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@SearchType", "S");
                                cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                if (quotDts[4].Trim() == "IB")
                                {
                                    cmd.Parameters.AddWithValue("@RecType", "IB");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Included");
                                    cmd.Parameters.AddWithValue("@Qty", "0");
                                    cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                }
                                else if (quotDts[4].Trim() == "NB")
                                {
                                    cmd.Parameters.AddWithValue("@RecType", "NB");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Charges");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    cmd.Parameters.AddWithValue("@PanelPrice", Convert.ToDouble(quotDts[5].Trim()));
                                    TranspotAmt = Convert.ToDouble(quotDts[5].Trim());
                                }
                                else if (quotDts[4].Trim() == "EX")
                                {
                                    cmd.Parameters.AddWithValue("@RecType", "EX");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Extra");
                                    cmd.Parameters.AddWithValue("@Qty", "0");
                                    cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                }
                                cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                cmd.Transaction = tran;
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                                // END Transaport

                                //GST 
                                double QtnPlusAddPartAmt = Math.Round(QouPartAmt + TranspotAmt + AddPartCodeAmt, 0);
                                double GSTAmt = 0;
                                if (enquirySaveModel.GSTInEx.Trim() == "IN")
                                {
                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "Total");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Total Amount");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    cmd.Parameters.AddWithValue("@PanelPrice", QtnPlusAddPartAmt.ToString().Trim());
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "GST");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "GST @18%");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    //aSK
                                    GSTAmt = Math.Round((QtnPlusAddPartAmt * 18) / 100);
                                    cmd.Parameters.AddWithValue("@PanelPrice", GSTAmt);
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                else if (enquirySaveModel.GSTInEx.Trim() == "EX")
                                {
                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "GST");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "GST Extra");
                                    cmd.Parameters.AddWithValue("@Qty", "0");
                                    cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                // END Quatation  GST taxation

                                if (AddPartCodeAmt == 0 && quotDts[4].Trim() == "EX" && enquirySaveModel.GSTInEx.Trim() == "EX")
                                {
                                    // Do not Save Grand Total. Because Transpot and GST Extra.
                                }
                                else
                                {
                                    //Quatation Grand Total
                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "GT");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Grand Total");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    cmd.Parameters.AddWithValue("@PanelPrice", Math.Round(QtnPlusAddPartAmt + GSTAmt));
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                    //END Quatation Grand Total
                                }
                                SrNo = 0;
                                #endregion

                                //Inst Offer
                                #region                                    
                                instOfferDts = Regex.Split(enquirySaveModel.InstallationOffer_items, "@#@");
                                if (enquirySaveModel.InstallationOffer_items != "")
                                {
                                    foreach (string strSub in instOfferDts)
                                    {
                                        instDts = Regex.Split(strSub.Trim(), "-->");
                                        //instKVA=QuotKva
                                        if (instDts[2].Trim() == quotDts[0].Trim())
                                        {
                                            sb.Remove(0, sb.Length);
                                            sb.Append("Insert into QuotationDetailsInstallationOffer ");
                                            sb.Append("(QtnNo,SeqNo,PartCode,ScopeType,InstOfferCode,InstOfferDesc,SizeParameter,Qty,UOM,Rate,InstPartCode)");
                                            sb.Append(" VALUES ('" + QtnNo.Trim() + "','" + instDts[1].Trim() + "','" + quotDts[1].Trim() + "','" + instDts[3].Trim() + "',");
                                            sb.Append("'" + instDts[0].Trim() + "','" + instDts[4].Trim() + "','" + instDts[6].Trim() + "','" + instDts[7].Trim() + "',");
                                            sb.Append("'" + instDts[8].Trim() + "','" + instDts[9].Trim() + "','" + instDts[5].Trim() + "')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();
                                        }
                                    }
                                }
                                #endregion
                            }
                        }

                        //Loginmst
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", enquirySaveModel.UserID.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "Quotation");
                        cmd.Parameters.AddWithValue("@TransactionNo", QtnNo.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", enquirySaveModel.CompID.Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        #endregion
                    }

                    //Plan Status Update
                    #region
                    strProc = "";
                    strProc = "Select isnull(sum(ed.qty),0) as EnqQty from enquiry e inner join enquirydetails ed on e.enqno=ed.enqno Where " +
                                    " e.EGPCode='" + EPGNo.Trim() + "' and e.DomainID='" + enquirySaveModel.DomainID.Trim() + "' and e.Auth='1' and e.Active='1'  ";

                    int ENQQty = Convert.ToInt32(cls.getTranName(strProc, "Enquiry", "EnqQty", con, tran));
                    strProc = "";
                    strProc = "select SUM(Qty) as PlanQty from EnquiryGenerationPlanDetails where EGPCode = '" + EPGNo.Trim() + "' and DomainID = '" + enquirySaveModel.DomainID.Trim() + "'";
                    int PlanQty = Convert.ToInt32(cls.getTranName(strProc, "EnquiryGenerationPlanDetails", "PlanQty", con, tran));
                    //Update Plan Status
                    if (ENQQty >= PlanQty)
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("UPDATE EnquiryGenerationPlanDetails SET EGPStatus='C' WHERE EGPCode='" + EPGNo.Trim() + "' and domainid='" + enquirySaveModel.DomainID.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    //Update Main Table Status
                    strProc = "";
                    strProc = "select Count(egpcode) as AtQty from Enquirygenerationplandetails Where egpCode='" + EPGNo.Trim() + "' and EgpStatus='P' group by egpcode ";
                    if (Convert.ToInt32(cls.getTranName(strProc, "Enquirygenerationplan", "AtQty", con, tran)) == 0)
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("UPDATE EnquiryGenerationPlan SET EGPStatus='C' WHERE EGPCode='" + EPGNo.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    #endregion
                }

                else if (enquirySaveModel.SaveType.Trim() == "FollowUp")
                {
                    //Enquiry
                    #region

                    //Details                    
                    enqPartDts = Regex.Split(enquirySaveModel.EnqPartCode_items, "@#@");
                    int FID = 0; SrNo = 0;
                    foreach (string enqSub in enqPartDts)
                    {
                        enqDts = Regex.Split(enqSub.Trim(), "-->");
                        SrNo += 1;
                        cmd = new SqlCommand("InsertUpdateEnquiryFollowupDetailsD", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EnqNo", enquirySaveModel.EnqNo.Trim());
                        cmd.Parameters.AddWithValue("@PartCode", enqDts[0].Trim());
                        cmd.Parameters.AddWithValue("@EmpID", enquirySaveModel.AssToEmpCode.Trim());
                        cmd.Parameters.AddWithValue("@NextFDate", enquirySaveModel.NextFDate.Trim());
                        if (enquirySaveModel.SendQuot.Trim() == "true")
                        {
                            cmd.Parameters.AddWithValue("@QtnStatus", "C");
                        }
                        else if (enquirySaveModel.SendQuot.Trim() == "false")
                        {
                            cmd.Parameters.AddWithValue("@QtnStatus", "P");
                        }
                        cmd.Parameters.AddWithValue("@FStatusID", enquirySaveModel.FStatusID.Trim());
                        cmd.Parameters.AddWithValue("@Remark", enquirySaveModel.FUpRemark.Trim());
                        cmd.Parameters.AddWithValue("@ToWhom", "");
                        cmd.Parameters.AddWithValue("@OrderStatus", "PQ");
                        cmd.Parameters.AddWithValue("@ActualReqQty", enquirySaveModel.ActualReqQty.Trim());
                        cmd.Parameters.AddWithValue("@UpdateBy", enquirySaveModel.UserID.Trim());
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        FID = (int)cmd.Parameters["@ID"].Value;
                        cmd.Dispose();

                        //Update CallRegister Followup Status
                        sb.Remove(0, sb.Length);
                        sb.Append("Update CallRegister Set FStatusID='" + enquirySaveModel.FStatusID.Trim() + "' Where CRGCode='" + CRGCode.Trim() + "' and CRGCode<>'0'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("Update EnquiryDetails SET FID='" + FID.ToString().Trim() + "' WHERE EnqNo='" + enquirySaveModel.EnqNo.Trim() + "' ");
                        sb.Append("and PartCode= '" + enqDts[0].Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", enquirySaveModel.UserID.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "FollowUp");
                    cmd.Parameters.AddWithValue("@TransactionNo", enquirySaveModel.EnqNo.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", enquirySaveModel.CompID.Trim());
                    cmd.Transaction = tran;
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    #endregion

                    //Quat        
                    if (enquirySaveModel.SendQuot.Trim() == "true")
                    {
                        #region

                        sb.Remove(0, sb.Length);
                        sb.Append("UPDATE Enquiry SET QuotSendStatus='Y' WHERE enqno='" + enquirySaveModel.EnqNo.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //Master
                        #region
                        QtnNo = cls.GetMaxNo("Quotation", "QTN", enquirySaveModel.CompID.Trim(), con, tran);

                        sb.Remove(0, sb.Length);
                        cmd = new SqlCommand("InsertUpdateQuotationD", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@QtnNo", QtnNo);
                        cmd.Parameters.AddWithValue("@MaxSrNo", QtnNo.Substring(10, 8));
                        cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Yr", QtnNo.Substring(4, 5));
                        cmd.Parameters.AddWithValue("@EnqNo", enquirySaveModel.EnqNo.Trim());
                        cmd.Parameters.AddWithValue("@AssignToEmpID", enquirySaveModel.AssToEmpCode.Trim());
                        cmd.Parameters.AddWithValue("@AdditionStand", "NIL");
                        cmd.Parameters.AddWithValue("@PriceCommercial", "NIL");
                        cmd.Parameters.AddWithValue("@Taxation", "NIL");
                        cmd.Parameters.AddWithValue("@TermsCondition", "NIL");
                        cmd.Parameters.AddWithValue("@QtnStatus", "P");
                        cmd.Parameters.AddWithValue("@QtnShow", "Y");
                        cmd.Parameters.AddWithValue("@ExciseDuty", "0");
                        cmd.Parameters.AddWithValue("@CESS", "0");
                        cmd.Parameters.AddWithValue("@HEdCESS", "0");
                        cmd.Parameters.AddWithValue("@VAT", "0");
                        cmd.Parameters.AddWithValue("@CST", "0");
                        cmd.Parameters.AddWithValue("@ServiceTax", "0");
                        cmd.Parameters.AddWithValue("@EntryTax", "0");
                        cmd.Parameters.AddWithValue("@Other", "0");
                        cmd.Parameters.AddWithValue("@SurCharge", "0");
                        cmd.Parameters.AddWithValue("@Octri", "0");
                        cmd.Parameters.AddWithValue("@OctriBy", "C");
                        cmd.Parameters.AddWithValue("@Transport", "0");
                        cmd.Parameters.AddWithValue("@TransportBy", "C");
                        cmd.Parameters.AddWithValue("@Unloading", "0");
                        cmd.Parameters.AddWithValue("@UnloadingBy", "C");
                        cmd.Parameters.AddWithValue("@Insurance", "0");
                        cmd.Parameters.AddWithValue("@InsuranceBy", "C");
                        cmd.Parameters.AddWithValue("@Packing", "0");
                        cmd.Parameters.AddWithValue("@PackingBy", "C");
                        cmd.Parameters.AddWithValue("@Specification", "");
                        cmd.Parameters.AddWithValue("@CompanyCode", enquirySaveModel.CompID.Trim());
                        //ASK pc CODE tO sVAE
                        //cmd.Parameters.AddWithValue("@BranchCode", enquirySaveModel.PCCode.Trim());
                        cmd.Parameters.AddWithValue("@BranchCode", AssToEmpPCCode.Trim());
                        cmd.Parameters.AddWithValue("@Remark", "record saved from angular app");
                        cmd.Parameters.AddWithValue("@IntsGrade", "N");
                        cmd.Parameters.AddWithValue("@IntsTaxType", "0");
                        cmd.Parameters.AddWithValue("@IntsVATPerc", "0");
                        cmd.Parameters.AddWithValue("@IntsCST", "0");
                        cmd.Parameters.AddWithValue("@IntsSrvTax", "0");
                        cmd.Parameters.AddWithValue("@IntsSurcharges", "0");
                        cmd.Parameters.AddWithValue("@IntsFrieght", "0");
                        cmd.Parameters.AddWithValue("@IntsOther", "0");
                        cmd.Parameters.AddWithValue("@IntsMaterialAmount", "0");
                        cmd.Parameters.AddWithValue("@IntsLabourAmount", "0");
                        cmd.Parameters.AddWithValue("@Auth", 1);
                        cmd.Parameters.AddWithValue("@Active", 1);
                        cmd.Parameters.AddWithValue("@SendMailType", "Q");
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        #endregion

                        //Details                    
                        quotPartDts = Regex.Split(enquirySaveModel.QuotPartCode_items, "@#@");
                        SrNo = 0;
                        if (enquirySaveModel.QuotPartCode_items != "")
                        {
                            foreach (string qoutSub in quotPartDts)
                            {
                                //Q Dtls
                                #region
                                double QouPartAmt = 0;
                                SrNo += 1;
                                quotDts = Regex.Split(qoutSub.Trim(), "-->");
                                cmd = new SqlCommand("InsertUpdateQuotationDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@SearchType", "S");
                                cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                cmd.Parameters.AddWithValue("@PartCode", quotDts[1].Trim());
                                cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(quotDts[2].Trim()));
                                cmd.Parameters.AddWithValue("@BasicPrice", Convert.ToDouble(quotDts[3].Trim()));
                                QouPartAmt = Math.Round(Convert.ToDouble(quotDts[2].Trim()) * Convert.ToDouble(quotDts[3].Trim()));
                                cmd.Parameters.AddWithValue("@QtRemark", quotDts[5].Trim());
                                cmd.Parameters.AddWithValue("@QtnStatus", "P");
                                cmd.Parameters.AddWithValue("@MailStatus", "1");
                                cmd.Parameters.AddWithValue("@PanelStatus", "N");
                                cmd.Parameters.AddWithValue("@PanelPart", "");
                                cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                cmd.Transaction = tran;
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                if (kva_items == "")
                                {
                                    kva_items = quotDts[0].Trim();
                                }
                                else
                                {
                                    string[] duplicate = Regex.Split(kva_items, ",");
                                    if (!duplicate.Contains(quotDts[0].Trim()))
                                    {
                                        kva_items = kva_items + "," + quotDts[0].Trim();
                                    }
                                }

                                #endregion

                                //For Addtinal Part 
                                addPartDts = Regex.Split(enquirySaveModel.AdditionalPart_items, "@#@");
                                double AddPartCodeAmt = 0;
                                if (enquirySaveModel.AdditionalPart_items != "")
                                {
                                    foreach (string strSub in addPartDts)
                                    {
                                        #region                               
                                        addDts = Regex.Split(strSub.Trim(), "-->");
                                        //  QtnPart = Add Part Parent Part
                                        if (quotDts[1].Trim() == addDts[0].Trim())
                                        {
                                            SrNo += 1;
                                            cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("@SearchType", "S");
                                            cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                            cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                            cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                            cmd.Parameters.AddWithValue("@RecType", "Part");
                                            cmd.Parameters.AddWithValue("@PanelDesc", addDts[1].Trim());
                                            cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(addDts[2].Trim()));
                                            cmd.Parameters.AddWithValue("@PanelPrice", Convert.ToDouble(addDts[3].Trim()));
                                            cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                            cmd.Transaction = tran;
                                            cmd.CommandTimeout = 0;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();
                                            AddPartCodeAmt = AddPartCodeAmt + Math.Round(Convert.ToDouble(addDts[2].Trim()) * Convert.ToDouble(addDts[3].Trim()), 0, MidpointRounding.AwayFromZero);
                                        }
                                        #endregion
                                    }
                                }

                                // Transaport & GST Individual Part.                                                                   
                                #region
                                //Transaport 
                                double TranspotAmt = 0;
                                SrNo += 1;
                                cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@SearchType", "S");
                                cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                if (quotDts[4].Trim() == "IB")
                                {
                                    cmd.Parameters.AddWithValue("@RecType", "IB");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Included");
                                    cmd.Parameters.AddWithValue("@Qty", "0");
                                    cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                }
                                else if (quotDts[4].Trim() == "NB")
                                {
                                    cmd.Parameters.AddWithValue("@RecType", "NB");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Charges");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    cmd.Parameters.AddWithValue("@PanelPrice", Convert.ToDouble(quotDts[5].Trim()));
                                    TranspotAmt = Convert.ToDouble(quotDts[5].Trim());
                                }
                                else if (quotDts[4].Trim() == "EX")
                                {
                                    cmd.Parameters.AddWithValue("@RecType", "EX");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Extra");
                                    cmd.Parameters.AddWithValue("@Qty", "0");
                                    cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                }
                                cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                cmd.Transaction = tran;
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                                // END Transaport

                                //GST 
                                double QtnPlusAddPartAmt = Math.Round(QouPartAmt + TranspotAmt + AddPartCodeAmt, 0);
                                double GSTAmt = 0;
                                if (enquirySaveModel.GSTInEx.Trim() == "IN")
                                {
                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "Total");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Total Amount");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    cmd.Parameters.AddWithValue("@PanelPrice", QtnPlusAddPartAmt.ToString().Trim());
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "GST");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "GST @18%");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    GSTAmt = Math.Round((QtnPlusAddPartAmt * 18) / 100);
                                    cmd.Parameters.AddWithValue("@PanelPrice", GSTAmt);
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                else if (enquirySaveModel.GSTInEx.Trim() == "EX")
                                {
                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "GST");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "GST Extra");
                                    cmd.Parameters.AddWithValue("@Qty", "0");
                                    cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                // END Quatation  GST taxation

                                if (AddPartCodeAmt == 0 && quotDts[4].Trim() == "EX" && enquirySaveModel.GSTInEx.Trim() == "EX")
                                {
                                    // Do not Save Grand Total. Because Transpot and GST Extra.
                                }
                                else
                                {
                                    //Quatation Grand Total
                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "GT");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Grand Total");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    cmd.Parameters.AddWithValue("@PanelPrice", Math.Round(QtnPlusAddPartAmt + GSTAmt));
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                    //END Quatation Grand Total
                                }
                                SrNo = 0;
                                #endregion

                                //Inst Offer
                                #region                                    
                                instOfferDts = Regex.Split(enquirySaveModel.InstallationOffer_items, "@#@");
                                if (enquirySaveModel.InstallationOffer_items != "")
                                {
                                    foreach (string strSub in instOfferDts)
                                    {
                                        instDts = Regex.Split(strSub.Trim(), "-->");
                                        //instKVA=QuotKva
                                        if (instDts[2].Trim() == quotDts[0].Trim())
                                        {
                                            sb.Remove(0, sb.Length);
                                            sb.Append("Insert into QuotationDetailsInstallationOffer ");
                                            sb.Append("(QtnNo,SeqNo,PartCode,ScopeType,InstOfferCode,InstOfferDesc,SizeParameter,Qty,UOM,Rate,InstPartCode)");
                                            sb.Append(" VALUES ('" + QtnNo.Trim() + "','" + instDts[1].Trim() + "','" + quotDts[1].Trim() + "','" + instDts[3].Trim() + "',");
                                            sb.Append("'" + instDts[0].Trim() + "','" + instDts[4].Trim() + "','" + instDts[6].Trim() + "','" + instDts[7].Trim() + "',");
                                            sb.Append("'" + instDts[8].Trim() + "','" + instDts[9].Trim() + "','" + instDts[5].Trim() + "')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();
                                        }
                                    }
                                }
                                #endregion
                            }
                        }

                        //Loginmst
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", enquirySaveModel.UserID.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "Quotation");
                        cmd.Parameters.AddWithValue("@TransactionNo", QtnNo.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", enquirySaveModel.CompID.Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        #endregion
                    }
                }

                // tran.Commit();

                if (enquirySaveModel.SendQuot.Trim() == "true")
                {
                    DataSet ds = cls.procDS("select cmobileno,compmailid from employee where ecode='" + enquirySaveModel.AssToEmpCode.Trim() + "'", "tbl_employee");

                    string url = "https://www.kalapms.com/QuotApi/Service.asmx/sendQuotEmailAngular?qtn_no=" + QtnNo.Trim() + "&kva=" + kva_items.Trim() + "&to_mail_id=" + enquirySaveModel.EMailID.Trim()
                        + "&cc_mail_id=" + enquirySaveModel.CCMailID.Trim() + "&ass_to_emp_id=" + enquirySaveModel.AssToEmpCode.Trim() + "&ass_to_emp_name=" + enquirySaveModel.AssToEmpName.Trim()
                        + "&ass_to_mob_no=" + ds.Tables["tbl_employee"].Rows[0]["cmobileno"].ToString().Trim() + "&ass_to_mail_id=" + ds.Tables["tbl_employee"].Rows[0]["compmailid"].ToString().Trim()
                        + "&cust_name=" + enquirySaveModel.CustName.Trim() + "&cust_add=" + enquirySaveModel.Address.Trim();
                    HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);
                    Request.Method = "GET";
                    Request.KeepAlive = true;
                    HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
                    if (Response.StatusCode == HttpStatusCode.OK)
                    {
                        emailStatus = "Successfully";
                    }
                    Response.Close();
                }
            }
            catch (Exception ex)
            {
                tran.Rollback();
                emailStatus = "Failed StackTrace " + ex.StackTrace + ", Message " + ex.Message;
            }
            finally
            {
                con.Close();
            }

            if (enquirySaveModel.SendQuot.Trim() == "true")
            {
                return EnqNo + " Saved Successfully, Email Send " + emailStatus;
            }
            else
            {
                return EnqNo + " Saved Successfully " + enquirySaveModel.SendQuot.Trim();
            }
        }


        public string Submit_EnqDashboard(Enquiry_Submit_EnqDashboard EnquirySubmitEnqDashboard)
        {

            if (EnquirySubmitEnqDashboard.FUpRemark != null)
            {
                EnquirySubmitEnqDashboard.FUpRemark.Trim().Replace("\r\n", " ");
            }

            if (EnquirySubmitEnqDashboard.txtLOSTToWhomDetails != null)
            {
                EnquirySubmitEnqDashboard.txtLOSTToWhomDetails.Trim().Replace("\r\n", " ");
            }

            if (EnquirySubmitEnqDashboard.Address != null)
            {
                EnquirySubmitEnqDashboard.Address.Trim().Replace("\r\n", " ");
            }

            if (EnquirySubmitEnqDashboard.FUpRemark != null)
            {
                EnquirySubmitEnqDashboard.FUpRemark.Trim().Replace("\r\n", " ");
            }

            //if (EnquirySubmitEnqDashboard.SaveType.Trim() == "NewEnquiry")
            //{
            //    //skk
            //    string alreadySave = cls.getName("select EnqNo from Enquiry where VPCode='" + EnquirySubmitEnqDashboard.VPCode.Trim() + "' and VPSrNo='" + EnquirySubmitEnqDashboard.VPSrNo.Trim() + "' and Active='1'", "Enquiry", "EnqNo");
            //    if (alreadySave != "0")
            //    {
            //        return EnquirySubmitEnqDashboard.VPCode + " Enquiry Already Saved for Customer " + EnquirySubmitEnqDashboard.CustName;   string CRGCode = cls.getName("select CRGCode from Enquiry where EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "' and active='1'", "tbl_Enquiry", "CRGCode");
            //    }
            //}
            string CRGCode = "0";
            emailStatus = "";
            SqlTransaction tran = null;
            int SrNo = 0;
            string EPGNo, EnqNo = "", StrQKAV = "", strProc = "", QtnNo = "", kva_items = "";
            string[] enqPartDts, quotPartDts, addPartDts, instOfferDts, enqDts, quotDts, addDts, instDts;
            string fromDt = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            string toDt = DateTime.Now.AddDays(60).ToString("yyyy-MM-dd") + " 23:59:59";
            string AssToEmpPCCode = "";
            string iGreen = "N", KG = "N";
            try
            {

                //if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "true")
                //{
                //    //Details                    
                //    quotPartDts = Regex.Split(EnquirySubmitEnqDashboard.QuotPartCode_items, "@#@");
                //    SrNo = 0;
                //    if (EnquirySubmitEnqDashboard.QuotPartCode_items != "")
                //    {
                //        foreach (string qoutSub in quotPartDts)
                //        {
                //            quotDts = Regex.Split(qoutSub.Trim(), "-->");

                //            if (StrQKAV == "")
                //            {
                //                StrQKAV = quotDts[0].Trim();
                //            }
                //            else
                //            {
                //                string[] duplicate = Regex.Split(kva_items, ",");
                //                if (!duplicate.Contains(quotDts[0].Trim()))
                //                {
                //                    StrQKAV = StrQKAV + "," + quotDts[0].Trim();
                //                }
                //            }

                //            // 125 6R(kg)
                //            if (Convert.ToDouble(quotDts[0].ToString().Trim().Trim()) == 125 && quotDts[0].Trim().ToString().Trim().Substring(0, 2).Trim() == "6R")
                //            {
                //                //KG = "Y";
                //            } // iGreen KVA
                //            else if ((Convert.ToDouble(quotDts[0].ToString().Trim()) >= 5 && Convert.ToDouble(quotDts[0].ToString().Trim()) <= 160) && quotDts[0].ToString().Trim().Substring(0, 2).Trim() != "CC")
                //            {
                //                iGreen = "Y";
                //            } // kg
                //            else
                //            {
                //                //KG = "Y";
                //            }
                //        }

                //        string strQuotkVA1 = "";
                //        string[] ch = Regex.Split(StrQKAV, ",");
                //        string[] chNew = new string[ch.Length];
                //        for (int i = 0; i < ch.Length; i++)
                //        {
                //            chNew[i] = ch[i].ToString();
                //        }
                //        string[] distVal = chNew.Distinct().ToArray();
                //        foreach (string c in distVal)
                //        {
                //            if (strQuotkVA1.Trim() == "")
                //            {
                //                strQuotkVA1 = c.ToString();
                //            }
                //            else if (strQuotkVA1.Trim() != "")
                //            {
                //                strQuotkVA1 = strQuotkVA1 + "," + c.ToString();
                //            }
                //        }

                //        //Send Quotation Mail
                //        if (iGreen == "Y" && KG == "Y")
                //        {
                //            // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditorResponse", "alert('You Cannot Quot iGreen & Non iGreen Product within same Quotation...!')", true);
                //        }
                //        else
                //        {
                //            if (iGreen.Trim() == "Y")
                //            {
                //                // SendEmail(ViewState["QtnNo"].ToString().Trim(), strQuotkVA.ToString().Trim(), StrCorrectCCmailid.ToString().Trim(), rblQtnInts.SelectedValue.ToString().Trim(), txtSubject.Text.Trim(), lblPCName.Text.Trim(), "iGreen");
                //            }
                //            else
                //            {
                //                // SendEmail(ViewState["QtnNo"].ToString().Trim(), strQuotkVA.ToString().Trim(), StrCorrectCCmailid.ToString().Trim(), rblQtnInts.SelectedValue.ToString().Trim(), txtSubject.Text.Trim(), lblPCName.Text.Trim(), "NoniGreen");
                //            }
                //        }

                //    }
                //}


                // AssToEmpPCCode = cls.getName("select ProfitCenter from Employee where Ecode='" + EnquirySubmitEnqDashboard.AssToEmpCode.Trim() + "'", "tbl_Emp", "ProfitCenter");
                AssToEmpPCCode = EnquirySubmitEnqDashboard.PCCode.Trim();
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); }
                ;
                tran = con.BeginTransaction();

                if (EnquirySubmitEnqDashboard.SaveType.Trim() == "ADD")
                {
                    //Plan
                    #region
                    //EPGNo = cls.getName("select top 1 ep.EGPCode from EnquiryGenerationPlan ep " +
                    //    " inner join EnquiryGenerationPlanDetails epd on ep.EGPCode=epd.EGPCode where ep.EmpCode='" + EnquirySubmitEnqDashboard.AssToEmpCode.Trim() + "'" +
                    //   " and epd.EGPStatus='P' and ep.EGPStatus='P' and ep.ProfitCenterCode='" + AssToEmpPCCode.Trim() + "'" +
                    //    " and ep.FromDate between '" + fromDt.Trim() + "' and '" + toDt.Trim() + "' and ep.Active='1' order by dt desc", "EnquiryGenerationPlan", "EGPCode");

                    EPGNo = cls.getName(" Select top 1 E.EGPCode,S.SName as DomainName,Ed.DomainID from Enquirygenerationplan e inner join Enquirygenerationplandetails ed on e.EGPCode=ed.EGPCode inner join Sector S on ed.DomainID=S.SID  " +
                       "  Where  e.Auth='1' and e.Active='1' and E.EmpCode='" + EnquirySubmitEnqDashboard.AssToEmpCode.Trim() + "' and ED.DomainID='" + EnquirySubmitEnqDashboard.DomainID.Trim() + "'   " +
                       "  and  FORMAT(E.fromDate, 'MM') + FORMAT(E.fromDate, 'yyyy') =  FORMAT(GETDATE(), 'MM') +  FORMAT(GETDATE(), 'yyyy') and  FORMAT(E.ToDate, 'MM') +  FORMAT(E.ToDate, 'yyyy') =  FORMAT(GETDATE(), 'MM') +  FORMAT(GETDATE(), 'yyyy') ", "EnquiryGenerationPlan", "EGPCode");


                    if (EPGNo == "0") // No Plan
                    {
                        EPGNo = cls.GetMaxNo("EnquiryGenerationPlan", "EGP", EnquirySubmitEnqDashboard.CompID.Trim(), con, tran);

                        sb.Remove(0, sb.Length);
                        sb.Append("SELECT  CONVERT(VARCHAR(10), DATEFROMPARTS(YEAR('" + DateTime.Now.ToString("yyyy-MM-dd").Trim() + "'), MONTH('" + DateTime.Now.ToString("yyyy-MM-dd").Trim() + "'), 1), 23) + ' 00:00:00' AS MonthStartDate ");
                        String PlanStartDt = cls.getTranName(sb.ToString(), "SD", "MonthStartDate", con, tran);
                        sb.Remove(0, sb.Length);


                        sb.Remove(0, sb.Length);
                        sb.Append("SELECT  CONVERT(VARCHAR(10), EOMONTH('" + DateTime.Now.ToString("yyyy-MM-dd").Trim() + "'), 23) + ' 23:59:00' AS MonthEndDate ");
                        String PlanEndDt = cls.getTranName(sb.ToString(), "SD", "MonthEndDate", con, tran);
                        sb.Remove(0, sb.Length);


                        cmd = new SqlCommand("InsertEnquiryGenerationPlan", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@EGPCode", EPGNo.Trim());
                        cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@MaxSrNo", EPGNo.Substring(10, 8));
                        cmd.Parameters.AddWithValue("@Yr", EPGNo.Substring(4, 5));
                        cmd.Parameters.AddWithValue("@FromDate", PlanStartDt.Trim());
                        cmd.Parameters.AddWithValue("@ToDate", PlanEndDt.Trim());
                        cmd.Parameters.AddWithValue("@EmpCode", EnquirySubmitEnqDashboard.AssToEmpCode.Trim());
                        cmd.Parameters.AddWithValue("@remark", "Auto");
                        cmd.Parameters.AddWithValue("@CompanyCode", EnquirySubmitEnqDashboard.CompID.Trim());
                        cmd.Parameters.AddWithValue("@ProfitCenterCode", AssToEmpPCCode.Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        cmd = new SqlCommand("InsertEnquiryGenerationPlanDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@EGPCode", EPGNo.Trim());
                        cmd.Parameters.AddWithValue("@SrNo", '1');
                        cmd.Parameters.AddWithValue("@DomainID ", EnquirySubmitEnqDashboard.DomainID.Trim());
                        cmd.Parameters.AddWithValue("@AreaID", EnquirySubmitEnqDashboard.CityID.Trim());
                        cmd.Parameters.AddWithValue("@KVA", "0");
                        cmd.Parameters.AddWithValue("@Qty", "50");
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", EnquirySubmitEnqDashboard.AssToEmpName.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "EnquiryGenerationPlan");
                        cmd.Parameters.AddWithValue("@TransactionNo", EPGNo.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", EnquirySubmitEnqDashboard.CompID.Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    #endregion

                    //Enquiry
                    #region
                    EnqNo = cls.GetMaxNo("Enquiry", "ENQ", EnquirySubmitEnqDashboard.CompID.Trim(), con, tran);

                    cmd = new SqlCommand("InsertUpdateEnquiry", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SearchType", "S");
                    cmd.Parameters.AddWithValue("@EnqNo", EnqNo);
                    cmd.Parameters.AddWithValue("@MaxSrNo", EnqNo.Substring(10, 8));
                    cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Yr", EnqNo.Substring(4, 5));
                    cmd.Parameters.AddWithValue("@PrvEnqNo", "0"); //ddlLoadENQ.SelectedValue.ToString().Trim()
                    cmd.Parameters.AddWithValue("@EGPCode", EPGNo.Trim());
                    cmd.Parameters.AddWithValue("@CRGCode", "0");

                    //cmd.Parameters.AddWithValue("@ECICode", ddlEnquiryConsultant.SelectedValue.ToString().Trim());

                    //if (string.IsNullOrEmpty(ddlManualENQ.SelectedValue.ToString().Trim()))
                    //{
                    //    cmd.Parameters.AddWithValue("@CRGCode", "0");
                    //}
                    //else
                    //{
                    //    cmd.Parameters.AddWithValue("@CRGCode", ddlManualENQ.SelectedValue.ToString().Trim());
                    //}

                    cmd.Parameters.AddWithValue("@ENQCODEStatus", "N");
                    cmd.Parameters.AddWithValue("@EnqCODCode", EnquirySubmitEnqDashboard.UserID.Trim());
                    cmd.Parameters.AddWithValue("@DomainID", EnquirySubmitEnqDashboard.DomainID.Trim());

                    cmd.Parameters.AddWithValue("@Title", EnquirySubmitEnqDashboard.Title.Trim()); //ADD New
                    cmd.Parameters.AddWithValue("@RefFromHead", EnquirySubmitEnqDashboard.RefFromID.Trim());

                    cmd.Parameters.AddWithValue("@CustName", EnquirySubmitEnqDashboard.CustName.Trim());
                    cmd.Parameters.AddWithValue("@RefFrom", EnquirySubmitEnqDashboard.RefFrom.Trim());

                    cmd.Parameters.AddWithValue("@MktExName", EnquirySubmitEnqDashboard.AssToEmpName.Trim());
                    cmd.Parameters.AddWithValue("@Address", EnquirySubmitEnqDashboard.Address.Trim());
                    cmd.Parameters.AddWithValue("@ContactPerson", EnquirySubmitEnqDashboard.ContactPerson.Trim());
                    cmd.Parameters.AddWithValue("@OfficePhNo", "");
                    cmd.Parameters.AddWithValue("@ResPhNo", "");
                    cmd.Parameters.AddWithValue("@MobileNo", EnquirySubmitEnqDashboard.MobileNo.Trim());
                    cmd.Parameters.AddWithValue("@EMailID", EnquirySubmitEnqDashboard.EMailID.Trim());
                    cmd.Parameters.AddWithValue("@FaxNo", "");

                    string[] items = null;

                    items = Regex.Split(EnquirySubmitEnqDashboard.CityID.Trim(), "-->");
                    cmd.Parameters.AddWithValue("@Country", items[0].ToString().Trim());
                    cmd.Parameters.AddWithValue("@State", items[1].ToString().Trim());
                    cmd.Parameters.AddWithValue("@City", items[2].ToString().Trim());
                    items = null;

                    //   cmd.Parameters.AddWithValue("@Country", "01");
                    // cmd.Parameters.AddWithValue("@State", cls.getName("select StateID from City where CID='" + EnquirySubmitEnqDashboard.CityID.Trim() + "'", "tbl_City", "StateID"));
                    // cmd.Parameters.AddWithValue("@City", EnquirySubmitEnqDashboard.CityID.Trim());

                    cmd.Parameters.AddWithValue("@Prodtype", "01");
                    cmd.Parameters.AddWithValue("@CompanyCode", EnquirySubmitEnqDashboard.CompID.Trim());
                    cmd.Parameters.AddWithValue("@BranchCode", AssToEmpPCCode.Trim());
                    cmd.Parameters.AddWithValue("@Remark", EnquirySubmitEnqDashboard.EnqRemark.Trim());
                    cmd.Parameters.AddWithValue("@AssignToEmpID", EnquirySubmitEnqDashboard.AssToEmpCode.Trim());
                    cmd.Parameters.AddWithValue("@ActualReqQty", "1");

                    cmd.Parameters.AddWithValue("@ConfEMail", EnquirySubmitEnqDashboard.ConfEMail.Trim());
                    cmd.Parameters.AddWithValue("@ConfCustAssign", "N");
                    cmd.Parameters.AddWithValue("@ConfCustName", EnquirySubmitEnqDashboard.ConfCustName.Trim());
                    cmd.Parameters.AddWithValue("@ConfMobile", EnquirySubmitEnqDashboard.ConfMobile.Trim());
                    cmd.Parameters.AddWithValue("@CPMktHead", 0);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();


                    //sb.Remove(0, sb.Length);
                    //if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "true")
                    //{
                    //    sb.Append("UPDATE Enquiry SET VPCode='" + EnquirySubmitEnqDashboard.VPCode.Trim() + "',VPSrNo='" + EnquirySubmitEnqDashboard.VPSrNo.Trim() + "',QuotSendStatus='Y' WHERE enqno='" + EnqNo.Trim() + "'");
                    //}
                    //else
                    //{
                    //    sb.Append("UPDATE Enquiry SET VPCode='" + EnquirySubmitEnqDashboard.VPCode.Trim() + "',VPSrNo='" + EnquirySubmitEnqDashboard.VPSrNo.Trim() + "' WHERE enqno='" + EnqNo.Trim() + "'");
                    //}
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                    //Details                    
                    enqPartDts = Regex.Split(EnquirySubmitEnqDashboard.EnqPartCode_items, "@#@");
                    int FID = 0; SrNo = 0;
                    foreach (string enqSub in enqPartDts)
                    {
                        enqDts = Regex.Split(enqSub.Trim(), "-->");
                        SrNo += 1;
                        cmd = new SqlCommand("InsertUpdateEnquiryFollowupDetailsD", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EnqNo", EnqNo.Trim());
                        cmd.Parameters.AddWithValue("@PartCode", enqDts[1].Trim());
                        cmd.Parameters.AddWithValue("@EmpID", EnquirySubmitEnqDashboard.AssToEmpCode.Trim());
                        cmd.Parameters.AddWithValue("@NextFDate", EnquirySubmitEnqDashboard.NextFDate.Trim());
                        if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "true")
                        {
                            cmd.Parameters.AddWithValue("@QtnStatus", "C");
                        }
                        else if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "false")
                        {
                            cmd.Parameters.AddWithValue("@QtnStatus", "P");
                        }
                        cmd.Parameters.AddWithValue("@FStatusID", EnquirySubmitEnqDashboard.FStatusID.Trim());
                        cmd.Parameters.AddWithValue("@Remark", EnquirySubmitEnqDashboard.FUpRemark.Trim());
                        cmd.Parameters.AddWithValue("@ToWhom", ""); // StrToWhom.ToString().Trim()); CHECK
                        cmd.Parameters.AddWithValue("@OrderStatus", "PQ");
                        cmd.Parameters.AddWithValue("@ActualReqQty", Convert.ToDouble(enqDts[2].Trim()));
                        cmd.Parameters.AddWithValue("@UpdateBy", EnquirySubmitEnqDashboard.UserID.Trim());



                        cmd.Parameters.AddWithValue("@DGRatingSuggestedBy", EnquirySubmitEnqDashboard.rbldGRatingSuggestedBy.Trim());
                        cmd.Parameters.AddWithValue("@LoadAnalysisRequired", EnquirySubmitEnqDashboard.rblloadStudyRequired.Trim());


                        // Handle the logic based on the selected value
                        if (EnquirySubmitEnqDashboard.rblloadStudyRequired.Trim() == "Y") // "Yes"
                        {
                            cmd.Parameters.AddWithValue("@LoadAnalysisStatus", EnquirySubmitEnqDashboard.ddlloadAnalysisStatus.Trim());

                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@LoadAnalysisStatus", "P");

                        }

                        cmd.Parameters.AddWithValue("@FStatusIDHeads", EnquirySubmitEnqDashboard.FlwStatusHeads.Trim()); //ADD New

                        if (EnquirySubmitEnqDashboard.FStatusID.Trim() == "03") // "LOAST"
                        {
                            cmd.Parameters.AddWithValue("@ToWhomID", EnquirySubmitEnqDashboard.ddlLostToWhom.Trim()); //ADD New
                            cmd.Parameters.AddWithValue("@ToWhomReasonID", EnquirySubmitEnqDashboard.ddlLostReason.Trim());  //ADD New
                            cmd.Parameters.AddWithValue("@LOSTPrice", EnquirySubmitEnqDashboard.txtLOSTPrice.Trim());  //ADD New
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ToWhomID", "0");   //ADD New
                            cmd.Parameters.AddWithValue("@ToWhomReasonID", "0");    //ADD New
                            cmd.Parameters.AddWithValue("@LOSTPrice", "0");  //ADD New
                        }

                        cmd.Parameters.AddWithValue("@FollowupFeedbackfrom", EnquirySubmitEnqDashboard.FollowupFeedbackfrom.Trim()); //ADD New

                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        FID = (int)cmd.Parameters["@ID"].Value;
                        cmd.Dispose();

                        ////Update CallRegister Followup Status
                        //sb.Remove(0, sb.Length);
                        //sb.Append("Update CallRegister Set FStatusID='" + EnquirySubmitEnqDashboard.FStatusID.Trim() + "' Where CRGCode='" + CRGCode.Trim() + "' and CRGCode<>'0'");
                        //cmd = new SqlCommand(sb.ToString(), con);
                        //cmd.Transaction = tran;
                        //cmd.ExecuteNonQuery();
                        //cmd.Dispose();

                        cmd = new SqlCommand("InsertUpdateEnquiryDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@EnqNo", EnqNo.Trim());
                        cmd.Parameters.AddWithValue("@SrNo", SrNo);
                        cmd.Parameters.AddWithValue("@PartCode", enqDts[1].Trim());
                        cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(enqDts[2].Trim()));
                        cmd.Parameters.AddWithValue("@FID", FID.ToString().Trim());
                        cmd.Parameters.AddWithValue("@Status", 0);
                        cmd.Parameters.AddWithValue("@SureYOrN", enqDts[0].Trim()); //ADD New
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }


                    //CHECKKKKKKKKKKKKKKKKKK TO ADDDDD
                    //code to Update Poup Record  in EnquiryLoginPopUpDtls --SKK
                    //sb.Remove(0, sb.Length);
                    //sb.Append("Update EnquiryLoginPopUpDtls Set EnqNo_New='" + EnqNo.Trim() + "' Where SearchDtTime='" + lblEnqPopDtlsSave_Dt.Text.ToString().Trim() + "' and SearchBy='" + EnquirySubmitEnqDashboard.UserID.Trim() + "'  ");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.CommandTimeout = 0;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();
                    //sb.Remove(0, sb.Length);
                    //END


                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", EnquirySubmitEnqDashboard.UserID.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "Enquiry");
                    cmd.Parameters.AddWithValue("@TransactionNo", EnqNo.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", EnquirySubmitEnqDashboard.CompID.Trim());
                    cmd.Transaction = tran;
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    #endregion

                    //Quat        
                    if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "true")
                    {
                        #region
                        //Master
                        #region
                        QtnNo = cls.GetMaxNo("Quotation", "QTN", EnquirySubmitEnqDashboard.CompID.Trim(), con, tran);

                        sb.Remove(0, sb.Length);
                        cmd = new SqlCommand("InsertUpdateQuotationD", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@QtnNo", QtnNo);
                        cmd.Parameters.AddWithValue("@MaxSrNo", QtnNo.Substring(10, 8));
                        cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Yr", QtnNo.Substring(4, 5));
                        cmd.Parameters.AddWithValue("@EnqNo", EnqNo.Trim());
                        cmd.Parameters.AddWithValue("@AssignToEmpID", EnquirySubmitEnqDashboard.AssToEmpCode.Trim());
                        cmd.Parameters.AddWithValue("@AdditionStand", "NIL");
                        cmd.Parameters.AddWithValue("@PriceCommercial", "NIL");
                        cmd.Parameters.AddWithValue("@Taxation", "NIL");
                        cmd.Parameters.AddWithValue("@TermsCondition", "NIL");
                        cmd.Parameters.AddWithValue("@QtnStatus", "P");
                        cmd.Parameters.AddWithValue("@QtnShow", "Y");
                        cmd.Parameters.AddWithValue("@ExciseDuty", "0");
                        cmd.Parameters.AddWithValue("@CESS", "0");
                        cmd.Parameters.AddWithValue("@HEdCESS", "0");
                        cmd.Parameters.AddWithValue("@VAT", "0");
                        cmd.Parameters.AddWithValue("@CST", "0");
                        cmd.Parameters.AddWithValue("@ServiceTax", "0");
                        cmd.Parameters.AddWithValue("@EntryTax", "0");
                        cmd.Parameters.AddWithValue("@Other", "0");
                        cmd.Parameters.AddWithValue("@SurCharge", "0");
                        cmd.Parameters.AddWithValue("@Octri", "0");
                        cmd.Parameters.AddWithValue("@OctriBy", "C");
                        cmd.Parameters.AddWithValue("@Transport", "0");
                        cmd.Parameters.AddWithValue("@TransportBy", "C");
                        cmd.Parameters.AddWithValue("@Unloading", "0");
                        cmd.Parameters.AddWithValue("@UnloadingBy", "C");
                        cmd.Parameters.AddWithValue("@Insurance", "0");
                        cmd.Parameters.AddWithValue("@InsuranceBy", "C");
                        cmd.Parameters.AddWithValue("@Packing", "0");
                        cmd.Parameters.AddWithValue("@PackingBy", "C");
                        cmd.Parameters.AddWithValue("@Specification", "");
                        cmd.Parameters.AddWithValue("@CompanyCode", EnquirySubmitEnqDashboard.CompID.Trim());
                        //ASK pc CODE tO sVAE
                        // cmd.Parameters.AddWithValue("@BranchCode", EnquirySubmitEnqDashboard.PCCode.Trim());
                        cmd.Parameters.AddWithValue("@BranchCode", AssToEmpPCCode.Trim());
                        cmd.Parameters.AddWithValue("@Remark", "Record saved from Angular App");
                        cmd.Parameters.AddWithValue("@IntsGrade", "N");
                        cmd.Parameters.AddWithValue("@IntsTaxType", "0");
                        cmd.Parameters.AddWithValue("@IntsVATPerc", "0");
                        cmd.Parameters.AddWithValue("@IntsCST", "0");
                        cmd.Parameters.AddWithValue("@IntsSrvTax", "0");
                        cmd.Parameters.AddWithValue("@IntsSurcharges", "0");
                        cmd.Parameters.AddWithValue("@IntsFrieght", "0");
                        cmd.Parameters.AddWithValue("@IntsOther", "0");
                        cmd.Parameters.AddWithValue("@IntsMaterialAmount", "0");
                        cmd.Parameters.AddWithValue("@IntsLabourAmount", "0");
                        cmd.Parameters.AddWithValue("@Auth", 1);
                        cmd.Parameters.AddWithValue("@Active", 1);
                        cmd.Parameters.AddWithValue("@SendMailType", "Q");

                        //if (rblQtnInts.SelectedValue.ToString().Trim() == "Q")
                        //{
                        //    cmd.Parameters.AddWithValue("@SendMailType", "Q");
                        //}
                        //else if (rblQtnInts.SelectedValue.ToString().Trim() == "I")
                        //{
                        //    cmd.Parameters.AddWithValue("@SendMailType", "I");
                        //}

                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        #endregion

                        sb.Remove(0, sb.Length);
                        sb.Append("UPDATE Enquiry SET QuotSendStatus='Y' WHERE enqno='" + EnqNo.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        sb.Remove(0, sb.Length);

                        //Details                    
                        quotPartDts = Regex.Split(EnquirySubmitEnqDashboard.QuotPartCode_items, "@#@");
                        SrNo = 0;
                        if (EnquirySubmitEnqDashboard.QuotPartCode_items != "")
                        {
                            foreach (string qoutSub in quotPartDts)
                            {
                                //Q Dtls
                                #region
                                double QouPartAmt = 0;
                                SrNo += 1;
                                quotDts = Regex.Split(qoutSub.Trim(), "-->");
                                cmd = new SqlCommand("InsertUpdateQuotationDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@SearchType", "S");
                                cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                cmd.Parameters.AddWithValue("@PartCode", quotDts[1].Trim());
                                cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(quotDts[2].Trim()));
                                cmd.Parameters.AddWithValue("@BasicPrice", Convert.ToDouble(quotDts[3].Trim()));
                                QouPartAmt = Math.Round(Convert.ToDouble(quotDts[2].Trim()) * Convert.ToDouble(quotDts[3].Trim()));
                                cmd.Parameters.AddWithValue("@QtRemark", quotDts[5].Trim());
                                cmd.Parameters.AddWithValue("@QtnStatus", "P");

                                cmd.Parameters.AddWithValue("@MailStatus", "1");
                                cmd.Parameters.AddWithValue("@PanelStatus", "N");
                                cmd.Parameters.AddWithValue("@PanelPart", "");
                                cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                cmd.Transaction = tran;
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                if (kva_items == "")
                                {
                                    kva_items = quotDts[0].Trim();
                                }
                                else
                                {
                                    string[] duplicate = Regex.Split(kva_items, ",");
                                    if (!duplicate.Contains(quotDts[0].Trim()))
                                    {
                                        kva_items = kva_items + "," + quotDts[0].Trim();
                                    }
                                }

                                #endregion

                                //For Addtinal Part 
                                addPartDts = Regex.Split(EnquirySubmitEnqDashboard.AdditionalPart_items, "@#@");
                                double AddPartCodeAmt = 0;
                                if (EnquirySubmitEnqDashboard.AdditionalPart_items != "")
                                {
                                    foreach (string strSub in addPartDts)
                                    {
                                        #region                               
                                        addDts = Regex.Split(strSub.Trim(), "-->");
                                        //  QtnPart = Add Part Parent Part
                                        if (quotDts[1].Trim() == addDts[0].Trim())
                                        {
                                            SrNo += 1;
                                            cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("@SearchType", "S");
                                            cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                            cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                            cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                            cmd.Parameters.AddWithValue("@RecType", "Part");
                                            cmd.Parameters.AddWithValue("@PanelDesc", addDts[1].Trim());
                                            cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(addDts[2].Trim()));
                                            cmd.Parameters.AddWithValue("@PanelPrice", Convert.ToDouble(addDts[3].Trim()));
                                            cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                            cmd.Transaction = tran;
                                            cmd.CommandTimeout = 0;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();
                                            AddPartCodeAmt = AddPartCodeAmt + Math.Round(Convert.ToDouble(addDts[2].Trim()) * Convert.ToDouble(addDts[3].Trim()), 0, MidpointRounding.AwayFromZero);
                                        }
                                        #endregion
                                    }
                                }

                                // Transaport & GST Individual Part.                                                                   
                                #region
                                //Transaport 
                                double TranspotAmt = 0;
                                SrNo += 1;
                                cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@SearchType", "S");
                                cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                if (quotDts[4].Trim() == "IB")
                                {
                                    cmd.Parameters.AddWithValue("@RecType", "IB");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Included");
                                    cmd.Parameters.AddWithValue("@Qty", "0");
                                    cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                }
                                else if (quotDts[4].Trim() == "NB")
                                {
                                    cmd.Parameters.AddWithValue("@RecType", "NB");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Charges");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    cmd.Parameters.AddWithValue("@PanelPrice", Convert.ToDouble(quotDts[5].Trim()));
                                    TranspotAmt = Convert.ToDouble(quotDts[5].Trim());
                                }
                                else if (quotDts[4].Trim() == "EX")
                                {
                                    cmd.Parameters.AddWithValue("@RecType", "EX");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Extra");
                                    cmd.Parameters.AddWithValue("@Qty", "0");
                                    cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                }
                                cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                cmd.Transaction = tran;
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                                // END Transaport

                                //GST 
                                double QtnPlusAddPartAmt = Math.Round(QouPartAmt + TranspotAmt + AddPartCodeAmt, 0);
                                double GSTAmt = 0;
                                if (EnquirySubmitEnqDashboard.GSTInEx.Trim() == "IN")
                                {
                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "Total");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Total Amount");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    cmd.Parameters.AddWithValue("@PanelPrice", QtnPlusAddPartAmt.ToString().Trim());
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "GST");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "GST @18%");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    //aSK
                                    GSTAmt = Math.Round((QtnPlusAddPartAmt * 18) / 100);
                                    cmd.Parameters.AddWithValue("@PanelPrice", GSTAmt);
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                else if (EnquirySubmitEnqDashboard.GSTInEx.Trim() == "EX")
                                {
                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "GST");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "GST Extra");
                                    cmd.Parameters.AddWithValue("@Qty", "0");
                                    cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                // END Quatation  GST taxation

                                if (AddPartCodeAmt == 0 && quotDts[4].Trim() == "EX" && EnquirySubmitEnqDashboard.GSTInEx.Trim() == "EX")
                                {
                                    // Do not Save Grand Total. Because Transpot and GST Extra.
                                }
                                else
                                {
                                    //Quatation Grand Total
                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "GT");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Grand Total");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    cmd.Parameters.AddWithValue("@PanelPrice", Math.Round(QtnPlusAddPartAmt + GSTAmt));
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                    //END Quatation Grand Total
                                }
                                SrNo = 0;
                                #endregion

                                //Inst Offer
                                #region                                    
                                instOfferDts = Regex.Split(EnquirySubmitEnqDashboard.InstallationOffer_items, "@#@");
                                if (EnquirySubmitEnqDashboard.InstallationOffer_items != "")
                                {
                                    foreach (string strSub in instOfferDts)
                                    {
                                        instDts = Regex.Split(strSub.Trim(), "-->");
                                        //instKVA=QuotKva
                                        if (instDts[2].Trim() == quotDts[0].Trim())
                                        {
                                            sb.Remove(0, sb.Length);
                                            sb.Append("Insert into QuotationDetailsInstallationOffer ");
                                            sb.Append("(QtnNo,SeqNo,PartCode,ScopeType,InstOfferCode,InstOfferDesc,SizeParameter,Qty,UOM,Rate,InstPartCode)");
                                            sb.Append(" VALUES ('" + QtnNo.Trim() + "','" + instDts[1].Trim() + "','" + quotDts[1].Trim() + "','" + instDts[3].Trim() + "',");
                                            sb.Append("'" + instDts[0].Trim() + "','" + instDts[4].Trim() + "','" + instDts[6].Trim() + "','" + instDts[7].Trim() + "',");
                                            sb.Append("'" + instDts[8].Trim() + "','" + instDts[9].Trim() + "','" + instDts[5].Trim() + "')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();
                                        }
                                    }
                                }
                                #endregion
                            }
                        }

                        //Loginmst
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", EnquirySubmitEnqDashboard.UserID.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "Quotation");
                        cmd.Parameters.AddWithValue("@TransactionNo", QtnNo.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", EnquirySubmitEnqDashboard.CompID.Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        #endregion
                    }

                    //Plan Status Update
                    #region
                    //strProc = "";
                    //strProc = "Select isnull(sum(ed.qty),0) as EnqQty from enquiry e inner join enquirydetails ed on e.enqno=ed.enqno Where " +
                    //                " e.EGPCode='" + EPGNo.Trim() + "' and e.DomainID='" + EnquirySubmitEnqDashboard.DomainID.Trim() + "' and e.Auth='1' and e.Active='1'  ";

                    //int ENQQty = Convert.ToInt32(cls.getTranName(strProc, "Enquiry", "EnqQty", con, tran));
                    //strProc = "";
                    //strProc = "select SUM(Qty) as PlanQty from EnquiryGenerationPlanDetails where EGPCode = '" + EPGNo.Trim() + "' and DomainID = '" + EnquirySubmitEnqDashboard.DomainID.Trim() + "'";
                    //int PlanQty = Convert.ToInt32(cls.getTranName(strProc, "EnquiryGenerationPlanDetails", "PlanQty", con, tran));
                    ////Update Plan Status
                    //if (ENQQty >= PlanQty)
                    //{
                    //    sb.Remove(0, sb.Length);
                    //    sb.Append("UPDATE EnquiryGenerationPlanDetails SET EGPStatus='C' WHERE EGPCode='" + EPGNo.Trim() + "' and domainid='" + EnquirySubmitEnqDashboard.DomainID.Trim() + "' ");
                    //    cmd = new SqlCommand(sb.ToString(), con);
                    //    cmd.Transaction = tran;
                    //    cmd.CommandTimeout = 0;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();
                    //}
                    ////Update Main Table Status
                    //strProc = "";
                    //strProc = "select Count(egpcode) as AtQty from Enquirygenerationplandetails Where egpCode='" + EPGNo.Trim() + "' and EgpStatus='P' group by egpcode ";
                    //if (Convert.ToInt32(cls.getTranName(strProc, "Enquirygenerationplan", "AtQty", con, tran)) == 0)
                    //{
                    //    sb.Remove(0, sb.Length);
                    //    sb.Append("UPDATE EnquiryGenerationPlan SET EGPStatus='C' WHERE EGPCode='" + EPGNo.Trim() + "' ");
                    //    cmd = new SqlCommand(sb.ToString(), con);
                    //    cmd.Transaction = tran;
                    //    cmd.CommandTimeout = 0;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();
                    //}

                    strProc = "";
                    int ENQQty = 0;
                    strProc += "Select isnull(sum(ed.qty),0) as EnqQty from enquiry e  ";
                    strProc += "inner join enquirydetails ed on e.enqno=ed.enqno ";
                    strProc += "Where e.EGPCode='" + EPGNo.Trim() + "' and e.DomainID='" + EnquirySubmitEnqDashboard.DomainID.Trim() + "' and e.Auth='1' and e.Active='1'  ";

                    ENQQty = Convert.ToInt32(cls.getTranName(strProc, "Enquiry", "EnqQty", con, tran));

                    // PlanQty = Convert.ToInt32(Request.QueryString["PlanQty"].ToString().Trim());

                    strProc = "";
                    int PlanQty = 0;
                    strProc += "Select isnull(sum(ed.qty),0) as PlanQty_D from Enquirygenerationplan e ";
                    strProc += "inner join Enquirygenerationplandetails ed on e.EGPCode=ed.EGPCode ";
                    strProc += "Where e.EGPCode='" + EPGNo.Trim() + "'  and ed.DomainID='" + EnquirySubmitEnqDashboard.DomainID.Trim() + "' and e.Active='1'  ";
                    PlanQty = Convert.ToInt32(cls.getTranName(strProc, "Enquirygenerationplan", "PlanQty_D", con, tran));

                    //Update Plan Status
                    if (ENQQty >= PlanQty)
                    {
                        //Plan Domain wise Completed --Status-C
                        sb.Remove(0, sb.Length);
                        sb.Append("UPDATE EnquiryGenerationPlanDetails SET EGPStatus='C' WHERE EGPCode='" + EPGNo.Trim() + "' ");
                        sb.Append((" and domainid='" + EnquirySubmitEnqDashboard.DomainID.Trim() + "' "));
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        sb.Remove(0, sb.Length);
                    }
                    else
                    {
                        //Plan Domain wise Pending  --Status-P
                        sb.Remove(0, sb.Length);
                        sb.Append("UPDATE EnquiryGenerationPlanDetails SET EGPStatus='P' WHERE EGPCode='" + EPGNo.Trim() + "' ");
                        sb.Append((" and domainid='" + EnquirySubmitEnqDashboard.DomainID.Trim() + "' "));
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        sb.Remove(0, sb.Length);
                    }

                    //Update Main Table Status
                    strProc = "";
                    strProc += "select Count(egpcode) as AtQty from Enquirygenerationplandetails  ";
                    strProc += "Where egpCode='" + EPGNo.Trim() + "' and EgpStatus='P' group by egpcode ";
                    if (Convert.ToInt32(cls.getTranName(strProc, "Enquirygenerationplan", "AtQty", con, tran)) == 0)
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("UPDATE EnquiryGenerationPlan SET EGPStatus='C' WHERE EGPCode='" + EPGNo.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        sb.Remove(0, sb.Length);
                    }

                    #endregion
                }

                else if (EnquirySubmitEnqDashboard.SaveType.Trim() == "FollowUp")
                {

                    strProc = "";
                    strProc = "Select EGPCode from enquiry  Where EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "'   ";
                    //and e.DomainID='" + Enq_Segment_BeforeUpdate.Trim() + "'
                    String Enq_EGPCode_BeforeUpdate = cls.getTranName(strProc, "Enquiry", "EGPCode", con, tran).ToString().Trim();



                    strProc = "";
                    strProc = "Select DomainID from enquiry  Where EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "'   ";
                    //and e.DomainID='" + Enq_Segment_BeforeUpdate.Trim() + "'
                    String Enq_Segment_BeforeUpdate = cls.getTranName(strProc, "Enquiry", "DomainID", con, tran).ToString().Trim();


                    //Details                    
                    enqPartDts = Regex.Split(EnquirySubmitEnqDashboard.EnqPartCode_items, "@#@");
                    int recCnt = 0;
                    recCnt = enqPartDts.Length;
                    int cntNoDtl = 0; int FID = 0;
                    //When  New Product Add Find SrNo Count
                    if (recCnt > 1)
                    {
                        strProc = "";
                        strProc = "Select isnull(COUNT(ENQNo),0)  as MSrNoCnt from enquirydetails  Where EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "'   ";
                        //and e.DomainID='" + Enq_Segment_BeforeUpdate.Trim() + "'
                        cntNoDtl = Convert.ToInt32(cls.getTranName(strProc, "EnquiryDTls", "MSrNoCnt", con, tran));
                    }

                    int row_1 = 0;
                    foreach (string enqSub in enqPartDts)
                    {
                        enqDts = Regex.Split(enqSub.Trim(), "-->");

                        if (row_1 == 0) // Ist row
                        {

                            row_1 = (row_1 + 1);
                            #region
                            //Save IN  PRV Enquiry
                            #region
                            //**Save IN  PRV Enquiry 
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO EnquiryPrevious Select * from Enquiry WHERE EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "'  ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            //**************************

                            //**Save IN  PRV EnquiryDetails 
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO EnquiryPreviousDetails Select * from EnquiryDetails WHERE EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "' and PartCode= '" + EnquirySubmitEnqDashboard.EnqPartCode_Selected.Trim() + "'  ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            //**************************

                            //**Save IN  PRV EnquiryDetails 
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO EnquiryPreviousFollowupDetails Select * from EnquiryFollowupDetails WHERE EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "' and PartCode= '" + EnquirySubmitEnqDashboard.EnqPartCode_Selected.Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            //**************************

                            //**Update IN  PRV Enquiry 
                            sb.Remove(0, sb.Length);
                            sb.Append("Update EnquiryPrevious SET EnqChangeDt= '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' Where EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "' and EnqChangeDt is null  ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            //**************************

                            //**Update IN  PRV EnquiryDetails 
                            sb.Remove(0, sb.Length);
                            sb.Append("Update EnquiryPreviousDetails SET EnqChangeDt= '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "' and EnqChangeDt is null and PartCode= '" + EnquirySubmitEnqDashboard.EnqPartCode_Selected.Trim() + "'  ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            //**************************

                            //**Update IN  PRV EnquiryDetails 
                            sb.Remove(0, sb.Length);
                            sb.Append("Update EnquiryPreviousFollowupDetails SET EnqChangeDt= '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' WHERE EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "' and EnqChangeDt is null and PartCode= '" + EnquirySubmitEnqDashboard.EnqPartCode_Selected.Trim() + "'  ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            // **************************
                            #endregion
                            // END Prevoius ENQ to table

                            //**Update Enquiry Last Followp Actual Qty
                            sb.Remove(0, sb.Length);
                            sb.Append("UPDATE Enquiry SET ActualReqQty='1'");
                            sb.Append(",Title='" + EnquirySubmitEnqDashboard.Title.Trim() + "'  "); //ADD New
                            sb.Append(",CustName='" + EnquirySubmitEnqDashboard.CustName.Trim() + "'  ");
                            sb.Append(",Address='" + EnquirySubmitEnqDashboard.Address.Trim() + "'  ");
                            sb.Append(",ContactPerson='" + EnquirySubmitEnqDashboard.ContactPerson.Trim() + "'  ");
                            sb.Append(",OfficePhNo=''  ");
                            sb.Append(",ResPhNo=''  ");
                            sb.Append(",MobileNo='" + EnquirySubmitEnqDashboard.MobileNo.Trim() + "'  ");
                            sb.Append(",EMailID='" + EnquirySubmitEnqDashboard.EMailID.Trim() + "'  ");
                            sb.Append(",FaxNo=''  ");
                            sb.Append(",RefFrom='" + EnquirySubmitEnqDashboard.RefFrom.Trim() + "'  ");
                            sb.Append(",RefFromHead='" + EnquirySubmitEnqDashboard.RefFromID.Trim() + "'  ");
                            sb.Append(",MktExName='" + EnquirySubmitEnqDashboard.AssToEmpName.Trim() + "'  ");
                            string[] items = null;

                            items = Regex.Split(EnquirySubmitEnqDashboard.CityID.Trim(), "-->");
                            sb.Append(",Country ='" + items[0].ToString().Trim() + "'  ");
                            sb.Append(",State ='" + items[1].ToString().Trim() + "'  ");
                            sb.Append(",City ='" + items[2].ToString().Trim() + "'  ");
                            items = null;
                            sb.Append(",Remark ='" + EnquirySubmitEnqDashboard.EnqRemark.Trim() + "' ");
                            sb.Append(",DomainID ='" + EnquirySubmitEnqDashboard.DomainID.Trim() + "' "); //SK
                            sb.Append("WHERE EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            //**************************

                            //**Update Enquiry Main FLW Field
                            sb.Remove(0, sb.Length);

                            sb.Append("UPDATE EnquiryDetails SET PartCode='" + enqDts[1].Trim() + "' , Qty= '" + Convert.ToDouble(enqDts[2].Trim()) + "'   "); //ADD New
                            sb.Append("  WHERE EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "' and PartCode= '" + EnquirySubmitEnqDashboard.EnqPartCode_Selected.Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            //**************************

                            //**Update Enquiry Main FLW Field
                            sb.Remove(0, sb.Length);
                            //sb.Append("UPDATE EnquiryFollowupDetails SET PartCode='" + dataItem["PartCode"].ToString().Trim() + "' "); 
                            sb.Append("UPDATE EnquiryFollowupDetails SET PartCode='" + enqDts[1].Trim() + "' , ActualReqQty= '" + Convert.ToDouble(enqDts[2].Trim()) + "' "); //SK
                            sb.Append(" WHERE EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "' and PartCode= '" + EnquirySubmitEnqDashboard.EnqPartCode_Selected.Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();




                            //Code to Save Current ENQ Latest FollowUp**************************************
                            //***************************************************************
                            sb.Remove(0, sb.Length);
                            cmd = new SqlCommand("InsertUpdateEnquiryFollowupDetailsD", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                            cmd.Parameters.AddWithValue("@SearchType", "S");
                            cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                            cmd.Parameters.AddWithValue("@EnqNo", EnquirySubmitEnqDashboard.EnqNo.Trim());
                            cmd.Parameters.AddWithValue("@PartCode", enqDts[1].Trim());
                            cmd.Parameters.AddWithValue("@EmpID", EnquirySubmitEnqDashboard.AssToEmpCode.Trim());
                            cmd.Parameters.AddWithValue("@NextFDate", EnquirySubmitEnqDashboard.NextFDate.Trim());
                            if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "true")
                            {
                                cmd.Parameters.AddWithValue("@QtnStatus", "C");
                            }
                            else if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "false")
                            {
                                cmd.Parameters.AddWithValue("@QtnStatus", "P");
                            }
                            cmd.Parameters.AddWithValue("@FStatusID", EnquirySubmitEnqDashboard.FStatusID.Trim());
                            cmd.Parameters.AddWithValue("@Remark", EnquirySubmitEnqDashboard.FUpRemark.Trim());
                            cmd.Parameters.AddWithValue("@ToWhom", ""); // StrToWhom.ToString().Trim()); CHECK
                            cmd.Parameters.AddWithValue("@OrderStatus", "PQ");
                            cmd.Parameters.AddWithValue("@ActualReqQty", Convert.ToDouble(enqDts[2].Trim()));
                            cmd.Parameters.AddWithValue("@UpdateBy", EnquirySubmitEnqDashboard.UserID.Trim());



                            cmd.Parameters.AddWithValue("@DGRatingSuggestedBy", EnquirySubmitEnqDashboard.rbldGRatingSuggestedBy.Trim());
                            cmd.Parameters.AddWithValue("@LoadAnalysisRequired", EnquirySubmitEnqDashboard.rblloadStudyRequired.Trim());


                            // Handle the logic based on the selected value
                            if (EnquirySubmitEnqDashboard.rblloadStudyRequired.Trim() == "Y") // "Yes"
                            {
                                cmd.Parameters.AddWithValue("@LoadAnalysisStatus", EnquirySubmitEnqDashboard.ddlloadAnalysisStatus.Trim());

                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@LoadAnalysisStatus", "P");

                            }

                            cmd.Parameters.AddWithValue("@FStatusIDHeads", EnquirySubmitEnqDashboard.FlwStatusHeads.Trim()); //ADD New

                            if (EnquirySubmitEnqDashboard.FStatusID.Trim() == "03") // "LOAST"
                            {
                                cmd.Parameters.AddWithValue("@ToWhomID", EnquirySubmitEnqDashboard.ddlLostToWhom.Trim()); //ADD New
                                cmd.Parameters.AddWithValue("@ToWhomReasonID", EnquirySubmitEnqDashboard.ddlLostReason.Trim());  //ADD New
                                cmd.Parameters.AddWithValue("@LOSTPrice", EnquirySubmitEnqDashboard.txtLOSTPrice.Trim());  //ADD New
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@ToWhomID", "0");   //ADD New
                                cmd.Parameters.AddWithValue("@ToWhomReasonID", "0");    //ADD New
                                cmd.Parameters.AddWithValue("@LOSTPrice", "0");  //ADD New
                            }
                            cmd.Parameters.AddWithValue("@FollowupFeedbackfrom", EnquirySubmitEnqDashboard.FollowupFeedbackfrom.Trim()); //ADD New

                            cmd.Transaction = tran;
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                            FID = (int)cmd.Parameters["@ID"].Value;
                            cmd.Dispose();




                            //Update CallRegister Followup Status
                            sb.Remove(0, sb.Length);
                            sb.Append("Update CallRegister Set FStatusID='" + EnquirySubmitEnqDashboard.FStatusID.Trim() + "' Where CRGCode='" + CRGCode.Trim() + "' and CRGCode<>'0'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();


                            //**Update EnquiryDetails with Latest FLW ID Field  @SureYOrN", dataItem["SureYOrN"].ToString().Trim()
                            sb.Remove(0, sb.Length);
                            sb.Append("UPDATE EnquiryDetails SET FID='" + FID.ToString().Trim() + "',SureYOrN='" + enqDts[0].Trim() + "'  WHERE "); //ADD
                            sb.Append(" EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "'");
                            sb.Append(" and PartCode= '" + enqDts[1].Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            //*******************************

                            //**Update Enquiry Last Followp Actual Qty 
                            sb.Remove(0, sb.Length);
                            //sb.Append("UPDATE Enquiry SET ActualReqQty='" + txtActReqQty.Text.ToString().Trim() + "' WHERE "); //SK  
                            sb.Append("UPDATE Enquiry SET ActualReqQty='1' WHERE "); //SK  
                            sb.Append(" EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@EmpID", EnquirySubmitEnqDashboard.UserID.Trim());
                            cmd.Parameters.AddWithValue("@TransactionType", "S");
                            cmd.Parameters.AddWithValue("@TransactionFrom", "FollowUp");
                            cmd.Parameters.AddWithValue("@TransactionNo", EnquirySubmitEnqDashboard.EnqNo.Trim());
                            cmd.Parameters.AddWithValue("@CompanyCode", EnquirySubmitEnqDashboard.CompID.Trim());
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            #endregion

                        }

                        //Code for New Added Product
                        else
                        {
                            cntNoDtl = (cntNoDtl + 1);
                            //Code to Save Current ENQ FollowUp New Product**************************************
                            //***************************************************************
                            sb.Remove(0, sb.Length);
                            cmd = new SqlCommand("InsertUpdateEnquiryFollowupDetailsD", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                            cmd.Parameters.AddWithValue("@SearchType", "S");
                            cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                            cmd.Parameters.AddWithValue("@EnqNo", EnquirySubmitEnqDashboard.EnqNo.Trim());
                            cmd.Parameters.AddWithValue("@PartCode", enqDts[1].Trim());
                            cmd.Parameters.AddWithValue("@EmpID", EnquirySubmitEnqDashboard.AssToEmpCode.Trim());
                            cmd.Parameters.AddWithValue("@NextFDate", EnquirySubmitEnqDashboard.NextFDate.Trim());
                            if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "true")
                            {
                                cmd.Parameters.AddWithValue("@QtnStatus", "C");
                            }
                            else if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "false")
                            {
                                cmd.Parameters.AddWithValue("@QtnStatus", "P");
                            }
                            cmd.Parameters.AddWithValue("@FStatusID", EnquirySubmitEnqDashboard.FStatusID.Trim());
                            cmd.Parameters.AddWithValue("@Remark", EnquirySubmitEnqDashboard.FUpRemark.Trim());
                            cmd.Parameters.AddWithValue("@ToWhom", ""); // StrToWhom.ToString().Trim()); CHECK
                            cmd.Parameters.AddWithValue("@OrderStatus", "PQ");
                            cmd.Parameters.AddWithValue("@ActualReqQty", Convert.ToDouble(enqDts[2].Trim()));
                            cmd.Parameters.AddWithValue("@UpdateBy", EnquirySubmitEnqDashboard.UserID.Trim());



                            cmd.Parameters.AddWithValue("@DGRatingSuggestedBy", EnquirySubmitEnqDashboard.rbldGRatingSuggestedBy.Trim());
                            cmd.Parameters.AddWithValue("@LoadAnalysisRequired", EnquirySubmitEnqDashboard.rblloadStudyRequired.Trim());


                            // Handle the logic based on the selected value
                            if (EnquirySubmitEnqDashboard.rblloadStudyRequired.Trim() == "Y") // "Yes"
                            {
                                cmd.Parameters.AddWithValue("@LoadAnalysisStatus", EnquirySubmitEnqDashboard.ddlloadAnalysisStatus.Trim());

                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@LoadAnalysisStatus", "P");

                            }

                            cmd.Parameters.AddWithValue("@FStatusIDHeads", EnquirySubmitEnqDashboard.FlwStatusHeads.Trim()); //ADD New

                            if (EnquirySubmitEnqDashboard.FStatusID.Trim() == "03") // "LOAST"
                            {
                                cmd.Parameters.AddWithValue("@ToWhomID", EnquirySubmitEnqDashboard.ddlLostToWhom.Trim()); //ADD New
                                cmd.Parameters.AddWithValue("@ToWhomReasonID", EnquirySubmitEnqDashboard.ddlLostReason.Trim());  //ADD New
                                cmd.Parameters.AddWithValue("@LOSTPrice", EnquirySubmitEnqDashboard.txtLOSTPrice.Trim());  //ADD New
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@ToWhomID", "0");   //ADD New
                                cmd.Parameters.AddWithValue("@ToWhomReasonID", "0");    //ADD New
                                cmd.Parameters.AddWithValue("@LOSTPrice", "0");  //ADD New
                            }

                            cmd.Transaction = tran;
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                            FID = (int)cmd.Parameters["@ID"].Value;
                            cmd.Dispose();



                            //*******************************
                            cmd = new SqlCommand("InsertUpdateEnquiryDetails", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@SearchType", "S");
                            cmd.Parameters.AddWithValue("@EnqNo", EnquirySubmitEnqDashboard.EnqNo.Trim());
                            cmd.Parameters.AddWithValue("@SrNo", cntNoDtl);
                            cmd.Parameters.AddWithValue("@PartCode", enqDts[1].Trim());
                            cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(enqDts[2].Trim()));
                            cmd.Parameters.AddWithValue("@FID", FID.ToString().Trim());
                            cmd.Parameters.AddWithValue("@Status", 0);
                            cmd.Parameters.AddWithValue("@SureYOrN", enqDts[0].Trim()); //ADD New
                            cmd.Transaction = tran;
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                    // end Enquiry

                    //Quat        
                    if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "true")
                    {
                        #region
                        //Master
                        #region
                        QtnNo = cls.GetMaxNo("Quotation", "QTN", EnquirySubmitEnqDashboard.CompID.Trim(), con, tran);

                        sb.Remove(0, sb.Length);
                        cmd = new SqlCommand("InsertUpdateQuotationD", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@QtnNo", QtnNo);
                        cmd.Parameters.AddWithValue("@MaxSrNo", QtnNo.Substring(10, 8));
                        cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Yr", QtnNo.Substring(4, 5));
                        cmd.Parameters.AddWithValue("@EnqNo", EnquirySubmitEnqDashboard.EnqNo.Trim());
                        cmd.Parameters.AddWithValue("@AssignToEmpID", EnquirySubmitEnqDashboard.AssToEmpCode.Trim());
                        cmd.Parameters.AddWithValue("@AdditionStand", "NIL");
                        cmd.Parameters.AddWithValue("@PriceCommercial", "NIL");
                        cmd.Parameters.AddWithValue("@Taxation", "NIL");
                        cmd.Parameters.AddWithValue("@TermsCondition", "NIL");
                        cmd.Parameters.AddWithValue("@QtnStatus", "P");
                        cmd.Parameters.AddWithValue("@QtnShow", "Y");
                        cmd.Parameters.AddWithValue("@ExciseDuty", "0");
                        cmd.Parameters.AddWithValue("@CESS", "0");
                        cmd.Parameters.AddWithValue("@HEdCESS", "0");
                        cmd.Parameters.AddWithValue("@VAT", "0");
                        cmd.Parameters.AddWithValue("@CST", "0");
                        cmd.Parameters.AddWithValue("@ServiceTax", "0");
                        cmd.Parameters.AddWithValue("@EntryTax", "0");
                        cmd.Parameters.AddWithValue("@Other", "0");
                        cmd.Parameters.AddWithValue("@SurCharge", "0");
                        cmd.Parameters.AddWithValue("@Octri", "0");
                        cmd.Parameters.AddWithValue("@OctriBy", "C");
                        cmd.Parameters.AddWithValue("@Transport", "0");
                        cmd.Parameters.AddWithValue("@TransportBy", "C");
                        cmd.Parameters.AddWithValue("@Unloading", "0");
                        cmd.Parameters.AddWithValue("@UnloadingBy", "C");
                        cmd.Parameters.AddWithValue("@Insurance", "0");
                        cmd.Parameters.AddWithValue("@InsuranceBy", "C");
                        cmd.Parameters.AddWithValue("@Packing", "0");
                        cmd.Parameters.AddWithValue("@PackingBy", "C");
                        cmd.Parameters.AddWithValue("@Specification", "");
                        cmd.Parameters.AddWithValue("@CompanyCode", EnquirySubmitEnqDashboard.CompID.Trim());
                        //ASK pc CODE tO sVAE
                        // cmd.Parameters.AddWithValue("@BranchCode", EnquirySubmitEnqDashboard.PCCode.Trim());
                        cmd.Parameters.AddWithValue("@BranchCode", AssToEmpPCCode.Trim());
                        cmd.Parameters.AddWithValue("@Remark", "Record saved from Angular App");
                        cmd.Parameters.AddWithValue("@IntsGrade", "N");
                        cmd.Parameters.AddWithValue("@IntsTaxType", "0");
                        cmd.Parameters.AddWithValue("@IntsVATPerc", "0");
                        cmd.Parameters.AddWithValue("@IntsCST", "0");
                        cmd.Parameters.AddWithValue("@IntsSrvTax", "0");
                        cmd.Parameters.AddWithValue("@IntsSurcharges", "0");
                        cmd.Parameters.AddWithValue("@IntsFrieght", "0");
                        cmd.Parameters.AddWithValue("@IntsOther", "0");
                        cmd.Parameters.AddWithValue("@IntsMaterialAmount", "0");
                        cmd.Parameters.AddWithValue("@IntsLabourAmount", "0");
                        cmd.Parameters.AddWithValue("@Auth", 1);
                        cmd.Parameters.AddWithValue("@Active", 1);
                        cmd.Parameters.AddWithValue("@SendMailType", "Q");


                        //if (rblQtnInts.SelectedValue.ToString().Trim() == "Q")
                        //{
                        //    cmd.Parameters.AddWithValue("@SendMailType", "Q");
                        //}
                        //else if (rblQtnInts.SelectedValue.ToString().Trim() == "I")
                        //{
                        //    cmd.Parameters.AddWithValue("@SendMailType", "I");
                        //}


                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        #endregion


                        sb.Remove(0, sb.Length);
                        sb.Append("UPDATE Enquiry SET QuotSendStatus='Y' WHERE enqno='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        sb.Remove(0, sb.Length);

                        //Details                    
                        quotPartDts = Regex.Split(EnquirySubmitEnqDashboard.QuotPartCode_items, "@#@");
                        SrNo = 0;
                        if (EnquirySubmitEnqDashboard.QuotPartCode_items != "")
                        {
                            foreach (string qoutSub in quotPartDts)
                            {
                                //Q Dtls
                                #region
                                double QouPartAmt = 0;
                                SrNo += 1;
                                quotDts = Regex.Split(qoutSub.Trim(), "-->");
                                cmd = new SqlCommand("InsertUpdateQuotationDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@SearchType", "S");
                                cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                cmd.Parameters.AddWithValue("@PartCode", quotDts[1].Trim());
                                cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(quotDts[2].Trim()));
                                cmd.Parameters.AddWithValue("@BasicPrice", Convert.ToDouble(quotDts[3].Trim()));
                                QouPartAmt = Math.Round(Convert.ToDouble(quotDts[2].Trim()) * Convert.ToDouble(quotDts[3].Trim()));
                                cmd.Parameters.AddWithValue("@QtRemark", quotDts[5].Trim());
                                cmd.Parameters.AddWithValue("@QtnStatus", "P");

                                cmd.Parameters.AddWithValue("@MailStatus", "1");
                                cmd.Parameters.AddWithValue("@PanelStatus", "N");
                                cmd.Parameters.AddWithValue("@PanelPart", "");
                                cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                cmd.Transaction = tran;
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                if (kva_items == "")
                                {
                                    kva_items = quotDts[0].Trim();
                                }
                                else
                                {
                                    string[] duplicate = Regex.Split(kva_items, ",");
                                    if (!duplicate.Contains(quotDts[0].Trim()))
                                    {
                                        kva_items = kva_items + "," + quotDts[0].Trim();
                                    }
                                }

                                #endregion

                                //For Addtinal Part 
                                addPartDts = Regex.Split(EnquirySubmitEnqDashboard.AdditionalPart_items, "@#@");
                                double AddPartCodeAmt = 0;
                                if (EnquirySubmitEnqDashboard.AdditionalPart_items != "")
                                {
                                    foreach (string strSub in addPartDts)
                                    {
                                        #region                               
                                        addDts = Regex.Split(strSub.Trim(), "-->");
                                        //  QtnPart = Add Part Parent Part
                                        if (quotDts[1].Trim() == addDts[0].Trim())
                                        {
                                            SrNo += 1;
                                            cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("@SearchType", "S");
                                            cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                            cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                            cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                            cmd.Parameters.AddWithValue("@RecType", "Part");
                                            cmd.Parameters.AddWithValue("@PanelDesc", addDts[1].Trim());
                                            cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(addDts[2].Trim()));
                                            cmd.Parameters.AddWithValue("@PanelPrice", Convert.ToDouble(addDts[3].Trim()));
                                            cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                            cmd.Transaction = tran;
                                            cmd.CommandTimeout = 0;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();
                                            AddPartCodeAmt = AddPartCodeAmt + Math.Round(Convert.ToDouble(addDts[2].Trim()) * Convert.ToDouble(addDts[3].Trim()), 0, MidpointRounding.AwayFromZero);
                                        }
                                        #endregion
                                    }
                                }

                                // Transaport & GST Individual Part.                                                                   
                                #region
                                //Transaport 
                                double TranspotAmt = 0;
                                SrNo += 1;
                                cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@SearchType", "S");
                                cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                if (quotDts[4].Trim() == "IB")
                                {
                                    cmd.Parameters.AddWithValue("@RecType", "IB");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Included");
                                    cmd.Parameters.AddWithValue("@Qty", "0");
                                    cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                }
                                else if (quotDts[4].Trim() == "NB")
                                {
                                    cmd.Parameters.AddWithValue("@RecType", "NB");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Charges");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    cmd.Parameters.AddWithValue("@PanelPrice", Convert.ToDouble(quotDts[5].Trim()));
                                    TranspotAmt = Convert.ToDouble(quotDts[5].Trim());
                                }
                                else if (quotDts[4].Trim() == "EX")
                                {
                                    cmd.Parameters.AddWithValue("@RecType", "EX");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Extra");
                                    cmd.Parameters.AddWithValue("@Qty", "0");
                                    cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                }
                                cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                cmd.Transaction = tran;
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                                // END Transaport

                                //GST 
                                double QtnPlusAddPartAmt = Math.Round(QouPartAmt + TranspotAmt + AddPartCodeAmt, 0);
                                double GSTAmt = 0;
                                if (EnquirySubmitEnqDashboard.GSTInEx.Trim() == "IN")
                                {
                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "Total");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Total Amount");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    cmd.Parameters.AddWithValue("@PanelPrice", QtnPlusAddPartAmt.ToString().Trim());
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "GST");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "GST @18%");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    //aSK
                                    GSTAmt = Math.Round((QtnPlusAddPartAmt * 18) / 100);
                                    cmd.Parameters.AddWithValue("@PanelPrice", GSTAmt);
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                else if (EnquirySubmitEnqDashboard.GSTInEx.Trim() == "EX")
                                {
                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "GST");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "GST Extra");
                                    cmd.Parameters.AddWithValue("@Qty", "0");
                                    cmd.Parameters.AddWithValue("@PanelPrice", "0");
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                // END Quatation  GST taxation

                                if (AddPartCodeAmt == 0 && quotDts[4].Trim() == "EX" && EnquirySubmitEnqDashboard.GSTInEx.Trim() == "EX")
                                {
                                    // Do not Save Grand Total. Because Transpot and GST Extra.
                                }
                                else
                                {
                                    //Quatation Grand Total
                                    SrNo += 1;
                                    cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@SearchType", "S");
                                    cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                                    cmd.Parameters.AddWithValue("@RecType", "GT");
                                    cmd.Parameters.AddWithValue("@PanelDesc", "Grand Total");
                                    cmd.Parameters.AddWithValue("@Qty", "1");
                                    cmd.Parameters.AddWithValue("@PanelPrice", Math.Round(QtnPlusAddPartAmt + GSTAmt));
                                    cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                                    cmd.Transaction = tran;
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                    //END Quatation Grand Total
                                }
                                SrNo = 0;
                                #endregion

                                //Inst Offer
                                #region                                    
                                instOfferDts = Regex.Split(EnquirySubmitEnqDashboard.InstallationOffer_items, "@#@");
                                if (EnquirySubmitEnqDashboard.InstallationOffer_items != "")
                                {
                                    foreach (string strSub in instOfferDts)
                                    {
                                        instDts = Regex.Split(strSub.Trim(), "-->");
                                        //instKVA=QuotKva
                                        if (instDts[2].Trim() == quotDts[0].Trim())
                                        {
                                            sb.Remove(0, sb.Length);
                                            sb.Append("Insert into QuotationDetailsInstallationOffer ");
                                            sb.Append("(QtnNo,SeqNo,PartCode,ScopeType,InstOfferCode,InstOfferDesc,SizeParameter,Qty,UOM,Rate,InstPartCode)");
                                            sb.Append(" VALUES ('" + QtnNo.Trim() + "','" + instDts[1].Trim() + "','" + quotDts[1].Trim() + "','" + instDts[3].Trim() + "',");
                                            sb.Append("'" + instDts[0].Trim() + "','" + instDts[4].Trim() + "','" + instDts[6].Trim() + "','" + instDts[7].Trim() + "',");
                                            sb.Append("'" + instDts[8].Trim() + "','" + instDts[9].Trim() + "','" + instDts[5].Trim() + "')");
                                            cmd = new SqlCommand(sb.ToString(), con);
                                            cmd.Transaction = tran;
                                            cmd.ExecuteNonQuery();
                                            cmd.Dispose();
                                        }
                                    }
                                }
                                #endregion
                            }
                        }

                        //Loginmst
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", EnquirySubmitEnqDashboard.UserID.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "Quotation");
                        cmd.Parameters.AddWithValue("@TransactionNo", QtnNo.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", EnquirySubmitEnqDashboard.CompID.Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        #endregion
                    }
                    //END QUO



                    // skk Add Do not remove code 
                    //When Add New Product to Existing ENQ then To Update related Plan Qty

                    // if (gvwDtls.Rows.Count > 1) //SK Need to Update
                    // {

                    strProc = "";
                    int ENQQty = 0;
                    strProc += "Select isnull(sum(ed.qty),0) as EnqQty from enquiry e  ";
                    strProc += "inner join enquirydetails ed on e.enqno=ed.enqno ";
                    strProc += "Where e.EGPCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "' and e.DomainID='" + Enq_Segment_BeforeUpdate.Trim() + "' and e.Auth='1' and e.Active='1'  ";

                    ENQQty = Convert.ToInt32(cls.getTranName(strProc, "Enquiry", "EnqQty", con, tran));

                    // PlanQty = Convert.ToInt32(lblPlanQty.Text.Trim());
                    strProc = "";
                    int PlanQty = 0;
                    strProc += "Select isnull(sum(ed.qty),0) as PlanQty_D from Enquirygenerationplan e ";
                    strProc += "inner join Enquirygenerationplandetails ed on e.EGPCode=ed.EGPCode ";
                    strProc += "Where e.EGPCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "'  and ed.DomainID='" + Enq_Segment_BeforeUpdate.ToString().Trim() + "' and e.Active='1'  ";
                    PlanQty = Convert.ToInt32(cls.getTranName(strProc, "Enquirygenerationplan", "PlanQty_D", con, tran));

                    //Update Plan Status
                    if (ENQQty >= PlanQty)
                    {
                        //Plan Domain wise Completed --Status-C
                        sb.Remove(0, sb.Length);
                        sb.Append("UPDATE EnquiryGenerationPlanDetails SET EGPStatus='C' , Qty='" + ENQQty + "' WHERE EGPCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "' and  DomainID='" + Enq_Segment_BeforeUpdate.ToString().Trim() + "' ");
                        sb.Append(("and domainid='" + Enq_Segment_BeforeUpdate.Trim() + "' "));
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    else
                    {
                        //Plan Domain wise Pending  --Status-P
                        sb.Remove(0, sb.Length);
                        sb.Append("UPDATE EnquiryGenerationPlanDetails SET EGPStatus='P' WHERE EGPCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "' and  DomainID='" + Enq_Segment_BeforeUpdate.ToString().Trim() + "'");
                        sb.Append(("and domainid='" + Enq_Segment_BeforeUpdate.Trim() + "' "));
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        // Segment Status for PlanNo in to Plan Master
                        sb.Remove(0, sb.Length);
                        sb.Append("UPDATE EnquiryGenerationPlan SET EGPStatus='P' WHERE EGPCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                    }

                    //Update Main Table Status
                    strProc = "";
                    strProc += "select Count(egpcode) as AtQty from Enquirygenerationplandetails Where  ";
                    strProc += "  egpCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "' and EgpStatus='P' group by egpcode ";
                    if (Convert.ToInt32(cls.getTranName(strProc, "Enquirygenerationplan", "AtQty", con, tran)) == 0)
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("UPDATE EnquiryGenerationPlan SET EGPStatus='C' WHERE EGPCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }


                    //Segment changes **************************
                    //**************************************************************************************
                    //SKK Add Date 19_05_2023 For Segment Change
                    if (Enq_Segment_BeforeUpdate.ToString().Trim() != EnquirySubmitEnqDashboard.DomainID.Trim())
                    {

                        // PlanQty = Convert.ToInt32(lblPlanQty.Text.Trim());
                        strProc = "";
                        PlanQty = 0;
                        strProc += "Select isnull(sum(ed.qty),0) as PlanQty_D from Enquirygenerationplan e ";
                        strProc += "inner join Enquirygenerationplandetails ed on e.EGPCode=ed.EGPCode ";
                        strProc += "Where e.EGPCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "'  and ed.DomainID='" + EnquirySubmitEnqDashboard.DomainID.Trim() + "' and e.Active='1'  ";
                        PlanQty = Convert.ToInt32(cls.getTranName(strProc, "Enquirygenerationplan", "PlanQty_D", con, tran));


                        strProc = "";
                        ENQQty = 0;
                        strProc += "Select isnull(sum(ed.qty),0) as EnqQty from enquiry e  ";
                        strProc += "inner join enquirydetails ed on e.enqno=ed.enqno ";
                        strProc += "Where e.EGPCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "' and e.DomainID='" + EnquirySubmitEnqDashboard.DomainID.Trim() + "' and e.Auth='1' and e.Active='1'  ";
                        ENQQty = Convert.ToInt32(cls.getTranName(strProc, "Enquiry", "EnqQty", con, tran));

                        if (PlanQty == 0)
                        {

                            // Added new record in Existing ENQ Selected Plan 
                            string[] items = null;
                            items = Regex.Split(EnquirySubmitEnqDashboard.CityID.ToString().Trim(), "-->");


                            sb.Remove(0, sb.Length);
                            sb.Append("Insert into Enquirygenerationplandetails ");
                            sb.Append("(EGPCode,SrNo,DomainID,AreaID,KVA,Qty,EGPStatus)");
                            sb.Append(" VALUES ('" + Enq_EGPCode_BeforeUpdate.Trim() + "','1',");
                            sb.Append("'" + EnquirySubmitEnqDashboard.DomainID.Trim() + "','" + items[2].ToString().Trim() + "','0' ,'" + (ENQQty + 5) + "','P')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            // Segment Status for PlanNo in to Plan Master
                            sb.Remove(0, sb.Length);
                            sb.Append("UPDATE EnquiryGenerationPlan SET EGPStatus='P' WHERE EGPCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                        else
                        {


                            //Update Plan Status
                            if (ENQQty >= PlanQty)
                            {
                                //Plan Domain wise Completed --Status-C
                                sb.Remove(0, sb.Length);
                                sb.Append("UPDATE EnquiryGenerationPlanDetails SET EGPStatus='C' , Qty='" + ENQQty + "' WHERE EGPCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "' and  DomainID='" + Enq_Segment_BeforeUpdate.ToString().Trim() + "' ");
                                sb.Append(("and domainid='" + Enq_Segment_BeforeUpdate.Trim() + "' "));
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                            else
                            {
                                //Plan Domain wise Pending  --Status-P
                                sb.Remove(0, sb.Length);
                                sb.Append("UPDATE EnquiryGenerationPlanDetails SET EGPStatus='P' WHERE EGPCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "' and  DomainID='" + Enq_Segment_BeforeUpdate.ToString().Trim() + "'");
                                sb.Append(("and domainid='" + Enq_Segment_BeforeUpdate.Trim() + "' "));
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                // Segment Status for PlanNo in to Plan Master
                                sb.Remove(0, sb.Length);
                                sb.Append("UPDATE EnquiryGenerationPlan SET EGPStatus='P' WHERE EGPCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "' ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }

                            //Update Main Table Status
                            strProc = "";
                            strProc += "select Count(egpcode) as AtQty from Enquirygenerationplandetails Where  ";
                            strProc += "  egpCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "' and EgpStatus='P' group by egpcode ";
                            if (Convert.ToInt32(cls.getTranName(strProc, "Enquirygenerationplan", "AtQty", con, tran)) == 0)
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("UPDATE EnquiryGenerationPlan SET EGPStatus='C' WHERE EGPCode='" + Enq_EGPCode_BeforeUpdate.Trim() + "' ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }


                        }
                    }

                    #region

                    //   //Details                    
                    //  enqPartDts = Regex.Split(EnquirySubmitEnqDashboard.EnqPartCode_items, "@#@");
                    //  int FID = 0; SrNo = 0;
                    //   foreach (string enqSub in enqPartDts)
                    // {
                    //enqDts = Regex.Split(enqSub.Trim(), "-->");
                    //SrNo += 1;
                    //cmd = new SqlCommand("InsertUpdateEnquiryFollowupDetailsD", con);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                    //cmd.Parameters.AddWithValue("@SearchType", "S");
                    //cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                    //cmd.Parameters.AddWithValue("@EnqNo", EnquirySubmitEnqDashboard.EnqNo.Trim());
                    //cmd.Parameters.AddWithValue("@PartCode", enqDts[0].Trim());
                    //cmd.Parameters.AddWithValue("@EmpID", EnquirySubmitEnqDashboard.AssToEmpCode.Trim());
                    //cmd.Parameters.AddWithValue("@NextFDate", EnquirySubmitEnqDashboard.NextFDate.Trim());
                    //if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "true")
                    //{
                    //    cmd.Parameters.AddWithValue("@QtnStatus", "C");
                    //}
                    //else if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "false")
                    //{
                    //    cmd.Parameters.AddWithValue("@QtnStatus", "P");
                    //}
                    //cmd.Parameters.AddWithValue("@FStatusID", EnquirySubmitEnqDashboard.FStatusID.Trim());
                    //cmd.Parameters.AddWithValue("@Remark", EnquirySubmitEnqDashboard.FUpRemark.Trim());
                    //cmd.Parameters.AddWithValue("@ToWhom", "");
                    //cmd.Parameters.AddWithValue("@OrderStatus", "PQ");
                    //cmd.Parameters.AddWithValue("@ActualReqQty", "1");
                    //cmd.Parameters.AddWithValue("@UpdateBy", EnquirySubmitEnqDashboard.UserID.Trim());
                    //cmd.Transaction = tran;
                    //cmd.CommandTimeout = 0;
                    //cmd.ExecuteNonQuery();
                    //FID = (int)cmd.Parameters["@ID"].Value;
                    //cmd.Dispose();

                    ////Update CallRegister Followup Status
                    //sb.Remove(0, sb.Length);
                    //sb.Append("Update CallRegister Set FStatusID='" + EnquirySubmitEnqDashboard.FStatusID.Trim() + "' Where CRGCode='" + CRGCode.Trim() + "' and CRGCode<>'0'");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                    //sb.Remove(0, sb.Length);
                    //sb.Append("Update EnquiryDetails SET FID='" + FID.ToString().Trim() + "' WHERE EnqNo='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "' ");
                    //sb.Append("and PartCode= '" + enqDts[0].Trim() + "'");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();
                    //   }

                    //  cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    //   cmd.CommandType = CommandType.StoredProcedure;
                    //   cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    //cmd.Parameters.AddWithValue("@EmpID", EnquirySubmitEnqDashboard.UserID.Trim());
                    //cmd.Parameters.AddWithValue("@TransactionType", "S");
                    //cmd.Parameters.AddWithValue("@TransactionFrom", "FollowUp");
                    //cmd.Parameters.AddWithValue("@TransactionNo", EnquirySubmitEnqDashboard.EnqNo.Trim());
                    //cmd.Parameters.AddWithValue("@CompanyCode", EnquirySubmitEnqDashboard.CompID.Trim());
                    //cmd.Transaction = tran;
                    //cmd.CommandTimeout = 0;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                    //  #endregion

                    ////Quat        
                    //if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "true")
                    //{
                    //    #region

                    //    sb.Remove(0, sb.Length);
                    //    sb.Append("UPDATE Enquiry SET QuotSendStatus='Y' WHERE enqno='" + EnquirySubmitEnqDashboard.EnqNo.Trim() + "'");
                    //    cmd = new SqlCommand(sb.ToString(), con);
                    //    cmd.Transaction = tran;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();

                    //    //Master
                    //    #region
                    //    QtnNo = cls.GetMaxNo("Quotation", "QTN", EnquirySubmitEnqDashboard.CompID.Trim(), con, tran);

                    //    sb.Remove(0, sb.Length);
                    //    cmd = new SqlCommand("InsertUpdateQuotationD", con);
                    //    cmd.CommandType = CommandType.StoredProcedure;
                    //    cmd.Parameters.AddWithValue("@SearchType", "S");
                    //    cmd.Parameters.AddWithValue("@QtnNo", QtnNo);
                    //    cmd.Parameters.AddWithValue("@MaxSrNo", QtnNo.Substring(10, 8));
                    //    cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                    //    cmd.Parameters.AddWithValue("@Yr", QtnNo.Substring(4, 5));
                    //    cmd.Parameters.AddWithValue("@EnqNo", EnquirySubmitEnqDashboard.EnqNo.Trim());
                    //    cmd.Parameters.AddWithValue("@AssignToEmpID", EnquirySubmitEnqDashboard.AssToEmpCode.Trim());
                    //    cmd.Parameters.AddWithValue("@AdditionStand", "NIL");
                    //    cmd.Parameters.AddWithValue("@PriceCommercial", "NIL");
                    //    cmd.Parameters.AddWithValue("@Taxation", "NIL");
                    //    cmd.Parameters.AddWithValue("@TermsCondition", "NIL");
                    //    cmd.Parameters.AddWithValue("@QtnStatus", "P");

                    //    cmd.Parameters.AddWithValue("@QtnShow", "N");
                    //    cmd.Parameters.AddWithValue("@ExciseDuty", "0");
                    //    cmd.Parameters.AddWithValue("@CESS", "0");
                    //    cmd.Parameters.AddWithValue("@HEdCESS", "0");
                    //    cmd.Parameters.AddWithValue("@VAT", "0");
                    //    cmd.Parameters.AddWithValue("@CST", "0");
                    //    cmd.Parameters.AddWithValue("@ServiceTax", "0");
                    //    cmd.Parameters.AddWithValue("@EntryTax", "0");
                    //    cmd.Parameters.AddWithValue("@Other", "0");
                    //    cmd.Parameters.AddWithValue("@SurCharge", "0");
                    //    cmd.Parameters.AddWithValue("@Octri", "0");
                    //    cmd.Parameters.AddWithValue("@OctriBy", "C");
                    //    cmd.Parameters.AddWithValue("@Transport", "0");
                    //    cmd.Parameters.AddWithValue("@TransportBy", "C");
                    //    cmd.Parameters.AddWithValue("@Unloading", "0");
                    //    cmd.Parameters.AddWithValue("@UnloadingBy", "C");
                    //    cmd.Parameters.AddWithValue("@Insurance", "0");
                    //    cmd.Parameters.AddWithValue("@InsuranceBy", "C");
                    //    cmd.Parameters.AddWithValue("@Packing", "0");
                    //    cmd.Parameters.AddWithValue("@PackingBy", "C");
                    //    cmd.Parameters.AddWithValue("@Specification", "");
                    //    cmd.Parameters.AddWithValue("@CompanyCode", EnquirySubmitEnqDashboard.CompID.Trim());
                    //    //ASK pc CODE tO sVAE
                    //    //cmd.Parameters.AddWithValue("@BranchCode", EnquirySubmitEnqDashboard.PCCode.Trim());
                    //    cmd.Parameters.AddWithValue("@BranchCode", AssToEmpPCCode.Trim());
                    //    cmd.Parameters.AddWithValue("@Remark", "record saved from angular app");
                    //    cmd.Parameters.AddWithValue("@IntsGrade", "N");
                    //    cmd.Parameters.AddWithValue("@IntsTaxType", "0");
                    //    cmd.Parameters.AddWithValue("@IntsVATPerc", "0");
                    //    cmd.Parameters.AddWithValue("@IntsCST", "0");
                    //    cmd.Parameters.AddWithValue("@IntsSrvTax", "0");
                    //    cmd.Parameters.AddWithValue("@IntsSurcharges", "0");
                    //    cmd.Parameters.AddWithValue("@IntsFrieght", "0");
                    //    cmd.Parameters.AddWithValue("@IntsOther", "0");
                    //    cmd.Parameters.AddWithValue("@IntsMaterialAmount", "0");
                    //    cmd.Parameters.AddWithValue("@IntsLabourAmount", "0");
                    //    cmd.Parameters.AddWithValue("@Auth", 1);
                    //    cmd.Parameters.AddWithValue("@Active", 1);
                    //    cmd.Parameters.AddWithValue("@SendMailType", "Q");
                    //    cmd.Transaction = tran;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();
                    //    #endregion

                    //    //Details                    
                    //    quotPartDts = Regex.Split(EnquirySubmitEnqDashboard.QuotPartCode_items, "@#@");
                    //    SrNo = 0;
                    //    if (EnquirySubmitEnqDashboard.QuotPartCode_items != "")
                    //    {
                    //        foreach (string qoutSub in quotPartDts)
                    //        {
                    //            //Q Dtls
                    //            #region
                    //            double QouPartAmt = 0;
                    //            SrNo += 1;
                    //            quotDts = Regex.Split(qoutSub.Trim(), "-->");
                    //            cmd = new SqlCommand("InsertUpdateQuotationDetails", con);
                    //            cmd.CommandType = CommandType.StoredProcedure;
                    //            cmd.Parameters.AddWithValue("@SearchType", "S");
                    //            cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                    //            cmd.Parameters.AddWithValue("@PartCode", quotDts[1].Trim());
                    //            cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(quotDts[2].Trim()));
                    //            cmd.Parameters.AddWithValue("@BasicPrice", Convert.ToDouble(quotDts[3].Trim()));
                    //            QouPartAmt = Math.Round(Convert.ToDouble(quotDts[2].Trim()) * Convert.ToDouble(quotDts[3].Trim()));
                    //            cmd.Parameters.AddWithValue("@QtRemark", quotDts[5].Trim());
                    //            cmd.Parameters.AddWithValue("@QtnStatus", "P");
                    //            cmd.Parameters.AddWithValue("@MailStatus", "1");
                    //            cmd.Parameters.AddWithValue("@PanelStatus", "N");
                    //            cmd.Parameters.AddWithValue("@PanelPart", "");
                    //            cmd.Parameters.AddWithValue("@PanelPrice", "0");
                    //            cmd.Transaction = tran;
                    //            cmd.CommandTimeout = 0;
                    //            cmd.ExecuteNonQuery();
                    //            cmd.Dispose();

                    //            if (kva_items == "")
                    //            {
                    //                kva_items = quotDts[0].Trim();
                    //            }
                    //            else
                    //            {
                    //                string[] duplicate = Regex.Split(kva_items, ",");
                    //                if (!duplicate.Contains(quotDts[0].Trim()))
                    //                {
                    //                    kva_items = kva_items + "," + quotDts[0].Trim();
                    //                }
                    //            }

                    //            #endregion

                    //            //For Addtinal Part 
                    //            addPartDts = Regex.Split(EnquirySubmitEnqDashboard.AdditionalPart_items, "@#@");
                    //            double AddPartCodeAmt = 0;
                    //            if (EnquirySubmitEnqDashboard.AdditionalPart_items != "")
                    //            {
                    //                foreach (string strSub in addPartDts)
                    //                {
                    //                    #region                               
                    //                    addDts = Regex.Split(strSub.Trim(), "-->");
                    //                    //  QtnPart = Add Part Parent Part
                    //                    if (quotDts[1].Trim() == addDts[0].Trim())
                    //                    {
                    //                        SrNo += 1;
                    //                        cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                    //                        cmd.CommandType = CommandType.StoredProcedure;
                    //                        cmd.Parameters.AddWithValue("@SearchType", "S");
                    //                        cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                    //                        cmd.Parameters.AddWithValue("@SrNo", SrNo);
                    //                        cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                    //                        cmd.Parameters.AddWithValue("@RecType", "Part");
                    //                        cmd.Parameters.AddWithValue("@PanelDesc", addDts[1].Trim());
                    //                        cmd.Parameters.AddWithValue("@Qty", Convert.ToDouble(addDts[2].Trim()));
                    //                        cmd.Parameters.AddWithValue("@PanelPrice", Convert.ToDouble(addDts[3].Trim()));
                    //                        cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                    //                        cmd.Transaction = tran;
                    //                        cmd.CommandTimeout = 0;
                    //                        cmd.ExecuteNonQuery();
                    //                        cmd.Dispose();
                    //                        AddPartCodeAmt = AddPartCodeAmt + Math.Round(Convert.ToDouble(addDts[2].Trim()) * Convert.ToDouble(addDts[3].Trim()), 0, MidpointRounding.AwayFromZero);
                    //                    }
                    //                    #endregion
                    //                }
                    //            }

                    //            // Transaport & GST Individual Part.                                                                   
                    //            #region
                    //            //Transaport 
                    //            double TranspotAmt = 0;
                    //            SrNo += 1;
                    //            cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                    //            cmd.CommandType = CommandType.StoredProcedure;
                    //            cmd.Parameters.AddWithValue("@SearchType", "S");
                    //            cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                    //            cmd.Parameters.AddWithValue("@SrNo", SrNo);
                    //            cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                    //            if (quotDts[4].Trim() == "IB")
                    //            {
                    //                cmd.Parameters.AddWithValue("@RecType", "IB");
                    //                cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Included");
                    //                cmd.Parameters.AddWithValue("@Qty", "0");
                    //                cmd.Parameters.AddWithValue("@PanelPrice", "0");
                    //            }
                    //            else if (quotDts[4].Trim() == "NB")
                    //            {
                    //                cmd.Parameters.AddWithValue("@RecType", "NB");
                    //                cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Charges");
                    //                cmd.Parameters.AddWithValue("@Qty", "1");
                    //                cmd.Parameters.AddWithValue("@PanelPrice", Convert.ToDouble(quotDts[5].Trim()));
                    //                TranspotAmt = Convert.ToDouble(quotDts[5].Trim());
                    //            }
                    //            else if (quotDts[4].Trim() == "EX")
                    //            {
                    //                cmd.Parameters.AddWithValue("@RecType", "EX");
                    //                cmd.Parameters.AddWithValue("@PanelDesc", "Transportation Extra");
                    //                cmd.Parameters.AddWithValue("@Qty", "0");
                    //                cmd.Parameters.AddWithValue("@PanelPrice", "0");
                    //            }
                    //            cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                    //            cmd.Transaction = tran;
                    //            cmd.CommandTimeout = 0;
                    //            cmd.ExecuteNonQuery();
                    //            cmd.Dispose();
                    //            // END Transaport

                    //            //GST 
                    //            double QtnPlusAddPartAmt = Math.Round(QouPartAmt + TranspotAmt + AddPartCodeAmt, 0);
                    //            double GSTAmt = 0;
                    //            if (EnquirySubmitEnqDashboard.GSTInEx.Trim() == "IN")
                    //            {
                    //                SrNo += 1;
                    //                cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                    //                cmd.CommandType = CommandType.StoredProcedure;
                    //                cmd.Parameters.AddWithValue("@SearchType", "S");
                    //                cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                    //                cmd.Parameters.AddWithValue("@SrNo", SrNo);
                    //                cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                    //                cmd.Parameters.AddWithValue("@RecType", "Total");
                    //                cmd.Parameters.AddWithValue("@PanelDesc", "Total Amount");
                    //                cmd.Parameters.AddWithValue("@Qty", "1");
                    //                cmd.Parameters.AddWithValue("@PanelPrice", QtnPlusAddPartAmt.ToString().Trim());
                    //                cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                    //                cmd.Transaction = tran;
                    //                cmd.CommandTimeout = 0;
                    //                cmd.ExecuteNonQuery();
                    //                cmd.Dispose();

                    //                SrNo += 1;
                    //                cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                    //                cmd.CommandType = CommandType.StoredProcedure;
                    //                cmd.Parameters.AddWithValue("@SearchType", "S");
                    //                cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                    //                cmd.Parameters.AddWithValue("@SrNo", SrNo);
                    //                cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                    //                cmd.Parameters.AddWithValue("@RecType", "GST");
                    //                cmd.Parameters.AddWithValue("@PanelDesc", "GST @18%");
                    //                cmd.Parameters.AddWithValue("@Qty", "1");
                    //                GSTAmt = Math.Round((QtnPlusAddPartAmt * 18) / 100);
                    //                cmd.Parameters.AddWithValue("@PanelPrice", GSTAmt);
                    //                cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                    //                cmd.Transaction = tran;
                    //                cmd.CommandTimeout = 0;
                    //                cmd.ExecuteNonQuery();
                    //                cmd.Dispose();
                    //            }
                    //            else if (EnquirySubmitEnqDashboard.GSTInEx.Trim() == "EX")
                    //            {
                    //                SrNo += 1;
                    //                cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                    //                cmd.CommandType = CommandType.StoredProcedure;
                    //                cmd.Parameters.AddWithValue("@SearchType", "S");
                    //                cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                    //                cmd.Parameters.AddWithValue("@SrNo", SrNo);
                    //                cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                    //                cmd.Parameters.AddWithValue("@RecType", "GST");
                    //                cmd.Parameters.AddWithValue("@PanelDesc", "GST Extra");
                    //                cmd.Parameters.AddWithValue("@Qty", "0");
                    //                cmd.Parameters.AddWithValue("@PanelPrice", "0");
                    //                cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                    //                cmd.Transaction = tran;
                    //                cmd.CommandTimeout = 0;
                    //                cmd.ExecuteNonQuery();
                    //                cmd.Dispose();
                    //            }
                    //            // END Quatation  GST taxation

                    //            if (AddPartCodeAmt == 0 && quotDts[4].Trim() == "EX" && EnquirySubmitEnqDashboard.GSTInEx.Trim() == "EX")
                    //            {
                    //                // Do not Save Grand Total. Because Transpot and GST Extra.
                    //            }
                    //            else
                    //            {
                    //                //Quatation Grand Total
                    //                SrNo += 1;
                    //                cmd = new SqlCommand("InsertUpdateQuotationDetailsSubFinal", con);
                    //                cmd.CommandType = CommandType.StoredProcedure;
                    //                cmd.Parameters.AddWithValue("@SearchType", "S");
                    //                cmd.Parameters.AddWithValue("@QtnNo", QtnNo.Trim());
                    //                cmd.Parameters.AddWithValue("@SrNo", SrNo);
                    //                cmd.Parameters.AddWithValue("@QtnPart", quotDts[1].Trim());
                    //                cmd.Parameters.AddWithValue("@RecType", "GT");
                    //                cmd.Parameters.AddWithValue("@PanelDesc", "Grand Total");
                    //                cmd.Parameters.AddWithValue("@Qty", "1");
                    //                cmd.Parameters.AddWithValue("@PanelPrice", Math.Round(QtnPlusAddPartAmt + GSTAmt));
                    //                cmd.Parameters.AddWithValue("@PanelGADrawNo", "");
                    //                cmd.Transaction = tran;
                    //                cmd.CommandTimeout = 0;
                    //                cmd.ExecuteNonQuery();
                    //                cmd.Dispose();
                    //                //END Quatation Grand Total
                    //            }
                    //            SrNo = 0;
                    //            #endregion

                    //            //Inst Offer
                    //            #region                                    
                    //            instOfferDts = Regex.Split(EnquirySubmitEnqDashboard.InstallationOffer_items, "@#@");
                    //            if (EnquirySubmitEnqDashboard.InstallationOffer_items != "")
                    //            {
                    //                foreach (string strSub in instOfferDts)
                    //                {
                    //                    instDts = Regex.Split(strSub.Trim(), "-->");
                    //                    //instKVA=QuotKva
                    //                    if (instDts[2].Trim() == quotDts[0].Trim())
                    //                    {
                    //                        sb.Remove(0, sb.Length);
                    //                        sb.Append("Insert into QuotationDetailsInstallationOffer ");
                    //                        sb.Append("(QtnNo,SeqNo,PartCode,ScopeType,InstOfferCode,InstOfferDesc,SizeParameter,Qty,UOM,Rate,InstPartCode)");
                    //                        sb.Append(" VALUES ('" + QtnNo.Trim() + "','" + instDts[1].Trim() + "','" + quotDts[1].Trim() + "','" + instDts[3].Trim() + "',");
                    //                        sb.Append("'" + instDts[0].Trim() + "','" + instDts[4].Trim() + "','" + instDts[6].Trim() + "','" + instDts[7].Trim() + "',");
                    //                        sb.Append("'" + instDts[8].Trim() + "','" + instDts[9].Trim() + "','" + instDts[5].Trim() + "')");
                    //                        cmd = new SqlCommand(sb.ToString(), con);
                    //                        cmd.Transaction = tran;
                    //                        cmd.ExecuteNonQuery();
                    //                        cmd.Dispose();
                    //                    }
                    //                }
                    //            }
                    //            #endregion
                    //        }
                    //    }

                    //    //Loginmst
                    //    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    //    cmd.CommandType = CommandType.StoredProcedure;
                    //    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    //    cmd.Parameters.AddWithValue("@EmpID", EnquirySubmitEnqDashboard.UserID.Trim());
                    //    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    //    cmd.Parameters.AddWithValue("@TransactionFrom", "Quotation");
                    //    cmd.Parameters.AddWithValue("@TransactionNo", QtnNo.Trim());
                    //    cmd.Parameters.AddWithValue("@CompanyCode", EnquirySubmitEnqDashboard.CompID.Trim());
                    //    cmd.Transaction = tran;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();
                    //   
                    //}
                    #endregion
                }

                tran.Commit();


                emailStatus = "";
                if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "true")
                {

                    emailStatus = "Failed";

                    iGreen = "N"; KG = "N";
                    StrQKAV = "";
                    DataSet ds = cls.procDS("select  cmobileno, compmailid from employee where ecode='" + EnquirySubmitEnqDashboard.AssToEmpCode.Trim() + "'", "tbl_employee");

                    //Details                    
                    quotPartDts = Regex.Split(EnquirySubmitEnqDashboard.QuotPartCode_items, "@#@");
                    SrNo = 0;
                    if (EnquirySubmitEnqDashboard.QuotPartCode_items != "")
                    {
                        foreach (string qoutSub in quotPartDts)
                        {
                            quotDts = Regex.Split(qoutSub.Trim(), "-->");

                            if (StrQKAV == "")
                            {
                                StrQKAV = quotDts[0].Trim();
                            }
                            else
                            {
                                string[] duplicate = Regex.Split(kva_items, ",");
                                if (!duplicate.Contains(quotDts[0].Trim()))
                                {
                                    StrQKAV = StrQKAV + "," + quotDts[0].Trim();
                                }
                            }

                            // 125 6R(kg)
                            if (Convert.ToDouble(quotDts[0].ToString().Trim().Trim()) == 125 && quotDts[6].Trim().ToString().Trim().Substring(0, 2).Trim() == "6R")
                            {
                                //KG = "Y";
                            } // iGreen KVA
                            else if ((Convert.ToDouble(quotDts[0].ToString().Trim()) >= 5 && Convert.ToDouble(quotDts[0].ToString().Trim()) <= 160) && quotDts[6].ToString().Trim().Substring(0, 2).Trim() != "CC")
                            {
                                iGreen = "Y";
                            } // kg
                            else
                            {
                                //KG = "Y";
                            }
                        }


                        string strQuotkVA = "";
                        string[] ch = Regex.Split(StrQKAV, ",");
                        string[] chNew = new string[ch.Length];
                        for (int i = 0; i < ch.Length; i++)
                        {
                            chNew[i] = ch[i].ToString();
                        }
                        string[] distVal = chNew.Distinct().ToArray();
                        foreach (string c in distVal)
                        {
                            if (strQuotkVA.Trim() == "")
                            {
                                strQuotkVA = c.ToString();
                            }
                            else if (strQuotkVA.Trim() != "")
                            {
                                strQuotkVA = strQuotkVA + "," + c.ToString();
                            }
                        }

                        //Send Quotation Mail
                        if (iGreen == "Y" && KG == "Y")
                        {
                            // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditorResponse", "alert('You Cannot Quot iGreen & Non iGreen Product within same Quotation...!')", true);
                        }
                        else
                        {
                            if (iGreen.Trim() == "Y")
                            {
                                // SendEmail(QtnNo.Trim(), strQuotkVA.ToString().Trim(), StrCorrectCCmailid.ToString().Trim(), rblQtnInts.SelectedValue.ToString().Trim(), txtSubject.Text.Trim(), lblPCName.Text.Trim(), "iGreen");



                                sendQuotEmail(AssToEmpPCCode, QtnNo.Trim(), strQuotkVA.Trim(), EnquirySubmitEnqDashboard.EMailID.Trim(), EnquirySubmitEnqDashboard.CCMailID.Trim(), EnquirySubmitEnqDashboard.AssToEmpCode.Trim(), EnquirySubmitEnqDashboard.AssToEmpName.Trim(), ds.Tables["tbl_employee"].Rows[0]["cmobileno"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["compmailid"].ToString().Trim(), EnquirySubmitEnqDashboard.CustName.Trim(), EnquirySubmitEnqDashboard.Address.Trim(), "iGreen");

                                //       string url = "https://www.kalapms.com/QuotApi/Service.asmx/sendQuotEmailAngular?qtn_no=" + QtnNo.Trim() + "&kva=" + kva_items.Trim() + "&to_mail_id=" + EnquirySubmitEnqDashboard.EMailID.Trim()
                                //+ "&cc_mail_id=" + EnquirySubmitEnqDashboard.CCMailID.Trim() + "&ass_to_emp_id=" + EnquirySubmitEnqDashboard.AssToEmpCode.Trim() + "&ass_to_emp_name=" + EnquirySubmitEnqDashboard.AssToEmpName.Trim()
                                //+ "&ass_to_mob_no=" + ds.Tables["tbl_employee"].Rows[0]["cmobileno"].ToString().Trim() + "&ass_to_mail_id=" + ds.Tables["tbl_employee"].Rows[0]["compmailid"].ToString().Trim()
                                //+ "&cust_name=" + EnquirySubmitEnqDashboard.CustName.Trim() + "&cust_add=" + EnquirySubmitEnqDashboard.Address.Trim()
                                //       + "&Model=iGreen";


                                //       HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);
                                //       Request.Method = "GET";
                                //       Request.KeepAlive = true;
                                //       HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
                                //       if (Response.StatusCode == HttpStatusCode.OK)
                                //       {
                                //  emailStatus = "Successfully";
                                //       }
                                //       Response.Close();
                            }
                            else
                            {
                                // SendEmail(ViewState["QtnNo"].ToString().Trim(), strQuotkVA.ToString().Trim(), StrCorrectCCmailid.ToString().Trim(), rblQtnInts.SelectedValue.ToString().Trim(), txtSubject.Text.Trim(), lblPCName.Text.Trim(), "NoniGreen"); 


                                sendQuotEmail(AssToEmpPCCode, QtnNo.Trim(), strQuotkVA.Trim(), EnquirySubmitEnqDashboard.EMailID.Trim(), EnquirySubmitEnqDashboard.CCMailID.Trim(), EnquirySubmitEnqDashboard.AssToEmpCode.Trim(), EnquirySubmitEnqDashboard.AssToEmpName.Trim(), ds.Tables["tbl_employee"].Rows[0]["cmobileno"].ToString().Trim(), ds.Tables["tbl_employee"].Rows[0]["compmailid"].ToString().Trim(), EnquirySubmitEnqDashboard.CustName.Trim(), EnquirySubmitEnqDashboard.Address.Trim(), "NoniGreen");



                                //      string url = "https://www.kalapms.com/QuotApi/Service.asmx/sendQuotEmailAngular?qtn_no=" + QtnNo.Trim() + "&kva=" + kva_items.Trim() + "&to_mail_id=" + EnquirySubmitEnqDashboard.EMailID.Trim()
                                //+ "&cc_mail_id=" + EnquirySubmitEnqDashboard.CCMailID.Trim() + "&ass_to_emp_id=" + EnquirySubmitEnqDashboard.AssToEmpCode.Trim() + "&ass_to_emp_name=" + EnquirySubmitEnqDashboard.AssToEmpName.Trim()
                                //+ "&ass_to_mob_no=" + ds.Tables["tbl_employee"].Rows[0]["cmobileno"].ToString().Trim() + "&ass_to_mail_id=" + ds.Tables["tbl_employee"].Rows[0]["compmailid"].ToString().Trim()
                                //+ "&cust_name=" + EnquirySubmitEnqDashboard.CustName.Trim() + "&cust_add=" + EnquirySubmitEnqDashboard.Address.Trim()
                                // + "&Model=NoniGreen";

                                //      HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);
                                //      Request.Method = "GET";
                                //      Request.KeepAlive = true;
                                //      HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
                                //      if (Response.StatusCode == HttpStatusCode.OK)
                                //      {
                                //          emailStatus = "Successfully";
                                //      }
                                //      Response.Close();
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                tran.Rollback();
                emailStatus = "Failed StackTrace " + ex.StackTrace + ", Message " + ex.Message;
            }
            finally
            {
                con.Close();
            }


            if (EnquirySubmitEnqDashboard.SendQuot.Trim() == "true")
            {
                if (EnquirySubmitEnqDashboard.SaveType.Trim() == "FollowUp")
                {
                    return "Enquiry: " + EnqNo + " Updated and Quotation :" + QtnNo.Trim() + " Saved Successfully, Email Send " + emailStatus;
                }
                else
                {
                    return "Enquiry: " + EnqNo + " and Quotation :" + QtnNo.Trim() + " Saved Successfully, Email Send " + emailStatus;
                }
            }
            else
            {
                if (EnquirySubmitEnqDashboard.SaveType.Trim() == "FollowUp")
                {
                    return "Enquiry: " + EnqNo + " Updated Successfully ";// + EnquirySubmitEnqDashboard.SendQuot.Trim();
                }
                else
                {
                    return "Enquiry: " + EnqNo + " Saved Successfully ";// + EnquirySubmitEnqDashboard.SendQuot.Trim();
                }
            }
        }

        //  string strFilePath_1 = ConfigurationManager.AppSettings["Quotationfile"] + "/" + "QTN_24-25_07013431_10-03-2025_16_21_53.pdf";
        //public void sendQuotEmail(string qtn_AssToEmpPCCode ,string qtn_no, string kva, string to_mail_id, string cc_mail_id, string ass_to_emp_id, string ass_to_emp_name, string ass_to_mob_no, string ass_to_mail_id, string cust_name, string cust_add, string model)
        //{
        //    SmtpClient sc;
        //    string Body = "";
        //    string Original_Qtn_No = qtn_no;
        //    qtn_no = qtn_no.Replace("/", "_");
        //   string strFilePath_1 = ConfigurationManager.AppSettings["Quotationfile"] + "/" + qtn_no + "_" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".pdf";

        //    string dt_time = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

        //    string strpdf_1 = qtn_no + "_" + dt_time + ".pdf";
        //    try
        //    {
        //        LoadReport(Original_Qtn_No.Trim(), strFilePath_1.ToString().Trim(), "Q", ass_to_emp_id.Trim(), ass_to_emp_name.Trim(), ass_to_mob_no.Trim(), ass_to_mail_id.Trim(), model.Trim());

        //        if (strFilePath_1.ToString().Trim() != "")
        //        {
        //            MailMessage m = new MailMessage();
        //            sc = new SmtpClient();
        //            FileStream fs1 = new FileStream(strFilePath_1, FileMode.Open, FileAccess.Read);
        //            Attachment a1 = new Attachment(fs1, strpdf_1, MediaTypeNames.Application.Octet);
        //            m.Attachments.Add(a1);

        //            #region
        //            int flg_anubandh = 0;
        //            int flg_2kW_5kVA = 0;
        //            int flg_R550 = 0;
        //            int flg_5_To_12_kVA = 0;
        //            int flg_15_To_30_kVA = 0;
        //            int flg_40_To_160_kVA = 0;
        //            int flg_200_To_250_kVA = 0;
        //            int flg_320_1010_kVA = 0;
        //            int flg_1250_kVA_1500 = 0;

        //            int flg_2_8_To_5_5_kVA = 0;
        //            int flg_7_5_To_20_kVA = 0;
        //            int flg_NG_CPCB_IV_15_to_250_kVA = 0;
        //            int flg_OP_CPCB_IV_117_2000_kVA;

        //            string[] split_kva_item = Regex.Split(kva.Trim(), ",");
        //            if (split_kva_item.Length > 0)
        //            {
        //                for (int j = 0; j < split_kva_item.Length; j++)
        //                {
        //                    string strFilePath_2 = "", strFilePath_3 = "";
        //                    string strpdf_2 = "", strpdf_3 = "";
        //                    string model_type = cls.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + Original_Qtn_No.Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
        //                    string Qtn_CPCB = cls.getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");
        //                    //int Opti = Convert.ToInt32 (clsCommonFunctions.getName("select isnull(sum(k.Qty),0) as EngCount from quotationdetails q inner join part p on q.partcode=p.PartCode inner join kitdetails k on k.kitcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and k.PartCode like '001%' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "EngCount"));
        //                    string Opti_CPCB = cls.getName("select top 1 substring(q.PartCode,10,1) as OptiCPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "OptiCPCB");

        //                    //if (Qtn_CPCB == "4")
        //                    //{
        //                    //if (model_type.Trim().PadRight(3).Trim() == "NG1")
        //                    if (model_type.Substring(model_type.Trim().Length - 3) == "NG1")
        //                    {
        //                        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/NG_CPCB IV+15-250_kVA.PDF";
        //                        strpdf_2 = "NG_CPCB IV+15-250_kVA.PDF";
        //                        flg_NG_CPCB_IV_15_to_250_kVA = 1;
        //                    }
        //                    else
        //                    {
        //                        if (Opti_CPCB == "5")
        //                        {
        //                            strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/OP_CPCB IV+117-2000_kVA.PDF";
        //                            strpdf_2 = "OP_CPCB IV+117-2000_kVA.PDF";
        //                            flg_OP_CPCB_IV_117_2000_kVA = 1;
        //                        }
        //                        else
        //                        {

        //                            //if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
        //                            //{
        //                            //    //strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/Kirloskar_2kW_5kVA.pdf";
        //                            //    //strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
        //                            //    //flg_2kW_5kVA = 1;
        //                            //}


        //                            //2. 8kW-5.5kVA
        //                            if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 7))
        //                            {
        //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+2.8-5.5_kVA.PDF";
        //                                strpdf_2 = "CPCB_IV+2.8-5.5_kVA.PDF";
        //                                flg_2_8_To_5_5_kVA = 1;
        //                            }
        //                            // EA Series
        //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 7 && Convert.ToDouble(split_kva_item[j].Trim()) <= 20))
        //                            {
        //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+7.5-20_kVA.PDF";
        //                                strpdf_2 = "CPCB_IV+7.5-20_kVA.PDF";
        //                                flg_7_5_To_20_kVA = 1;
        //                            }
        //                            //3R550
        //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 25 && Convert.ToDouble(split_kva_item[j].Trim()) <= 58.5))
        //                            {
        //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+25-58.5_kVA.PDF";
        //                                strpdf_2 = "CPCB_IV+25-58.5_kVA.PDF";
        //                                flg_R550 = 1;
        //                            }
        //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 80 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160))
        //                            {
        //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+82.5-160_kVA.PDF";
        //                                strpdf_2 = "CPCB_IV+82.5-160_kVA.PDF";
        //                                flg_15_To_30_kVA = 1;
        //                            }
        //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250))
        //                            {
        //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+200-250_kVA.PDF";
        //                                strpdf_2 = "CPCB_IV+200-250_kVA.PDF";
        //                                flg_40_To_160_kVA = 1;
        //                            }
        //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 750))
        //                            {
        //                                //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
        //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+320-750_kVA.PDF";
        //                                strpdf_2 = "CPCB_IV+320-750_kVA.PDF";
        //                                flg_320_1010_kVA = 1;
        //                            }
        //                            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1010 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500))
        //                            {
        //                                //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
        //                                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+1010-1500_kVA.PDF";
        //                                strpdf_2 = "CPCB_IV+1010-1500_kVA.PDF";
        //                                flg_320_1010_kVA = 1;
        //                            }
        //                        }
        //                    }

        //                    //}

        //                    // Commented by KB on 23/07/2024 as CPCB 2 is Excluded
        //                    //else
        //                    //{
        //                    //    if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
        //                    //    {
        //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_2kW_5kVA.pdf";
        //                    //        strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
        //                    //        flg_2kW_5kVA = 1;
        //                    //    }
        //                    //    // EA Series
        //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 12.5) && flg_5_To_12_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
        //                    //    {
        //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/iGreen-5-160kVA.pdf";
        //                    //        strpdf_2 = "Catlogue_iGreen_5_To_160_kVA.pdf";
        //                    //        flg_5_To_12_kVA = 1;
        //                    //    }
        //                    //    //3R550
        //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 15) && flg_R550 == 0 && model_type.Trim().Substring(0, 5).Trim() == "3R550")
        //                    //    {
        //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_R550_15_kVA.pdf";
        //                    //        strpdf_2 = "Kirloskar_R550_15_kVA_Catlogue.pdf";
        //                    //        flg_R550 = 1;
        //                    //    }
        //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 30) && flg_15_To_30_kVA == 0)
        //                    //    {
        //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_15_To_30_kVA.pdf";
        //                    //        strpdf_2 = "Kirloskar_15_To_30_kVA_Catlogue.pdf";
        //                    //        flg_15_To_30_kVA = 1;
        //                    //    }
        //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 40 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_40_To_160_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "6R")
        //                    //    {
        //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_40_To_160_kVA.pdf";
        //                    //        strpdf_2 = "Kirloskar_40_To_160_kVA_Catalogue.pdf";
        //                    //        flg_40_To_160_kVA = 1;
        //                    //    }
        //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250) && flg_200_To_250_kVA == 0)
        //                    //    {
        //                    //        // Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
        //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_200_To_250_kVA.pdf";
        //                    //        strpdf_2 = "Kirloskar_200_To_250_kVA_Catlogue.pdf";
        //                    //        flg_200_To_250_kVA = 1;
        //                    //    }
        //                    //    //else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1010) && flg_320_1010_kVA == 0)
        //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) < 1000) && flg_320_1010_kVA == 0)
        //                    //    {
        //                    //        // Commented by KB on 08/05/2023 as per mail from Kalpesh Sir on 06/05/2023
        //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_320_1010_kVA.pdf";
        //                    //        strpdf_2 = "Kirloskar_320_1010_kVA_Catlogue.pdf";
        //                    //        flg_320_1010_kVA = 1;
        //                    //    }
        //                    //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1250 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500) && flg_1250_kVA_1500 == 0)
        //                    //    {
        //                    //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_1250_To_1500_kVA.pdf";
        //                    //        strpdf_2 = "Kirloskar_1250_To_1500_kVA_Catlogue.pdf";
        //                    //        flg_1250_kVA_1500 = 1;
        //                    //    }
        //                    //}
        //                    // Commented by KB on 23/07/2024

        //                    if (File.Exists(strFilePath_2))
        //                    {
        //                        FileStream fs2 = new FileStream(strFilePath_2, FileMode.Open, FileAccess.Read);
        //                        Attachment a2 = new Attachment(fs2, strpdf_2, MediaTypeNames.Application.Octet);
        //                        m.Attachments.Add(a2);
        //                    }

        //                    // New Anubandh
        //                    if (Qtn_CPCB != "4")
        //                    {
        //                        if (Convert.ToDouble(split_kva_item[j].Trim()) == 125 && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() == "6R")
        //                        {
        //                            //Skip Anubandh by KB on 21/12/2023

        //                            //Skip Anubandh
        //                        }
        //                        else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
        //                        {
        //                            //Skip Anubandh by KB on 21/12/2023
        //                            //strFilePath_3 = AppDomain.CurrentDomain.BaseDirectory + "quotation/quotation/Anubandh.pdf";
        //                            //strpdf_3 = "Anubandh.pdf";
        //                            //flg_anubandh = 1;

        //                            if (File.Exists(strFilePath_3))
        //                            {
        //                                //Skip Anubandh by KB on 21/12/2023

        //                                //FileStream fs3 = new FileStream(strFilePath_3, FileMode.Open, FileAccess.Read);
        //                                //Attachment a3 = new Attachment(fs3, strpdf_3, MediaTypeNames.Application.Octet);
        //                                //m.Attachments.Add(a3);
        //                            }
        //                        }
        //                    }
        //                    //End 
        //                }
        //            }
        //            #endregion

        //            m.From = new MailAddress("erp@kalabiz.com", "Kala Genset Pvt. Ltd. - ERP System");

        //            #region
        //            //ToMail
        //            if (to_mail_id.Length > 0 && !string.IsNullOrEmpty(to_mail_id))
        //            {
        //                string[] ToMailIDitems = Regex.Split(to_mail_id.Trim(), ",");
        //                foreach (string strTo in ToMailIDitems)
        //                {
        //                    m.To.Add(new MailAddress(strTo));
        //                }
        //            }
        //            //ToMail END

        //            if (string.IsNullOrEmpty(cc_mail_id))
        //            {
        //                cc_mail_id = ass_to_mail_id;
        //            }

        //            //Add sales.support7@kalabiz.com and Indore and bhopal HOD EmailID IN CC
        //            if (qtn_AssToEmpPCCode.Trim() == "07.005")//  Indore 
        //            {
        //                if (string.IsNullOrEmpty(cc_mail_id))
        //                {
        //                    cc_mail_id = "vijay.nikam@kalabiz.com" + "," + "sales.indore@kalabiz.com";
        //                }
        //                else
        //                {
        //                    cc_mail_id = cc_mail_id.ToString().Trim() + "," + "vijay.nikam@kalabiz.com" + "," + "sales.indore@kalabiz.com" + "," + "vaishakh.pathak@kalabiz.com";
        //                }
        //            }
        //            if (qtn_AssToEmpPCCode.Trim() == "07.014")// Bhopal
        //            {

        //                if (string.IsNullOrEmpty(cc_mail_id))
        //                {
        //                    cc_mail_id = "vijay.nikam@kalabiz.com" + "," + "sales.bhopal@kalabiz.com";
        //                }
        //                else
        //                {
        //                    cc_mail_id = cc_mail_id.ToString().Trim() + "," + "vijay.nikam@kalabiz.com" + "," + "sales.bhopal@kalabiz.com";
        //                }
        //            }


        //            if (!string.IsNullOrEmpty(cc_mail_id))
        //            {
        //                cc_mail_id = cc_mail_id.ToString().Trim() + "," + ass_to_mail_id;
        //            }

        //            //CCMail 
        //            if (cc_mail_id.Length > 0 & !string.IsNullOrEmpty(cc_mail_id))
        //            {
        //                string[] CCMailIDitems = Regex.Split(cc_mail_id.Trim(), ",");
        //                foreach (string strCC in CCMailIDitems)
        //                {
        //                    m.CC.Add(new MailAddress(strCC));
        //                }
        //            }

        //            //CCMail  END

        //            //BCCMail                 
        //            string[] BCCMailIDitems = Regex.Split("skk@kalabiz.com", ",");
        //            foreach (string strBCC in BCCMailIDitems)
        //            {
        //                m.Bcc.Add(new MailAddress(strBCC));
        //            }

        //            //BCCMail  END
        //            #endregion

        //            #region
        //            Body = "";
        //            Body += "<p>To,";
        //            Body += "<BR><span style='color:#1F497D'>" + cust_name.Trim() + " </span>";
        //            Body += "<BR><span style='color:#1F497D'>" + cust_add.Trim() + " </span></p>";

        //            Body += "<p>Subject: Proposal for " + kva.ToString().Trim() + " kVA Silent DG Set</p>";

        //            Body += "<p>KALA a popular name synonymous with power, stands tall as a market leader ";
        //            Body += "serving industrial, commercial and residential segments in the domestic as well ";
        //            Body += "as international markets.</p>";
        //            Body += "<p>KALA an IMS(Integrated Management System) Certified company has grown to be one of the biggest names in power ";
        //            Body += "generation solutions. Today, our team continues to focus on constant innovation ";
        //            Body += "and commitment towards Customer Satisfaction. We concentrate on our ";
        //            Body += "efforts in bringing the highest quality power solutions to your corporate/business/residence.</p>";

        //            Body += "<p>We as an authorized Genset Original Equipment Manufacturers (G.O.E.M's) for ";

        //            Body += "Kirloskar Oil Engines Limited-Pune, manufacture quality KOEL Green ";
        //            Body += "Generating Sets in the range of 2.1 kVA to 1500 kVA for captive and standby ";
        //            Body += "applications with options such as liquid cooled and air cooled DG sets.</p>";

        //            Body += "<p>We provide complete turnkey solutions which include site selection, load study, ";
        //            Body += "application engineering, installation, commissioning and obtaining statutory ";
        //            Body += "approvals from the Government agencies.</p>";

        //            Body += "<p>With our headquarters at Pune, We are having 3 state of the art, ultra-modern ";
        //            Body += "manufacturing and testing facilities at Chakan, Silvassa, Belgaum. We are able ";
        //            Body += "to cater to both logistic requirements as well as commercial benefits in ";
        //            Body += "taxations. We manufacture world class accoustic enclosures (Sound Proof ";
        //            Body += "Canopies) as per Central Pollution Control Board (CPCB) Norms.</p>";

        //            Body += "<p style='text-align:justify'>";
        //            Body += "Our dedicated branch offices at Pune, Chinchwad, Mumbai(Kharghar), Kolhapur, ";
        //            Body += "Solapur, Sangli, Aurangabad, Latur, Beed, Nanded, Indore, Bhopal, Gwalior, Bangalore, Belgaon, Hyderabad along with the business associates &amp; dealers are available to ";
        //            Body += "provide sales and service to our widespread customer network. Our ";
        //            Body += "marketing team is always available at your service to meet your requirements ";
        //            Body += "and satisfy all your needs 24/7. We understand the popular saying 'Customers ";
        //            Body += "have a choice' and we are grateful to you for considering us as your ";
        //            Body += "preferred choice.</p>";


        //            Body += "<p>For Genset & Commercial Details, Please find attachment.</p>";

        //            Body += "<p style='text-align: justify'><span><span style='color:#1F497D'>Thank You/Regards.</span></span></p>";

        //            Body += "<p style='text-align: justify'><span><span style='color:#1F497D'>" + ass_to_emp_name.Trim() + " <br> MailID - " + ass_to_mail_id.Trim() + " <br> MobileNo - " + ass_to_mob_no.Trim() + " <br> Toll Free No - 1800 123 0018</span></span></p>";

        //            Body += "<BR><img alt=\"\" hspace=0 src=\"cid:imageId\" align=baseline border=0>";

        //            //if (Original_Qtn_No.Trim().Substring(10, 2).Trim() == "16")
        //            //{
        //            //    Body += "<p style='text-align: justify'><span><span style='color:#1F497D'><b>Kala International LLC</b></span></span></p>";
        //            //}
        //            //else
        //            //{
        //                Body += "<p style='text-align: justify'><span><span style='color:#1F497D'><b>Kala Genset Pvt ltd.</b></span></span></p>";
        //           // }

        //            #endregion

        //            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");

        //            //  var filePath = Path.Combine("~//images//kalalogo.JPG", "images", "kalalogo.JPG");

        //            LinkedResource imagelink = new LinkedResource(AppDomain.CurrentDomain.BaseDirectory + "images/kalalogo.JPG", "image/jpeg");

        //            //  LinkedResource imagelink = new LinkedResource(Server.MapPath("~//images//kalalogo.JPG"), "image/png");
        //            imagelink.ContentId = "imageId";
        //            imagelink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
        //            htmlView.LinkedResources.Add(imagelink);
        //            m.AlternateViews.Add(htmlView);

        //            m.Subject = "Proposal for " + kva.ToString().Trim() + " kVA Silent DG Set";

        //            m.IsBodyHtml = true;
        //            m.Body = Body;
        //            m.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

        //            if (!string.IsNullOrEmpty(ass_to_mail_id.ToString().Trim()))
        //            {
        //                m.ReplyTo = new MailAddress(ass_to_mail_id.ToString().Trim());
        //            }

        //            if (sc != null)
        //            {
        //                sc.Credentials = new System.Net.NetworkCredential("erp@kalabiz.com", "wanv ftwc dobq blrr");
        //                sc.Port = 587;
        //                sc.Host = "smtp.gmail.com";
        //                sc.EnableSsl = true;
        //                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
        //                sc.Timeout = 999999999;
        //                sc.ServicePoint.MaxIdleTime = 1;
        //               sc.Send(m);
        //            }

        //            m.Dispose();
        //            sc = null;

        //            fs1.Close();
        //            fs1.Dispose();
        //            //Code to Delete The Temp File
        //            //if (File.Exists(strFilePath_1))
        //            //{
        //            //    File.Delete(strFilePath_1);
        //            //}
        //        }
        //        //  return "Successfully";
        //        emailStatus = "Successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        //if (File.Exists(strFilePath_1))
        //        //{
        //        //    File.Delete(strFilePath_1);
        //        //}
        //        //  return "Failed" + ex.StackTrace;
        //        emailStatus = "Failed";
        //        //return "Failed";
        //    }
        //}


        public void sendQuotEmail(string qtn_AssToEmpPCCode, string qtn_no, string kva, string to_mail_id, string cc_mail_id, string ass_to_emp_id, string ass_to_emp_name, string ass_to_mob_no, string ass_to_mail_id, string cust_name, string cust_add, string model)
        {

            SmtpClient sc;
            string Body = "";
            string Original_Qtn_No = qtn_no;
            qtn_no = qtn_no.Replace("/", "_");
            string strFilePath_1 = ConfigurationManager.AppSettings["Quotationfile"] + "/" + qtn_no + "_" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".pdf";

            string dt_time = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            string strpdf_1 = qtn_no + "_" + dt_time + ".pdf";
            try
            {
                LoadReport(Original_Qtn_No.Trim(), strFilePath_1.ToString().Trim(), "Q", ass_to_emp_id.Trim(), ass_to_emp_name.Trim(), ass_to_mob_no.Trim(), ass_to_mail_id.Trim(), model.Trim());

                if (strFilePath_1.ToString().Trim() != "")
                {
                    MailMessage m = new MailMessage();
                    sc = new SmtpClient();
                    FileStream fs1 = new FileStream(strFilePath_1, FileMode.Open, FileAccess.Read);
                    Attachment a1 = new Attachment(fs1, strpdf_1, MediaTypeNames.Application.Octet);
                    m.Attachments.Add(a1);

                    #region
                    int flg_anubandh = 0;
                    //int flg_2kW_5kVA = 0;
                    //int flg_R550 = 0;
                    //int flg_5_To_12_kVA = 0;
                    //int flg_15_To_30_kVA = 0;
                    //int flg_40_To_160_kVA = 0;
                    //int flg_200_To_250_kVA = 0;
                    //int flg_320_1010_kVA = 0;
                    //int flg_1250_kVA_1500 = 0;

                    //int flg_2_8_To_5_5_kVA = 0;
                    //int flg_7_5_To_20_kVA = 0;
                    //int flg_NG_CPCB_IV_15_to_250_kVA = 0;
                    //int flg_OP_CPCB_IV_117_2000_kVA;

                    int flg_NG_CPCB_IV_15_to_250_kVA = 0;
                    int flg_2_To_7_kVA = 0;
                    int flg_7_To_20_kVA = 0;
                    int flg_25_To_58_5_kVA = 0;
                    int flg_80_To_160_kVA = 0;
                    int flg_200_To_250_kVA = 0;
                    int flg_320_TO_750_kVA = 0;
                    int flg_1010_TO_1500_kVA = 0;
                    int flg_OP_CPCB_IV_117_To_2000_kVA = 0;


                    //GK Product 

                    int flg_7_5_To_25_kVA_GK = 0;

                    string strProc = "";
                    strProc += " Select  QD.QtnNo,QD.Qty,QD.BasicPrice, ";
                    strProc += " QD.Partcode,PT.PartDesc,Pt.KVA,Pt.Phase,Pt.Model,Pt.cfm , substring(QD.PartCode,15,1) as CPCB ,substring(QD.PartCode,10,1) as OptiCPCB ";
                    strProc += " ,PT.rating from Quotation Q inner join QuotationDetails  QD on Q.QtnNo=QD.QtnNo inner join Part  PT on PT.PartCode=QD.PartCode ";
                    strProc += " WHERE Q.QtNNo='" + Original_Qtn_No.Trim() + "' ";
                    DataSet ds = cls.procDS(strProc, "QQ1");



                    // Prv Ouotation Details
                    if (ds.Tables["QQ1"].Rows.Count > 0)
                    {
                        for (int i = 0; (i < ds.Tables["QQ1"].Rows.Count); i++)
                        {
                            string str_FilePath = "";
                            string str_pdf_Name = "";


                            string Str_KVA = ds.Tables["QQ1"].Rows[i]["KVA"].ToString().Trim();
                            string model_type = ds.Tables["QQ1"].Rows[i]["Model"].ToString().Trim();
                            string Qtn_CPCB = ds.Tables["QQ1"].Rows[i]["CPCB"].ToString().Trim();
                            string Opti_CPCB = ds.Tables["QQ1"].Rows[i]["OptiCPCB"].ToString().Trim();


                            //GK Product
                            string Str_Product_GKOrNOt = ds.Tables["QQ1"].Rows[i]["rating"].ToString().Trim();


                            //string model_type = clsCommonFunctions.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
                            //string Qtn_CPCB = clsCommonFunctions.getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");

                            ////int Opti = Convert.ToInt32 (clsCommonFunctions.getName("select isnull(sum(k.Qty),0) as EngCount from quotationdetails q inner join part p on q.partcode=p.PartCode inner join kitdetails k on k.kitcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and k.PartCode like '001%' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "EngCount"));
                            //string Opti_CPCB = clsCommonFunctions.getName("select top 1 substring(q.PartCode,10,1) as OptiCPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "OptiCPCB");



                            //if (Qtn_CPCB == "4")
                            //{
                            //if (model_type.Trim().PadRight(3).Trim() == "NG1")
                            if (model_type.Substring(model_type.Trim().Length - 3).Trim() == "NG1")
                            {

                                if (flg_NG_CPCB_IV_15_to_250_kVA == 0)
                                {

                                    str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/NG_CPCB IV+15-250_kVA.PDF";
                                    str_pdf_Name = "NG_CPCB IV+15-250_kVA.PDF";
                                    if (File.Exists(str_FilePath))
                                    {
                                        FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                        Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                        m.Attachments.Add(a2);
                                    }
                                }

                                flg_NG_CPCB_IV_15_to_250_kVA = flg_NG_CPCB_IV_15_to_250_kVA + 1;
                            }
                            else
                            {


                                if (Opti_CPCB.Trim() == "5") //OPTIPrime
                                {
                                    if (flg_OP_CPCB_IV_117_To_2000_kVA == 0)
                                    {
                                        str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/OP_CPCB IV+117-2000_kVA.PDF";
                                        str_pdf_Name = "OP_CPCB IV+117-2000_kVA.PDF";
                                        if (File.Exists(str_FilePath))
                                        {
                                            FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                            Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                            m.Attachments.Add(a2);
                                        }
                                    }

                                    flg_OP_CPCB_IV_117_To_2000_kVA = flg_OP_CPCB_IV_117_To_2000_kVA + 1;


                                }
                                else
                                {

                                    //GK Product
                                    if (Str_Product_GKOrNOt.Trim() == "GK")
                                    {


                                        if ((Convert.ToDouble(Str_KVA.Trim()) >= 7.5 && Convert.ToDouble(Str_KVA.Trim()) <= 25))
                                        {
                                            if (flg_7_5_To_25_kVA_GK == 0)
                                            {
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV_GK+7.5-25_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV_GK+7.5-25_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }

                                            flg_7_5_To_25_kVA_GK = flg_7_5_To_25_kVA_GK + 1;
                                        }



                                    }

                                    else

                                    {
                                        //if ((Convert.ToDouble(Str_KVA.Trim()) >= 2 && Convert.ToDouble(Str_KVA.Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
                                        //{
                                        //    //str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/Kirloskar_2kW_5kVA.pdf";
                                        //    //str_pdf_Name = "Kirloskar_2kW_5kVA_Catlogue.pdf";
                                        //    //flg_2kW_5kVA = 1;
                                        //}


                                        if ((Convert.ToDouble(Str_KVA.Trim()) >= 2 && Convert.ToDouble(Str_KVA.Trim()) <= 7))
                                        {
                                            if (flg_2_To_7_kVA == 0)
                                            {
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+2.8-5.5_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV+2.8-5.5_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }

                                            flg_2_To_7_kVA = flg_2_To_7_kVA + 1;
                                        }
                                        // EA Series
                                        else if ((Convert.ToDouble(Str_KVA.Trim()) >= 7 && Convert.ToDouble(Str_KVA.Trim()) <= 20))
                                        {
                                            if (flg_7_To_20_kVA == 0)
                                            {
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+7.5-20_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV+7.5-20_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }

                                            flg_7_To_20_kVA = flg_7_To_20_kVA + 1;
                                        }

                                        else if ((Convert.ToDouble(Str_KVA.Trim()) >= 25 && Convert.ToDouble(Str_KVA.Trim()) <= 58.5))
                                        {
                                            if (flg_25_To_58_5_kVA == 0)
                                            {
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+25-58.5_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV+25-58.5_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }
                                            flg_25_To_58_5_kVA = flg_25_To_58_5_kVA + 1;
                                        }
                                        else if ((Convert.ToDouble(Str_KVA.Trim()) >= 80 && Convert.ToDouble(Str_KVA.Trim()) <= 160))
                                        {
                                            if (flg_80_To_160_kVA == 0)
                                            {
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+82.5-160_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV+82.5-160_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }
                                            flg_80_To_160_kVA = flg_80_To_160_kVA + 1;
                                        }
                                        else if ((Convert.ToDouble(Str_KVA.Trim()) >= 200 && Convert.ToDouble(Str_KVA.Trim()) <= 250))
                                        {
                                            if (flg_200_To_250_kVA == 0)
                                            {
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+200-250_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV+200-250_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }
                                            flg_200_To_250_kVA = flg_200_To_250_kVA + 1;
                                        }
                                        else if ((Convert.ToDouble(Str_KVA.Trim()) >= 320 && Convert.ToDouble(Str_KVA.Trim()) <= 750))
                                        {
                                            if (flg_320_TO_750_kVA == 0)
                                            {
                                                //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+320-750_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV+320-750_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }
                                            flg_320_TO_750_kVA = flg_320_TO_750_kVA + 1;
                                        }
                                        else if ((Convert.ToDouble(Str_KVA.Trim()) >= 1010 && Convert.ToDouble(Str_KVA.Trim()) <= 1500))
                                        {
                                            if (flg_1010_TO_1500_kVA == 0)
                                            {
                                                //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
                                                str_FilePath = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+1010-1500_kVA.PDF";
                                                str_pdf_Name = "CPCB_IV+1010-1500_kVA.PDF";
                                                if (File.Exists(str_FilePath))
                                                {
                                                    FileStream fs2 = new FileStream(str_FilePath, FileMode.Open, FileAccess.Read);
                                                    Attachment a2 = new Attachment(fs2, str_pdf_Name, MediaTypeNames.Application.Octet);
                                                    m.Attachments.Add(a2);
                                                }
                                            }
                                            flg_1010_TO_1500_kVA = flg_1010_TO_1500_kVA + 1;
                                        }
                                    }

                                }
                            }


                            // New Anubandh
                            if (Qtn_CPCB != "4")
                            {
                                if (Convert.ToDouble(Str_KVA.Trim()) == 125 && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() == "6R")
                                {
                                    //Skip Anubandh by KB on 21/12/2023

                                    //Skip Anubandh
                                }
                                else if ((Convert.ToDouble(Str_KVA.Trim()) >= 5 && Convert.ToDouble(Str_KVA.Trim()) <= 160) && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
                                {
                                    //Skip Anubandh by KB on 21/12/2023
                                    //strFilePath_3 = AppDomain.CurrentDomain.BaseDirectory + "Pages/Marketing/Reports/quotation/Anubandh.pdf";
                                    //strpdf_3 = "Anubandh.pdf";
                                    //flg_anubandh = 1;

                                    // if (File.Exists(strFilePath_3))
                                    // {
                                    //Skip Anubandh by KB on 21/12/2023

                                    //FileStream fs3 = new FileStream(strFilePath_3, FileMode.Open, FileAccess.Read);
                                    //Attachment a3 = new Attachment(fs3, strpdf_3, MediaTypeNames.Application.Octet);
                                    //m.Attachments.Add(a3);
                                    //}
                                }
                            }
                            //End 
                        }
                    }


                    #region

                    // string[] split_kva_item = Regex.Split(kva.Trim(), ",");
                    //if (split_kva_item.Length > 0)
                    //{
                    //    for (int j = 0; j < split_kva_item.Length; j++)
                    //    {
                    //        string strFilePath_2 = "", strFilePath_3 = "";
                    //        string strpdf_2 = "", strpdf_3 = "";
                    //        string model_type = cls.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + Original_Qtn_No.Trim() + "') and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "Model");
                    //        string Qtn_CPCB = cls.getName("select substring(q.PartCode,15,1) as CPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "CPCB");
                    //        //int Opti = Convert.ToInt32 (clsCommonFunctions.getName("select isnull(sum(k.Qty),0) as EngCount from quotationdetails q inner join part p on q.partcode=p.PartCode inner join kitdetails k on k.kitcode=p.PartCode where qtnno='" + ViewState["QtnNo"].ToString().Trim() + "' and k.PartCode like '001%' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "EngCount"));
                    //        string Opti_CPCB = cls.getName("select top 1 substring(q.PartCode,10,1) as OptiCPCB from quotationdetails q inner join part p on q.partcode=p.PartCode where qtnno='" + Original_Qtn_No.Trim() + "' and KVA='" + split_kva_item[j].Trim() + "'", "QuotationDetails", "OptiCPCB");

                    //        //if (Qtn_CPCB == "4")
                    //        //{
                    //        //if (model_type.Trim().PadRight(3).Trim() == "NG1")
                    //        if (model_type.Substring(model_type.Trim().Length - 3) == "NG1")
                    //        {
                    //            strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/NG_CPCB IV+15-250_kVA.PDF";
                    //            strpdf_2 = "NG_CPCB IV+15-250_kVA.PDF";
                    //            flg_NG_CPCB_IV_15_to_250_kVA = 1;
                    //        }
                    //        else
                    //        {
                    //            if (Opti_CPCB == "5")
                    //            {
                    //                strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/OP_CPCB IV+117-2000_kVA.PDF";
                    //                strpdf_2 = "OP_CPCB IV+117-2000_kVA.PDF";
                    //                flg_OP_CPCB_IV_117_2000_kVA = 1;
                    //            }
                    //            else
                    //            {

                    //                //if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
                    //                //{
                    //                //    //strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/Kirloskar_2kW_5kVA.pdf";
                    //                //    //strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
                    //                //    //flg_2kW_5kVA = 1;
                    //                //}


                    //                //2. 8kW-5.5kVA
                    //                if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 7))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+2.8-5.5_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+2.8-5.5_kVA.PDF";
                    //                    flg_2_8_To_5_5_kVA = 1;
                    //                }
                    //                // EA Series
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 7 && Convert.ToDouble(split_kva_item[j].Trim()) <= 20))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+7.5-20_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+7.5-20_kVA.PDF";
                    //                    flg_7_5_To_20_kVA = 1;
                    //                }
                    //                //3R550
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 25 && Convert.ToDouble(split_kva_item[j].Trim()) <= 58.5))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+25-58.5_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+25-58.5_kVA.PDF";
                    //                    flg_R550 = 1;
                    //                }
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 80 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+82.5-160_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+82.5-160_kVA.PDF";
                    //                    flg_15_To_30_kVA = 1;
                    //                }
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250))
                    //                {
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+200-250_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+200-250_kVA.PDF";
                    //                    flg_40_To_160_kVA = 1;
                    //                }
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 750))
                    //                {
                    //                    //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+320-750_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+320-750_kVA.PDF";
                    //                    flg_320_1010_kVA = 1;
                    //                }
                    //                else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1010 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500))
                    //                {
                    //                    //Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
                    //                    strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/CPCB4/CPCB_IV+1010-1500_kVA.PDF";
                    //                    strpdf_2 = "CPCB_IV+1010-1500_kVA.PDF";
                    //                    flg_320_1010_kVA = 1;
                    //                }
                    //            }
                    //        }

                    //        //}

                    //        // Commented by KB on 23/07/2024 as CPCB 2 is Excluded
                    //        //else
                    //        //{
                    //        //    if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 2 && Convert.ToDouble(split_kva_item[j].Trim()) <= 5) && flg_2kW_5kVA == 0 && model_type.Trim().Substring(0, 2).Trim() == "CC")
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_2kW_5kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_2kW_5kVA_Catlogue.pdf";
                    //        //        flg_2kW_5kVA = 1;
                    //        //    }
                    //        //    // EA Series
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 12.5) && flg_5_To_12_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/iGreen-5-160kVA.pdf";
                    //        //        strpdf_2 = "Catlogue_iGreen_5_To_160_kVA.pdf";
                    //        //        flg_5_To_12_kVA = 1;
                    //        //    }
                    //        //    //3R550
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 15) && flg_R550 == 0 && model_type.Trim().Substring(0, 5).Trim() == "3R550")
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_R550_15_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_R550_15_kVA_Catlogue.pdf";
                    //        //        flg_R550 = 1;
                    //        //    }
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 15 && Convert.ToDouble(split_kva_item[j].Trim()) <= 30) && flg_15_To_30_kVA == 0)
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_15_To_30_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_15_To_30_kVA_Catlogue.pdf";
                    //        //        flg_15_To_30_kVA = 1;
                    //        //    }
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 40 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_40_To_160_kVA == 0 && model_type.Trim().Substring(0, 2).Trim() != "6R")
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_40_To_160_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_40_To_160_kVA_Catalogue.pdf";
                    //        //        flg_40_To_160_kVA = 1;
                    //        //    }
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 200 && Convert.ToDouble(split_kva_item[j].Trim()) <= 250) && flg_200_To_250_kVA == 0)
                    //        //    {
                    //        //        // Commented by KB on 02/11/2023 as per call from Kalpesh Sir 
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_200_To_250_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_200_To_250_kVA_Catlogue.pdf";
                    //        //        flg_200_To_250_kVA = 1;
                    //        //    }
                    //        //    //else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1010) && flg_320_1010_kVA == 0)
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 320 && Convert.ToDouble(split_kva_item[j].Trim()) < 1000) && flg_320_1010_kVA == 0)
                    //        //    {
                    //        //        // Commented by KB on 08/05/2023 as per mail from Kalpesh Sir on 06/05/2023
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_320_1010_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_320_1010_kVA_Catlogue.pdf";
                    //        //        flg_320_1010_kVA = 1;
                    //        //    }
                    //        //    else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 1250 && Convert.ToDouble(split_kva_item[j].Trim()) <= 1500) && flg_1250_kVA_1500 == 0)
                    //        //    {
                    //        //        strFilePath_2 = AppDomain.CurrentDomain.BaseDirectory + "quotation/catloge/Kirloskar_1250_To_1500_kVA.pdf";
                    //        //        strpdf_2 = "Kirloskar_1250_To_1500_kVA_Catlogue.pdf";
                    //        //        flg_1250_kVA_1500 = 1;
                    //        //    }
                    //        //}
                    //        // Commented by KB on 23/07/2024

                    //        if (File.Exists(strFilePath_2))
                    //        {
                    //            FileStream fs2 = new FileStream(strFilePath_2, FileMode.Open, FileAccess.Read);
                    //            Attachment a2 = new Attachment(fs2, strpdf_2, MediaTypeNames.Application.Octet);
                    //            m.Attachments.Add(a2);
                    //        }

                    //        // New Anubandh
                    //        if (Qtn_CPCB != "4")
                    //        {
                    //            if (Convert.ToDouble(split_kva_item[j].Trim()) == 125 && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() == "6R")
                    //            {
                    //                //Skip Anubandh by KB on 21/12/2023

                    //                //Skip Anubandh
                    //            }
                    //            else if ((Convert.ToDouble(split_kva_item[j].Trim()) >= 5 && Convert.ToDouble(split_kva_item[j].Trim()) <= 160) && flg_anubandh == 0 && model_type.Trim().Substring(0, 2).Trim() != "CC")
                    //            {
                    //                //Skip Anubandh by KB on 21/12/2023
                    //                //strFilePath_3 = AppDomain.CurrentDomain.BaseDirectory + "quotation/quotation/Anubandh.pdf";
                    //                //strpdf_3 = "Anubandh.pdf";
                    //                //flg_anubandh = 1;

                    //                if (File.Exists(strFilePath_3))
                    //                {
                    //                    //Skip Anubandh by KB on 21/12/2023

                    //                    //FileStream fs3 = new FileStream(strFilePath_3, FileMode.Open, FileAccess.Read);
                    //                    //Attachment a3 = new Attachment(fs3, strpdf_3, MediaTypeNames.Application.Octet);
                    //                    //m.Attachments.Add(a3);
                    //                }
                    //            }
                    //        }
                    //        //End 
                    //    }
                    //}
                    #endregion
                    #endregion

                    m.From = new MailAddress("erp@kalabiz.com", "Kala Genset Pvt. Ltd. - ERP System");

                    #region
                    //ToMail
                    if (to_mail_id.Length > 0 && !string.IsNullOrEmpty(to_mail_id))
                    {
                        string[] ToMailIDitems = Regex.Split(to_mail_id.Trim(), ",");
                        foreach (string strTo in ToMailIDitems)
                        {
                            m.To.Add(new MailAddress(strTo));
                        }
                    }
                    //ToMail END

                    if (string.IsNullOrEmpty(cc_mail_id))
                    {
                        cc_mail_id = ass_to_mail_id;
                    }

                    //Add sales.support7@kalabiz.com and Indore and bhopal HOD EmailID IN CC
                    if (qtn_AssToEmpPCCode.Trim() == "07.005")//  Indore 
                    {
                        if (string.IsNullOrEmpty(cc_mail_id))
                        {
                            cc_mail_id = "vijay.nikam@kalabiz.com" + "," + "sales.indore@kalabiz.com";
                        }
                        else
                        {
                            cc_mail_id = cc_mail_id.ToString().Trim() + "," + "vijay.nikam@kalabiz.com" + "," + "sales.indore@kalabiz.com" + "," + "vaishakh.pathak@kalabiz.com";
                        }
                    }
                    if (qtn_AssToEmpPCCode.Trim() == "07.014")// Bhopal
                    {

                        if (string.IsNullOrEmpty(cc_mail_id))
                        {
                            cc_mail_id = "vijay.nikam@kalabiz.com" + "," + "sales.bhopal@kalabiz.com";
                        }
                        else
                        {
                            cc_mail_id = cc_mail_id.ToString().Trim() + "," + "vijay.nikam@kalabiz.com" + "," + "sales.bhopal@kalabiz.com";
                        }
                    }


                    if (!string.IsNullOrEmpty(cc_mail_id))
                    {
                        if (!string.IsNullOrEmpty(ass_to_mail_id))
                        {
                            cc_mail_id = cc_mail_id.ToString().Trim() + "," + ass_to_mail_id;
                        }
                        else
                        {
                            cc_mail_id = cc_mail_id.ToString().Trim();
                        }


                    }

                    //CCMail 
                    if (cc_mail_id.Length > 0 & !string.IsNullOrEmpty(cc_mail_id))
                    {
                        string[] CCMailIDitems = Regex.Split(cc_mail_id.Trim(), ",");
                        foreach (string strCC in CCMailIDitems)
                        {
                            m.CC.Add(new MailAddress(strCC));
                        }
                    }

                    //CCMail  END

                    //BCCMail                 
                    string[] BCCMailIDitems = Regex.Split("skk@kalabiz.com", ",");
                    foreach (string strBCC in BCCMailIDitems)
                    {
                        m.Bcc.Add(new MailAddress(strBCC));
                    }

                    //BCCMail  END
                    #endregion

                    #region
                    Body = "";
                    Body += "<p>To,";
                    Body += "<BR><span style='color:#1F497D'>" + cust_name.Trim() + " </span>";
                    Body += "<BR><span style='color:#1F497D'>" + cust_add.Trim() + " </span></p>";

                    Body += "<p>Subject: Proposal for " + kva.ToString().Trim() + " kVA Silent DG Set</p>";

                    Body += "<p>KALA a popular name synonymous with power, stands tall as a market leader ";
                    Body += "serving industrial, commercial and residential segments in the domestic as well ";
                    Body += "as international markets.</p>";
                    Body += "<p>KALA an IMS(Integrated Management System) Certified company has grown to be one of the biggest names in power ";
                    Body += "generation solutions. Today, our team continues to focus on constant innovation ";
                    Body += "and commitment towards Customer Satisfaction. We concentrate on our ";
                    Body += "efforts in bringing the highest quality power solutions to your corporate/business/residence.</p>";

                    Body += "<p>We as an authorized Genset Original Equipment Manufacturers (G.O.E.M's) for ";

                    Body += "Kirloskar Oil Engines Limited-Pune, manufacture quality KOEL Green ";
                    Body += "Generating Sets in the range of 2.1 kVA to 1500 kVA for captive and standby ";
                    Body += "applications with options such as liquid cooled and air cooled DG sets.</p>";

                    Body += "<p>We provide complete turnkey solutions which include site selection, load study, ";
                    Body += "application engineering, installation, commissioning and obtaining statutory ";
                    Body += "approvals from the Government agencies.</p>";

                    Body += "<p>With our headquarters at Pune, We are having 3 state of the art, ultra-modern ";
                    Body += "manufacturing and testing facilities at Chakan, Silvassa, Belgaum. We are able ";
                    Body += "to cater to both logistic requirements as well as commercial benefits in ";
                    Body += "taxations. We manufacture world class accoustic enclosures (Sound Proof ";
                    Body += "Canopies) as per Central Pollution Control Board (CPCB) Norms.</p>";

                    Body += "<p style='text-align:justify'>";
                    Body += "Our dedicated branch offices at Pune, Chinchwad, Mumbai(Kharghar), Kolhapur, ";
                    Body += "Solapur, Sangli, Aurangabad, Latur, Beed, Nanded, Indore, Bhopal, Gwalior, Bangalore, Belgaon, Hyderabad along with the business associates &amp; dealers are available to ";
                    Body += "provide sales and service to our widespread customer network. Our ";
                    Body += "marketing team is always available at your service to meet your requirements ";
                    Body += "and satisfy all your needs 24/7. We understand the popular saying 'Customers ";
                    Body += "have a choice' and we are grateful to you for considering us as your ";
                    Body += "preferred choice.</p>";


                    Body += "<p>For Genset & Commercial Details, Please find attachment.</p>";

                    Body += "<p style='text-align: justify'><span><span style='color:#1F497D'>Thank You/Regards.</span></span></p>";

                    Body += "<p style='text-align: justify'><span><span style='color:#1F497D'>" + ass_to_emp_name.Trim() + " <br> MailID - " + ass_to_mail_id.Trim() + " <br> MobileNo - " + ass_to_mob_no.Trim() + " <br> Toll Free No - 1800 123 0018</span></span></p>";

                    Body += "<BR><img alt=\"\" hspace=0 src=\"cid:imageId\" align=baseline border=0>";

                    //if (Original_Qtn_No.Trim().Substring(10, 2).Trim() == "16")
                    //{
                    //    Body += "<p style='text-align: justify'><span><span style='color:#1F497D'><b>Kala International LLC</b></span></span></p>";
                    //}
                    //else
                    //{
                    Body += "<p style='text-align: justify'><span><span style='color:#1F497D'><b>Kala Genset Pvt ltd.</b></span></span></p>";
                    // }

                    #endregion

                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");

                    //  var filePath = Path.Combine("~//images//kalalogo.JPG", "images", "kalalogo.JPG");

                    LinkedResource imagelink = new LinkedResource(AppDomain.CurrentDomain.BaseDirectory + "images/kalalogo.JPG", "image/jpeg");

                    //  LinkedResource imagelink = new LinkedResource(Server.MapPath("~//images//kalalogo.JPG"), "image/png");
                    imagelink.ContentId = "imageId";
                    imagelink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                    htmlView.LinkedResources.Add(imagelink);
                    m.AlternateViews.Add(htmlView);

                    m.Subject = "Proposal for " + kva.ToString().Trim() + " kVA Silent DG Set";

                    m.IsBodyHtml = true;
                    m.Body = Body;
                    m.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    if (!string.IsNullOrEmpty(ass_to_mail_id.ToString().Trim()))
                    {
                        m.ReplyTo = new MailAddress(ass_to_mail_id.ToString().Trim());
                    }

                    if (sc != null)
                    {
                        sc.Credentials = new System.Net.NetworkCredential("erp@kalabiz.com", "wanv ftwc dobq blrr");
                        sc.Port = 587;
                        sc.Host = "smtp.gmail.com";
                        sc.EnableSsl = true;
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                        sc.Timeout = 999999999;
                        sc.ServicePoint.MaxIdleTime = 1;
                        sc.Send(m);
                    }

                    m.Dispose();
                    sc = null;

                    fs1.Close();
                    fs1.Dispose();
                    //Code to Delete The Temp File
                    //if (File.Exists(strFilePath_1))
                    //{
                    //    File.Delete(strFilePath_1);
                    //}
                }
                //  return "Successfully";
                emailStatus = "Successfully";
            }
            catch (Exception ex)
            {
                //if (File.Exists(strFilePath_1))
                //{
                //    File.Delete(strFilePath_1);
                //}
                //  return "Failed" + ex.StackTrace;
                emailStatus = "Failed";
                //return "Failed";
            }
        }

        protected void LoadReport(string QtnNo, string ReportName, string loadRptType, string AssignToEmpID, string AssignToEmpName, string AssignToMobileNo, string AssignToMailID, string Model)
        {
            var rpt = new ReportDocument();
            if (loadRptType.ToString().Trim() == "Q")
            {
                // iGreen

                int Qtn_CPCB = Convert.ToInt32(cls.getName("select count(qtnno) as CPCB from quotationdetails where qtnno = '" + QtnNo + "' and substring(PartCode,15,1) ='4'", "QuotationDetails", "CPCB"));
                string model_Gas = cls.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + QtnNo + "')", "QuotationDetails", "Model");


                //if (Qtn_CPCB == 0)
                //{
                //    if (Model.Trim() == "iGreen")
                //    {
                //        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreen.rpt");
                //    }
                //    else
                //    {
                //        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALA.rpt");
                //    }
                //}
                //else if (Qtn_CPCB > 0)
                //{

                if (model_Gas.Substring(model_Gas.Trim().Length - 3) == "NG1")
                {
                    rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAGasDGCPCBIV.rpt");

                }
                else
                {
                    if (Model.Trim() == "iGreen")
                    {
                        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreenCPCBIV.rpt");
                    }
                    // Added by KB on 22/01/2026 as for 7.5 KVA CCModel we require Warranty as 12 Months
                    else if (Model.Trim() == "Warranty")
                    {
                        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreenCPCBIV1yrWarranty.rpt");
                    }

                    else
                    {

                        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreenCPCBIV.rpt");

                        //rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "Pages/Marketing/Reports/quotation/KALACPCBIV.rpt");
                    }
                }


                //}
                rpt.SetDatabaseLogon("sa", "3HMungiIMR", "Localhost", "ERP");
                rpt.SetParameterValue(0, QtnNo.ToString().Trim());
                rpt.SetParameterValue(1, AssignToEmpName.Trim());
                rpt.SetParameterValue(2, AssignToMobileNo.Trim());
                rpt.SetParameterValue(3, AssignToMailID.Trim());
                rpt.SetParameterValue(4, "'" + QtnNo.ToString().Trim() + "'");
                rpt.ExportToDisk(ExportFormatType.PortableDocFormat, ReportName.ToString().Trim());
                rpt.Close();
            }
            else if (loadRptType.ToString().Trim() == "I")
            {
                rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAInstallation.rpt");
                rpt.SetDatabaseLogon("sa", "3HMungiIMR", "Localhost", "ERP");
                rpt.SetParameterValue(0, QtnNo.ToString().Trim());
                rpt.SetParameterValue(1, AssignToEmpName.Trim());
                rpt.SetParameterValue(2, AssignToMobileNo.Trim());
                rpt.SetParameterValue(3, AssignToMailID.Trim());
                rpt.ExportToDisk(ExportFormatType.PortableDocFormat, ReportName.ToString().Trim());
                rpt.Close();
            }
        }


        //Commented by KB on 23/01/2026 as Changes Made realted to warranty for 7.5 Kva 
        //protected void LoadReport(string QtnNo, string ReportName, string loadRptType, string AssignToEmpID, string AssignToEmpName, string AssignToMobileNo, string AssignToMailID, string Model)
        //{
        //    var rpt = new ReportDocument();
        //    if (loadRptType.ToString().Trim() == "Q")
        //    {
        //        // iGreen

        //        int Qtn_CPCB = Convert.ToInt32(cls.getName("select count(qtnno) as CPCB from quotationdetails where qtnno = '" + QtnNo + "' and substring(PartCode,15,1) ='4'", "QuotationDetails", "CPCB"));
        //        string model_Gas = cls.getName("select Model from Part where PartCode in(select PartCode from quotationdetails where qtnno='" + QtnNo + "')", "QuotationDetails", "Model");


        //        //if (Qtn_CPCB == 0)
        //        //{
        //        //    if (Model.Trim() == "iGreen")
        //        //    {
        //        //        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreen.rpt");
        //        //    }
        //        //    else
        //        //    {
        //        //        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALA.rpt");
        //        //    }
        //        //}
        //        //else if (Qtn_CPCB > 0)
        //        //{

        //        if (model_Gas.Substring(model_Gas.Trim().Length - 3) == "NG1")
        //        {
        //            rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAGasDGCPCBIV.rpt");

        //        }
        //        else
        //        {
        //            if (Model.Trim() == "iGreen")
        //            {
        //                rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreenCPCBIV.rpt");
        //            }

        //            else
        //            {

        //                rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAiGreenCPCBIV.rpt");

        //                //rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "Pages/Marketing/Reports/quotation/KALACPCBIV.rpt");
        //            }
        //        }


        //        //}
        //        rpt.SetDatabaseLogon("sa", "3HMungiIMR", "Localhost", "ERP");
        //        rpt.SetParameterValue(0, QtnNo.ToString().Trim());
        //        rpt.SetParameterValue(1, AssignToEmpName.Trim());
        //        rpt.SetParameterValue(2, AssignToMobileNo.Trim());
        //        rpt.SetParameterValue(3, AssignToMailID.Trim());
        //        rpt.SetParameterValue(4, "'" + QtnNo.ToString().Trim() + "'");
        //        rpt.ExportToDisk(ExportFormatType.PortableDocFormat, ReportName.ToString().Trim());
        //        rpt.Close();
        //    }
        //    else if (loadRptType.ToString().Trim() == "I")
        //    {
        //        rpt.Load(AppDomain.CurrentDomain.BaseDirectory + "quotation/KALAInstallation.rpt");
        //        rpt.SetDatabaseLogon("sa", "3HMungiIMR", "Localhost", "ERP");
        //        rpt.SetParameterValue(0, QtnNo.ToString().Trim());
        //        rpt.SetParameterValue(1, AssignToEmpName.Trim());
        //        rpt.SetParameterValue(2, AssignToMobileNo.Trim());
        //        rpt.SetParameterValue(3, AssignToMailID.Trim());
        //        rpt.ExportToDisk(ExportFormatType.PortableDocFormat, ReportName.ToString().Trim());
        //        rpt.Close();
        //    }
        //}

    }
}