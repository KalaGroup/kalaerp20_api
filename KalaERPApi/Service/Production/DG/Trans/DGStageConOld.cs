using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.Http;
using System.Text.RegularExpressions;
using KalaERPApi.Models.Request.Production.DG.Trans;

namespace KalaERPApi.Service.Production.DG.Trans
{

    public class DGStageConOld
    {
        //        
        //        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        //        StringBuilder sb = new StringBuilder();
        //        DataSet dsChkStage = new DataSet();
        //        DataSet dsDetailsCP = new DataSet();
        //        DataSet dsDetailsSubV = new DataSet();
        //        CommonCon ComCon = new CommonCon();
        //        string strSql = "";
        //        public SqlTransaction tran = null;

        //        public object SqlDbTypeChar { get; private set; }

        //        public DataTable GetScanDts(string strSrNo, string strPartCode, string strCat, string strStage)
        //        {
        //            SqlDataAdapter dAd = new SqlDataAdapter("GetStageScanDts", con);
        //            // SqlDataAdapter dAd = new SqlDataAdapter("GetStageScanDtsNew", con);
        //            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
        //            dAd.SelectCommand.Parameters.Add("@strSrNo", SqlDbType.Char).Value = strSrNo;
        //            dAd.SelectCommand.Parameters.Add("@strPartCode", SqlDbType.Char).Value = strPartCode;
        //            dAd.SelectCommand.Parameters.Add("@strCat", SqlDbType.Char).Value = strCat;
        //            dAd.SelectCommand.Parameters.Add("@strStage", SqlDbType.Char).Value = strStage;
        //            dAd.SelectCommand.CommandTimeout = 0;
        //            DataSet dSet = new DataSet();
        //            dAd.Fill(dSet);
        //            return dSet.Tables[0];
        //        }

        //        public Boolean ChkStage(int StageNo, string jobcard, string EngSrNo)
        //        {
        //            Boolean strChkStage = false;
        //            CommonCon ComCon = new CommonCon();
        //            if (StageNo == 0)
        //            {
        //                dsChkStage = ComCon.procDS("Select  isNull(Count(SerialNo),0)  as CntEngSRNo From JobCardDetailsSub where Jobcode='" + jobcard + "' and SerialNo='" + EngSrNo + "'  and Stage1StartStatus='D' ", "tbl_ChkStgDone");
        //            }
        //            else if (StageNo == 1)
        //            {
        //                dsChkStage = ComCon.procDS("Select  isNull(Count(SerialNo),0)  as CntEngSRNo From JobCardDetailsSub where Jobcode='" + jobcard + "' and SerialNo='" + EngSrNo + "'  and Stage1Status='D' ", "tbl_ChkStgDone");
        //            }
        //            else if (StageNo == 3)
        //            {
        //                dsChkStage = ComCon.procDS("Select  isNull(Count(SerialNo),0)  as CntEngSRNo From JobCardDetailsSub where Jobcode='" + jobcard + "' and SerialNo='" + EngSrNo + "'  and Stage3Status='D' ", "tbl_ChkStgDone");
        //            }
        //            else if (StageNo == 4)
        //            {
        //                dsChkStage = ComCon.procDS("Select  isNull(Count(pfd.SerialNo),0)  as CntEngSRNo From ProcessFeedBack Pf Inner Join ProcessFeedBackDetailsSub Pfd on Pf.PfbCode=Pfd.PfbCode where TRFcode='" + jobcard + "' and pfd.SerialNo='" + EngSrNo + "' ", "tbl_ChkStgDone");
        //            }

        //            if (dsChkStage != null && dsChkStage.Tables["tbl_ChkStgDone"].Rows.Count == 1)
        //            {
        //                if (int.Parse(dsChkStage.Tables["tbl_ChkStgDone"].Rows[0]["CntEngSRNo"].ToString().Trim()) > 0)

        //                {
        //                    strChkStage = true;
        //                }
        //                else
        //                {
        //                    strChkStage = false;
        //                }
        //            }
        //            else
        //            {
        //                strChkStage = false;

        //            }
        //            return strChkStage;

        //        }

        //        public string Submit([FromBody] DGStageRequest DGStageReq)
        //        {
        //            string strkVA = "";
        //            strkVA = ComCon.getName("SELECT KVA FROM Part WHERE PartCode='" + DGStageReq.ProductCode.Trim() + "'", "tblPart", "KVA");

        //            if (ChkStage(DGStageReq.StageNo, DGStageReq.JBCode, DGStageReq.EngSrNo) == true)
        //            {

        //                return "This Stage for Jobcard " + DGStageReq.JBCode.Trim() + " With Eng Serial No " + DGStageReq.EngSrNo + " alredy Completed!";
        //            }

        //            try
        //            {
        //                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
        //                tran = con.BeginTransaction();

        //                SqlCommand cmd = new SqlCommand();
        //                // string cntStageDtsStatus = "0";
        //                string cntStageStatus = "0";
        //                int recCount1 = ComCon.CountChars(DGStageReq.PrcChkDts, ",");
        //                string[] strPrcChkDts = Regex.Split(DGStageReq.PrcChkDts, ",");
        //                int SrNo = 0;
        //                if (DGStageReq.StageNo == 0)//S1 Start
        //                {
        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                    sb.Append(" values('03.051','" + DGStageReq.ProductCode.Trim() + "',");
        //                    sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageI')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    if (DGStageReq.EngPartCode.Trim().Substring(0, 3) == "001")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetailssub set Stage1StartStatus='D',Stage1StartPlay='" + DGStageReq.EngPlay.Trim() + "',stage1StartDate=GetDate() where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Serialno='" + DGStageReq.EngSrNo.Trim() + "' and  SrNoPartcode='" + DGStageReq.EngPartCode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                        sb.Append(" values('03.051','" + DGStageReq.EngPartCode.Trim() + "',");
        //                        sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageI')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                    }
        //                    if (DGStageReq.AltPartcode.Trim().Substring(0, 3) == "002")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetailssub set Stage1StartStatus='D',stage1StartDate=GetDate() where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Serialno='" + DGStageReq.AltSrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.AltPartcode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                        sb.Append(" values('03.051','" + DGStageReq.AltPartcode.Trim() + "',");
        //                        sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageI')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }
        //                }
        //                else if (DGStageReq.StageNo == 1) //S1 End
        //                {
        //                    #region

        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                    sb.Append(" values('03.051','" + DGStageReq.ProductCode.Trim() + "',");
        //                    sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageI')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                    sb.Append(" values('03.051','" + DGStageReq.ProductCode.Trim() + "',");
        //                    sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageIII')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    if (DGStageReq.EngPartCode.Trim().Substring(0, 3) == "001")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetailssub set ");
        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Append("Stage1Status='D',stage1Date=GetDate(),Stage2Status='D',Stage1EndPlay='" + DGStageReq.EngPlay.Trim() + "',stage2Date=GetDate() ");
        //                        }
        //                        else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                        {
        //                            sb.Append("Stage1Status='R' ");
        //                        }
        //                        sb.Append(" where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Serialno='" + DGStageReq.EngSrNo.Trim() + "' and  SrNoPartcode='" + DGStageReq.EngPartCode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }
        //                    if (DGStageReq.AltPartcode.Trim().Substring(0, 3) == "002")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetailssub set ");
        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Append("Stage1Status='D',stage1Date=GetDate(),Stage2Status='D',stage2Date=GetDate() ");
        //                        }
        //                        else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                        {
        //                            sb.Append("Stage1Status='R',Stage2Status='R' ");
        //                        }
        //                        sb.Append(" where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Serialno='" + DGStageReq.AltSrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.AltPartcode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }

        //                    for (int cSub1 = 0; cSub1 <= recCount1; cSub1++)
        //                    {
        //                        SrNo += 1;
        //                        string[] PrcChkDts = Regex.Split(strPrcChkDts[cSub1].ToString().Trim(), "-->");
        //                        string GetDGStartTime = "";

        //                        GetDGStartTime = ComCon.getTranName("Select convert(nvarchar(20),Stage1StartDate,120) as Stage1StartDate from JobCardDetailssub Where JobCode='" + DGStageReq.JBCode.Trim() + "' and SerialNo='" + DGStageReq.EngSrNo + "' ", "TblJobcard", "Stage1StartDate", con, tran);
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("insert into PrcChkDetails(TransCode,Dt,MainSerialNo,PrcName,ChkPointId,PrcChkPoints,PrcStatus,DGStartTime,QA6M)");
        //                        sb.Append("values('" + DGStageReq.JBCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
        //                        sb.Append("'" + DGStageReq.EngSrNo.Trim() + "','DG Stage1',");
        //                        sb.Append("'" + PrcChkDts[0].Trim() + "','" + PrcChkDts[1].Trim() + "',");
        //                        sb.Append("'" + DGStageReq.PrcStatus + "',");
        //                        sb.Append("'" + GetDGStartTime + "','" + DGStageReq.QA6M + "')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                    }
        //                    cntStageStatus = "0";
        //                    cntStageStatus = ComCon.getTranName("select isnull(Count(Stage1Status),0) as Stg1 from JobCardDetailssub where Stage1Status='P'  and jobCode='" + DGStageReq.JBCode.Trim() + "'", "JobCard", "Stg1", con, tran);
        //                    if (cntStageStatus == "0")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCard set Stage1Status='D',Stage2Status='D' where  JobCode='" + DGStageReq.JBCode.Trim() + "'  ");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                    }
        //                    #endregion
        //                }
        //                else if (DGStageReq.StageNo == 2) //S2
        //                {
        //                    #region
        //                    //if (DGStageReq.EngPartCode.Trim().Substring(0, 3) == "001")
        //                    //{
        //                    //    sb.Remove(0, sb.Length);
        //                    //    sb.Append("Update JobCardDetailssub set ");
        //                    //    if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                    //    {
        //                    //        sb.Append("Stage2Status='D',stage2Date=GetDate() ");
        //                    //    }
        //                    //    else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                    //    {
        //                    //        sb.Append("Stage2Status='R' ");
        //                    //    }
        //                    //    sb.Append(" where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                    //    sb.Append("Serialno='" + DGStageReq.EngSrNo.Trim() + "' and  SrNoPartcode='" + DGStageReq.EngPartCode.Trim() + "' and ");
        //                    //    sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                    //    cmd = new SqlCommand(sb.ToString(), con);
        //                    //    cmd.Transaction = tran;
        //                    //    cmd.ExecuteNonQuery();
        //                    //    cmd.Dispose();

        //                    //}
        //                    //if (DGStageReq.AltPartcode.Trim().Substring(0, 3) == "002")
        //                    //{
        //                    //    sb.Remove(0, sb.Length);
        //                    //    sb.Append("Update JobCardDetailssub set ");
        //                    //    if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                    //    {
        //                    //        sb.Append("Stage2Status='D',");
        //                    //    }
        //                    //    else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                    //    {
        //                    //        sb.Append("Stage2Status='R',");
        //                    //    }
        //                    //    sb.Append("stage2Date=GetDate() where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                    //    sb.Append("Serialno='" + DGStageReq.AltSrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.AltPartcode.Trim() + "' and ");
        //                    //    sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                    //    cmd = new SqlCommand(sb.ToString(), con);
        //                    //    cmd.Transaction = tran;
        //                    //    cmd.ExecuteNonQuery();
        //                    //    cmd.Dispose();
        //                    //}

