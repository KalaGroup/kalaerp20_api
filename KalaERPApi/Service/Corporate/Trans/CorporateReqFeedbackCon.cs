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
using KalaERPApi.Models.Request.Corporate.Trans;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Globalization;
using System.Xml;

namespace KalaERPApi.Service.Corporate.Trans
{
        public class CorporateReqFeedbackCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        SqlTransaction tran = null;
        SqlCommand cmd = null;
        public object SqlDbTypeChar { get; private set; }

        public DataTable GetCPRTFeedbackPending(string Type, string ReqCode, string P_PCCode, string P_EmpCode, string P_FromDt, string P_ToDt)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("[PenCopReqFeedback]", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            //dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            //dAd.SelectCommand.Parameters.Add("@VPlanNo", SqlDbType.Char).Value = VPPlanNo;
            //dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = P_PCCode;
            dAd.SelectCommand.Parameters.Add("@FronDt", SqlDbType.Char).Value = P_FromDt;
            dAd.SelectCommand.Parameters.Add("@ToDt", SqlDbType.Char).Value = P_ToDt;
            dAd.SelectCommand.Parameters.Add("@UserID", SqlDbType.Char).Value = P_EmpCode;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetCPRTView(string Type, string ReqCode, string UserId)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("CopReqFeedbackView_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;

            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.Parameters.Add("@ReqCode", SqlDbType.Char).Value = ReqCode;
            dAd.SelectCommand.Parameters.Add("@UserId", SqlDbType.Char).Value = UserId;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }


        public DataTable GetCPRTFeedbackFile(string Type, string ReqCode, string UserId)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("CopReqFeedbackView_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;

            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.Parameters.Add("@ReqCode", SqlDbType.Char).Value = ReqCode;
            dAd.SelectCommand.Parameters.Add("@UserId", SqlDbType.Char).Value = UserId;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetCorpReqFeedbackPrvDtls(string Type, string ReqCode, string UserId)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("CopReqFeedbackView_Sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;

            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.Parameters.Add("@ReqCode", SqlDbType.Char).Value = ReqCode;
            dAd.SelectCommand.Parameters.Add("@UserId", SqlDbType.Char).Value = UserId;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

      
        public string Submit(CorporateReqFeedbackRequest CorporateReqFeedback)
        {

            string[] strDts;
            int SrNo;
            string[] Dts;
            string StrDispCode = "";
            string StrDisplayMsg = "";
            string deleteAttachpath = "";
            string StrErwDispCode = "";
            CommonCon ComCon = new CommonCon();
            DateTime sysdate = new DateTime(Convert.ToInt16(DateTime.Now.Year), Convert.ToInt16(DateTime.Now.Month), Convert.ToInt16(DateTime.Now.Day));
            try
            {
               
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                //Save
                if (CorporateReqFeedback.StrType.Trim() == "Save")
                {

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO CorporateRequisitionFeedback");
                    sb.Append("(Dt,ActNo,ReqCode,Rating,Feedback,FeedbackStatus,Active,Discard)");
                    sb.Append(" VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                    sb.Append("'" + CorporateReqFeedback.PrvActionID.Trim() + "',");
                    sb.Append("'" + CorporateReqFeedback.ReqCode.Trim() + "',");
                    sb.Append("'" + CorporateReqFeedback.FeedbackRating.Trim() + "',");
                    sb.Append("'" + CorporateReqFeedback.Feedback.Trim() + "',");
                    //sb.Append("'" + CorporateReqFeedback.FeedbackYOrNStatus.Trim() + "',");

                    string Status = "";
                    if (CorporateReqFeedback.FeedbackYOrNStatus.Trim() == "Yes")
                    {
                        Status = "A";
                        sb.Append("'A',");
                    }
                    else if (CorporateReqFeedback.FeedbackYOrNStatus.Trim() == "No")
                    {
                        Status = "R";
                        sb.Append("'R',");
                    }
                   
                    sb.Append(" '1','1');SELECT @@Identity;");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    object lblDispID = cmd.ExecuteScalar();
                    cmd.Dispose();


                    //Update COR REQ
                    sb.Remove(0, sb.Length);
                    sb.Append("Update CorporateRequisition set FeedbackRemark='" + CorporateReqFeedback.Feedback.Trim() + "',FeedbackStatus='" + Status.Trim() + "' , FeedbackRating='" + CorporateReqFeedback.FeedbackRating.Trim() + "',FeedbackDt='" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "'");
                    sb.Append(" where ReqCode='" + CorporateReqFeedback.ReqCode.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                                    
                    //Feedback File Attachment
                    #region
                    if (!string.IsNullOrEmpty(CorporateReqFeedback.AttachFileDts.ToString().Trim()))
                    {
                        strDts = null;
                        strDts = Regex.Split(CorporateReqFeedback.AttachFileDts, "@#@");
                        SrNo = 0;
                        foreach (String StrSub in strDts)
                        {
                            SrNo += 1;
                            Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            string FileName = Convert.ToInt32(lblDispID) + "-" + (SrNo) + Path.GetExtension(Dts[1].ToString().Trim());
                            String StrMpath = ComCon.getMainFilePath("CRPTFeedback") + "/" + FileName.ToString().Trim();
                            string StrTpath = "C:/TempERPFile/TempCRPTFeedback" + "/" + CorporateReqFeedback.EmpCode.Trim() + "/" + Dts[1].ToString().Trim();

                            string StrTempPath = "C:/TempERPFile/TempCRPTFeedback" + "/" + CorporateReqFeedback.EmpCode.Trim();
                            if (Directory.Exists(StrTempPath))
                            {
                                Directory.GetAccessControl(StrTpath);
                                File.Copy(StrTpath, StrMpath);
                            }

                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO CorporateRequisitionFeedbackFiles");
                            sb.Append("(FID,SrNo,FileName)");
                            sb.Append(" VALUES('" + Convert.ToInt32(lblDispID) + "' ,'" + SrNo + "','" + FileName.ToString().Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                    #endregion

                    
                        //DateTime.Now.ToString("yyyy-MM-dd")
                        //****************User Acivity****************
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", CorporateReqFeedback.EmpCode.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "Corp Req Feedback");
                        cmd.Parameters.AddWithValue("@TransactionNo", Convert.ToInt32(lblDispID));
                        cmd.Parameters.AddWithValue("@CompanyCode", CorporateReqFeedback.CompanyCode.Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();


                    #region   
                    //cmt As per Diss FS sir 6/12/2025
                    // Send Email
                    //If rejected 

                    //if (CorporateReqFeedback.FeedbackYOrNStatus.Trim() == "No")
                    //{

                    //    String strProc = "";
                    //    DataSet dsEmail = null;
                    //    //Find all Branch Plan.
                    //    strProc = "exec CorReqEmail_Sp  '" + CorporateReqFeedback.ReqCode.Trim() + "'  ";
                    //    dsEmail = ComCon.procTranDS(strProc, "EmailDs", con, tran);
                    //    // dsEmail = ComCon.procDS(strProc, "EmailDs");
                    //   if (dsEmail.Tables["EmailDs"].Rows.Count > 0)
                    //    {
                    //        //Email
                    //        #region
                    //        /* Send Mail Create HTML Table____*/
                    //        #region
                    //        string messageBody = "";
                    //        string htmlTableStart2 = "<table  style=\"border-collapse:collapse; text-align:center;font-family: Calibri;\" >";
                    //        string htmlTableEnd2 = "</table>";
                    //        string htmlHeader = "<tr style =\"background-color:#6FA1D2; color:#ffffff; height: 24px;font-family: Calibri; font-size:14; width:auto;  white-space: inherit;text-align:center;align:center; vertical-align: middle; \">";
                    //        // string htmlHeaderEnd = "</tr>";
                    //        string hrmlTdStartHeader = "<td colspan='9' style=\"text-align:center;font-family: Calibri; font-size:14; width:auto; align:center; vertical-align: middle; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: pre;  padding:1px;\">";
                    //        string hrmlTdEndHeader = "</td>";

                    //        messageBody += htmlTableStart2;
                    //        // messageBody += htmlHeader;
                    //        string htmlHeaderRowStart2 = "<tr style =\"background-color:#6FA1D2; color:#ffffff; height: 15px; font-family: Calibri; font-size:12; \">";
                    //        string htmlHeaderRowEnd2 = "</tr>";

                    //        string htmlTrStart2 = "<tr style =\"color:#555555;font-family: Calibri; font-size:12;\">";
                    //        string htmlTrEnd2 = "</tr>";
                    //        // string htmlTdStart2R = "<td style=\"text-align:right; font-family: Calibri; font-size:12; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px;\">";
                    //        string htmlTdStart2C = "<td style=\"text-align:right; font-family: Calibri; font-size:12; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; text-align:center;\">";
                    //        string htmlTdStart2L = "<td  style=\"text-align:left; font-family: Calibri; font-size:12; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: wrap;  padding:12px;\">";
                    //        string htmlTdEnd2 = "</td>";

                    //        //string hrmlTdStartCenterYelowColor = "<td  style=\"text-align:center; font-family: Calibri; font-size:12; background-color:#FFFF00; align:center; vertical-align: middle; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: pre;  padding:2px;\">";
                    //        //string hrmlTdEndCenterYelowColor = "</td>";
                    //        // string hrmlTdStartRightYelowColor = "<td  style=\"text-align:right; font-family: Calibri; font-size:12; background-color:#FFFF00; align:center; vertical-align: middle; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: pre;  padding:2px;\">";
                    //        // string hrmlTdEndRightYelowColor = "</td>";

                    //        messageBody += htmlHeaderRowStart2;
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:25;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>Sr.No</b>";
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:100;height: 15px ;border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>ReqCode</b>";
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:100;height: 15px ;border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>ReqDtTime</b>";
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:85;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>From [Employee] </b>";
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:85;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>From ProfitCenter</b>";
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:85;height: 15px ;border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>To ProfitCenter</b>";
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:85;height: 15px ;border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>To [Employee]</b>";
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:250;height: 25px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>Requisition Message</b>";

                    //        //Action
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:85;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>ActNo</b>";
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:105;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>ActDtTime</b>";
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:200;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>ActionTaken</b>";
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:105;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>ActStatus</b>";

                    //        //Action Feddback
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:105;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>FeedbackStatus</b>";
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:105;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>FeedbackDtTime</b>";
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:105;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>FeedbackRating</b>";
                    //        messageBody += "<td style=\" font-family: Calibri; font-size:12; word-wrap:break-word;width:150;height: 15px; border-color:#5c87b2; border-style:solid; border-width:thin; white-space: nowrap; padding:2px; \"><b>FeedbackRemark</b>";
                    //        messageBody += htmlHeaderRowEnd2;
                    //        #endregion

                    //        string messageBodyData = "", subj = "";
                    //        string ToMailID = "";
                    //        string CCMailID = "";
                    //        String BCCMAILID = "";
                    //        String Content = "";

                           
                    //            // Status = "Assign to Next"
                    //            //  sb.Append("'" + CorporateReqAction.AssignToNext.Trim() + "',");
                    //            subj = "Corporate requisition feedback rejected by - " + dsEmail.Tables["EmailDs"].Rows[0]["EmpName"].ToString().Trim() + " with number :- " + CorporateReqFeedback.ReqCode.Trim() + " .";
                    //            messageBodyData = "<p style='color:#1F497D;'>" + "Dear  " + dsEmail.Tables["EmailDs"].Rows[0]["AssignToName"].ToString().Trim() + " ," + "</p>";
                    //            Content = "<p><span><span style='color:#1F497D'>" + "You are requested to look into the below requisition and takeup the necessary action immediately." + "</span></span></p>";
                    //            messageBodyData += Content.ToString().Trim();

                    //            //TO
                    //            if (dsEmail.Tables["EmailDs"].Rows[0]["ToEmpEmailID"].ToString().Trim() == "")
                    //            {
                    //                if (dsEmail.Tables["EmailDs"].Rows[0]["ToPCHODMailID"].ToString().Trim() != "")
                    //                {
                    //                    ToMailID = dsEmail.Tables["EmailDs"].Rows[0]["ToPCHODMailID"].ToString().Trim();
                    //                }
                    //                else
                    //                {
                    //                    ToMailID = "skk@kalabiz.com";
                    //                }
                    //            }
                    //            else
                    //            {
                    //                ToMailID = dsEmail.Tables["EmailDs"].Rows[0]["ToEmpEmailID"].ToString().Trim();
                    //            }

                    //            //CC
                    //            if (dsEmail.Tables["EmailDs"].Rows[0]["FromEmpEmailID"].ToString().Trim() == "")
                    //            {
                    //                if (dsEmail.Tables["EmailDs"].Rows[0]["FromPCHODMailID"].ToString().Trim() != "")
                    //                {
                    //                    CCMailID = dsEmail.Tables["EmailDs"].Rows[0]["FromPCHODMailID"].ToString().Trim();
                    //                }
                    //                else
                    //                {
                    //                    CCMailID = "skk@kalabiz.com";
                    //                }
                    //            }
                    //            else
                    //            {
                    //                CCMailID = dsEmail.Tables["EmailDs"].Rows[0]["FromEmpEmailID"].ToString().Trim();
                    //                if (dsEmail.Tables["EmailDs"].Rows[0]["FromPCHODMailID"].ToString().Trim() != "")
                    //                {
                    //                    CCMailID += "," + dsEmail.Tables["EmailDs"].Rows[0]["FromPCHODMailID"].ToString().Trim();
                    //                }
                    //            }

                    //        String Content1 = "";
                    //        Content1 = "<p><span><span style='color:#1F497D'>" + "Requisition details as follows :- " + "</span></span></p>";

                    //        messageBodyData += Content1.ToString().Trim();
                    //        messageBodyData = messageBodyData + messageBody;

                    //        #region
                    //        messageBodyData = messageBodyData + htmlTrStart2;
                    //        messageBodyData = messageBodyData + htmlTdStart2C + 1 + htmlTdEnd2;
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["ReqCode"].ToString().Trim() + htmlTdEnd2;
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["DtTime"].ToString().Trim() + htmlTdEnd2;
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["EmpName"].ToString().Trim() + htmlTdEnd2;
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["FromPCName"].ToString().Trim() + htmlTdEnd2;
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["ToPCName"].ToString().Trim() + htmlTdEnd2;
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["AssignToName"].ToString().Trim() + htmlTdEnd2;
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["ReqMsg"].ToString().Trim() + htmlTdEnd2;
                    //        //Action 
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["ActNo"].ToString().Trim() + htmlTdEnd2;
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["ActionDtTime"].ToString().Trim() + htmlTdEnd2;
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["ActionTaken"].ToString().Trim() + htmlTdEnd2;
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["Status"].ToString().Trim() + htmlTdEnd2;
                    //        //Action Feedback
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["FeedbackStatus"].ToString().Trim() + htmlTdEnd2;
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["FeedbackDtTime"].ToString().Trim() + htmlTdEnd2;
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["FeedbackRating"].ToString().Trim() + htmlTdEnd2;
                    //        messageBodyData = messageBodyData + htmlTdStart2L + dsEmail.Tables["EmailDs"].Rows[0]["FeedbackRemark"].ToString().Trim() + htmlTdEnd2;

                    //        //  messageBodyData = messageBodyData + htmlTdStart2R + INRCurrency(dataViewMainDSOS[j]["180 and Above"].ToString().Trim()) + htmlTdEnd2;
                    //        messageBodyData = messageBodyData + htmlTrEnd2;
                    //        #endregion

                    //        messageBodyData = messageBodyData + htmlTableEnd2;
                    //        messageBodyData += "<br><br><span style='color:#1F497D'><font><b>System generated mail, please don't reply.</b></font></span><br><br>";
                    //        messageBodyData += "<span style='color:#1F497D'><font><b>With Regards,</b></font></span><br><br>";
                    //        messageBodyData += "<span style='color:#1F497D'><font><b>Kala Genset Pvt Ltd.,</b></font></span><br>";
                    //        messageBodyData += "<span style='color:#1F497D'><font><b>Chakan,Pune,</b></font></span><br>";
                    //        messageBodyData += "<span style='color:#1F497D'><font><b>INDIA.</b></font></span><br><br>";
                    //        MailMessage message = new MailMessage();
                    //        SmtpClient client = new SmtpClient();

                    //        message.From = new MailAddress("erp@kalabiz.com", "Kala Genset Pvt. Ltd. - ERP System");
                    //        message.ReplyTo = new MailAddress("skk@kalabiz.com");

                            
                    //        //ToMailID = "skk@kalabiz.com";
                    //        //CCMailID = "skk@kalabiz.com";
                    //        //BCCMAILID = "skk@kalabiz.com";

                    //        //  CCMailID = dsEmail.Tables["EmailDs"].Rows[0]["ToPCHODMailID"].ToString().Trim() + "," + dsEmail.Tables["EmailDs"].Rows[0]["FromEmpEmailID"].ToString().Trim() + "," + dsEmail.Tables["EmailDs"].Rows[0]["FromPCHODMailID"].ToString().Trim();
                    //        BCCMAILID = "skk@kalabiz.com";

                    //        if (!string.IsNullOrEmpty(ToMailID))
                    //        {
                    //            string[] arrToMailID = Regex.Split(ToMailID, ",");
                    //            foreach (string strTo in arrToMailID)
                    //            {
                    //                message.To.Add(new MailAddress(strTo));
                    //            }
                    //        }

                    //        if (!string.IsNullOrEmpty(CCMailID))
                    //        {
                    //            string[] arrCCMailID = Regex.Split(CCMailID, ",");
                    //            foreach (string strCC in arrCCMailID)
                    //            {
                    //                message.CC.Add(new MailAddress(strCC));
                    //            }
                    //        }
                    //        if (!string.IsNullOrEmpty(BCCMAILID.ToString().Trim()))
                    //        {
                    //            String[] addrBCC = BCCMAILID.ToString().Trim().Split(',');

                    //            foreach (string strBCC in addrBCC)
                    //            {
                    //                message.Bcc.Add(new MailAddress(strBCC));
                    //            }
                    //        }

                    //        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(messageBodyData, null, "text/html");
                    //        message.AlternateViews.Add(htmlView);
                    //        message.IsBodyHtml = true;
                    //        message.Body = messageBodyData;
                    //        message.Subject = subj;
                    //        if (client != null)
                    //        {
                    //            client.Credentials = new System.Net.NetworkCredential("erp@kalabiz.com", "wanv ftwc dobq blrr");
                    //            client.Port = 587;
                    //            client.Host = "smtp.gmail.com";
                    //            client.EnableSsl = true;
                    //            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    //            client.Timeout = 999999999;
                    //            client.ServicePoint.MaxIdleTime = 1;
                    //            client.Send(message);
                    //        }
                    //        message.Dispose();
                    //        client = null;

                    //        #endregion
                    //    }
                    //}
                    #endregion 

                    tran.Commit();

                    deleteAttachpath = "C:/TempERPFile/TempCRPTFeedback" + "/" + CorporateReqFeedback.EmpCode.Trim();
                    if (Directory.Exists(deleteAttachpath))
                    {
                        foreach (string strFile in System.IO.Directory.GetFiles(deleteAttachpath, "*.*"))
                        {
                            File.Delete(strFile);
                        }
                        Directory.Delete(deleteAttachpath);
                    }
                    

                    StrDisplayMsg = "";
                    //  StrDisplayMsg = "Feedback Saved Successfully with code : " + Convert.ToInt16(lblDispID) + " ";
                    StrDisplayMsg = "Feedback Saved Successfully";
                 }
                //Update 
                else if (CorporateReqFeedback.StrType.Trim() == "Update")
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