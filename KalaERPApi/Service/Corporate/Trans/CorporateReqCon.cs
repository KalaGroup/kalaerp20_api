using KalaERPApi.Models.Request.Corporate.Trans;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
namespace KalaERPApi.Service.Corporate.Trans
{
    public class CorporateReqCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        SqlTransaction tran = null;
        SqlCommand cmd = null;
        public object SqlDbTypeChar { get; private set; }

        //public DataTable GetCPRTActionPending(string Type, string VPPlanNo, string P_PCCode, string P_EmpCode, string P_FromDt, string P_ToDt)
        //{
        //    SqlDataAdapter dAd = new SqlDataAdapter("[PenCopReqAction]", con);
        //    dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
        //    //dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
        //    //dAd.SelectCommand.Parameters.Add("@VPlanNo", SqlDbType.Char).Value = VPPlanNo;
        //    //dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = P_PCCode;

        //    dAd.SelectCommand.Parameters.Add("@FronDt", SqlDbType.Char).Value = P_FromDt;
        //    dAd.SelectCommand.Parameters.Add("@ToDt", SqlDbType.Char).Value = P_ToDt;
        //    dAd.SelectCommand.Parameters.Add("@UserID", SqlDbType.Char).Value = P_EmpCode;

        //    dAd.SelectCommand.CommandTimeout = 0;
        //    DataSet dSet = new DataSet();
        //    dAd.Fill(dSet);
        //    return dSet.Tables[0];
        //}