        //                    //for (int cSub1 = 0; cSub1 <= recCount1; cSub1++)
        //                    //{
        //                    //    SrNo += 1;
        //                    //    string[] PrcChkDts = Regex.Split(strPrcChkDts[cSub1].ToString().Trim(), "-->");

        //                    //    sb.Remove(0, sb.Length);
        //                    //    sb.Append("insert into PrcChkDetails(TransCode,Dt,MainSerialNo,PrcName,ChkPointId,PrcChkPoints,PrcStatus,DGStartTime,QA6M)");
        //                    //    sb.Append("values('" + DGStageReq.JBCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
        //                    //    sb.Append("'" + DGStageReq.EngSrNo.Trim() + "','DG Stage2',");
        //                    //    sb.Append("'" + PrcChkDts[0].Trim() + "','" + PrcChkDts[1].Trim() + "',");
        //                    //    sb.Append("'" + DGStageReq.PrcStatus + "',");
        //                    //    sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DGStageReq.QA6M + "')");
        //                    //    cmd = new SqlCommand(sb.ToString(), con);
        //                    //    cmd.Transaction = tran;
        //                    //    cmd.ExecuteNonQuery();
        //                    //    cmd.Dispose();
        //                    //}

        //                    //cntStageStatus = "0";
        //                    //cntStageStatus = ComCon.getTranName("select isnull(Count(Stage2Status),0) as Stg2 from JobCardDetailssub where Stage2Status='P'  and jobCode='" + DGStageReq.JBCode.Trim() + "'", "JobCard", "Stg2", con, tran);
        //                    //if (cntStageStatus == "0")
        //                    //{
        //                    //    sb.Remove(0, sb.Length);
        //                    //    sb.Append("Update JobCard set Stage2Status='D' where  JobCode='" + DGStageReq.JBCode.Trim() + "' ");
        //                    //    cmd = new SqlCommand(sb.ToString(), con);
        //                    //    cmd.Transaction = tran;
        //                    //    cmd.ExecuteNonQuery();
        //                    //    cmd.Dispose();
        //                    //}
        //                    #endregion
        //                }
        //                else if (DGStageReq.StageNo == 3) //S2(Now S2)
        //                {
        //                    #region
        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                    sb.Append(" values('03.051','" + DGStageReq.ProductCode.Trim() + "',");
        //                    sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageIII')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                    sb.Append(" values('03.051','" + DGStageReq.ProductCode.Trim() + "',");
        //                    sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageIV')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    if (DGStageReq.EngPartCode.Trim().Substring(0, 3) == "001")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetailssub set ");
        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Append("Stage3Status='D',stage3Date=GetDate() ");
        //                        }
        //                        else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                        {
        //                            sb.Append("Stage3Status='R' ");
        //                        }
        //                        sb.Append("where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Serialno='" + DGStageReq.EngSrNo.Trim() + "' and  SrNoPartcode='" + DGStageReq.EngPartCode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("Update JobCard2Detailssub set ");
        //                            if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                                sb.Append("Stage3Status='D', JobCard1='" + DGStageReq.JBCode.Trim() + "'");
        //                            sb.Append(" where Serialno='" + DGStageReq.EngSrNo.Trim() + "' and  SrNoPartcode='" + DGStageReq.EngPartCode.Trim() + "' and ");
        //                            sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();
        //                        }
        //                    }
        //                    if (DGStageReq.AltPartcode.Trim().Substring(0, 3) == "002")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetailssub set ");
        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Append("Stage3Status='D',stage3Date=GetDate() ");
        //                        }
        //                        else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                        {
        //                            sb.Append("Stage3Status='R' ");
        //                        }
        //                        sb.Append("where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Serialno='" + DGStageReq.AltSrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.AltPartcode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("Update JobCard2Detailssub set Stage3Status='D', JobCard1='" + DGStageReq.JBCode.Trim() + "' ");
        //                            sb.Append(" where Serialno='" + DGStageReq.AltSrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.AltPartcode.Trim() + "' and ");
        //                            sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();
        //                        }
        //                    }
        //                    if (DGStageReq.CpyPartcode.Trim().Substring(0, 2) == "40")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetailssub set ");
        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Append("Stage3Status='D',stage3Date=GetDate() ");
        //                        }
        //                        else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                        {
        //                            sb.Append("Stage3Status='R' ");
        //                        }
        //                        sb.Append("where JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Serialno='" + DGStageReq.CpySrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.CpyPartcode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("Update JobCard2Detailssub set Stage3Status='D', JobCard1='" + DGStageReq.JBCode.Trim() + "' ");
        //                            sb.Append(" where Serialno='" + DGStageReq.CpySrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.CpyPartcode.Trim() + "' and ");
        //                            sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();
        //                        }

        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                        sb.Append(" values('03.051','" + DGStageReq.CpyPartcode.Trim() + "',");
        //                        sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageIII')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }

        //                    if (DGStageReq.BatPartcode.Trim().Length > 2)
        //                    {
        //                        if (DGStageReq.BatPartcode.Trim().Substring(0, 3) == "010")
        //                        {
        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("Update JobCardDetailssub set ");
        //                            if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                            {
        //                                sb.Append("Stage3Status='D',stage3Date=GetDate() ");
        //                            }
        //                            else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                            {
        //                                sb.Append("Stage3Status='R' ");
        //                            }
        //                            sb.Append("where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                            sb.Append("Serialno='" + DGStageReq.BatSrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.BatPartcode.Trim() + "' and ");
        //                            sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();

        //                            if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                            {
        //                                sb.Remove(0, sb.Length);
        //                                sb.Append("Update JobCard2Detailssub set Stage3Status='D', JobCard1='" + DGStageReq.JBCode.Trim() + "' ");
        //                                sb.Append(" where  Serialno='" + DGStageReq.BatSrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.BatPartcode.Trim() + "' and ");
        //                                sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                                cmd = new SqlCommand(sb.ToString(), con);
        //                                cmd.Transaction = tran;
        //                                cmd.ExecuteNonQuery();
        //                                cmd.Dispose();
        //                            }

        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                            sb.Append(" values('03.051','" + DGStageReq.BatPartcode.Trim() + "',");
        //                            sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageIII')");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();
        //                        }
        //                    }

        //                    if (double.Parse(strkVA) > 160)
        //                    {
        //                        if (DGStageReq.Bat2Partcode.Trim().Length > 2)
        //                        {
        //                            if (DGStageReq.Bat2Partcode.Trim().Substring(0, 3) == "010")
        //                            {
        //                                sb.Remove(0, sb.Length);
        //                                sb.Append("Update JobCardDetailssub set ");
        //                                if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                                {
        //                                    sb.Append("Stage3Status='D',stage3Date=GetDate() ");
        //                                }
        //                                else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                                {
        //                                    sb.Append("Stage3Status='R' ");
        //                                }
        //                                sb.Append("where JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                                sb.Append("Serialno='" + DGStageReq.Bat2Srno.Trim() + "' and  SrNoPartcode='" + DGStageReq.Bat2Partcode.Trim() + "' and ");
        //                                sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                                cmd = new SqlCommand(sb.ToString(), con);
        //                                cmd.Transaction = tran;
        //                                cmd.ExecuteNonQuery();
        //                                cmd.Dispose();

        //                                if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                                {
        //                                    sb.Remove(0, sb.Length);
        //                                    sb.Append("Update JobCard2Detailssub set Stage3Status='D',JobCard1='" + DGStageReq.JBCode.Trim() + "'   ");
        //                                    sb.Append("where Serialno='" + DGStageReq.Bat2Srno.Trim() + "' and  SrNoPartcode='" + DGStageReq.Bat2Partcode.Trim() + "' and ");
        //                                    sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                                    cmd = new SqlCommand(sb.ToString(), con);
        //                                    cmd.Transaction = tran;
        //                                    cmd.ExecuteNonQuery();
        //                                    cmd.Dispose();
        //                                }

        //                                sb.Remove(0, sb.Length);
        //                                sb.Append("Update StockWIP set IssueQty=IssueQty+1 where FromProfitCenterCode='03.051' and Issuecode='" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "' and StageName='StageIII' ");
        //                                sb.Append(" and Partcode='" + DGStageReq.BatPartcode.Trim() + "' and ToProfitCenterCode='03.051' ");
        //                                cmd = new SqlCommand(sb.ToString(), con);
        //                                cmd.Transaction = tran;
        //                                cmd.ExecuteNonQuery();
        //                                cmd.Dispose();
        //                            }

        //                        }
        //                    }

        //                    for (int cSub1 = 0; cSub1 <= recCount1; cSub1++)
        //                    {

        //                        SrNo += 1;
        //                        string[] PrcChkDts = Regex.Split(strPrcChkDts[cSub1].ToString().Trim(), "-->");
        //                        //string GetDGStartTime = "";

        //                        //GetDGStartTime = ComCon.getTranName("Select convert(nvarchar(20),DGStartTime,120) as DGStartTime from TestReport Where TRCode='" + TestReportReq.TRCode.Trim() + "' ", "TblTestReport", "DGStartTime", con, tran);
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("insert into PrcChkDetails(TransCode,Dt,MainSerialNo,PrcName,ChkPointId,PrcChkPoints,PrcStatus,DGStartTime,QA6M)");
        //                        sb.Append("values('" + DGStageReq.JBCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
        //                        sb.Append("'" + DGStageReq.EngSrNo.Trim() + "','DG Stage3',");
        //                        sb.Append("'" + PrcChkDts[0].Trim() + "','" + PrcChkDts[1].Trim() + "',");
        //                        sb.Append("'" + DGStageReq.PrcStatus + "',");
        //                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DGStageReq.QA6M + "')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                    }

        //                    if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetails set Stage3Qty=Stage3Qty+1 where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }

        //                    cntStageStatus = "0";
        //                    cntStageStatus = ComCon.getTranName("select isnull(Count(Stage3Status),0) as Stg3 from JobCardDetailssub where Stage3Status='P'  and jobCode='" + DGStageReq.JBCode.Trim() + "'", "JobCard", "Stg3", con, tran);
        //                    if (cntStageStatus == "0")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCard set Stage3Status='D' where  JobCode='" + DGStageReq.JBCode.Trim() + "'  ");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                    }
        //                    #endregion
        //                }
        //                tran.Commit();
        //                // tran.Rollback();
        //                return DGStageReq.JBCode.Trim();
        //            }
        //            catch (Exception ex)
        //            {
        //                tran.Rollback();
        //                return ("StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString());
        //            }

        //            finally
        //            {
        //                con.Close();
        //            }
        //        }

        //        /*//Old
        //        public string Submit([FromBody] DGStageRequest DGStageReq)
        //        {
        //            string strkVA = "";
        //            strkVA = ComCon.getName("SELECT KVA FROM Part WHERE PartCode='" + DGStageReq.ProductCode.Trim() + "'", "tblPart", "KVA");

