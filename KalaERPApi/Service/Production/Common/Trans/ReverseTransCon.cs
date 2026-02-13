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
using KalaERPApi.Models.Production.Common.Trans;
using System.Web.Http;
using KalaERPApi.Controllers.Common.Trans;

namespace KalaERPApi.Service.Production.Common.Trans
{
    public class ReverseTransCon
    {
        #region
        CommonCon ComCon = new CommonCon();
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        public SqlTransaction tran = null;
        DataSet dsDetailsSub = new DataSet();
        DataSet dsDetailsSubV = new DataSet();
        string strProc = "";
        string strSql = "";
        #endregion

        public object SqlDbTypeChar { get; private set; }

        public DataTable GetReverseMst(string PCCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetReversetransMstDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetReverseddl(int TransType, string ddlType, string PCCode, string KVA)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getddlRevTransDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@TransType", SqlDbType.Char).Value = TransType;
            dAd.SelectCommand.Parameters.Add("@ddlType", SqlDbType.Char).Value = ddlType;
            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
            dAd.SelectCommand.Parameters.Add("@KVA", SqlDbType.Char).Value = KVA;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetRevTransDts(int TransType, string PCCode, string KVA, string Model)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetRevTransDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@TransType", SqlDbType.Char).Value = TransType;
            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = PCCode;
            dAd.SelectCommand.Parameters.Add("@KVA", SqlDbType.Char).Value = KVA;
            dAd.SelectCommand.Parameters.Add("@Model", SqlDbType.Char).Value = Model;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }


        public string Submit(ReverseTransRequest revtranreq)
        {
            string RTNCode = "";
            string RTNCodeAll = "";
            int JobDGQty = 0;

            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                int recCount = ComCon.CountChars(revtranreq.RevDGDts, ",");
                string[] strRevDGDts = Regex.Split(revtranreq.RevDGDts, ",");
                int SrNo = 0;
                for (int cSub = 0; cSub <= recCount; cSub++)
                {
                    SrNo += 1;
                    string[] Dts = Regex.Split(strRevDGDts[cSub].ToString().Trim(), "-->");

                    RTNCode = "";
                    RTNCode = ComCon.GetMaxNo("StageRevTrans", "RTC", revtranreq.PCCode.Trim().Substring(0, 2), con, tran);

                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO stageRevTrans(RTCOde,Dt,Yr,MaxSrNo,PCCode,RevMstId,Stage4Code,TRCode,TransCode,ProductCode,CPType,Jobcard1,Jpriority,Remark,CompanyCode,Active,Auth)");
                    sb.Append(" VALUES('" + RTNCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:MM") + "','" + ComCon.yearEnd(con, tran) + "','" + (RTNCode.Substring(10, 8)) + "','" + revtranreq.PCCode.Trim() + "',");
                    sb.Append("'" + revtranreq.RevTransFor + "','" + Dts[6].Trim() + "','" + Dts[7].Trim() + "','" + Dts[1].Trim() + "','" + Dts[3].Trim() + "','" + Dts[5].Trim() + "','" + Dts[4].Trim() + "','" + Dts[2].Trim() + "',");
                    sb.Append("'" + revtranreq.Remark.Trim() + "','" + revtranreq.PCCode.Trim().Substring(0, 2) + "','1','1')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    // item.EngSrNo,item.JobCode,item.J2Priority,item.Partcode ,item.JobCard1,item.PanelType;
                    dsDetailsSub = ComCon.procTranDS("exec GetRevTransDetailSave '" + revtranreq.RevTransFor + "','" + Dts[1] + "','" + Dts[3].Trim() + "','" + Dts[4].Trim() + "','" + Dts[2].Trim() + "'", "tbl_RevTransDetailSave", con, tran);
                    JobDGQty = 0;
                    if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows.Count > 0)
                        JobDGQty = int.Parse(dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[0]["Qty"].ToString().Trim());
                    {
                        int SrNoD = 0;
                        for (int k = 0; k < dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows.Count; k++)
                        {
                            SrNoD += 1;
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO stageRevTransDts(RTCOde,SrNo,PartCode,SrNoPartcode,SerialNo,TRFCode)");
                            sb.Append(" VALUES('" + RTNCode.Trim() + "','" + SrNoD + "','" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["PartCode"].ToString().Trim() + "',");
                            sb.Append(" '" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "','" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "','" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            if (revtranreq.RevTransFor != 4)
                            {
                                //CP MTF/Process Status Updated
                                #region

                                if (dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "003" &&
                                dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim().Substring(0, 3) == "MTF")
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  MTFDetailsSub set JobCardStatus='P' where MTFCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  Processfeedbackdetailssub set JobCardStatus='P' where TRFCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                else if (dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "003" &&
                                    dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim().Substring(0, 3) == "PSH")
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  Processfeedbackdetailssub set JobCardStatus='P' where PFBCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  giirdetailssub set JobCardStatus='P' where giirCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                #endregion
                                //CP MTF/Process Status Updated
                            }
                            else if (revtranreq.RevTransFor == 4) // Stage 1
                            {
                                //CPY MTF/Process Status Updated
                                #region

                                if (dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "401" &&
                                dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim().Substring(0, 3) == "MTF")
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  MTFDetailsSub set JobCardStatus='P' where MTFCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();


                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  Processfeedbackdetailssub set JobCardStatus='P' where TRFCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }

                                if ((dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "001"))
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("Delete from Stockwip where IssueCode ='" + Dts[4].Trim() + "-->" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' and ToProfitcenterCode ='" + revtranreq.PCCode.Trim() + "'  "); // and StageName='StageIV'
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("Delete from Stockwip where receivedCode ='" + Dts[4].Trim() + "-->" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' and ToProfitcenterCode ='" + revtranreq.PCCode.Trim() + "'  "); // and StageName='StageIV'
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  Giirdetailssub set JobCardStatus='P' where TRFCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  Giirdetailssub set JobCardStatus='P' where GiirCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }

                                else if ((dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "001" ||
                                   dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "002" ||
                                   dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "010") &&
                                        dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim().Substring(0, 3) == "MTF")
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  MTFDetailsSub set JobCardStatus='P' where MTFCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  Giirdetailssub set JobCardStatus='P' where TRFCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update GatereceiptInternalDetailsSub set JobCardStatus='P' where TRFCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();



                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  ConvertSerialNoDetails set JobCardStatus='P' where CMTFCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append("  SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                   
                                }
                                else if ((dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "001" ||
                                   dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "002" ||
                                   dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "010") &&
                                        (dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim().Substring(0, 3) == "GIR" || dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim().Substring(0, 3) == "GRI"
                                       || dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim().Substring(0, 3) == "CNV"))
                                {


                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  GatereceiptInternalDetailsSub set JobCardStatus='P' where GRICode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  Giirdetailssub set JobCardStatus='P' where GIIRCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();


                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  ConvertSerialNoDetails set JobCardStatus='P' where CNVCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append("   SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                }
                                else if (dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "401" &&
                                    dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim().Substring(0, 3) == "PSH")
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update  Processfeedbackdetailssub set JobCardStatus='P' where PFBCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                    sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                #endregion
                                //CPY MTF/Process Status Updated
                            }
                        }

                    }
                    //Delete TestReport
                    #region
                    if (revtranreq.RevTransFor == 2)
                    {

                        sb.Remove(0, sb.Length);
                        sb.Append("Update  TestReport set Active='0' where TRCode='" + Dts[7].Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("Update  PDIR set Active='0' where TRCode='" + Dts[7].Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                    }
                    #endregion
                    //Delete TestReport

                    //Delete Stage4
                    #region
                    if (revtranreq.RevTransFor == 3 || revtranreq.RevTransFor == 2)
                    {

                        sb.Remove(0, sb.Length);
                        sb.Append("Delete from Stockwip  where IssueCode ='" + Dts[6].Trim() + "' and fromProfitcenterCode ='" + revtranreq.PCCode.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("Delete from Stockwip  where ReceivedCode ='" + Dts[6].Trim() + "' and ToProfitcenterCode ='" + revtranreq.PCCode.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("Delete from Stockwip where IssueCode ='" + Dts[4].Trim() + "'-->'" + Dts[0].Trim() + "' and ToProfitcenterCode ='" + revtranreq.PCCode.Trim() + "' and StageName='StageIV' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("Update ProcessFeedback set Active='0' where PfbCode='" + Dts[6].Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    #endregion
                    //Delete Stage4

                    if (revtranreq.RevTransFor != 4)
                    {
                        //JobCard2 Deleted
                        #region
                        sb.Remove(0, sb.Length);
                        sb.Append("Delete from  JobCard2detailssub where JobCode='" + Dts[1].Trim() + "' and Partcode='" + Dts[3].Trim() + "' and JobCard1='" + Dts[4].Trim() + "' and J2priority='" + Dts[2].Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        if (JobDGQty == 1)
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Delete from JobCard2details where JobCode='" + Dts[1].Trim() + "' and Partcode='" + Dts[3].Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //Check ForOther Parts in Jobcard
                            string strJobCnt = "0";
                            strJobCnt = ComCon.getTranName("select isNull(Count(JobCode),0) as JobCnt From JobCard2details where JobCode= '" + Dts[1].Trim() + "'", "Job2", "JobCnt", con, tran);
                            if (int.Parse(strJobCnt) == 0)
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("Delete from JobCard2 where JobCode='" + Dts[1].Trim() + "' ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                        }
                        else
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Update  JobCard2details set Qty=Qty-1 where JobCode='" + Dts[1].Trim() + "' and Partcode='" + Dts[3].Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                        #endregion
                        //JobCard2 Deleted

                        //JobCard1 Qty/Status Updated
                        #region
                        sb.Remove(0, sb.Length);
                        sb.Append("Update  JobCarddetailsSub set JobCard2Status='P' where JobCode='" + Dts[4].Trim() + "' and Partcode='" + Dts[3].Trim() + "' and jPriority='" + Dts[2].Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("Update  JobCarddetails set JobCard2Qty=JobCard2Qty-1 where JobCode='" + Dts[4].Trim() + "' and Partcode='" + Dts[3].Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        #endregion
                        //JobCard1 Qty/Status Updated
                    }
                    else if (revtranreq.RevTransFor == 4) //JobCard1 Deleted
                    {
                        //JobCard1 Deleted
                        #region
                        sb.Remove(0, sb.Length);
                        sb.Append("Delete from  JobCarddetailssub where JobCode='" + Dts[1].Trim() + "' and Partcode='" + Dts[3].Trim() + "'  and Jpriority='" + Dts[2].Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                   
                            if (JobDGQty == 1)
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Delete from JobCarddetails where JobCode='" + Dts[1].Trim() + "' and Partcode='" + Dts[3].Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //Check ForOther Parts in Jobcard
                            string strJobCnt = "0";
                            strJobCnt = ComCon.getTranName("select isNull(Count(JobCode),0) as JobCnt From JobCarddetails where JobCode= '" + Dts[1].Trim() + "'", "Job1", "JobCnt", con, tran);
                            if (int.Parse(strJobCnt) == 0)
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("Delete from JobCard where JobCode='" + Dts[1].Trim() + "' ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                        }
                        else
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Update JobCarddetails set Qty=Qty-1 where JobCode='" + Dts[1].Trim() + "' and Partcode='" + Dts[3].Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                        #endregion
                        //JobCard1 Deleted
                    }
                    if (cSub == 0)
                    {
                        RTNCodeAll = RTNCode;
                    }
                    else
                    {
                        RTNCodeAll = RTNCodeAll + "," + RTNCode;
                    }


                }
                tran.Commit();
                //tran.Rollback();
                return RTNCodeAll;
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

        /*
        public string Submit(ReverseTransRequest revtranreq)
        {
            string RTNCode = "";
            string RTNCodeAll = "";
            int JobDGQty = 0;

            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                int recCount = ComCon.CountChars(revtranreq.RevDGDts, ",");
                string[] strRevDGDts = Regex.Split(revtranreq.RevDGDts, ",");
                int SrNo = 0;
                for (int cSub = 0; cSub <= recCount; cSub++)
                {
                    SrNo += 1;
                    string[] Dts = Regex.Split(strRevDGDts[cSub].ToString().Trim(), "-->");

                    RTNCode = "";
                    RTNCode = ComCon.GetMaxNo("StageRevTrans", "RTC", revtranreq.PCCode.Trim().Substring(0, 2), con, tran);

                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO stageRevTrans(RTCOde,Dt,Yr,MaxSrNo,PCCode,RevMstId,Stage4Code,TRCode,TransCode,ProductCode,CPType,Jobcard1,Jpriority,Remark,CompanyCode,Active,Auth)");
                    sb.Append(" VALUES('" + RTNCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:MM") + "','" + ComCon.yearEnd(con, tran) + "','" + (RTNCode.Substring(10, 8)) + "','" + revtranreq.PCCode.Trim() + "',");
                    sb.Append("'" + revtranreq.RevTransFor + "','" + Dts[6].Trim() + "','" + Dts[7].Trim() + "','" + Dts[1].Trim() + "','" + Dts[3].Trim() + "','" + Dts[5].Trim() + "','" + Dts[4].Trim() + "','" + Dts[2].Trim() + "',");
                    sb.Append("'" + revtranreq.Remark.Trim() + "','" + revtranreq.PCCode.Trim().Substring(0, 2) + "','1','1')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    // item.EngSrNo,item.JobCode,item.J2Priority,item.Partcode ,item.JobCard1,item.PanelType;
                    dsDetailsSub = ComCon.procTranDS("exec GetRevTransDetailSave '" + revtranreq.RevTransFor + "','" + Dts[1] + "','" + Dts[3].Trim() + "','" + Dts[4].Trim() + "','" + Dts[2].Trim() + "'", "tbl_RevTransDetailSave", con, tran);
                    JobDGQty = 0;
                    if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows.Count > 0)
                        JobDGQty = int.Parse(dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[0]["Qty"].ToString().Trim());
                    {
                        int SrNoD = 0;
                        for (int k = 0; k < dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows.Count; k++)
                        {
                            SrNoD += 1;
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO stageRevTransDts(RTCOde,SrNo,PartCode,SrNoPartcode,SerialNo,TRFCode)");
                            sb.Append(" VALUES('" + RTNCode.Trim() + "','" + SrNoD + "','" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["PartCode"].ToString().Trim() + "',");
                            sb.Append(" '" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "','" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "','" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //CP MTF/Process Status Updated
                            #region
                            if (dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "003" &&
                                dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim().Substring(0, 3) == "MTF")
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("Update  MTFDetailsSub set JobCardStatus='P' where MTFCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();

                                sb.Remove(0, sb.Length);
                                sb.Append("Update  Processfeedbackdetailssub set JobCardStatus='P' where TRFCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                            else if (dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "003" &&
                                dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim().Substring(0, 3) == "PSH")
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("Update  Processfeedbackdetailssub set JobCardStatus='P' where PFBCode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["TRFCode"].ToString().Trim() + "' and ");
                                sb.Append(" Partcode='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SrNoPartcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_RevTransDetailSave"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                            #endregion
                            //CP MTF/Process Status Updated
                        }

                    }
                    //Delete TestReport
                    #region
                    if (revtranreq.RevTransFor == 2)
                    {

                        sb.Remove(0, sb.Length);
                        sb.Append("Update  TestReport set Active='0' where TRCode='" + Dts[7].Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("Update  PDIR set Active='0' where TRCode='" + Dts[7].Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                    }
                    #endregion
                    //Delete TestReport

                    //Delete Stage4
                    #region
                    if (revtranreq.RevTransFor == 3 || revtranreq.RevTransFor == 2)
                    {

                        sb.Remove(0, sb.Length);
                        sb.Append("Delete from Stockwip  where IssueCode ='" + Dts[6].Trim() + "' and fromProfitcenterCode ='" + revtranreq.PCCode.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("Delete from Stockwip  where ReceivedCode ='" + Dts[6].Trim() + "' and ToProfitcenterCode ='" + revtranreq.PCCode.Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("Delete from Stockwip where IssueCode ='" + Dts[4].Trim() + "'-->'" + Dts[0].Trim() + "' and ToProfitcenterCode ='" + revtranreq.PCCode.Trim() + "' and StageName='StageIV' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("Update ProcessFeedback set Active='0' where PfbCode='" + Dts[6].Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    #endregion
                    //Delete Stage4

                    //JobCard2 Deleted
                    #region
                    sb.Remove(0, sb.Length);
                    sb.Append("Delete from  JobCard2detailssub where JobCode='" + Dts[1].Trim() + "' and Partcode='" + Dts[3].Trim() + "' and JobCard1='" + Dts[4].Trim() + "' and J2priority='" + Dts[2].Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    if (JobDGQty == 1)
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Delete from JobCard2details where JobCode='" + Dts[1].Trim() + "' and Partcode='" + Dts[3].Trim() + "' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //Check ForOther Parts in Jobcard
                        string strJobCnt = "0";
                        strJobCnt = ComCon.getTranName("select isNull(Count(JobCode),0) as JobCnt From JobCard2details where JobCode= '" + Dts[1].Trim() + "'", "Job2", "JobCnt", con, tran);
                        if (int.Parse(strJobCnt) == 0)
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Delete from JobCard2 where JobCode='" + Dts[1].Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                    else
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update  JobCard2details set Qty=Qty-1 where JobCode='" + Dts[1].Trim() + "' and Partcode='" + Dts[3].Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    #endregion
                    //JobCard2 Deleted

                    //JobCard1 Qty/Status Updated
                    #region
                    sb.Remove(0, sb.Length);
                    sb.Append("Update  JobCarddetailsSub set JobCard2Status='P' where JobCode='" + Dts[4].Trim() + "' and Partcode='" + Dts[3].Trim() + "' and jPriority='" + Dts[2].Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("Update  JobCarddetails set JobCard2Qty=JobCard2Qty-1 where JobCode='" + Dts[4].Trim() + "' and Partcode='" + Dts[3].Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    #endregion
                    //JobCard1 Qty/Status Updated
                    if (cSub == 0)
                    {
                        RTNCodeAll = RTNCode;
                    }
                    else
                    {
                        RTNCodeAll = RTNCodeAll + "," + RTNCode;
                    }


                }
                tran.Commit();
                //tran.Rollback();
                return RTNCodeAll;
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

     
        */
    }
}