        public DataTable GetToProfitCenter()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("CPRTReqProfitCenter_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }


        public string Submit(CorporateReqRequest CorporateReqreq)
        {
            string[] strPlanDts;
            int SrNo;
            string[] Dts;
            string StrDispCode = "";
            string StrDisplayMsg = "";
            string deleteAttachpath = "";
            CommonCon ComCon = new CommonCon();
            DateTime sysdate = new DateTime(Convert.ToInt16(DateTime.Now.Year), Convert.ToInt16(DateTime.Now.Month), Convert.ToInt16(DateTime.Now.Day));
            try
            {

                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); }
                ;
                tran = con.BeginTransaction();
                //Save
                if (CorporateReqreq.StrType.Trim() == "Save")
                {

                    StrDispCode = ComCon.GetMaxNo("CorporateRequisition", "COR", CorporateReqreq.CompanyCode.Trim(), con, tran);

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO CorporateRequisition ");
                    sb.Append("(ReqCode,Dt,Yr,MaxSrNo,EmpCode,FromPCCode,ToEmpCode,ToPCCode,Priority,ReqMsg,CompanyCode,AssignStatus,Active,Discard)");
                    sb.Append(" VALUES('" + StrDispCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                    sb.Append("'" + (StrDispCode.Substring(4, 5)) + "','" + (StrDispCode.Substring(10, 8)) + "',");
                    sb.Append("'" + CorporateReqreq.EmpCode.Trim() + "' ,'" + CorporateReqreq.FromPCCode.Trim() + "','" + CorporateReqreq.ToEmpCode.Trim() + "',");
                    sb.Append("'" + CorporateReqreq.ToPCCode.Trim() + "' ,'" + CorporateReqreq.Priority.Trim() + "','" + CorporateReqreq.ReqMsg.Trim() + "',");
                    sb.Append("'" + CorporateReqreq.CompanyCode.Trim() + "','P','1','1')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Action Taken File Attachment
                    #region
                    if (!string.IsNullOrEmpty(CorporateReqreq.AttachFileDts.ToString().Trim()))
                    {
                        strPlanDts = null;
                        strPlanDts = Regex.Split(CorporateReqreq.AttachFileDts, "@#@");
                        SrNo = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNo += 1;
                            Dts = Regex.Split(StrSub.ToString().Trim(), "-->");

                            string FileName = StrDispCode.ToString().Trim().Substring(4, 5).Trim() + StrDispCode.ToString().Trim().Substring(10, 8).Trim() + "-" + (SrNo) + Path.GetExtension(Dts[1].ToString().Trim());
                            String StrMpath = ComCon.getMainFilePath("CRPTReq") + "/" + FileName.ToString().Trim();
                            string StrTpath = "C:/TempERPFile/TempCRPTReq" + "/" + CorporateReqreq.EmpCode.Trim() + "/" + Dts[1].ToString().Trim();

                            string StrTempPath = "C:/TempERPFile/TempCRPTReq" + "/" + CorporateReqreq.EmpCode.Trim();
                            if (Directory.Exists(StrTempPath))
                            {
                                Directory.GetAccessControl(StrTpath);
                                File.Copy(StrTpath, StrMpath);
                            }

                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO CorporateRequisitionFiles");
                            sb.Append("(ReqCode,SrNo,FileName)");
                            sb.Append(" VALUES('" + StrDispCode.Trim() + "' ,'" + SrNo + "','" + FileName.ToString().Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //cmd = new SqlCommand("InsertUpdateActionTakenFileDetailsBio", con);
                            //cmd.CommandType = CommandType.StoredProcedure;
                            //cmd.Parameters.AddWithValue("@SearchType", "S");
                            //cmd.Parameters.AddWithValue("@ACTNo", StrDispCode.Trim());
                            //cmd.Parameters.AddWithValue("@PSrNo", SrNo);
                            //cmd.Parameters.AddWithValue("@FileType", Dts[1].ToString().Trim());
                            //cmd.Parameters.AddWithValue("@FSRNo", Dts[2].ToString().Trim());
                            ////string FileName = maxNo.ToString().Trim().Substring(4, 5) + maxNo.ToString().Trim().Substring(10, 8) + "-" + (i + 1) + Path.GetExtension(((LinkButton)gvdAttachment.Rows[i].FindControl("lblFileName")).Text.ToString().Trim());
                            //// StrMpath = clsCommonFunctions.getMainFilePath("ActionTakenfile") + "/" + FileName.ToString().Trim();
                            //// StrTpath = ConfigurationManager.AppSettings["TempActionTaken"] + "/" + Session["UserID"].ToString() + "/" + ((LinkButton)gvdAttachment.Rows[i].FindControl("lblFileName")).Text.ToString().Trim();
                            //// File.Copy(StrTpath, StrMpath);

                            //if (!string.IsNullOrEmpty(Dts[3].ToString().Trim()))
                            //{
                            //    string FileName = StrDispCode.ToString().Trim().Substring(4, 5).Trim() + StrDispCode.ToString().Trim().Substring(10, 8).Trim() + "-" + (SrNo) + Path.GetExtension(Dts[3].ToString().Trim());
                            //    String StrMpath = ComCon.getMainFilePath("ActionTakenfileBio") + "/" + FileName.ToString().Trim();
                            //    string StrTpath = "C:/TempERPFile/TempActionTakenfileBio" + "/" + CorporateReqreq.EmpCode.Trim() + "/" + Dts[3].ToString().Trim();
                            //    File.Copy(StrTpath, StrMpath);
                            //    cmd.Parameters.AddWithValue("@FileName", FileName.ToString().Trim());
                            //}
                            //else
                            //{
                            //    cmd.Parameters.AddWithValue("@FileName", "");
                            //}
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();
                        }
                    }
                    #endregion

                    //DateTime.Now.ToString("yyyy-MM-dd")
                    //****************User Acivity****************
                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", CorporateReqreq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "CorporateRequisition");
                    cmd.Parameters.AddWithValue("@TransactionNo", StrDispCode.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", CorporateReqreq.CompanyCode.Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    //}


                    //**************
                    //**************Complaint Allocation to HOD

                    #region
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO CorporateRequisitionActionTaken");
                    sb.Append("(Dt,ReqCode,AssignByCode,AssignToCode,ActionTaken,Priority,ActionStatus,AssOrAction,Active,Discard)");
                    sb.Append(" VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                    sb.Append("'" + StrDispCode.Trim() + "',");
                    sb.Append("'" + CorporateReqreq.EmpCode.Trim() + "',");
                    sb.Append("'" + CorporateReqreq.ToEmpCode.Trim() + "',");
                    // sb.Append("'" + ((TextBox)(Repeater1.Items[i].FindControl("txtActionTaken"))).Text.Trim() + "','P','ASS','1','1');SELECT @@Identity;");
                   // sb.Append(" '','Low Priority','P','ASS','1','1');SELECT @@Identity;");
                    sb.Append(" '','" + CorporateReqreq.Priority.Trim() + "','P','ASS','1','1');SELECT @@Identity;");
                 
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    object lblDispID = cmd.ExecuteScalar();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("Update CorporateRequisition set AssignStatus='C'");
                    sb.Append(" where ReqCode='" + StrDispCode.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //*********************User Acivity ***************************
                    cmd = new SqlCommand("insertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                    cmd.Parameters.AddWithValue("@EmpID", CorporateReqreq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "Corp Req ActionTaken");
                    cmd.Parameters.AddWithValue("@TransactionNo", Convert.ToInt32(lblDispID));
                    cmd.Parameters.AddWithValue("@CompanyCode", CorporateReqreq.CompanyCode.Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    #endregion

                    #region 
                    //cmt As per Diss FS sir 6/12/2025
                   
                    ////Email
                    //String strProc = "";
                    //DataSet dsEmail = null;
                    ////Find all Branch Plan.
                    //strProc = "exec CorReqEmail_Sp  '" + StrDispCode.Trim() + "'  ";
                    //dsEmail = ComCon.procTranDS(strProc, "EmailDs", con, tran);
                    //// dsEmail = ComCon.procDS(strProc, "EmailDs");
                    ////Add Footer Row
                    //if (dsEmail.Tables["EmailDs"].Rows.Count > 0)
                    //{
                    //    //Email
                    //    #region
                    //    //try
                    //    //{
                    //    //if (con != null && con.State == ConnectionState.Closed)
                    //    //{
                    //    //    con.Open();
                    //    //}

                    //    /* Send Mail Create HTML Table____*/
                    //    #region
                    //    string messageBody = "";
                    //    string htmlTableStart2 = "<table  style=\"border-collapse:collapse; text-align:center;font-family: Calibri;\" >";
                    //    string htmlTableEnd2 = "</table>";
                    //    string htmlHeader = "<tr style =\"background-color:#6FA1D2; color:#ffffff; height: 24px;font-family: Calibri; font-size:14; width:auto;  white-space: inherit;text-align:center;align:center; vertical-align: middle; \">";
                    //    // string htmlHeaderEnd = "</tr>";
                    //    string hrmlTdStartHeader = "<td colspan='9' style=\"text-align:center;font-family: Calibri; font-size:14; width:auto; align:center; vertical-align: middle; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: pre;  padding:1px;\">";
                    //    string hrmlTdEndHeader = "</td>";

                    //    messageBody += htmlTableStart2;
                    //    // messageBody += htmlHeader;
                    //    string htmlHeaderRowStart2 = "<tr style =\"background-color:#6FA1D2; color:#ffffff; height: 15px; font-family: Calibri; font-size:12; \">";
                    //    string htmlHeaderRowEnd2 = "</tr>";

                    //    string htmlTrStart2 = "<tr style =\"color:#555555;font-family: Calibri; font-size:12;\">";
                    //    string htmlTrEnd2 = "</tr>";
                    //    // string htmlTdStart2R = "<td style=\"text-align:right; font-family: Calibri; font-size:12; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px;\">";
                    //    string htmlTdStart2C = "<td style=\"text-align:right; font-family: Calibri; font-size:12; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; text-align:center;\">";
                    //    string htmlTdStart2L = "<td  style=\"text-align:left; font-family: Calibri; font-size:12; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: wrap;  padding:12px;\">";
                    //    string htmlTdEnd2 = "</td>";

                    //    //string hrmlTdStartCenterYelowColor = "<td  style=\"text-align:center; font-family: Calibri; font-size:12; background-color:#FFFF00; align:center; vertical-align: middle; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: pre;  padding:2px;\">";
                    //    //string hrmlTdEndCenterYelowColor = "</td>";
                    //    // string hrmlTdStartRightYelowColor = "<td  style=\"text-align:right; font-family: Calibri; font-size:12; background-color:#FFFF00; align:center; vertical-align: middle; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: pre;  padding:2px;\">";
                    //    // string hrmlTdEndRightYelowColor = "</td>";

                    //    messageBody += htmlHeaderRowStart2;
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:25;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>Sr.No</b>";
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:100;height: 15px ;border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>ReqCode</b>";
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:100;height: 15px ;border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>ReqDtTime</b>";
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:85;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>From [Employee] </b>";
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:85;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>From ProfitCenter</b>";
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:85;height: 15px ;border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>To ProfitCenter</b>";
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:85;height: 15px ;border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>To [Employee]</b>";
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:250;height: 25px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>Requisition Message</b>";

                    //    //Action
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:85;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>ActNo</b>";
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:105;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>ActDtTime</b>";
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:200;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>ActionTaken</b>";
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:105;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>ActStatus</b>";

                    //    //Action Feddback
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:105;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>FeedbackStatus</b>";
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:105;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>FeedbackDtTime</b>";
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:105;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>FeedbackRating</b>";
                    //    messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:150;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>FeedbackRemark</b>";
                    //    messageBody += htmlHeaderRowEnd2;
                    //    #endregion


                    //    string messageBodyData = "", subj = "";
                    //    // messageBodyData = "<p style='color:#1F497D;'> Dear  All ,</p>";
                    //    messageBodyData = "<p style='color:#1F497D;'>" + "Dear  " + dsEmail.Tables["EmailDs"].Rows[0]["AssignToName"].ToString().Trim() + " ," + "</p>";

                    //    String Content = "";
                    //    Content = "<p><span><span style='color:#1F497D'>" + "You are requested to look into the below requisition and takeup the necessary action immediately." + "</span></span></p>";
                    //    messageBodyData += Content.ToString().Trim();

                    //    String Content1 = "";
                    //    Content1 = "<p><span><span style='color:#1F497D'>" + "Requisition details as follows :- " + "</span></span></p>";

                    //    messageBodyData += Content1.ToString().Trim();
                    //    messageBodyData = messageBodyData + messageBody;

                    //    // messageBody += hrmlTdStartHeader + "<b>Corporate Requisition from - " + dsEmail.Tables["EmailDs"].Rows[0]["EmpName"].ToString().Trim() + " with number :- " + StrDispCode.Trim() + " . </b>" + hrmlTdEndHeader;
                    //    subj = "Corporate requisition from - " + dsEmail.Tables["EmailDs"].Rows[0]["EmpName"].ToString().Trim() + " with number :- " + StrDispCode.Trim() + " .";

                    //    #region
                    //    messageBodyData = messageBodyData + htmlTrStart2;
                    //    messageBodyData = messageBodyData + htmlTdStart2C + 1 + htmlTdEnd2;
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["ReqCode"].ToString().Trim() + htmlTdEnd2;
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["DtTime"].ToString().Trim() + htmlTdEnd2;
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["EmpName"].ToString().Trim() + htmlTdEnd2;
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["FromPCName"].ToString().Trim() + htmlTdEnd2;
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["ToPCName"].ToString().Trim() + htmlTdEnd2;
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["AssignToName"].ToString().Trim() + htmlTdEnd2;
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["ReqMsg"].ToString().Trim() + htmlTdEnd2;
                    //    //Action 
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["ActNo"].ToString().Trim() + htmlTdEnd2;
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["ActionDtTime"].ToString().Trim() + htmlTdEnd2;
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["ActionTaken"].ToString().Trim() + htmlTdEnd2;
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["Status"].ToString().Trim() + htmlTdEnd2;
                    //    //Action Feedback
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["FeedbackStatus"].ToString().Trim() + htmlTdEnd2;
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["FeedbackDtTime"].ToString().Trim() + htmlTdEnd2;
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["FeedbackRating"].ToString().Trim() + htmlTdEnd2;
                    //    messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["FeedbackRemark"].ToString().Trim() + htmlTdEnd2;

                    //    //  messageBodyData = messageBodyData + htmlTdStart2R + INRCurrency(dataViewMainDSOS[j]["180 and Above"].ToString().Trim()) + htmlTdEnd2;
                    //    messageBodyData = messageBodyData + htmlTrEnd2;
                    //    #endregion

                    //    messageBodyData = messageBodyData + htmlTableEnd2;
                    //    messageBodyData += "<br><br><span style='color:#1F497D'><font><b>System generated mail, please don't reply.</b></font></span><br><br>";
                    //    messageBodyData += "<span style='color:#1F497D'><font><b>With Regards,</b></font></span><br><br>";
                    //    messageBodyData += "<span style='color:#1F497D'><font><b>Kala Genset Pvt Ltd.,</b></font></span><br>";
                    //    messageBodyData += "<span style='color:#1F497D'><font><b>Chakan,Pune,</b></font></span><br>";
                    //    messageBodyData += "<span style='color:#1F497D'><font><b>INDIA.</b></font></span><br><br>";
                    //    MailMessage message = new MailMessage();
                    //    SmtpClient client = new SmtpClient();

                    //    message.From = new MailAddress("erp@kalabiz.com", "Kala Genset Pvt. Ltd. - ERP System");
                    //    message.ReplyTo = new MailAddress("skk@kalabiz.com");

                    //    string ToMailID = "";
                    //    string CCMailID = "";
                    //    String BCCMAILID = "";

                    //    //TO
                    //    if (dsEmail.Tables["EmailDs"].Rows[0]["ToEmpEmailID"].ToString().Trim() == "")
                    //    {
                    //        if (dsEmail.Tables["EmailDs"].Rows[0]["ToPCHODMailID"].ToString().Trim() != "")
                    //        {
                    //            ToMailID = dsEmail.Tables["EmailDs"].Rows[0]["ToPCHODMailID"].ToString().Trim();
                    //        }
                    //        else
                    //        {
                    //            ToMailID = "skk@kalabiz.com";
                    //        }
                    //    }
                    //    else
                    //    {
                    //        ToMailID = dsEmail.Tables["EmailDs"].Rows[0]["ToEmpEmailID"].ToString().Trim();
                    //    }

                    //    //CC
                    //    if (dsEmail.Tables["EmailDs"].Rows[0]["FromEmpEmailID"].ToString().Trim() == "")
                    //    {
                    //        if (dsEmail.Tables["EmailDs"].Rows[0]["FromPCHODMailID"].ToString().Trim() != "")
                    //        {
                    //            CCMailID = dsEmail.Tables["EmailDs"].Rows[0]["FromPCHODMailID"].ToString().Trim();
                    //        }
                    //        else
                    //        {
                    //            CCMailID = "skk@kalabiz.com";
                    //        }
                    //    }
                    //    else
                    //    {
                    //        CCMailID = dsEmail.Tables["EmailDs"].Rows[0]["FromEmpEmailID"].ToString().Trim();
                    //        if (dsEmail.Tables["EmailDs"].Rows[0]["FromPCHODMailID"].ToString().Trim() != "")
                    //        {
                    //            CCMailID += "," + dsEmail.Tables["EmailDs"].Rows[0]["FromPCHODMailID"].ToString().Trim();
                    //        }
                    //    }

                    //    //ToMailID = "skk@kalabiz.com";
                    //    //CCMailID = "skk@kalabiz.com";
                    //    //BCCMAILID = "skk@kalabiz.com";

                    //    //  CCMailID = dsEmail.Tables["EmailDs"].Rows[0]["ToPCHODMailID"].ToString().Trim() + "," + dsEmail.Tables["EmailDs"].Rows[0]["FromEmpEmailID"].ToString().Trim() + "," + dsEmail.Tables["EmailDs"].Rows[0]["FromPCHODMailID"].ToString().Trim();
                    //    BCCMAILID = "skk@kalabiz.com";

                    //    if (!string.IsNullOrEmpty(ToMailID))
                    //    {
                    //        string[] arrToMailID = Regex.Split(ToMailID, ",");
                    //        foreach (string strTo in arrToMailID)
                    //        {
                    //            message.To.Add(new MailAddress(strTo));
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(CCMailID))
                    //    {
                    //        string[] arrCCMailID = Regex.Split(CCMailID, ",");
                    //        foreach (string strCC in arrCCMailID)
                    //        {
                    //            message.CC.Add(new MailAddress(strCC));
                    //        }
                    //    }
                    //    if (!string.IsNullOrEmpty(BCCMAILID.ToString().Trim()))
                    //    {
                    //        String[] addrBCC = BCCMAILID.ToString().Trim().Split(',');

                    //        foreach (string strBCC in addrBCC)
                    //        {
                    //            message.Bcc.Add(new MailAddress(strBCC));
                    //        }
                    //    }

                    //    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(messageBodyData, null, "text/html");
                    //    message.AlternateViews.Add(htmlView);
                    //    message.IsBodyHtml = true;
                    //    message.Body = messageBodyData;
                    //    message.Subject = subj;
                    //    if (client != null)
                    //    {
                    //        client.Credentials = new System.Net.NetworkCredential("erp@kalabiz.com", "wanv ftwc dobq blrr");
                    //        client.Port = 587;
                    //        client.Host = "smtp.gmail.com";
                    //        client.EnableSsl = true;
                    //        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    //        client.Timeout = 999999999;
                    //        client.ServicePoint.MaxIdleTime = 1;
                    //        client.Send(message);
                    //    }
                    //    message.Dispose();
                    //    client = null;

                    //    //}
                    //    //catch (Exception ex)
                    //    //{

                    //    //    //fs = new FileStream("C:/Error/MailSendTotalCustOstDatError.txt", FileMode.OpenOrCreate, FileAccess.Write);
                    //    //    //m_streamWriter = new StreamWriter(fs);
                    //    //    //m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                    //    //    //m_streamWriter.WriteLine();
                    //    //    //m_streamWriter.WriteLine("**********************TotalCustOst Summary Mail Send Error on " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:tt") + " **********************");
                    //    //    //m_streamWriter.WriteLine(ex.Message.ToString());
                    //    //    //m_streamWriter.WriteLine(ex.StackTrace.ToString());
                    //    //    //m_streamWriter.Flush();
                    //    //    //m_streamWriter.Close();
                    //    //}
                    //    //finally
                    //    //{
                    //    //    //if (con != null && con.State == ConnectionState.Open)
                    //    //    //{
                    //    //    //    con.Close();
                    //    //    //}
                    //    //}
                    //    #endregion
                    //}

                    #endregion

                    tran.Commit();

                    deleteAttachpath = "C:/TempERPFile/TempCRPTReq" + "/" + CorporateReqreq.EmpCode.Trim();
                    if (Directory.Exists(deleteAttachpath))
                    {
                        foreach (string strFile in System.IO.Directory.GetFiles(deleteAttachpath, "*.*"))
                        {
                            File.Delete(strFile);
                        }
                        Directory.Delete(deleteAttachpath);
                    }

                    StrDisplayMsg = "";
                    StrDisplayMsg = "Record Saved Successfully with ReqCode : " + StrDispCode.Trim() + " ";
                }
                //Update 
                else if (CorporateReqreq.StrType.Trim() == "Update")
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



        //Add new 28/10/25
        public DataTable GetToEmpNamePCCode()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("CPRTReqEmpNamePCName_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        //End

    }
}