        //            if (ChkStage(DGStageReq.StageNo, DGStageReq.JBCode, DGStageReq.EngSrNo) == true)
        //            {

        //                return "This Stage for Jobcard " + DGStageReq.JBCode.Trim() + " With Eng Serial No " + DGStageReq.EngSrNo + " alredy Completed!";
        //            }

        //            try
        //            {
        //                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
        //                tran = con.BeginTransaction();

        //                SqlCommand cmd = new SqlCommand();
        //                // string cntStageDtsStatus = "0";
        //                string cntStageStatus = "0";
        //                int recCount1 = ComCon.CountChars(DGStageReq.PrcChkDts, ",");
        //                string[] strPrcChkDts = Regex.Split(DGStageReq.PrcChkDts, ",");
        //                int SrNo = 0;
        //                if (DGStageReq.StageNo == 0)//S1 Start
        //                {
        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                    sb.Append(" values('03.051','" + DGStageReq.ProductCode.Trim() + "',");
        //                    sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageI')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    if (DGStageReq.EngPartCode.Trim().Substring(0, 3) == "001")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetailssub set Stage1StartStatus='D',Stage1StartPlay='" + DGStageReq.EngPlay.Trim() + "',stage1StartDate=GetDate() where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Serialno='" + DGStageReq.EngSrNo.Trim() + "' and  SrNoPartcode='" + DGStageReq.EngPartCode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                        sb.Append(" values('03.051','" + DGStageReq.EngPartCode.Trim() + "',");
        //                        sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageI')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                    }
        //                    if (DGStageReq.AltPartcode.Trim().Substring(0, 3) == "002")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetailssub set Stage1StartStatus='D',stage1StartDate=GetDate() where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Serialno='" + DGStageReq.AltSrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.AltPartcode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                        sb.Append(" values('03.051','" + DGStageReq.AltPartcode.Trim() + "',");
        //                        sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageI')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }
        //                }
        //                else if (DGStageReq.StageNo == 1) //S1 End
        //                {
        //                    #region

        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                    sb.Append(" values('03.051','" + DGStageReq.ProductCode.Trim() + "',");
        //                    sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageI')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                    sb.Append(" values('03.051','" + DGStageReq.ProductCode.Trim() + "',");
        //                    sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageIII')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    if (DGStageReq.EngPartCode.Trim().Substring(0, 3) == "001")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetailssub set ");
        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Append("Stage1Status='D',stage1Date=GetDate(),Stage2Status='D',Stage1EndPlay='" + DGStageReq.EngPlay.Trim() + "',stage2Date=GetDate() ");
        //                        }
        //                        else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                        {
        //                            sb.Append("Stage1Status='R' ");
        //                        }
        //                        sb.Append(" where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Serialno='" + DGStageReq.EngSrNo.Trim() + "' and  SrNoPartcode='" + DGStageReq.EngPartCode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }
        //                    if (DGStageReq.AltPartcode.Trim().Substring(0, 3) == "002")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetailssub set ");
        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Append("Stage1Status='D',stage1Date=GetDate(),Stage2Status='D',stage2Date=GetDate() ");
        //                        }
        //                        else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                        {
        //                            sb.Append("Stage1Status='R',Stage2Status='R' ");
        //                        }
        //                        sb.Append(" where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Serialno='" + DGStageReq.AltSrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.AltPartcode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }

        //                    for (int cSub1 = 0; cSub1 <= recCount1; cSub1++)
        //                    {
        //                        SrNo += 1;
        //                        string[] PrcChkDts = Regex.Split(strPrcChkDts[cSub1].ToString().Trim(), "-->");
        //                        string GetDGStartTime = "";

        //                        GetDGStartTime = ComCon.getTranName("Select convert(nvarchar(20),Stage1StartDate,120) as Stage1StartDate from JobCardDetailssub Where JobCode='" + DGStageReq.JBCode.Trim() + "' and SerialNo='" + DGStageReq.EngSrNo + "' ", "TblJobcard", "Stage1StartDate", con, tran);
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("insert into PrcChkDetails(TransCode,Dt,MainSerialNo,PrcName,ChkPointId,PrcChkPoints,PrcStatus,DGStartTime,QA6M)");
        //                        sb.Append("values('" + DGStageReq.JBCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
        //                        sb.Append("'" + DGStageReq.EngSrNo.Trim() + "','DG Stage1',");
        //                        sb.Append("'" + PrcChkDts[0].Trim() + "','" + PrcChkDts[1].Trim() + "',");
        //                        sb.Append("'" + DGStageReq.PrcStatus + "',");
        //                        sb.Append("'" + GetDGStartTime + "','" + DGStageReq.QA6M + "')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                    }
        //                    cntStageStatus = "0";
        //                    cntStageStatus = ComCon.getTranName("select isnull(Count(Stage1Status),0) as Stg1 from JobCardDetailssub where Stage1Status='P'  and jobCode='" + DGStageReq.JBCode.Trim() + "'", "JobCard", "Stg1", con, tran);
        //                    if (cntStageStatus == "0")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCard set Stage1Status='D',Stage2Status='D' where  JobCode='" + DGStageReq.JBCode.Trim() + "'  ");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                    }
        //                    #endregion
        //                }
        //                else if (DGStageReq.StageNo == 2) //S2
        //                {
        //                    #region
        //                    //if (DGStageReq.EngPartCode.Trim().Substring(0, 3) == "001")
        //                    //{
        //                    //    sb.Remove(0, sb.Length);
        //                    //    sb.Append("Update JobCardDetailssub set ");
        //                    //    if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                    //    {
        //                    //        sb.Append("Stage2Status='D',stage2Date=GetDate() ");
        //                    //    }
        //                    //    else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                    //    {
        //                    //        sb.Append("Stage2Status='R' ");
        //                    //    }
        //                    //    sb.Append(" where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                    //    sb.Append("Serialno='" + DGStageReq.EngSrNo.Trim() + "' and  SrNoPartcode='" + DGStageReq.EngPartCode.Trim() + "' and ");
        //                    //    sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                    //    cmd = new SqlCommand(sb.ToString(), con);
        //                    //    cmd.Transaction = tran;
        //                    //    cmd.ExecuteNonQuery();
        //                    //    cmd.Dispose();

        //                    //}
        //                    //if (DGStageReq.AltPartcode.Trim().Substring(0, 3) == "002")
        //                    //{
        //                    //    sb.Remove(0, sb.Length);
        //                    //    sb.Append("Update JobCardDetailssub set ");
        //                    //    if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                    //    {
        //                    //        sb.Append("Stage2Status='D',");
        //                    //    }
        //                    //    else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                    //    {
        //                    //        sb.Append("Stage2Status='R',");
        //                    //    }
        //                    //    sb.Append("stage2Date=GetDate() where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                    //    sb.Append("Serialno='" + DGStageReq.AltSrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.AltPartcode.Trim() + "' and ");
        //                    //    sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                    //    cmd = new SqlCommand(sb.ToString(), con);
        //                    //    cmd.Transaction = tran;
        //                    //    cmd.ExecuteNonQuery();
        //                    //    cmd.Dispose();
        //                    //}

        //                    //for (int cSub1 = 0; cSub1 <= recCount1; cSub1++)
        //                    //{
        //                    //    SrNo += 1;
        //                    //    string[] PrcChkDts = Regex.Split(strPrcChkDts[cSub1].ToString().Trim(), "-->");

        //                    //    sb.Remove(0, sb.Length);
        //                    //    sb.Append("insert into PrcChkDetails(TransCode,Dt,MainSerialNo,PrcName,ChkPointId,PrcChkPoints,PrcStatus,DGStartTime,QA6M)");
        //                    //    sb.Append("values('" + DGStageReq.JBCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
        //                    //    sb.Append("'" + DGStageReq.EngSrNo.Trim() + "','DG Stage2',");
        //                    //    sb.Append("'" + PrcChkDts[0].Trim() + "','" + PrcChkDts[1].Trim() + "',");
        //                    //    sb.Append("'" + DGStageReq.PrcStatus + "',");
        //                    //    sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DGStageReq.QA6M + "')");
        //                    //    cmd = new SqlCommand(sb.ToString(), con);
        //                    //    cmd.Transaction = tran;
        //                    //    cmd.ExecuteNonQuery();
        //                    //    cmd.Dispose();
        //                    //}

        //                    //cntStageStatus = "0";
        //                    //cntStageStatus = ComCon.getTranName("select isnull(Count(Stage2Status),0) as Stg2 from JobCardDetailssub where Stage2Status='P'  and jobCode='" + DGStageReq.JBCode.Trim() + "'", "JobCard", "Stg2", con, tran);
        //                    //if (cntStageStatus == "0")
        //                    //{
        //                    //    sb.Remove(0, sb.Length);
        //                    //    sb.Append("Update JobCard set Stage2Status='D' where  JobCode='" + DGStageReq.JBCode.Trim() + "' ");
        //                    //    cmd = new SqlCommand(sb.ToString(), con);
        //                    //    cmd.Transaction = tran;
        //                    //    cmd.ExecuteNonQuery();
        //                    //    cmd.Dispose();
        //                    //}
        //                    #endregion
        //                }
        //                else if (DGStageReq.StageNo == 3) //S2(Now S2)
        //                {
        //                    #region
        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                    sb.Append(" values('03.051','" + DGStageReq.ProductCode.Trim() + "',");
        //                    sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageIII')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                    sb.Append(" values('03.051','" + DGStageReq.ProductCode.Trim() + "',");
        //                    sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageIV')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    if (DGStageReq.EngPartCode.Trim().Substring(0, 3) == "001")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetailssub set ");
        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Append("Stage3Status='D',stage3Date=GetDate() ");
        //                        }
        //                        else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                        {
        //                            sb.Append("Stage3Status='R' ");
        //                        }
        //                        sb.Append("where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Serialno='" + DGStageReq.EngSrNo.Trim() + "' and  SrNoPartcode='" + DGStageReq.EngPartCode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("Update JobCard2Detailssub set ");
        //                            if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                                sb.Append("Stage3Status='D', JobCard1='" + DGStageReq.JBCode.Trim() + "'");
        //                            sb.Append(" where Serialno='" + DGStageReq.EngSrNo.Trim() + "' and  SrNoPartcode='" + DGStageReq.EngPartCode.Trim() + "' and ");
        //                            sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();
        //                        }
        //                    }
        //                    if (DGStageReq.AltPartcode.Trim().Substring(0, 3) == "002")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetailssub set ");
        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Append("Stage3Status='D',stage3Date=GetDate() ");
        //                        }
        //                        else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                        {
        //                            sb.Append("Stage3Status='R' ");
        //                        }
        //                        sb.Append("where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Serialno='" + DGStageReq.AltSrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.AltPartcode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("Update JobCard2Detailssub set Stage3Status='D', JobCard1='" + DGStageReq.JBCode.Trim() + "' ");
        //                            sb.Append(" where Serialno='" + DGStageReq.AltSrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.AltPartcode.Trim() + "' and ");
        //                            sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();
        //                        }
        //                    }
        //                    if (DGStageReq.CpyPartcode.Trim().Substring(0, 2) == "40")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetailssub set ");
        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Append("Stage3Status='D',stage3Date=GetDate() ");
        //                        }
        //                        else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                        {
        //                            sb.Append("Stage3Status='R' ");
        //                        }
        //                        sb.Append("where JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Serialno='" + DGStageReq.CpySrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.CpyPartcode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                        {
        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("Update JobCard2Detailssub set Stage3Status='D', JobCard1='" + DGStageReq.JBCode.Trim() + "' ");
        //                            sb.Append(" where Serialno='" + DGStageReq.CpySrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.CpyPartcode.Trim() + "' and ");
        //                            sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();
        //                        }

        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                        sb.Append(" values('03.051','" + DGStageReq.CpyPartcode.Trim() + "',");
        //                        sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageIII')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }

