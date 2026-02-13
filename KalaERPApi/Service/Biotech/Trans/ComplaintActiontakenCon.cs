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
        public class ComplaintActiontakenCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        SqlTransaction tran = null;
        SqlCommand cmd = null;
        
        public DataTable GetMtlProcuct()
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("ServiceMaterialListBio_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable GetExpenditure()
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("GetExpenditureBio_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable getCGST()
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("getCGST_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable getSGST()
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("getSGST_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable getIGST()
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("getIGST_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable getTDS()
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("getTDS_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }


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
        public DataTable GetQtnPerandExpAmt(string Code)
        {
            // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("LoadQtnPerandExpAmt_Bio_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = Code;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }
        public DataTable GetActionPendingComp(string Type)
        {
           // string[] Dts;
            SqlDataAdapter dAd = new SqlDataAdapter("LoadBiotechCompActionTakenDetail_Sp", con);
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

        public string Submit(ComplaintActiontakenRequest ComplaintActiontakenreq)
        {
            string[] strPlanDts;
            int SrNo;
            string[] Dts;
            string StrErwDispCode = "";
            string StrMemoDispCode = "";
            string StrDispCode = "";
            string StrDisplayMsg = "";
            string deleteExpAttachpath = "";
            string deleteActionAttachpath = "";

            CommonCon ComCon = new CommonCon();
            DateTime sysdate = new DateTime(Convert.ToInt16(DateTime.Now.Year), Convert.ToInt16(DateTime.Now.Month), Convert.ToInt16(DateTime.Now.Day));

            //DateTime sysdate = new DateTime(Convert.ToInt16(DateTime.Now.Year), Convert.ToInt16(DateTime.Now.Month), Convert.ToInt16(DateTime.Now.Day));
            DateTime flwdt = new DateTime(Convert.ToInt16((ComplaintActiontakenreq.NextFlwDt.Trim()).Substring(0, 4)), Convert.ToInt16((ComplaintActiontakenreq.NextFlwDt.Trim()).Substring(5, 2)), Convert.ToInt16((ComplaintActiontakenreq.NextFlwDt.Trim()).Substring(8, 2)));
            if (DateTime.Compare(flwdt, sysdate) < 0)
            {
                StrDisplayMsg = "Please Select Proper Next Folloup Date !";
                return StrDisplayMsg;
            }
            // return StrDisplayMsg;
            //if (ComplaintActiontakenreq.ActionStatus.Trim() == "F")
            //{
            //    StrFinish = "F";
            //    txtNextDt.Text = "01/01/1900";
            //}
            //else if (ddlCompStatus.SelectedValue == "A")
            //{
            //    StrFinish = "A";
            //}
            //else if (ddlCompStatus.SelectedValue == "W")
            //{
            //    StrFinish = "W";
            //}
            //else 
            //if (ComplaintActiontakenreq.ActionStatus.Trim() == "P")
            //{
            //   // StrFinish = "P";
            //    DateTime dt1 = new DateTime(Convert.ToInt16((txtSolvedDt.Text.Trim()).Substring(6, 4)), Convert.ToInt16((txtSolvedDt.Text.Trim()).Substring(3, 2)), Convert.ToInt16((txtSolvedDt.Text.Trim()).Substring(0, 2)));
            //    DateTime dt2 = new DateTime(Convert.ToInt16((txtNextDt.Text.Trim()).Substring(6, 4)), Convert.ToInt16((txtNextDt.Text.Trim()).Substring(3, 2)), Convert.ToInt16((txtNextDt.Text.Trim()).Substring(0, 2))); ;
            //    if (DateTime.Compare(dt1, dt2) > 0)
            //    {
            //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditorResponse", "alert('followUp Date less than Seleted date .. !')", true);
            //        return;
            //    }
            //}

            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                //SaveComplaint
                if (ComplaintActiontakenreq.StrType.Trim() == "Save")
                {
                    #region
                    StrDispCode = ComCon.GetMaxNo("ActionTakenBio", "CAB", ComplaintActiontakenreq.CompCode.Trim(), con, tran);
                    //string Max =MTFCode.Substring(10,7).ToString();
                    // SqlCommand cmd = new SqlCommand();
                    cmd = new SqlCommand("InsertUpdateActionTakenBio", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SearchType", "S");
                    cmd.Parameters.AddWithValue("@ACTNo", StrDispCode.Trim());
                    cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@SolvedDt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@MaxSrNo", (StrDispCode.Substring(10, 8)));
                    cmd.Parameters.AddWithValue("@Yr", (StrDispCode.Substring(4, 5)));
                    cmd.Parameters.AddWithValue("@PCACode", ComplaintActiontakenreq.PCACode.Trim());
                    cmd.Parameters.AddWithValue("@EngSrNo", 0);
                    cmd.Parameters.AddWithValue("@GenSrNo", 0);
                    cmd.Parameters.AddWithValue("@DGHours", 0);
                    cmd.Parameters.AddWithValue("@NosOfStart", 0);
                    cmd.Parameters.AddWithValue("@ActionStatus", ComplaintActiontakenreq.ActionStatus.Trim());
                    cmd.Parameters.AddWithValue("@NextDt", ComplaintActiontakenreq.NextFlwDt.Trim());
                   // cmd.Parameters.AddWithValue("@NextDt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CommissioningDt", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Remark", ComplaintActiontakenreq.ActRemark.Trim());
                    cmd.Parameters.AddWithValue("@FeedBackStatus", "P");
                    cmd.Parameters.AddWithValue("@QualityStatus", "P");
                    cmd.Parameters.AddWithValue("@MailStatus", "C");
                    //if ((gvwMail.TotalRowCount > 0) || (!string.IsNullOrEmpty(txtEmail.Text.ToString().Trim())))
                    //{
                    //    cmd.Parameters.AddWithValue("@MailStatus", "P");
                    //}
                    //else
                    //{
                    //    cmd.Parameters.AddWithValue("@MailStatus", "C");
                    //}
                    cmd.Parameters.AddWithValue("@MailCC", "");
                    cmd.Parameters.AddWithValue("@CustAckDt", DBNull.Value);
                    cmd.Parameters.AddWithValue("@CustAckName", "NIL");
                    cmd.Parameters.AddWithValue("@CustAckSign", "NIL");
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    // Action Details                    
                    strPlanDts = Regex.Split(ComplaintActiontakenreq.ActCorrServAnalysisDts, "@#@");
                    SrNo = 0;
                    foreach (String StrSub in strPlanDts)
                    {
                        SrNo += 1;
                        Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                        cmd = new SqlCommand("InsertUpdateActionTakenDetailsBio", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@ACTNo", StrDispCode.Trim());
                        cmd.Parameters.AddWithValue("@SrNo", SrNo);
                        //items = null;
                        //items = Regex.Split(dataItem["Problem"].ToString().Trim(), "-->");
                        cmd.Parameters.AddWithValue("@ProblemCode", Dts[0].ToString().Trim());
                        //items = null;
                        // items = Regex.Split(dataItem["SubProblem"].ToString().Trim(), "-->");
                        cmd.Parameters.AddWithValue("@SubProblemcode", Dts[1].ToString().Trim());
                        cmd.Parameters.AddWithValue("@ActionTaken", Dts[2].ToString().Trim());
                        cmd.Parameters.AddWithValue("@ServiceAnalysis", Dts[3].ToString().Trim());
                        cmd.Parameters.AddWithValue("@RootCause", "");
                        cmd.Parameters.AddWithValue("@PreventiveAction", "");
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    #endregion


                    //Action Taken File Attachment
                    #region
                   if (!string.IsNullOrEmpty(ComplaintActiontakenreq.ActionAttachFileDts.ToString().Trim()))
                    {
                        strPlanDts = null;
                        strPlanDts = Regex.Split(ComplaintActiontakenreq.ActionAttachFileDts, "@#@");
                        SrNo = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNo += 1;
                            Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            cmd = new SqlCommand("InsertUpdateActionTakenFileDetailsBio", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@SearchType", "S");
                            cmd.Parameters.AddWithValue("@ACTNo", StrDispCode.Trim());
                            cmd.Parameters.AddWithValue("@PSrNo", SrNo);
                            cmd.Parameters.AddWithValue("@FileType", Dts[1].ToString().Trim());
                            cmd.Parameters.AddWithValue("@FSRNo", Dts[2].ToString().Trim());
                            //string FileName = maxNo.ToString().Trim().Substring(4, 5) + maxNo.ToString().Trim().Substring(10, 8) + "-" + (i + 1) + Path.GetExtension(((LinkButton)gvdAttachment.Rows[i].FindControl("lblFileName")).Text.ToString().Trim());
                            // StrMpath = clsCommonFunctions.getMainFilePath("ActionTakenfile") + "/" + FileName.ToString().Trim();
                            // StrTpath = ConfigurationManager.AppSettings["TempActionTaken"] + "/" + Session["UserID"].ToString() + "/" + ((LinkButton)gvdAttachment.Rows[i].FindControl("lblFileName")).Text.ToString().Trim();
                            // File.Copy(StrTpath, StrMpath);

                            if (!string.IsNullOrEmpty(Dts[3].ToString().Trim()))
                            {
                                string FileName = StrDispCode.ToString().Trim().Substring(4, 5).Trim() + StrDispCode.ToString().Trim().Substring(10, 8).Trim() + "-" + (SrNo) + Path.GetExtension(Dts[3].ToString().Trim());
                                String StrMpath = ComCon.getMainFilePath("ActionTakenfileBio") + "/" + FileName.ToString().Trim();
                                string StrTpath = "C:/TempERPFile/TempActionTakenfileBio" + "/" + ComplaintActiontakenreq.EmpCode.Trim() + "/" + Dts[3].ToString().Trim();
                                File.Copy(StrTpath, StrMpath);
                                cmd.Parameters.AddWithValue("@FileName", FileName.ToString().Trim());
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@FileName", "");
                            }
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                    #endregion

                    //Action Taken Material
                    #region
                    if (!string.IsNullOrEmpty(ComplaintActiontakenreq.ActMaterialDts.ToString().Trim()))
                    {
                        strPlanDts = null;
                        strPlanDts = Regex.Split(ComplaintActiontakenreq.ActMaterialDts, "@#@");
                        SrNo = 0;
                        ////Action Taken Memo 
                        //StrMemoDispCode = ComCon.GetMaxNo("MemoCommerCial", "MCCode", ComplaintActiontakenreq.CompCode.Trim(), con, tran);
                        //cmd = new SqlCommand("InsertActionTakenMemoCBio", con);
                        //cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@MCCode", StrMemoDispCode.Trim());
                        //cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                        //cmd.Parameters.AddWithValue("@MaxSrNo", (StrMemoDispCode.Substring(10, 8)));
                        //cmd.Parameters.AddWithValue("@Yr", ComCon.yearEnd(con, tran));
                        //cmd.Parameters.AddWithValue("@fromprofitcentercode", ComplaintActiontakenreq.PCACode.Trim());
                        //cmd.Parameters.AddWithValue("@ToProfitcenterCode", ComplaintActiontakenreq.ExpSupEmpType.Trim());
                        //cmd.Parameters.AddWithValue("@TallyHeadCode", ComplaintActiontakenreq.ExpSupEmpcode.Trim());
                        //cmd.Parameters.AddWithValue("@ConsigneeCode", ComplaintActiontakenreq.ExpTypeAdv.Trim());
                        //cmd.Parameters.AddWithValue("@TransportModeCode", ComplaintActiontakenreq.ExpTypeAdv.Trim());
                        //cmd.Parameters.AddWithValue("@OPStatus", ComplaintActiontakenreq.ExpTypeAdv.Trim());
                        //cmd.Parameters.AddWithValue("@PVType", ComplaintActiontakenreq.ExpTypeAdv.Trim());
                        //cmd.Parameters.AddWithValue("@ConsigneeCode", ComplaintActiontakenreq.ExpTypeAdv.Trim());
                        //cmd.Parameters.AddWithValue("@CompanyCode", ComplaintActiontakenreq.CompCode.Trim());
                        //cmd.Parameters.AddWithValue("@Auth", "WOP");
                        //cmd.Parameters.AddWithValue("@ACTNo", StrDispCode.Trim());
                        //cmd.Transaction = tran;
                        //cmd.ExecuteNonQuery();
                        //cmd.Dispose();

                        foreach (String StrSub in strPlanDts)
                        {
                            SrNo += 1;
                            Dts = Regex.Split(StrSub.ToString().Trim(), "-->");

                            //Add To ActionTakenMatDetails
                            cmd = new SqlCommand("InsertUpdateActionTakenMatDetailsBio", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@SearchType", "S");
                            cmd.Parameters.AddWithValue("@ACTNo", StrDispCode.Trim());
                            cmd.Parameters.AddWithValue("@PSrNo", SrNo);
                            cmd.Parameters.AddWithValue("@PartCode", Dts[1].ToString().Trim());
                            cmd.Parameters.AddWithValue("@Qty", Dts[2].ToString().Trim());
                            cmd.Parameters.AddWithValue("@Rate", Dts[3].ToString().Trim());
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            //End  To ActionTakenMatDetails


                            // //Memo Details
                            // cmd = new SqlCommand("InsertActionTakenMemoCDetailsBio", con);
                            // cmd.CommandType = CommandType.StoredProcedure;
                            // //cmd.Parameters.AddWithValue("@SearchType", "S");
                            // cmd.Parameters.AddWithValue("@MCCode", StrMemoDispCode.Trim());
                            // cmd.Parameters.AddWithValue("@SrNo", SrNo);
                            // cmd.Parameters.AddWithValue("@PartCode", Dts[1].ToString().Trim());
                            // cmd.Parameters.AddWithValue("@Qty", Dts[2].ToString().Trim());
                            // cmd.Parameters.AddWithValue("@GiirCode", 0);
                            // cmd.Parameters.AddWithValue("@MRate", Dts[3].ToString().Trim());
                            // cmd.Parameters.AddWithValue("@InvQty", Dts[2].ToString().Trim());
                            // String PatrUOM = "";
                            //// PatrUOM = clsCommonFunctions.getTranName("Select UOMCode from part where partcode='" + Dts[1].ToString().Trim() + "'", "part", "UOMCode", con, tran);
                            // cmd.Parameters.AddWithValue("@InvUOM", PatrUOM.ToString().Trim());
                            // cmd.Transaction = tran;
                            // cmd.ExecuteNonQuery();
                            // cmd.Dispose();


                            // sb.Remove(0, sb.Length);
                            // sb.Append("Insert into Stock01 ");
                            // sb.Append("(PartCode,ReceivedCode,IssueCode,IssueDate,IssueQty )");
                            // sb.Append(" VALUES ('" + Dts[1].ToString().Trim() + "',");
                            // sb.Append("'" + StrMemoDispCode.Trim() + "', ");
                            // sb.Append("'" + StrMemoDispCode.Trim() + "', ");
                            // sb.Append("'" + System.DateTime.Now + "', ");
                            // sb.Append("'" + Dts[2].ToString().Trim() + "' )");
                            // cmd = new SqlCommand(sb.ToString(), con);
                            // cmd.Transaction = tran;
                            // cmd.ExecuteNonQuery();
                            // cmd.Dispose();

                            // //Issue
                            // sb.Remove(0, sb.Length);
                            // sb.Append("Insert Into StockWIP ");
                            // sb.Append("(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode )");
                            // sb.Append(" VALUES ('01.012',");
                            // sb.Append("'" + Dts[1].ToString().Trim() + "', ");
                            // sb.Append("'" + StrMemoDispCode.ToString().Trim() + "', ");
                            // sb.Append("'" + System.DateTime.Now + "', ");
                            // sb.Append("'" + Dts[2].ToString().Trim() + "','01.018' )");
                            // cmd = new SqlCommand(sb.ToString(), con);
                            // cmd.Transaction = tran;
                            // cmd.ExecuteNonQuery();
                            // cmd.Dispose();

                            // //Received
                            // sb.Remove(0, sb.Length);
                            // sb.Append("Insert Into StockWIP ");
                            // sb.Append("(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode)");
                            // sb.Append(" VALUES ('01.012',");
                            // sb.Append("'" + Dts[1].ToString().Trim() + "', ");
                            // sb.Append("'" + StrMemoDispCode.ToString().Trim() + "', ");
                            // sb.Append("'" + System.DateTime.Now + "', ");
                            // sb.Append("'" + Dts[2].ToString().Trim() + "','01.018' )");
                            // cmd = new SqlCommand(sb.ToString(), con);
                            // cmd.Transaction = tran;
                            // cmd.ExecuteNonQuery();
                            // cmd.Dispose();
                        }

                    }
                    #endregion

                    //Action Taken ERW
                    #region
                    if (!string.IsNullOrEmpty(ComplaintActiontakenreq.ExpDts.ToString().Trim()))
                    {
                        //Main ERW
                        #region
                        StrErwDispCode = ComCon.GetMaxNo("ExpenceRequisitionWithPlan", "ERW", ComplaintActiontakenreq.CompCode.Trim(), con, tran);
                        cmd = new SqlCommand("InsertUpdateActionTakenERWBio", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchType", "S");
                        cmd.Parameters.AddWithValue("@REQCode", StrErwDispCode.Trim());
                        cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@MaxSrNo", (StrErwDispCode.Substring(10, 8)));
                        cmd.Parameters.AddWithValue("@Yr", (StrDispCode.Substring(4, 5)));
                        cmd.Parameters.AddWithValue("@PCCode", ComplaintActiontakenreq.PCCode.Trim());
                        cmd.Parameters.AddWithValue("@ExpType", ComplaintActiontakenreq.ExpSupEmpType.Trim());
                        cmd.Parameters.AddWithValue("@SupEmpcode", ComplaintActiontakenreq.ExpSupEmpcode.Trim());
                        //cmd.Parameters.AddWithValue("@Advance", ComplaintActiontakenreq.ExpTypeAdv.Trim());

                        if (ComplaintActiontakenreq.ExpTypeAdv.Trim() == "Normal")
                        {
                            cmd.Parameters.AddWithValue("@Advance", "0");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Advance", "1");
                        }

                        String ERWRemark = "*Site - " + ComplaintActiontakenreq.SiteName.Trim() + " *Address - " + ComplaintActiontakenreq.SiteAddress.Trim() + "  *Product Desc -  " + ComplaintActiontakenreq.CompProduct.Trim();
                        cmd.Parameters.AddWithValue("@Remark", ERWRemark.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", ComplaintActiontakenreq.CompCode.Trim());
                    //   cmd.Parameters.AddWithValue("@ReqType", "WOP");
                        cmd.Parameters.AddWithValue("@SRVNo", "BIO");
                        cmd.Parameters.AddWithValue("@ACTNo", StrDispCode.Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        #endregion

                        //Action Taken ERW Details 
                        strPlanDts = null;
                        strPlanDts = Regex.Split(ComplaintActiontakenreq.ExpDts, "@#@");
                        SrNo = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNo += 1;
                            Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            cmd = new SqlCommand("InsertUpdateActionTakenERWDetailsBio", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@SearchType", "S");
                            cmd.Parameters.AddWithValue("@REQCode", StrErwDispCode.Trim());
                            cmd.Parameters.AddWithValue("@SrNo", SrNo);
                            cmd.Parameters.AddWithValue("@EXPcode", Dts[1].ToString().Trim());
                            cmd.Parameters.AddWithValue("@Amount", Dts[4].ToString().Trim());
                            cmd.Parameters.AddWithValue("@ExpReqVATPer", 0);
                            cmd.Parameters.AddWithValue("@ExpReqCSTPer", 0);
                            cmd.Parameters.AddWithValue("@ExpReqServieTPer", 0);
                            cmd.Parameters.AddWithValue("@ExpReqSBCessPer", 0);
                            cmd.Parameters.AddWithValue("@ExpReqKKCessPer", 0);
                            cmd.Parameters.AddWithValue("@ExpReqCessTPer", 0);
                            cmd.Parameters.AddWithValue("@ExpReqHedCessTPer", 0);
                            cmd.Parameters.AddWithValue("@ExpReqCGSTPer", Dts[5].ToString().Trim());
                            cmd.Parameters.AddWithValue("@ExpReqSGSTPer", Dts[6].ToString().Trim());
                            cmd.Parameters.AddWithValue("@ExpReqIGSTPer", Dts[7].ToString().Trim());
                            cmd.Parameters.AddWithValue("@ExpReqTDSPer", Dts[8].ToString().Trim());
                            cmd.Parameters.AddWithValue("@ExpBillNo", Dts[2].ToString().Trim());
                             cmd.Parameters.AddWithValue("@ExpBillDt", Dts[3].ToString().Trim());
                            //cmd.Parameters.AddWithValue("@ExpBillDt", DateTime.Now);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //ActionTakenExpDetails
                            cmd = new SqlCommand("InsertUpdateActionTakenExpDetailsBio", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@SearchType", "S");
                            cmd.Parameters.AddWithValue("@ACTNo", StrDispCode.Trim());
                            cmd.Parameters.AddWithValue("@PSrNo", SrNo);
                            cmd.Parameters.AddWithValue("@EXPcode", Dts[1].ToString().Trim());
                            cmd.Parameters.AddWithValue("@Amount", Dts[4].ToString().Trim());
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }

                        //Action Taken ERW File Attachment
                        strPlanDts = null;
                        strPlanDts = Regex.Split(ComplaintActiontakenreq.ExpDts, "@#@");
                        SrNo = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNo += 1;
                            Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            if (!string.IsNullOrEmpty(Dts[9].ToString().Trim()))
                            {
                                cmd = new SqlCommand("InsertUpdateActionTakenERWFileDetailsBio", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@SearchType", "S");
                                cmd.Parameters.AddWithValue("@REQCode", StrErwDispCode.Trim());
                                cmd.Parameters.AddWithValue("@SrNo", SrNo);

                                string FileName = StrErwDispCode.ToString().Trim().Substring(4, 5).Trim() + StrErwDispCode.ToString().Trim().Substring(10, 8).Trim() + "-" + (SrNo) + Path.GetExtension(Dts[9].ToString().Trim());
                                string StrMpath = ComCon.getMainFilePath("ExpRequisitionfile") + "/" + FileName.ToString().Trim();
                                string StrTpath = "C:/TempERPFile/TempActionTakenExpfileBio" + "/" + ComplaintActiontakenreq.EmpCode.Trim() + "/" + Dts[9].ToString().Trim();
                                File.Copy(StrTpath, StrMpath);

                                cmd.Parameters.AddWithValue("@FileName", FileName.ToString().Trim());
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                        }

                        //****************User Acivity****************
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", ComplaintActiontakenreq.EmpCode.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "ExpenseRequisition");
                        cmd.Parameters.AddWithValue("@TransactionNo", StrErwDispCode.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", ComplaintActiontakenreq.CompCode.Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        #endregion

                        //DateTime.Now.ToString("yyyy-MM-dd")
                        //****************User Acivity****************
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", ComplaintActiontakenreq.EmpCode.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "ActionTakenBio");
                        cmd.Parameters.AddWithValue("@TransactionNo", StrDispCode.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", ComplaintActiontakenreq.CompCode.Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    //END ERW

                    //Update PrimaryCompAssign
                    sb.Remove(0, sb.Length);
                    sb.Append("UPDATE PrimaryCompAssignBio SET AStatus='" + ComplaintActiontakenreq.ActionStatus.Trim() + "' WHERE PCACode='" + ComplaintActiontakenreq.PCACode.Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    //Update PrimaryCompAssign
                                       
                    tran.Commit();

                 
                    deleteExpAttachpath = "C:/TempERPFile/TempActionTakenExpfileBio" + "/" + ComplaintActiontakenreq.EmpCode.Trim();
                    if (Directory.Exists(deleteExpAttachpath))
                    {
                        foreach (string strFile in System.IO.Directory.GetFiles(deleteExpAttachpath, "*.*"))
                        {
                            File.Delete(strFile);
                        }
                        Directory.Delete(deleteExpAttachpath);
                    }


                    deleteActionAttachpath = "C:/TempERPFile/TempActionTakenfileBio" + "/" + ComplaintActiontakenreq.EmpCode.Trim();
                    if (Directory.Exists(deleteExpAttachpath))
                    {
                        foreach (string strFile in System.IO.Directory.GetFiles(deleteActionAttachpath, "*.*"))
                        {
                            File.Delete(strFile);
                        }
                        Directory.Delete(deleteActionAttachpath);
                    }

                    StrDisplayMsg = "";

                    if (StrErwDispCode.Trim() == "")
                    {
                        StrDisplayMsg = "ActionTaken saved successfully with No : " + StrDispCode.Trim() + " ";
                    }
                    else
                    {
                        StrDisplayMsg = "ActionTaken saved successfully with No: " + StrDispCode.Trim() + ", ERW No: " + StrErwDispCode.Trim();
                    }                   
                  
                }
                //Update 
                else if (ComplaintActiontakenreq.StrType.Trim() == "Update")
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