        //                    if (DGStageReq.BatPartcode.Trim().Length > 2)
        //                    {
        //                        if (DGStageReq.BatPartcode.Trim().Substring(0, 3) == "010")
        //                        {
        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("Update JobCardDetailssub set ");
        //                            if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                            {
        //                                sb.Append("Stage3Status='D',stage3Date=GetDate() ");
        //                            }
        //                            else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                            {
        //                                sb.Append("Stage3Status='R' ");
        //                            }
        //                            sb.Append("where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                            sb.Append("Serialno='" + DGStageReq.BatSrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.BatPartcode.Trim() + "' and ");
        //                            sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();

        //                            if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                            {
        //                                sb.Remove(0, sb.Length);
        //                                sb.Append("Update JobCard2Detailssub set Stage3Status='D', JobCard1='" + DGStageReq.JBCode.Trim() + "' ");
        //                                sb.Append(" where  Serialno='" + DGStageReq.BatSrno.Trim() + "' and  SrNoPartcode='" + DGStageReq.BatPartcode.Trim() + "' and ");
        //                                sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                                cmd = new SqlCommand(sb.ToString(), con);
        //                                cmd.Transaction = tran;
        //                                cmd.ExecuteNonQuery();
        //                                cmd.Dispose();
        //                            }

        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                            sb.Append(" values('03.051','" + DGStageReq.BatPartcode.Trim() + "',");
        //                            sb.Append("'" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "',GetDate(),'1','03.051',0,'0','StageIII')");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();
        //                        }
        //                    }

        //                    if (double.Parse(strkVA) > 160)
        //                    {
        //                        if (DGStageReq.Bat2Partcode.Trim().Length > 2)
        //                        {
        //                            if (DGStageReq.Bat2Partcode.Trim().Substring(0, 3) == "010")
        //                            {
        //                                sb.Remove(0, sb.Length);
        //                                sb.Append("Update JobCardDetailssub set ");
        //                                if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                                {
        //                                    sb.Append("Stage3Status='D',stage3Date=GetDate() ");
        //                                }
        //                                else if (DGStageReq.PrcStatus == "Rework" || DGStageReq.PrcStatus == "Rejection")
        //                                {
        //                                    sb.Append("Stage3Status='R' ");
        //                                }
        //                                sb.Append("where JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                                sb.Append("Serialno='" + DGStageReq.Bat2Srno.Trim() + "' and  SrNoPartcode='" + DGStageReq.Bat2Partcode.Trim() + "' and ");
        //                                sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                                cmd = new SqlCommand(sb.ToString(), con);
        //                                cmd.Transaction = tran;
        //                                cmd.ExecuteNonQuery();
        //                                cmd.Dispose();

        //                                if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                                {
        //                                    sb.Remove(0, sb.Length);
        //                                    sb.Append("Update JobCard2Detailssub set Stage3Status='D',JobCard1='" + DGStageReq.JBCode.Trim() + "'   ");
        //                                    sb.Append("where Serialno='" + DGStageReq.Bat2Srno.Trim() + "' and  SrNoPartcode='" + DGStageReq.Bat2Partcode.Trim() + "' and ");
        //                                    sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                                    cmd = new SqlCommand(sb.ToString(), con);
        //                                    cmd.Transaction = tran;
        //                                    cmd.ExecuteNonQuery();
        //                                    cmd.Dispose();
        //                                }

        //                                sb.Remove(0, sb.Length);
        //                                sb.Append("Update StockWIP set IssueQty=IssueQty+1 where FromProfitCenterCode='03.051' and Issuecode='" + DGStageReq.JBCode + "-->" + DGStageReq.EngSrNo + "' and StageName='StageIII' ");
        //                                sb.Append(" and Partcode='" + DGStageReq.BatPartcode.Trim() + "' and ToProfitCenterCode='03.051' ");
        //                                cmd = new SqlCommand(sb.ToString(), con);
        //                                cmd.Transaction = tran;
        //                                cmd.ExecuteNonQuery();
        //                                cmd.Dispose();
        //                            }

        //                        }
        //                    }

        //                    for (int cSub1 = 0; cSub1 <= recCount1; cSub1++)
        //                    {

        //                        SrNo += 1;
        //                        string[] PrcChkDts = Regex.Split(strPrcChkDts[cSub1].ToString().Trim(), "-->");
        //                        //string GetDGStartTime = "";

        //                        //GetDGStartTime = ComCon.getTranName("Select convert(nvarchar(20),DGStartTime,120) as DGStartTime from TestReport Where TRCode='" + TestReportReq.TRCode.Trim() + "' ", "TblTestReport", "DGStartTime", con, tran);
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("insert into PrcChkDetails(TransCode,Dt,MainSerialNo,PrcName,ChkPointId,PrcChkPoints,PrcStatus,DGStartTime,QA6M)");
        //                        sb.Append("values('" + DGStageReq.JBCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
        //                        sb.Append("'" + DGStageReq.EngSrNo.Trim() + "','DG Stage3',");
        //                        sb.Append("'" + PrcChkDts[0].Trim() + "','" + PrcChkDts[1].Trim() + "',");
        //                        sb.Append("'" + DGStageReq.PrcStatus + "',");
        //                        sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DGStageReq.QA6M + "')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                    }

        //                    if (DGStageReq.PrcStatus == "Accepted(OK)")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCardDetails set Stage3Qty=Stage3Qty+1 where  JobCode='" + DGStageReq.JBCode.Trim() + "' and ");
        //                        sb.Append("Partcode='" + DGStageReq.ProductCode.Trim() + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }

        //                    cntStageStatus = "0";
        //                    cntStageStatus = ComCon.getTranName("select isnull(Count(Stage3Status),0) as Stg3 from JobCardDetailssub where Stage3Status='P'  and jobCode='" + DGStageReq.JBCode.Trim() + "'", "JobCard", "Stg3", con, tran);
        //                    if (cntStageStatus == "0")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCard set Stage3Status='D' where  JobCode='" + DGStageReq.JBCode.Trim() + "'  ");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                    }
        //                    #endregion
        //                }
        //                tran.Commit();
        //                // tran.Rollback();
        //                return DGStageReq.JBCode.Trim();
        //            }
        //            catch (Exception ex)
        //            {
        //                tran.Rollback();
        //                return ("StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString());
        //            }

        //            finally
        //            {
        //                con.Close();
        //            }
        //        }
        //        */

        //        public DataTable GetPodprcDts(string strPrdPartCode, string strPCCode)
        //        {
        //            string StrBOMCode = "";

        //            DataSet dSet = new DataSet();
        //            SqlDataAdapter adapter = new SqlDataAdapter();
        //            CommonCon ComCon = new CommonCon();

        //            StrBOMCode = ComCon.getName("select BOMCode+'-->'+convert(nvarchar(10),WgtHR)+'-->'+convert(nvarchar(10),WgtCR) as BOMCode From BOM where Partcode='" + strPrdPartCode.Trim() + "' and Active='1' and Discard='1' ", "TblBOM", "BOMCode");
        //            string[] ProdDts = Regex.Split(StrBOMCode.Trim(), "-->");
        //            string RatePCCode = "";
        //            if (strPCCode.Trim() == "01.005")
        //            {
        //                RatePCCode = "01.007','01.005','03.008";
        //            }
        //            else if (strPCCode.Trim() == "03.038")
        //            {
        //                RatePCCode = "03.001','03.038','03.008";
        //            }
        //            else if (strPCCode.Trim() == "01.004" || strPCCode.Trim() == "04.001" || strPCCode.Trim() == "14.001" || strPCCode.Trim() == "03.051")
        //            {
        //                RatePCCode = "01.005','01.004','04.001','01.064','14.001','01.023','03.040','03.051";
        //            }
        //            else if (strPCCode.Trim() == "01.023")
        //            {
        //                RatePCCode = "01.007','01.023','01.064";
        //            }
        //            else if (strPCCode.Trim() == "03.040")
        //            {
        //                RatePCCode = "01.007','01.023','03.040','01.064";
        //            }
        //            else
        //            {
        //                RatePCCode = strPCCode.Trim();
        //            }

        //            if (con.State == ConnectionState.Open)
        //            {
        //                con.Close();
        //            }
        //            else
        //            {
        //                con.Open();
        //            }

        //            if (StrBOMCode.Trim() != "0")
        //            {
        //                SqlCommand cmd = new SqlCommand();
        //                sb.Remove(0, sb.Length);
        //                sb.Append("Create table #tempstockwip(FromProfitCenterCode nvarchar(50),PartCode nvarchar(50),");
        //                sb.Append("IssueQty float,ToProfitCenterCode nvarchar(50),ReceivedQty float,stockType Int)");
        //                sb.Append(" insert Into #tempstockwip(FromProfitCenterCode,PartCode,IssueQty,ToProfitCenterCode,ReceivedQty,stockType)");
        //                sb.Append(" select FromProfitCenterCode,PartCode,IssueQty,ToProfitCenterCode,ReceivedQty,stockType ");
        //                sb.Append(" from stockwip where fromprofitcentercode in ('" + strPCCode + "') and IssueQty>'0' and ");
        //                sb.Append(" len(ToProfitCenterCode)='6' and len(FromProfitCenterCode)='6'");
        //                sb.Append(" insert Into #tempstockwip(FromProfitCenterCode,PartCode,IssueQty,ToProfitCenterCode,ReceivedQty,stockType)");
        //                sb.Append(" select FromProfitCenterCode,PartCode,IssueQty,ToProfitCenterCode,ReceivedQty,stockType");
        //                sb.Append(" from stockwip where toprofitcentercode in ('" + strPCCode + "') and ReceivedQty>'0' and");
        //                sb.Append(" len(ToProfitCenterCode)='6' and len(FromProfitCenterCode)='6'");
        //                cmd = new SqlCommand(sb.ToString(), con);
        //                cmd.CommandTimeout = 0;
        //                cmd.ExecuteNonQuery();
        //                cmd.Dispose();

        //                sb.Remove(0, sb.Length);
        //                sb.Append("Create table #TempPricelist(ProfitCenterCode nvarchar(50),PartCode nvarchar(50),PurRate Float,Rate Float,Pwt float,PSqFt float)");
        //                sb.Append("insert Into #TempPricelist(ProfitCenterCode,PartCode,PurRate,Rate,Pwt,PSqFt)");
        //                sb.Append("select ProfitCenterCode,PartCode,PurRate,Rate,Pwt,PSqFt");
        //                sb.Append(" from ProfitcenterPLdetails where ProfitcenterCode in ('" + RatePCCode + "')");
        //                cmd = new SqlCommand(sb.ToString(), con);
        //                cmd.CommandTimeout = 0;
        //                cmd.ExecuteNonQuery();
        //                cmd.Dispose();

        //                sb.Remove(0, sb.Length);
        //                sb.Append("Create table #TempBOMdetails(BOMCode nvarchar(50),PartCode nvarchar(50),Qty Float,SuppRate Float,MOB nvarchar(50),KitCode nvarchar(50),StockQty float, TotalQty float, QAP float, Amount float,PrcCostUnitKG Float )");
        //                sb.Append("insert Into #TempBOMdetails(BOMCode,PartCode,Qty,SuppRate,MOB,KitCode,StockQty,TotalQty,QAP,Amount,PrcCostUnitKG)");
        //                sb.Append("select BOMCode,PartCode,Qty,SuppRate,MOB,KitCode,'0' as StockQty,'0' as TotalQty,'0' AS QAP,'0' AS Amount,PrcCostUnitKG from BOMdetails where BOMCode='" + ProdDts[0].Trim() + "'");
        //                cmd = new SqlCommand(sb.ToString(), con);
        //                cmd.CommandTimeout = 0;
        //                cmd.ExecuteNonQuery();
        //                cmd.Dispose();

        //                strSql = "";
        //                if (strPCCode.ToString().Trim() == "01.023" || strPCCode.ToString().Trim() == "03.040")
        //                {
        //                    strSql = "select BD.PartCode,partdesc+' [' +BD.partcode +'] ' as PartDesc,u.UName,Qty,BD.Mob," +
        //                         "P.Kit,p.conversionvalue,p.UomCode,'0' as StockQty,'0' as TotalQty, " +
        //                         "SuppRate as Rate,0.00 as  QAP, P.CategoryID " +
        //                         "from #TempBOMdetails BD Inner Join Part P On Bd.Partcode=P.partcode " +
        //                         "Inner join UOM U On P.UOMCode=U.Uid " +
        //                         "where bd.BOMCode='" + ProdDts[0].Trim() + "' and kitcode='" + strPrdPartCode.Trim() + "' " +
        //                         "and Kit='1' and BD.PartCode LIKE '004%' " +
        //                         " Union all " +
        //                         "select BD2.PartCode,partdesc+' [' +BD2.partcode +'] ' as PartDesc,u.UName,Bd2.Qty,BD2.Mob," +
        //                         "P.Kit,p.conversionvalue,p.UomCode,'0' as StockQty,'0' as TotalQty, " +
        //                         "Bd2.SuppRate as Rate,0.00 as  QAP,P.CategoryID " +
        //                         "from #TempBOMdetails BD Inner JOin #TempBOMdetails Bd1 On Bd.BomCode=Bd1.BomCode and Bd.partcode=Bd1.KitCode " +
        //                         "Inner JOin #TempBOMdetails Bd2 On Bd1.BomCode=Bd2.BomCode and Bd1.partcode=Bd2.KitCode " +
        //                         "Inner Join Part P On Bd2.Partcode=P.partcode  " +
        //                         "Inner join UOM U On P.UOMCode=U.Uid " +
        //                         "where bd.BOMCode='" + ProdDts[0].Trim() + "' and Bd.kitcode='" + strPrdPartCode.Trim() + "' " +
        //                         "and BD1.PartCode LIKE '0125%' and Substring(BD1.PartCode,12,2) in ('12') " +
        //                         "ORDER BY PartDesc ";
        //                }
        //                else
        //                {
        //                    strSql = "select BD.PartCode,partdesc+' [' +BD.partcode +'] ' as PartDesc,'' as SerialNo,u.UName,Qty,BD.Mob,P.Kit,p.conversionvalue,p.UomCode,'0' as StockQty,'0' as TotalQty," +
        //                             "SuppRate as Rate, QAP, Amount, P.CategoryID " +
        //                             " from #TempBOMdetails BD Inner Join Part P On Bd.Partcode=P.partcode Inner join UOM U On P.UOMCode=U.Uid " +
        //                             " where bd.KitCode='" + strPrdPartCode.Trim() + "' and bd.BOMCode='" + ProdDts[0].Trim() + "' and substring(BD.PartCode,1,3) Not in ('001','002','010','401')" +
        //                             " ORDER BY P.PartDesc";
        //                }

        //                cmd = new SqlCommand(strSql, con);
        //                adapter.SelectCommand = cmd;
        //                adapter.Fill(dSet, "TempBOMdetails");
        //                cmd.Dispose();
        //                double gridAmt = 0;
        //                if (dSet.Tables["TempBOMdetails"].Rows.Count > 0)
        //                {
        //                    for (int i = 0; i < dSet.Tables["TempBOMdetails"].Rows.Count; i++)
        //                    {

        //                        dSet.Tables["TempBOMdetails"].Rows[i]["TotalQty"] = 1 * double.Parse(dSet.Tables["TempBOMdetails"].Rows[i]["Qty"].ToString().Trim());
        //                        dSet.Tables["TempBOMdetails"].Rows[i]["Rate"] = Math.Round(getRate4Process(dSet.Tables["TempBOMdetails"].Rows[i]["PartCode"].ToString().Trim(), strPCCode.Trim(), double.Parse(dSet.Tables["TempBOMdetails"].Rows[i]["conversionvalue"].ToString().Trim()), dSet.Tables["TempBOMdetails"].Rows[i]["UomCode"].ToString().Trim(), dSet.Tables["TempBOMdetails"].Rows[i]["MOB"].ToString().Trim(), strPrdPartCode.Trim(), "N", "", ProdDts[0].Trim()), 2);
        //                        dSet.Tables["TempBOMdetails"].Rows[i]["StockQty"] = Math.Round(getWipStock(dSet.Tables["TempBOMdetails"].Rows[i]["PartCode"].ToString().Trim(), strPCCode.Trim()), 2);
        //                        dSet.Tables["TempBOMdetails"].Rows[i]["QAP"] = Math.Round((double.Parse(dSet.Tables["TempBOMdetails"].Rows[i]["StockQty"].ToString().Trim()) - double.Parse(dSet.Tables["TempBOMdetails"].Rows[i]["TotalQty"].ToString().Trim())), 2);
        //                        dSet.Tables["TempBOMdetails"].Rows[i]["Amount"] = Math.Round((double.Parse(dSet.Tables["TempBOMdetails"].Rows[i]["Rate"].ToString().Trim()) * double.Parse(dSet.Tables["TempBOMdetails"].Rows[i]["TotalQty"].ToString().Trim())), 2);
        //                        gridAmt = gridAmt + double.Parse(dSet.Tables["TempBOMdetails"].Rows[i]["Amount"].ToString().Trim());

        //                    }

        //                }

        //            }
        //            return dSet.Tables["TempBOMdetails"];
        //        }

        //        protected double getWipStock(string PartCode, string strPCCode)
        //        {
        //            DataSet RecQty = new DataSet();
        //            DataSet IssueQty = new DataSet();
        //            double tempRecQty = 0;
        //            double tempIssueQty = 0;
        //            SqlDataAdapter adapter = new SqlDataAdapter();
        //            SqlCommand cmd = new SqlCommand();
        //            strSql = "";
        //            strSql = "SELECT Sum(ReceivedQty) AS RecQty FROM #tempstockwip WHERE ToProfitCenterCode='" + strPCCode.Trim() + "' AND PartCode='" + PartCode.Trim() + "' ";
        //            cmd = new SqlCommand(strSql, con);
        //            adapter.SelectCommand = cmd;
        //            adapter.Fill(RecQty, "tempstockwip");
        //            cmd.Dispose();

        //            strSql = "";
        //            strSql = "SELECT Sum(IssueQty) AS IssueQty FROM #tempstockwip WHERE FromProfitCenterCode='" + strPCCode.Trim() + "' AND PartCode='" + PartCode.Trim() + "' ";
        //            cmd = new SqlCommand(strSql, con);
        //            adapter.SelectCommand = cmd;
        //            adapter.Fill(IssueQty, "tempstockwip");
        //            cmd.Dispose();

        //            if (RecQty.Tables["tempstockwip"].Rows[0]["RecQty"].ToString().Trim() == "")
        //            {
        //                tempRecQty = 0;
        //            }
        //            else
        //            {
        //                tempRecQty = double.Parse(RecQty.Tables["tempstockwip"].Rows[0]["RecQty"].ToString().Trim());
        //            }

        //            if (IssueQty.Tables["tempstockwip"].Rows[0]["IssueQty"].ToString().Trim() == "")
        //            {
        //                tempIssueQty = 0;
        //            }
        //            else
        //            {
        //                tempIssueQty = double.Parse(IssueQty.Tables["tempstockwip"].Rows[0]["IssueQty"].ToString().Trim());
        //            }

        //            return tempRecQty - tempIssueQty;

        //        }

        //        public double getRate4Process(String strPartCode, String PCCode, Double ConvValue, String struom, String strMOB, String strKIT, String RateType, String strRateFor, String strBOmCode)
        //        {
        //            string strOrignalPCCode = string.Empty;
        //            strOrignalPCCode = PCCode;
        //            string strFeild = "Rate";
        //            double prate = 0;
        //            string strsql = string.Empty;
        //            DataSet ds1 = new DataSet();

        //            if (PCCode == "01.005" || PCCode == "03.038") // Canopy Assembly
        //            {
        //                if (strPartCode.Trim().Substring(0, 3) == "005") //GoemKit
        //                {
        //                    PCCode = "01.064";
        //                }
        //                else if (strPartCode.Trim().Substring(0, 5) == "00712" || strPartCode.Trim().Substring(0, 11) == "00002060211" || strPartCode.Trim().Substring(0, 3) == "012") //Foam
        //                {
        //                    PCCode = "03.008";
        //                }
        //                else if (PCCode == "01.005" && strPartCode.Trim().Substring(0, 3) == "004" && strPartCode.Trim().Substring(11, 1) == "0" || strPartCode.Trim().Substring(11, 1) == "1" && strPartCode.Trim().Substring(10, 1) == "1") //canopy assly U1
        //                {
        //                    PCCode = "01.005";
        //                }
        //                else if (PCCode == "03.038" && strPartCode.Trim().Substring(0, 3) == "004" && strPartCode.Trim().Substring(11, 1) == "0" || strPartCode.Trim().Substring(11, 1) == "1" && strPartCode.Trim().Substring(10, 1) == "1") //canopy assly U4
        //                {
        //                    PCCode = "03.038";
        //                }
        //                else if (PCCode == "01.005" && strPartCode.Trim().Substring(0, 3) == "004" && strPartCode.Trim().Substring(10, 1) == "4" || strPartCode.Trim().Substring(10, 1) == "5") //Canopy assly U1
        //                {
        //                    PCCode = "01.007";
        //                }
        //                else if (PCCode == "03.038" && strPartCode.Trim().Substring(0, 3) == "004" && strPartCode.Trim().Substring(10, 1) == "4" || strPartCode.Trim().Substring(10, 1) == "5") //PowderCoating U4
        //                {
        //                    PCCode = "03.001";
        //                }
        //                else if (PCCode == "01.005" && strPartCode.Trim().Substring(0, 3) == "004")  //PowderCoating U1
        //                {
        //                    PCCode = "01.007";
        //                }
        //            }
        //            else if (PCCode == "03.040" || PCCode == "01.004" || PCCode == "05.001" || PCCode == "04.001" || PCCode == "14.001" || PCCode == "03.051") // DG Assembly
        //            {
        //                if (strPartCode.Trim().Substring(0, 3) == "005") //GoemKit
        //                {
        //                    PCCode = "01.064";
        //                }
        //                else if (strPartCode.Trim().Substring(0, 2) == "40") // Canopy
        //                {
        //                    PCCode = "01.005";
        //                }
        //                else if ((strPartCode.Trim().Substring(0, 3) == "004") && (strPartCode.Trim().Substring(11, 1) == "5" || strPartCode.Trim().Substring(11, 1) == "6"))// CP stand
        //                {
        //                    PCCode = "01.005','01.007','01.023";
        //                }
        //                else
        //                {
        //                    PCCode = "01.004','01.023','01.005','01.007','03.051','03.040";
        //                }
        //            }

        //            strsql = "";
        //            if (strMOB.Trim() == "B")
        //            {
        //                //Part Related To BOM
        //                if (strPartCode.Trim().Substring(0, 3) == "006")
        //                {
        //                    strsql = "Select isnull(Rate,0) as Rate,CostingCode From Supplierpricelistchanged Where PartCode='" + strPartCode + "' and " +
        //                   " ChangeDateTime=(Select max(ChangeDateTime) From supplierpricelistchanged SPC Inner Join PurchaseCosting PC On SPC.CostingCode=PC.PCCode  " +
        //                   "Where SPC.PartCode='" + strPartCode + "' and (Rateselected='M' or Rateselected='T') and PC.Active='1' and PC.Discard='1' and ChangeDateTime<=getdate())";
        //                }
        //                else
        //                {
        //                    strsql = "Select top 1 isnull(SuppRate+PrcCostUnitKG,0) as Rate from #TempBOMdetails where partcode='" + strPartCode + "'";
        //                }
        //            }
        //            else if (strMOB.Trim() == "M" || strMOB.Trim() == "O") //MFg Parts And strBOMcode <> ""
        //            {
        //                strsql = "";
        //                strsql = "select isnull(" + strFeild + ",0) as Rate from #TempPricelist where ProfitCenterCode in ('" + PCCode + "') and partcode='" + strPartCode + "'";
        //            }

        //            ds1.Clear();
        //            SqlDataAdapter dAd1 = new SqlDataAdapter(strsql, con);
        //            dAd1.Fill(ds1, "TempBOMRate");
        //            if (ds1.Tables["TempBOMRate"].Rows.Count > 0)
        //            {
        //                prate = (double)ds1.Tables["TempBOMRate"].Rows[0]["Rate"];
        //            }
        //            ds1.Dispose();
        //            dAd1.Dispose();

        //            return prate;
        //        }

        //        public string SubmitStage4(DGStage4Request Stage4Req)
        //        {
        //            string StrBOMCode = "";
        //            //int strEngQty = 0, strAltQty = 0, strCpyQty = 0, strBatQty = 0, strCPQty = 0;

        //            string PrcNo = "", StrDGRate = "0", StrCPRate = "0", StrCP2Rate = "0", StrCRRate = "", StrHRRate = "0";
        //            string[] CPType = Regex.Split(Stage4Req.CPType.Trim(), "-->");

        //            CommonCon ComCon = new CommonCon();
        //            if (Stage4Req.Remark == "Start")
        //            {

        //                if (ChkStage(4, Stage4Req.JobCardCode, Stage4Req.EngSrNo) == true)
        //                {
        //                    return "This Stage for Jobcard " + Stage4Req.JobCardCode + " With Eng Serial No " + Stage4Req.EngSrNo + " alredy Completed!";
        //                }
        //                string strkVA = "";
        //                string CladdingRate = "0";
        //                dsDetailsSubV = ComCon.procDS("SELECT KVA,Model FROM Part WHERE PartCode='" + Stage4Req.ProductCode.Trim() + "'", "tblPart");
        //                if (dsDetailsSubV.Tables["tblPart"].Rows.Count > 0)
        //                {
        //                    CladdingRate = ComCon.getTranName("SELECT Rate FROM SilCladdingrate WHERE KVA='" + dsDetailsSubV.Tables["tblPart"].Rows[0]["KVA"].ToString().Trim() + "' " +
        //                                    "AND Model='" + dsDetailsSubV.Tables["tblPart"].Rows[0]["Model"].ToString().Trim() + "' " +
        //                                    "AND CompanyCode='" + Stage4Req.PCCode.Trim().Substring(0, 2) + "' " +
        //                                    "AND Active='1' and Discard='1' AND Auth='1'", "tblCldRate", "Rate", con, tran);
        //                    strkVA = dsDetailsSubV.Tables["tblPart"].Rows[0]["KVA"].ToString().Trim();
        //                    dsDetailsSubV.Tables["tblPart"].Dispose();
        //                    dsDetailsSubV.Tables["tblPart"].Clear();
        //                }

        //                #region
        //                //Serial No Validation For Scanning
        //                if (Stage4Req.EngSrNo.Trim() == "0" || Stage4Req.EngSrNo.Trim() == "")
        //                {
        //                    PrcNo = "Please Scan Engine SerialNo";
        //                    return PrcNo;
        //                }
        //                if (Stage4Req.AltSrno.Trim() == "0" || Stage4Req.AltSrno.Trim() == "")
        //                {
        //                    PrcNo = "Please Scan Alternator SerialNo";
        //                    return PrcNo;
        //                }
        //                if (Stage4Req.CpySrno.Trim() == "0" || Stage4Req.CpySrno.Trim() == "")
        //                {
        //                    PrcNo = "Please Scan Canopy SerialNo";
        //                    return PrcNo;
        //                }
        //                if (double.Parse(CPType[1].Trim()) != 0 && double.Parse(CPType[1].Trim()) != 150)
        //                {
        //                    if (Stage4Req.CPSrno.Trim() == "0" || Stage4Req.CPSrno.Trim() == "")
        //                    {
        //                        PrcNo = "Please Scan Control Panel(1) SerialNo";
        //                        return PrcNo;
        //                    }
        //                }
        //                //if (Stage4Req.CPSrno.Trim() == "0" || Stage4Req.CPSrno.Trim() == "")
        //                //{
        //                //    PrcNo = "Please Scan Control Panel SerialNo";
        //                //    return PrcNo;
        //                //}

        //                //if (Stage4Req.BatSrno.Trim() == "0" || Stage4Req.BatSrno.Trim() == "")
        //                //{
        //                //    PrcNo = "Please Scan Battery SerialNo";
        //                //    return PrcNo;
        //                //}
        //                if (double.Parse(strkVA.Trim()) >= 180)
        //                {
        //                    if (Stage4Req.BatSrno.Trim() == "0" || Stage4Req.BatSrno.Trim() == "")
        //                    {
        //                        PrcNo = "Please Scan Battery(2) SerialNo";
        //                        return PrcNo;
        //                    }
        //                }


        //                //Details Validation For Stock
        //                int recCountV = ComCon.CountChars(Stage4Req.PrcDts, ",");
        //                string[] strJCDDtsV = Regex.Split(Stage4Req.PrcDts, ",");

        //                int SrNoV = 0;
        //                for (int cSubV = 0; cSubV <= recCountV; cSubV++)
        //                {
        //                    SrNoV += 1;
        //                    string[] DtsV = Regex.Split(strJCDDtsV[cSubV].ToString().Trim(), "-->");

        //                    if (double.Parse(DtsV[5].Trim()) < 0)
        //                    {
        //                        PrcNo = "Insufficient Stock For Part= " + DtsV[0].Trim();
        //                        return PrcNo;
        //                    }

        //                }
        //                #endregion

        //                StrBOMCode = ComCon.getName("select BOMCode+'-->'+convert(nvarchar(10),Wgt)+'-->'+convert(nvarchar(10),sqft)+'-->'+convert(nvarchar(10),WgtHR)+'-->'+convert(nvarchar(10),WgtCR) as BOMCode From BOM where Partcode='" + Stage4Req.ProductCode.Trim() + "' and Active='1' and Discard='1' ", "TblBOM", "BOMCode");
        //                string[] ProdDts = System.Text.RegularExpressions.Regex.Split(StrBOMCode.Trim(), "-->");
        //                StrDGRate = ComCon.getName("SELECT top 1 Rate AS DGRate FROM ProfitcenterPlDetails WHERE Profitcentercode='" + Stage4Req.PCCode.Trim() + "' and Partcode='" + Stage4Req.ProductCode.Trim() + "'", "tblDGRate", "DGRate");
        //                if (double.Parse(CPType[1].Trim()) != 0 && double.Parse(CPType[1].Trim()) != 150)
        //                {
        //                    if (Stage4Req.CPSrno.Trim() != "0")
        //                    {
        //                        StrCPRate = ComCon.getName("SELECT top 1 Rate AS CPRate FROM ProfitcenterPlDetails WHERE Profitcentercode='03.040' and Partcode='" + Stage4Req.CPPartcode.Trim() + "'", "tblCPRate", "CPRate");
        //                    }
        //                    if (Stage4Req.CP2Srno.Trim() != "0")
        //                    {
        //                        StrCP2Rate = ComCon.getName("SELECT top 1 Rate AS CPRate FROM ProfitcenterPlDetails WHERE Profitcentercode='03.040' and Partcode='" + Stage4Req.CP2Partcode.Trim() + "'", "tblCPRate", "CPRate");
        //                    }
        //                }
        //                StrCRRate = ComCon.getName("SELECT top 1 SteelRate AS CRRate FROM BomDetails WHERE BOMCode='" + ProdDts[0].Trim() + "' AND Thickness<='1.5' AND SteelRate>0 ", "tblCRRate", "CRRate");
        //                StrHRRate = ComCon.getName("SELECT top 1 SteelRate AS HRRate FROM bomdetails WHERE BOMCode='" + ProdDts[0].Trim() + "' AND Thickness>'1.5' AND SteelRate>0", "tblHRRate", "HRRate");

        //                if (double.Parse(StrDGRate.Trim()) == 0)
        //                {
        //                    PrcNo = "DG Rate Cannot Be Zero Please Contact CIA/DOcument Control Dept";
        //                    return PrcNo;
        //                }

        //                if (double.Parse(CPType[1].Trim()) != 0 && double.Parse(CPType[1].Trim()) != 150)
        //                {
        //                    if (double.Parse(StrCPRate.Trim()) == 0)
        //                    {
        //                        PrcNo = "Control Panel Rate Cannot Be Zero Please Contact CIA/DOcument Control Dept";
        //                        return PrcNo;
        //                    }
        //                }
        //                if (double.Parse(CPType[1].Trim()) != 0 && double.Parse(CPType[1].Trim()) != 150)
        //                {
        //                    dsDetailsCP = ComCon.procDS("exec GetCPPartcode " + CPType[1].Trim() + ",'0'", "tbl_JoBCardCPPart");
        //                    if (dsDetailsCP != null && dsDetailsCP.Tables["tbl_JoBCardCPPart"].Rows.Count > 1)
        //                    {

        //                        if (double.Parse(CPType[1].Trim()) != 0)
        //                        {
        //                            if (Stage4Req.CP2Srno.Trim() == "0" || Stage4Req.CP2Srno.Trim() == "")
        //                            {
        //                                PrcNo = "Please Scan Control Panel(2) SerialNo";
        //                                return PrcNo;
        //                            }
        //                        }

        //                        if (double.Parse(StrCP2Rate.Trim()) == 0)
        //                        {
        //                            PrcNo = "Control Panel(2) Rate Cannot Be Zero Please Contact CIA/DOcument Control Dept";
        //                            return PrcNo;
        //                        }
        //                    }

        //                    dsDetailsCP.Tables["tbl_JoBCardCPPart"].Dispose();
        //                    dsDetailsCP.Tables["tbl_JoBCardCPPart"].Clear();
        //                }

        //                try
        //                {

        //                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
        //                    tran = con.BeginTransaction();

        //                    string DGNo = ComCon.getTranName("select cast(year(getdate()) as char(4))+'.'+RIGHT('0'+ RTRIM(MONTH(getdate())), 2)+'.'+RIGHT('000'+ RTRIM((isnull(max(SerialNo),0)+1)), 4) as DGNo " +
        //                      " from processfeedback where companycode='" + Stage4Req.PCCode.Trim().Substring(0, 2) + "' and profitcentercode='" + Stage4Req.PCCode.Trim() + "' and yr='" + ComCon.yearEnd(con, tran) + "'", "processfeedback", "DGNo", con, tran);
        //                    if (DGNo == "0")
        //                    {
        //                        PrcNo = "DG Serial No Creation Problem";
        //                        return PrcNo;
        //                    }
        //                    //PrcNo = ComCon.GetMaxNo("ProcessFeedback", "PSH", Stage4Req.PCCode.Trim().Substring(0, 2), con, tran);
        //                    PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), Stage4Req.PCCode.Trim().Substring(0, 2), con, tran);

        //                    SqlCommand cmd = new SqlCommand();
        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,Yr,MachineCode,SerialNo,ProfitCenterCode,TurretKitCode,");
        //                    sb.Append("PartCode,PPWCode,MOFCode,VersionCode,ProcessQty,PKitQty,PLength,PWidth,PThickness, NstWtPerUt,NstSqftPerUt,WtPerUt,SqftPerUt,CRWt,HRWt,CRRate,HRRate ,CompanyCode,");
        //                    sb.Append("PPDIRStatus,TRStatus,PFBType,PFBRate,SilCladdingRate,Remark,PrcBOMCode,QPCStatus)");
        //                    sb.Append(" values('" + PrcNo.Trim() + "','" + PrcNo.Trim() + "','" + (PrcNo.Substring(10, 8)) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ");
        //                    sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + DGNo.Trim() + "','" + DGNo.Substring(8, 4).Trim() + "','" + Stage4Req.PCCode.Trim() + "','0','" + Stage4Req.ProductCode.Trim() + "','0', ");
        //                    sb.Append("'MOF','" + CPType[1].Trim() + "','1','0','0','0','0', '" + Convert.ToDouble(ProdDts[1].Trim()) + "', '" + Convert.ToDouble(ProdDts[2].Trim()) + "', ");
        //                    sb.Append("'" + Convert.ToDouble(ProdDts[1].Trim()) + "', '" + Convert.ToDouble(ProdDts[2].Trim()) + "', '" + ProdDts[4].Trim() + "', '" + ProdDts[3].Trim() + "', '" + StrCRRate.Trim() + "', '" + StrCRRate.Trim() + "', '" + Stage4Req.PCCode.Trim().Substring(0, 2) + "','P','P',");
        //                    if (double.Parse(CPType[1].Trim()) == 0)
        //                    {
        //                        sb.Append("'', ");
        //                    }
        //                    else if (double.Parse(CPType[1].Trim()) != 0)
        //                    {
        //                        sb.Append("'" + CPType[0].Trim() + "', ");
        //                    }
        //                    sb.Append("'" + double.Parse(StrDGRate.Trim()) + "','" + CladdingRate.Trim() + "','" + Stage4Req.Remark.Trim() + "','" + ProdDts[0].Trim() + "','D')"); ;
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,issueDate,issueQty,ToProfitCenterCode,StockType,StageName)");
        //                    sb.Append(" values('" + Stage4Req.PCCode.Trim() + "','" + Stage4Req.ProductCode.Trim() + "',");
        //                    sb.Append("'" + PrcNo.Trim() + "',GetDate(),'1','" + Stage4Req.PCCode.Trim() + "',0,'StageIV')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    int recCount = ComCon.CountChars(Stage4Req.PrcDts, ",");
        //                    string[] strPrcDts = Regex.Split(Stage4Req.PrcDts, ",");
        //                    int SrNo = 0;

        //                    for (int cSub = 0; cSub <= recCount; cSub++)
        //                    {
        //                        SrNo += 1;
        //                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");

        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,StockQty,");
        //                        sb.Append("PFBRate,PLength,PWidth,PThickness,PLossWt,PHeight,PLength1,PLength2,");
        //                        sb.Append("PWidth1,PWidth2,PLossSqft,PCatagoryCode)");
        //                        sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
        //                        sb.Append("'" + Dts[0].Trim() + "',");
        //                        sb.Append("'" + Convert.ToDouble(Dts[1].Trim()) + "',");
        //                        sb.Append("'" + Convert.ToDouble(Dts[2].Trim()) + "',");
        //                        sb.Append("'" + Convert.ToDouble(Dts[3].Trim()) + "',");
        //                        sb.Append("'" + Convert.ToDouble(Dts[4].Trim()) + "',");
        //                        sb.Append("'0','0','0','0','0','0','0','0','0','0','0')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
        //                        sb.Append(" values('" + Stage4Req.PCCode.Trim() + "','" + Dts[0].ToString().Trim() + "',");
        //                        sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','" + Stage4Req.PCCode.Trim() + "',0)");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }
        //                    //Eng
        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo, PFBBOTSerialNo,TrfCode, Status,QPCStatus, RWStatus ) ");
        //                    sb.Append("Values('" + PrcNo.Trim() + "','1','" + Stage4Req.EngPartCode.Trim() + "',");
        //                    sb.Append("'" + Stage4Req.EngSrNo.Trim() + "','" + Stage4Req.EngSrNo.Trim() + "','" + Stage4Req.JobCardCode.Trim() + "','P','OK','OK')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("Update JobCard2DetailsSub set PrcStatus='D' where  SerialNo='" + Stage4Req.EngSrNo.Trim() + "' and JobCode='" + Stage4Req.JobCardCode + "' ");
        //                    sb.Append("and SrNoPartCode='" + Stage4Req.EngPartCode.Trim() + "' and Partcode='" + Stage4Req.ProductCode + "'");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();
        //                    //Eng
        //                    //Alt
        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo, PFBBOTSerialNo,TrfCode, Status,QPCStatus, RWStatus ) ");
        //                    sb.Append("Values('" + PrcNo.Trim() + "','1','" + Stage4Req.AltPartcode.Trim() + "',");
        //                    sb.Append("'" + Stage4Req.AltSrno.Trim() + "','" + Stage4Req.AltSrno.Trim() + "','" + Stage4Req.JobCardCode.Trim() + "','P','OK','OK')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("Update JobCard2DetailsSub set PrcStatus='D' where  SerialNo='" + Stage4Req.AltSrno.Trim() + "' and JobCode='" + Stage4Req.JobCardCode + "' ");
        //                    sb.Append("and SrNoPartCode='" + Stage4Req.AltPartcode.Trim() + "' and Partcode='" + Stage4Req.ProductCode + "'");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();
        //                    //Alt

        //                    //Cpy
        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo, PFBBOTSerialNo,TrfCode, Status,QPCStatus, RWStatus ) ");
        //                    sb.Append("Values('" + PrcNo.Trim() + "','1','" + Stage4Req.CpyPartcode.Trim() + "',");
        //                    sb.Append("'" + Stage4Req.CpySrno.Trim() + "','" + Stage4Req.CpySrno.Trim() + "','" + Stage4Req.JobCardCode.Trim() + "','P','OK','OK')");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("Update JobCard2DetailsSub set PrcStatus='D' where  SerialNo='" + Stage4Req.CpySrno.Trim() + "' and JobCode='" + Stage4Req.JobCardCode + "' ");
        //                    sb.Append("and SrNoPartCode='" + Stage4Req.CpyPartcode.Trim() + "' and Partcode='" + Stage4Req.ProductCode + "'");
        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();
        //                    //Cpy
        //                    //CP

        //                    if (double.Parse(CPType[1].Trim()) != 0 && double.Parse(CPType[1].Trim()) != 150)
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,StockQty,");
        //                        sb.Append("PFBRate,PLength,PWidth,PThickness,PLossWt,PHeight,PLength1,PLength2,");
        //                        sb.Append("PWidth1,PWidth2,PLossSqft,PCatagoryCode)");
        //                        sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
        //                        sb.Append("'" + Stage4Req.CPPartcode.Trim() + "',");
        //                        sb.Append("'1',");
        //                        sb.Append("'1',");
        //                        sb.Append("'1',");
        //                        sb.Append("'" + double.Parse(StrCPRate.Trim()) + "',");
        //                        sb.Append("'0','0','0','0','0','0','0','0','0','0','0')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
        //                        sb.Append(" values('" + Stage4Req.PCCode.Trim() + "','" + Stage4Req.CPPartcode.Trim() + "',");
        //                        sb.Append("'" + PrcNo.Trim() + "',GetDate(),'1','" + Stage4Req.PCCode.Trim() + "',0)");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo, PFBBOTSerialNo,TrfCode, Status,QPCStatus, RWStatus ) ");
        //                        sb.Append("Values('" + PrcNo.Trim() + "','1','" + Stage4Req.CPPartcode.Trim() + "',");
        //                        sb.Append("'" + Stage4Req.CPSrno.Trim() + "','" + Stage4Req.CPSrno.Trim() + "','" + Stage4Req.JobCardCode.Trim() + "','P','OK','OK')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCard2DetailsSub set PrcStatus='D' where  SerialNo='" + Stage4Req.CPSrno.Trim() + "' and JobCode='" + Stage4Req.JobCardCode + "' ");
        //                        sb.Append("and SrNoPartCode='" + Stage4Req.CPPartcode.Trim() + "' and Partcode='" + Stage4Req.ProductCode + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                    }

        //                    //CP

        //                    //CP2
        //                    if (Stage4Req.CP2Srno.Trim() != "0")
        //                    {
        //                        if (double.Parse(CPType[2].Trim()) > 1)
        //                        {
        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,StockQty,");
        //                            sb.Append("PFBRate,PLength,PWidth,PThickness,PLossWt,PHeight,PLength1,PLength2,");
        //                            sb.Append("PWidth1,PWidth2,PLossSqft,PCatagoryCode)");
        //                            sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
        //                            sb.Append("'" + Stage4Req.CP2Partcode.Trim() + "',");
        //                            sb.Append("'1',");
        //                            sb.Append("'1',");
        //                            sb.Append("'1',");
        //                            sb.Append("'" + double.Parse(StrCP2Rate.Trim()) + "',");
        //                            sb.Append("'0','0','0','0','0','0','0','0','0','0','0')");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();

        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
        //                            sb.Append(" values('" + Stage4Req.PCCode.Trim() + "','" + Stage4Req.CP2Partcode.Trim() + "',");
        //                            sb.Append("'" + PrcNo.Trim() + "',GetDate(),'1','" + Stage4Req.PCCode.Trim() + "',0)");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();

        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo, PFBBOTSerialNo,TrfCode, Status,QPCStatus, RWStatus ) ");
        //                            sb.Append("Values('" + PrcNo.Trim() + "','1','" + Stage4Req.CP2Partcode.Trim() + "',");
        //                            sb.Append("'" + Stage4Req.CP2Srno.Trim() + "','" + Stage4Req.CP2Srno.Trim() + "','" + Stage4Req.JobCardCode.Trim() + "','P','OK','OK')");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();

        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("Update JobCard2DetailsSub set PrcStatus='D' where  SerialNo='" + Stage4Req.CP2Srno.Trim() + "' and JobCode='" + Stage4Req.JobCardCode + "' ");
        //                            sb.Append("and SrNoPartCode='" + Stage4Req.CP2Partcode.Trim() + "' and Partcode='" + Stage4Req.ProductCode + "'");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();
        //                        }

        //                    }
        //                    //CP2
        //                    //Bat
        //                    if (Stage4Req.BatSrno.Trim() != "0")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo, PFBBOTSerialNo,TrfCode, Status,QPCStatus, RWStatus ) ");
        //                        sb.Append("Values('" + PrcNo.Trim() + "','1','" + Stage4Req.BatPartcode.Trim() + "',");
        //                        sb.Append("'" + Stage4Req.BatSrno.Trim() + "','" + Stage4Req.BatSrno.Trim() + "','" + Stage4Req.JobCardCode.Trim() + "','P','OK','OK')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCard2DetailsSub set PrcStatus='D' where  SerialNo='" + Stage4Req.BatSrno.Trim() + "' and JobCode='" + Stage4Req.JobCardCode + "' ");
        //                        sb.Append("and SrNoPartCode='" + Stage4Req.BatPartcode.Trim() + "' and Partcode='" + Stage4Req.ProductCode + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }
        //                    //Bat

        //                    if (Stage4Req.Bat2Srno.Trim() != "0")
        //                    {
        //                        if (double.Parse(strkVA) > 160)
        //                        {
        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo, PFBBOTSerialNo,TrfCode, Status,QPCStatus, RWStatus ) ");
        //                            sb.Append("Values('" + PrcNo.Trim() + "','1','" + Stage4Req.Bat2Partcode.Trim() + "',");
        //                            sb.Append("'" + Stage4Req.Bat2Srno.Trim() + "','" + Stage4Req.Bat2Srno.Trim() + "','" + Stage4Req.JobCardCode.Trim() + "','P','OK','OK')");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();

        //                            sb.Remove(0, sb.Length);
        //                            sb.Append("Update JobCard2DetailsSub set PrcStatus='D' where  SerialNo='" + Stage4Req.Bat2Srno.Trim() + "' and JobCode='" + Stage4Req.JobCardCode + "' ");
        //                            sb.Append("and SrNoPartCode='" + Stage4Req.Bat2Partcode.Trim() + "' and Partcode='" + Stage4Req.ProductCode + "'");
        //                            cmd = new SqlCommand(sb.ToString(), con);
        //                            cmd.Transaction = tran;
        //                            cmd.ExecuteNonQuery();
        //                            cmd.Dispose();
        //                        }
        //                    }
        //                    if (Stage4Req.KRMSrno.Trim() != "0")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo, PFBBOTSerialNo,TrfCode, Status,QPCStatus, RWStatus ) ");
        //                        sb.Append("Values('" + PrcNo.Trim() + "','1','" + Stage4Req.KRMPartcode.Trim() + "',");
        //                        sb.Append("'" + Stage4Req.KRMSrno.Trim() + "','" + Stage4Req.KRMSrno.Trim() + "','" + Stage4Req.JobCardCode.Trim() + "','P','OK','OK')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();

        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("Update JobCard2DetailsSub set PrcStatus='D' where  SerialNo='" + Stage4Req.KRMSrno.Trim() + "' and JobCode='" + Stage4Req.JobCardCode + "' ");
        //                        sb.Append("and SrNoPartCode='" + Stage4Req.KRMPartcode.Trim() + "' and Partcode='" + Stage4Req.ProductCode + "'");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }
        //                    tran.Commit();                   
        //                    PrcNo = "Process Started For ProcessCode=" + PrcNo + " and DG SrNo=" + DGNo.Trim();
        //                }
        //                catch (Exception ex)
        //                {
        //                    tran.Rollback();
        //                    return ("StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString());

        //                }
        //                finally
        //                {
        //                    con.Close();
        //                }

        //            }
        //            else if (Stage4Req.Remark == "End")
        //            {
        //                try
        //                {
        //                    if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
        //                    tran = con.BeginTransaction();
        //                    SqlCommand cmd = new SqlCommand();

        //                    string chkAlready = ComCon.getName("select isnull(sum(ReceivedQty),0) as PrcQty from StockWIP where StageName='0' and StockType='0' and ReceivedCode='" + Stage4Req.PfbCode.Trim() + "' and ReceivedQty>0 ", "tbl_StockWIP", "PrcQty");
        //                    if (chkAlready.Trim() == "0")
        //                    {
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
        //                        sb.Append(" values('" + Stage4Req.PCCode.Trim() + "','" + Stage4Req.ProductCode.Trim() + "',");
        //                        sb.Append("'" + Stage4Req.PfbCode.Trim() + "',GetDate(),'1','" + Stage4Req.PCCode.Trim() + "',0,'" + CPType[1].Trim() + "','0')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }

        //                    int recCount1 = ComCon.CountChars(Stage4Req.PrcChkDts, ",");
        //                    string[] strPrcChkDts = Regex.Split(Stage4Req.PrcChkDts, ",");
        //                    int SrNo = 0;
        //                    for (int cSub1 = 0; cSub1 <= recCount1; cSub1++)
        //                    {
        //                        SrNo += 1;
        //                        string[] PrcChkDts = Regex.Split(strPrcChkDts[cSub1].ToString().Trim(), "-->");
        //                        string GetDGStartTime = "";

        //                        GetDGStartTime = ComCon.getTranName("Select convert(nvarchar(20),Dt,120) as Stage4StartDate from ProcessFeedBack Where PFBCode='" + Stage4Req.PfbCode.Trim() + "' ", "TblProcess", "Stage4StartDate", con, tran);
        //                        sb.Remove(0, sb.Length);
        //                        sb.Append("insert into PrcChkDetails(TransCode,Dt,MainSerialNo,PrcName,ChkPointId,PrcChkPoints,PrcStatus,DGStartTime,QA6M)");
        //                        sb.Append("values('" + Stage4Req.PfbCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
        //                        sb.Append("'" + Stage4Req.EngSrNo.Trim() + "','DG Stage4',");
        //                        sb.Append("'" + PrcChkDts[0].Trim() + "','" + PrcChkDts[1].Trim() + "',");
        //                        sb.Append("'" + Stage4Req.PrcStatus + "',");
        //                        sb.Append("'" + GetDGStartTime + "','" + Stage4Req.QA6M + "')");
        //                        cmd = new SqlCommand(sb.ToString(), con);
        //                        cmd.Transaction = tran;
        //                        cmd.ExecuteNonQuery();
        //                        cmd.Dispose();
        //                    }

        //                    sb.Remove(0, sb.Length);
        //                    sb.Append("Update ProcessFeedBack set ");
        //                    if (Stage4Req.PrcStatus == "Accepted(OK)")
        //                    {
        //                        sb.Append(" EDt=GetDate() ");
        //                    }
        //                    else if (Stage4Req.PrcStatus == "Rework" || Stage4Req.PrcStatus == "Rejected")
        //                    {
        //                        sb.Append(" EDt=Null ");
        //                    }
        //                    sb.Append(" where PFbCode='" + Stage4Req.JobCardCode + "' ");

        //                    cmd = new SqlCommand(sb.ToString(), con);
        //                    cmd.Transaction = tran;
        //                    cmd.ExecuteNonQuery();
        //                    cmd.Dispose();

        //                    tran.Commit();                   
        //                    if (Stage4Req.PrcStatus == "Accepted(OK)")
        //                    {
        //                        PrcNo = "Process Ended For ProcessCode=" + Stage4Req.JobCardCode;
        //                    }
        //                    else if (Stage4Req.PrcStatus == "Rework" || Stage4Req.PrcStatus == "Rejected")
        //                    {
        //                        PrcNo = "Process " + Stage4Req.PrcStatus + "  For ProcessCode=" + Stage4Req.JobCardCode;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    tran.Rollback();
        //                    return ("StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString());
        //                }
        //                finally
        //                {
        //                    con.Close();
        //                }

        //            }
        //            return PrcNo;
        //        }

        //        public string GetmaxPrc(string tablename, string fieldname, string Yr, string CompCode, SqlConnection con, SqlTransaction tran)
        //        {

        //            string PrcCode = "";
        //            string Max = "0";
        //            SqlCommand cmd = new SqlCommand("select max(substring(" + fieldname + ",13,7)) as MX from " + tablename.Trim() + " where yr='" + Yr.Trim() + "' and CompanyCode='" + CompCode.Trim() + "'", con, tran);
        //            cmd.CommandTimeout = 0;
        //            cmd.Transaction = tran;
        //            if ((cmd.ExecuteScalar().ToString() == System.DBNull.Value.ToString()))
        //            {
        //                Max = (CompCode + "000001");
        //            }
        //            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 9))
        //            {
        //                Max = (CompCode + ("00000" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
        //            }
        //            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 99))
        //            {
        //                Max = (CompCode + ("0000" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
        //            }
        //            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 999))
        //            {
        //                Max = (CompCode + ("000" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
        //            }
        //            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 9999))
        //            {
        //                Max = (CompCode + ("00" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
        //            }
        //            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 99999))
        //            {
        //                Max = (CompCode + ("0" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
        //            }
        //            else
        //            {
        //                Max = (CompCode + (Convert.ToInt32(cmd.ExecuteScalar()) + 1));
        //            }
        //            PrcCode = "PSH" + "/" + Yr + "/" + Max;

        //            cmd.Dispose();
        //            return PrcCode;
        //        }
    }

}