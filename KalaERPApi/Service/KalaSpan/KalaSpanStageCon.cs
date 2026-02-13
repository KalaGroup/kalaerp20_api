using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.Http;
using System.Text.RegularExpressions;
using KalaERPApi.Models.KalaSpan.Trans;
using System.IO;

namespace KalaERPApi.Service.KalaSpan.Trans
{
    public class KalaSpanStageCon
    {
        #region
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        StringBuilder sb = new StringBuilder();
        DataSet dsChkStage = new DataSet();
        CommonCon ComCon = new CommonCon();
        string strSql = "";
        public SqlTransaction tran = null;
        SqlCommand cmd;
        #endregion
              
        public DataTable GetScanDts(string strSrNo, string strPartCode, string strCat, string strStage)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetKalaSpanStageScanDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@strSrNo", SqlDbType.Char).Value = strSrNo;
            dAd.SelectCommand.Parameters.Add("@strPartCode", SqlDbType.Char).Value = strPartCode;
            dAd.SelectCommand.Parameters.Add("@strCat", SqlDbType.Char).Value = strCat;
            dAd.SelectCommand.Parameters.Add("@strStage", SqlDbType.Char).Value = strStage;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetProdDts(string strDGSrNo, string StrPartcode, string StrPfbCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetTRProdDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Partcode", SqlDbType.Char).Value = strDGSrNo;
            dAd.SelectCommand.Parameters.Add("@DGSrNo", SqlDbType.Char).Value = StrPartcode;
            dAd.SelectCommand.Parameters.Add("@PFBCode", SqlDbType.Char).Value = StrPfbCode;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public Boolean ChkStage(int StageNo, string jobcard, string VehSrNo)
        {
            Boolean strChkStage = false;
            CommonCon ComCon = new CommonCon();
            if (StageNo == 0)
            {
                dsChkStage = ComCon.procDS("Select  isNull(Count(SerialNo),0)  as CntEngSRNo From JobcardDetailsSubDefence where Jobcode='" + jobcard + "' and SerialNo='" + VehSrNo + "'  and Stage1StartStatus='D' ", "tbl_ChkStgDone");
            }
            else if (StageNo == 1)
            {
                dsChkStage = ComCon.procDS("Select  isNull(Count(SerialNo),0)  as CntEngSRNo From JobcardDetailsSubDefence where Jobcode='" + jobcard + "' and SerialNo='" + VehSrNo + "'  and Stage1Status='D' ", "tbl_ChkStgDone");
            }
            else if (StageNo == 2)
            {
                dsChkStage = ComCon.procDS("Select  isNull(Count(SerialNo),0)  as CntEngSRNo From JobcardDetailsSubDefence where Jobcode='" + jobcard + "' and SerialNo='" + VehSrNo + "'  and Stage2Status='D' ", "tbl_ChkStgDone");
            }
            else if (StageNo == 3)
            {
                dsChkStage = ComCon.procDS("Select  isNull(Count(SerialNo),0)  as CntEngSRNo From JobcardDetailsSubDefence where Jobcode='" + jobcard + "' and SerialNo='" + VehSrNo + "'  and Stage3Status='D' ", "tbl_ChkStgDone");
            }
            else if (StageNo == 4)
            {
                dsChkStage = ComCon.procDS("Select  isNull(Count(SerialNo),0)  as CntEngSRNo From JobcardDetailsSubDefence where Jobcode='" + jobcard + "' and SerialNo='" + VehSrNo + "'  and Stage4Status='D' ", "tbl_ChkStgDone");
            }
            else if (StageNo == 5)
            {
                dsChkStage = ComCon.procDS("Select  isNull(Count(SerialNo),0)  as CntEngSRNo From JobcardDetailsSubDefence where Jobcode='" + jobcard + "' and SerialNo='" + VehSrNo + "'  and Stage5Status='D' ", "tbl_ChkStgDone");
            }
            else if (StageNo == 6)
            {
                dsChkStage = ComCon.procDS("Select  isNull(Count(SerialNo),0)  as CntEngSRNo From JobcardDetailsSubDefence where Jobcode='" + jobcard + "' and SerialNo='" + VehSrNo + "'  and Stage6Status='D' ", "tbl_ChkStgDone");
            }

            else if (StageNo == 7)
            {
                dsChkStage = ComCon.procDS("Select  isNull(Count(Trcode),0)  as CntEngSRNo From testreport  where processcode='" + jobcard + "' and active='1'", "tbl_ChkStgDone");
            }
            else if (StageNo == 8)
            {
                dsChkStage = ComCon.procDS("Select  isNull(Count(Trcode),0)  as CntEngSRNo From testreport  where processcode='" + jobcard + "' and active='1' and TrEndtime is not NULL", "tbl_ChkStgDone");
            }
            else if (StageNo == 9)
            {
                dsChkStage = ComCon.procDS("Select  isNull(Count(Trcode),0)  as CntEngSRNo From testreport  where processcode='" + jobcard + "' and active='1' and DGStartTime is not NULL", "tbl_ChkStgDone");
            }
            else if (StageNo == 10)
            {
                dsChkStage = ComCon.procDS("Select  isNull(Count(Trcode),0)  as CntEngSRNo From testreport  where processcode='" + jobcard + "' and active='1' and DGEndtime is not NULL", "tbl_ChkStgDone");
            }

            if (dsChkStage != null && dsChkStage.Tables["tbl_ChkStgDone"].Rows.Count == 1)
            {
                if (int.Parse(dsChkStage.Tables["tbl_ChkStgDone"].Rows[0]["CntEngSRNo"].ToString().Trim()) > 0)

                {
                    strChkStage = true;
                }
                else
                {
                    strChkStage = false;
                }
            }
            else
            {
                strChkStage = false;
            }
            return strChkStage;

        }

        public string Submit(string JBCode, int StageNo, string ProductCode, string VehPartCode,
            string VehSrNo, string AltPartcode, string AltSrno, string CpyPartcode, string CpySrno,
            string BatPartcode, string BatSrno, string Bat2Partcode, string Bat2Srno, string QA6M,
            string PrcStatus, string PrcChkDts, string PrcDts, string TeamName)
        {
            string strkVA = "";
            strkVA = ComCon.getName("SELECT KVA FROM Part WHERE PartCode='" + ProductCode.Trim() + "'", "tblPart", "KVA");

            if (ChkStage(StageNo, JBCode, VehSrNo) == true)
            {
                return "This Stage for Jobcard " + JBCode.Trim() + " With Eng Serial No " + VehSrNo + " alredy Completed!";
            }

            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                SqlCommand cmd = new SqlCommand();
                int recCount1 = ComCon.CountChars(PrcChkDts, ",");
                string[] strPrcChkDts = Regex.Split(PrcChkDts, ",");
                int SrNo = 0;

                if (StageNo == 0)//S1 Start
                {
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "',");
                    sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageI')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("Update JobcardDetailsSubDefence set Stage1StartStatus='D',stage1StartDate=GetDate(),Team='" + TeamName.Trim() + "' where ");
                    sb.Append("JobCode='" + JBCode.Trim() + "' and Serialno='" + VehSrNo.Trim() + "' and SrNoPartcode='" + VehPartCode.Trim() + "' and Partcode='" + ProductCode.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + VehPartCode.Trim() + "',");
                    sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageI')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    int recCount = ComCon.CountChars(PrcDts, ",");
                    string[] strPrcDts = Regex.Split(PrcDts, ",");

                    for (int cSub = 0; cSub <= recCount; cSub++)
                    {
                        SrNo += 1;
                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");

                        sb.Remove(0, sb.Length);
                        sb.Append("insert into jobcardaccessoriesdefence(JobCode,SrNo,VehicleNo,AccID,StandardQty,RemovalQty,AssembleQty)");
                        sb.Append("values('" + JBCode.Trim() + "','" + SrNo + "','" + VehSrNo + "',");
                        sb.Append("'" + Dts[0].Trim() + "',");
                        sb.Append("'" + Convert.ToInt32(Dts[1].Trim()) + "',");
                        sb.Append("'" + Convert.ToInt32(Dts[2].Trim()) + "',");
                        sb.Append("'" + Convert.ToInt32(Dts[3].Trim()) + "')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                else if (StageNo == 1) //S1 End
                {
                    #region
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "',");
                    sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageI')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "',");
                    sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageII')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("Update JobcardDetailsSubDefence set Stage1Status='D',stage1Date=GetDate() where ");
                    sb.Append("JobCode='" + JBCode.Trim() + "' and Serialno='" + VehSrNo.Trim() + "' and ");
                    sb.Append("SrNoPartcode='" + VehPartCode.Trim() + "' and Partcode='" + ProductCode.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    #endregion
                }
                else if (StageNo == 2) //S2 
                {
                    #region
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "',");
                    sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageII')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "',");
                    sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageIII')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("Update JobcardDetailsSubDefence set Stage2Status='D',stage2Date=GetDate() where ");
                    sb.Append("JobCode='" + JBCode.Trim() + "' and Serialno='" + VehSrNo.Trim() + "' ");
                    sb.Append("and SrNoPartcode='" + VehPartCode.Trim() + "' and Partcode='" + ProductCode.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    #endregion
                }
                else if (StageNo == 3) //S3 
                {
                    #region
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "',");
                    sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageIII')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "',");
                    sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageIV')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Update Stage
                    sb.Remove(0, sb.Length);
                    sb.Append("Update JobcardDetailsSubDefence set Stage3Status='D',stage3Date=GetDate() where ");
                    sb.Append("JobCode='" + JBCode.Trim() + "' and Serialno='" + VehSrNo.Trim() + "' ");
                    sb.Append("and SrNoPartcode='" + VehPartCode.Trim() + "' and Partcode='" + ProductCode.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    string JPlanDtls = ComCon.getTranName("select cast(JPriority as varchar)+'-->'+Team as Dtls from JobcardDetailsSubDefence " +
                    "where JobCode='" + JBCode.Trim() + "' and SerialNo='" + VehSrNo.Trim() + "'", "JobcardDetailsSubDefence", "Dtls", con, tran);
                    string[] JPlanDtls_Items = Regex.Split(JPlanDtls, "-->");
                    if (JPlanDtls_Items.Length > 1)
                    {
                        //Insert Hydraulik Pump No, TeamName having Hydraulik Pump No 
                        sb.Remove(0, sb.Length);
                        sb.Append("Insert InTo JobcardDetailsSubDefence(JobCode,SrNo,PartCode,SrNoPartCode,SerialNo,TransferCode,JPriority,Team,");
                        sb.Append("Stage1StartStatus,Stage1Status,Stage2Status,Stage3Status,Stage4Status,Stage5Status,Stage6Status,Stage7Status)");
                        sb.Append("Values('" + JBCode.Trim() + "','6','1032185000000000000','0041010113130302012','" + TeamName.Trim() + "','0',");
                        sb.Append("'" + JPlanDtls_Items[0].Trim() + "','" + JPlanDtls_Items[1].Trim() + "','D','D','D','D','D','D','D','D')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    #endregion
                }
                else if (StageNo == 4) //S4
                {
                    #region
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "',");
                    sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageIV')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "',");
                    sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageV')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("Update JobcardDetailsSubDefence set Stage4Status='D',stage4Date=GetDate() ");
                    sb.Append("where JobCode='" + JBCode.Trim() + "' and Serialno='" + VehSrNo.Trim() + "' ");
                    sb.Append("and SrNoPartcode='" + VehPartCode.Trim() + "' and Partcode='" + ProductCode.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    #endregion
                }
                else if (StageNo == 5) //S5 
                {
                    #region
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "','" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageV')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "','" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageVI')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("Update JobcardDetailsSubDefence set Stage5Status='D',stage5Date=GetDate() where ");
                    sb.Append("JobCode='" + JBCode.Trim() + "' and Serialno='" + VehSrNo.Trim() + "' ");
                    sb.Append("and SrNoPartcode='" + VehPartCode.Trim() + "' and Partcode='" + ProductCode.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    #endregion
                }
                else if (StageNo == 6) //S6 
                {
                    #region
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "',");
                    sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageVI')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "',");
                    sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageVII')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("Update JobcardDetailsSubDefence set Stage6Status='D',stage6Date=GetDate() ");
                    sb.Append("where JobCode='" + JBCode.Trim() + "' and Serialno='" + VehSrNo.Trim() + "' ");
                    sb.Append("and SrNoPartcode='" + VehPartCode.Trim() + "' and Partcode='" + ProductCode.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    if (AltPartcode.Trim().Substring(0, 8) == "00003030") // Crane
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update JobcardDetailsSubDefence set Serialno='" + AltSrno.Trim() + "',TransferCode='" + BatSrno.Trim() + "',");
                        sb.Append("Stage6Status='D',stage6Date=GetDate() where JobCode='" + JBCode.Trim() + "' and ");
                        sb.Append("jPriority='" + BatPartcode.Trim() + "' and SrNoPartcode='" + AltPartcode.Trim() + "' and ");
                        sb.Append("Partcode='" + ProductCode.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                        sb.Append(" values('20.005','" + AltPartcode.Trim() + "',");
                        sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageVI')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("Update Giirdetailssub set Jobcardstatus='J' where  Partcode='" + AltPartcode.Trim() + "' and  SerialNo='" + AltSrno.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    if (CpyPartcode.Trim().Substring(0, 3) == "004") // Tank
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update JobcardDetailsSubDefence set Serialno='" + CpySrno.Trim() + "',TransferCode='" + BatSrno.Trim() + "',");
                        sb.Append("Stage4Status='D',stage4Date=GetDate() where JobCode='" + JBCode.Trim() + "' and ");
                        sb.Append("Jpriority='" + BatPartcode.Trim() + "' and  SrNoPartcode='" + CpyPartcode.Trim() + "' and ");
                        sb.Append("Partcode='" + ProductCode.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                        sb.Append(" values('20.005','" + CpyPartcode.Trim() + "',");
                        sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageVI')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("Update Giirdetailssub set Jobcardstatus='J' where Partcode='" + CpyPartcode.Trim() + "' and  SerialNo='" + CpySrno.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    #endregion
                }
                else if (StageNo == 7) //S7
                {
                    string PrcNo = "";
                    int recCountV = ComCon.CountChars(PrcDts, ",");
                    string[] strJCDDtsV = Regex.Split(PrcDts, ",");
                    int SrNoV = 0;
                    for (int cSubV = 0; cSubV <= recCountV; cSubV++)
                    {
                        SrNoV += 1;
                        string[] DtsV = Regex.Split(strJCDDtsV[cSubV].ToString().Trim(), "-->");

                        if (double.Parse(DtsV[5].Trim()) < 0)
                        {
                            PrcNo = "Insufficient Stock For Part= " + DtsV[0].Trim();
                            return PrcNo;
                        }
                    }

                    #region
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "',");
                    sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'1','20.005',0,'0','StageVII')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    if (VehPartCode.Trim().Substring(0, 8) == "00002200")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update JobcardDetailsSubDefence set Stage7Status='D',stage7Date=GetDate() ");
                        sb.Append("where JobCode='" + JBCode.Trim() + "' and Serialno='" + VehSrNo.Trim() + "' ");
                        sb.Append("and SrNoPartcode='" + VehPartCode.Trim() + "' and Partcode='" + ProductCode.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    if (AltPartcode.Trim().Substring(0, 8) == "00003030")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update JobcardDetailsSubDefence set Stage7Status='D',stage7Date=GetDate() ");
                        sb.Append("where JobCode='" + JBCode.Trim() + "' and Serialno='" + AltSrno.Trim() + "' ");
                        sb.Append("and SrNoPartcode='" + AltPartcode.Trim() + "' and Partcode='" + ProductCode.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    if (CpyPartcode.Trim().Substring(0, 3) == "004")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update JobcardDetailsSubDefence set Stage7Status='D',stage7Date=GetDate() ");
                        sb.Append("where JobCode='" + JBCode.Trim() + "' and Serialno='" + CpySrno.Trim() + "' ");
                        sb.Append("and SrNoPartcode='" + CpyPartcode.Trim() + "' and Partcode='" + ProductCode.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    //Fire Ext1
                    if (BatPartcode.Trim().Length > 2)
                    {
                        if (BatPartcode.Trim().Substring(0, 8) == "00002230")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Update JobcardDetailsSubDefence set Serialno='" + BatSrno.Trim() + "',");
                            sb.Append("TransferCode='" + Bat2Srno.Trim() + "',Stage7Status='D',stage7Date=GetDate() where ");
                            sb.Append("JobCode='" + JBCode.Trim() + "' and JPriority='" + Bat2Partcode.Trim() + "' ");
                            sb.Append("and SrNoPartcode='" + BatPartcode.Trim() + "' and Partcode='" + ProductCode.Trim() + "' and Serialno='FX1'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            sb.Append("Update Giirdetailssub set Jobcardstatus='J' where Partcode='" + BatPartcode.Trim() + "' and SerialNo='" + BatSrno.Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }

                    //Fire Ext2
                    if (QA6M.Trim().Length > 2)
                    {
                        if (BatPartcode.Trim().Substring(0, 8) == "00002230")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("Update JobcardDetailsSubDefence set Serialno='" + QA6M.Trim() + "',");
                            sb.Append("TransferCode='" + TeamName.Trim() + "',Stage7Status='D',stage7Date=GetDate() where ");
                            sb.Append("JobCode='" + JBCode.Trim() + "' and JPriority='" + Bat2Partcode.Trim() + "' ");
                            sb.Append("and SrNoPartcode='" + BatPartcode.Trim() + "' and Partcode='" + ProductCode.Trim() + "' and Serialno='FX2'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            sb.Append("Update Giirdetailssub set Jobcardstatus='J' where Partcode='" + BatPartcode.Trim() + "' and SerialNo='" + QA6M.Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }

                    if (BatPartcode.Trim().Length > 2)
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                        sb.Append(" values('20.005','" + BatPartcode.Trim() + "',");
                        sb.Append("'" + JBCode + "-->" + VehSrNo + "',GetDate(),'2','20.005',0,'0','StageVII')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    #endregion

                    // Process & TR Start
                    #region
                    string DGNo = ComCon.getTranName("select cast(year(getdate()) as char(4))+'.'+RIGHT('0'+ RTRIM(MONTH(getdate())), 2)+'.'+RIGHT('000'+ RTRIM((isnull(max(SerialNo),0)+1)), 4) as DGNo " +
                     " from processfeedback where companycode='20' and profitcentercode='20.005' and yr='" + ComCon.yearEnd(con, tran) + "'", "processfeedback", "DGNo", con, tran);
                    if (DGNo == "0")
                    {
                        PrcNo = "DG Serial No Creation Problem";
                        return PrcNo;
                    }
                    PrcNo = GetmaxPrc("ProcessFeedback", "PFbCode", ComCon.yearEnd(con, tran), "20", con, tran);
                    string StrBOMCode = ComCon.getName("select BOMCode From BOM where Partcode='" + ProductCode.Trim() + "' and Active='1' and Discard='1' ", "TblBOM", "BOMCode");

                    sb.Remove(0, sb.Length);
                    sb.Append("insert into processfeedback(GroupPFBCode,PFBCode,MaxSrNo,Dt,Yr,MachineCode,SerialNo,ProfitCenterCode,TurretKitCode,");
                    sb.Append("PartCode,PPWCode,MOFCode,VersionCode,ProcessQty,PKitQty,PLength,PWidth,PThickness, NstWtPerUt,NstSqftPerUt,WtPerUt,SqftPerUt,CRWt,HRWt,CRRate,HRRate ,CompanyCode,");
                    sb.Append("PPDIRStatus,TRStatus,PFBType,PFBRate,SilCladdingRate,Remark,PrcBOMCode,QPCStatus)");
                    sb.Append(" values('" + PrcNo.Trim() + "','" + PrcNo.Trim() + "','" + (PrcNo.Substring(10, 8)) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ");
                    sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + DGNo.Trim() + "','" + DGNo.Substring(8, 4).Trim() + "','20.005','0','" + ProductCode.Trim() + "','0', ");
                    sb.Append("'MOF','0','1','0','0','0','0', '0', '0', ");
                    sb.Append("'0', '0', '0', '0', '0', '0', '20','P','P',");
                    sb.Append("'','0','0','OK','" + StrBOMCode + "','D')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    int recCount = ComCon.CountChars(PrcDts, ",");
                    string[] strPrcDts = Regex.Split(PrcDts, ",");
                    for (int cSub = 0; cSub <= recCount; cSub++)
                    {
                        SrNo += 1;
                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");

                        sb.Remove(0, sb.Length);
                        sb.Append("insert into processfeedbackdetails(PFBCode,SrNo,PartCode,KITQty,TotQty,StockQty,");
                        sb.Append("PFBRate,PLength,PWidth,PThickness,PLossWt,PHeight,PLength1,PLength2,");
                        sb.Append("PWidth1,PWidth2,PLossSqft,PCatagoryCode)");
                        sb.Append("values('" + PrcNo.Trim() + "','" + SrNo + "',");
                        sb.Append("'" + Dts[0].Trim() + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[1].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[2].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[3].Trim()) + "',");
                        sb.Append("'" + Convert.ToDouble(Dts[4].Trim()) + "',");
                        sb.Append("'0','0','0','0','0','0','0','0','0','0','0')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                        sb.Append(" values('20.005','" + Dts[0].ToString().Trim() + "',");
                        sb.Append("'" + PrcNo.Trim() + "',GetDate(),'" + double.Parse(Dts[2].ToString().Trim()) + "','20.005',0)");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    //Truck
                    sb.Remove(0, sb.Length);
                    sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo, PFBBOTSerialNo,TrfCode, Status,QPCStatus, RWStatus ) ");
                    sb.Append("Values('" + PrcNo.Trim() + "','1','" + VehPartCode.Trim() + "',");
                    sb.Append("'" + VehSrNo.Trim() + "','" + VehSrNo.Trim() + "','" + JBCode.Trim() + "','P','OK','OK')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Crain
                    sb.Remove(0, sb.Length);
                    sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo, PFBBOTSerialNo,TrfCode, Status,QPCStatus, RWStatus ) ");
                    sb.Append("Values('" + PrcNo.Trim() + "','2','" + AltPartcode.Trim() + "',");
                    sb.Append("'" + AltSrno.Trim() + "','" + AltSrno.Trim() + "','" + JBCode.Trim() + "','P','OK','OK')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Oil Tank
                    sb.Remove(0, sb.Length);
                    sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo, PFBBOTSerialNo,TrfCode, Status,QPCStatus, RWStatus ) ");
                    sb.Append("Values('" + PrcNo.Trim() + "','3','" + CpyPartcode.Trim() + "',");
                    sb.Append("'" + CpySrno.Trim() + "','" + CpySrno.Trim() + "','" + JBCode.Trim() + "','P','OK','OK')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Fire Ex1
                    sb.Remove(0, sb.Length);
                    sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo, PFBBOTSerialNo,TrfCode, Status,QPCStatus, RWStatus ) ");
                    sb.Append("Values('" + PrcNo.Trim() + "','4','" + BatPartcode.Trim() + "',");
                    sb.Append("'" + BatSrno.Trim() + "','" + BatSrno.Trim() + "','" + JBCode.Trim() + "','P','OK','OK')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Fire Ex2
                    if (QA6M.Trim().Length > 0)
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Insert Into ProcessFeedbackDetailsSub(PFBCode, SrNo, PartCode, SerialNo, PFBBOTSerialNo,TrfCode, Status,QPCStatus, RWStatus ) ");
                        sb.Append("Values('" + PrcNo.Trim() + "','5','" + BatPartcode.Trim() + "',");
                        sb.Append("'" + QA6M.Trim() + "','" + QA6M.Trim() + "','" + JBCode.Trim() + "','P','OK','OK')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,ReceivedCode,ReceivedDate,ReceivedQty,ToProfitCenterCode,StockType,PanelTypeId,StageName)");
                    sb.Append(" values('20.005','" + ProductCode.Trim() + "','" + PrcNo.Trim() + "',GetDate(),'1','20.005',0,'0','0')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    // TestReport Start
                    string StrTRCode = "";
                    string StrPDIRCode = "";
                    StrTRCode = ComCon.GetMaxNo("TestReport", "TRC", "20", con, tran);
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into TestReport(TRCode,Dt,Yr,MaxSrNo,ProcessCode,MachineNo,Remark,");
                    sb.Append("RevTRCode,BalQty,EngineModel,HP,KW,Speed,Alternator,RatedKVA,RatedVolt,RatedAMPS,");
                    sb.Append("Ph,PF, Frequency,AMBTemp,RY,YB,BR,VoltageRegulation,RoomTempreture,RPMRegulation,LLOP,HWT,HCT,OSD,DieselRate,PerUnitQty,PDIRStatus,CompanyCode,TRStartTime)");
                    sb.Append(" values('" + StrTRCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ComCon.yearEnd(con, tran) + "','" + (StrTRCode.Substring(10, 8)) + "', ");
                    sb.Append("'" + PrcNo.Trim() + "','" + DGNo.Trim() + "','OK',");
                    sb.Append("'0','0','0','0','0','0','0','0','0','0',");
                    sb.Append("'1','2','3','4','5','6','7','8','9','10','11','12','13','14','0','0','C','20','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    StrPDIRCode = ComCon.GetMaxNo("PDIR", "PDI", "20", con, tran);
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into PDIR(PDICode,Dt,Yr,MaxSrNo,DICode,TRCode,CLEng,CLAlt,CLBat,CLExhFan,CLCanopy,CLFuelTank,CLCP,CLGA,CLDOC,Remark,");
                    sb.Append("DISPStatus,PSStatus,CompanyCode)");
                    sb.Append(" values('" + StrPDIRCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ComCon.yearEnd(con, tran) + "','" + (StrTRCode.Substring(10, 8)) + "', ");
                    sb.Append("'0','" + StrTRCode.Trim() + "','CHK/09-10/01000010','CHK/09-10/01000009','CHK/09-10/01000002','NIL','CHK/09-10/01000004','NIL','CHK/13-14/01000006',");
                    sb.Append("'CHK/13-14/01000001','CHK/11-12/01000002','Nil','P','P','20')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("insert into TestReportSerialNoDetails(TRCode,SrNo,PartCode,SerialNo,SerialStatus,Qty,GiirCode)");
                    sb.Append("values('" + StrTRCode.Trim() + "','1','" + VehPartCode.Trim() + "',");
                    sb.Append("'" + VehSrNo.Trim() + "','D',1,'" + JBCode.Trim() + "')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Crane     
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into TestReportSerialNoDetails(TRCode,SrNo,PartCode,SerialNo,SerialStatus,Qty,GiirCode)");
                    sb.Append("values('" + StrTRCode.Trim() + "','2','" + AltPartcode.Trim() + "',");
                    sb.Append("'" + AltSrno.Trim() + "','D',1,'" + JBCode.Trim() + "')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Oil Tank   
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into TestReportSerialNoDetails(TRCode,SrNo,PartCode,SerialNo,SerialStatus,Qty,GiirCode)");
                    sb.Append("values('" + StrTRCode.Trim() + "','3',");
                    sb.Append("'" + CpyPartcode.Trim() + "',");
                    sb.Append("'" + CpySrno.Trim() + "','D',1,'" + JBCode.Trim() + "')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd.Dispose();

                    //Fire Ex1    
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into TestReportSerialNoDetails(TRCode,SrNo,PartCode,SerialNo,SerialStatus,Qty,GiirCode)");
                    sb.Append("values('" + StrTRCode.Trim() + "','4','" + BatPartcode.Trim() + "',");
                    sb.Append("'" + BatSrno.Trim() + "','D',1,'" + JBCode.Trim() + "')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Fire Ex2
                    if (QA6M.Trim().Length > 0)
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("insert into TestReportSerialNoDetails(TRCode,SrNo,PartCode,SerialNo,SerialStatus,Qty,GiirCode)");
                        sb.Append("values('" + StrTRCode.Trim() + "','5','" + BatPartcode.Trim() + "',");
                        sb.Append("'" + QA6M.Trim() + "','D',1,'" + JBCode.Trim() + "')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    sb.Remove(0, sb.Length);
                    sb.Append("Update ProcessFeedbackDetailssub set TRSerialStatus='D' where PFBCode='" + PrcNo.Trim() + "'  ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("Update ProcessFeedback set TRStatus='D' where PFBCode='" + PrcNo.Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    // TestReport End
                    #endregion
                    // Process & TR End
                }

                tran.Commit();
                return JBCode.Trim();
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream("C:/Error/ApiError.txt", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter m_streamWriter = new StreamWriter(fs);
                m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                m_streamWriter.WriteLine();
                m_streamWriter.WriteLine("***************** " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:tt") + "*****************");
                m_streamWriter.WriteLine("StackTrace " + ex.StackTrace.ToString());
                m_streamWriter.WriteLine("Message " + ex.Message.ToString());
                m_streamWriter.Flush();
                m_streamWriter.Close();

                tran.Rollback();
                return ("StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString());
            }

            finally
            {
                con.Close();
            }
        }

        public DataTable GetPrcStatus()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetTRPrcStatus", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetAccChkDts()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetAccChkDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetAccStageVChkDts(string SerialNo, string Type)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("KSpanAccQRCodes", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@VNo", SqlDbType.Char).Value = SerialNo;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable Get6M()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("Get6M", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;


            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetPrcChkDts(string strStageNo, string PrcStatusName)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetTRPrcChkDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@strStageNo", SqlDbType.Char).Value = strStageNo;
            dAd.SelectCommand.Parameters.Add("@PrcStatusName", SqlDbType.Char).Value = PrcStatusName;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetPodprcDts(string strPrdPartCode, string strPCCode)
        {
            string StrBOMCode = "";

            DataSet dSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            CommonCon ComCon = new CommonCon();

            StrBOMCode = ComCon.getName("select BOMCode+'-->'+convert(nvarchar(10),WgtHR)+'-->'+convert(nvarchar(10),WgtCR) as BOMCode From BOM where Partcode='" + strPrdPartCode.Trim() + "' and Active='1' and Discard='1' ", "TblBOM", "BOMCode");
            string[] ProdDts = Regex.Split(StrBOMCode.Trim(), "-->");
            string RatePCCode = "";
            if (strPCCode.Trim() == "01.005")
            {
                RatePCCode = "01.007','01.005','03.008";
            }
            else if (strPCCode.Trim() == "03.038")
            {
                RatePCCode = "03.001','03.038','03.008";
            }
            else if (strPCCode.Trim() == "01.004" || strPCCode.Trim() == "04.001" || strPCCode.Trim() == "14.001" || strPCCode.Trim() == "20.005")
            {
                RatePCCode = "01.005','01.004','04.001','01.064','14.001','01.023','03.040','20.005";
            }
            else if (strPCCode.Trim() == "01.023")
            {
                RatePCCode = "01.007','01.023','01.064";
            }
            else if (strPCCode.Trim() == "03.040")
            {
                RatePCCode = "01.007','01.023','03.040','01.064";
            }
            else
            {
                RatePCCode = strPCCode.Trim();
            }

            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            else
            {
                con.Open();
            }

            if (StrBOMCode.Trim() != "0")
            {
                SqlCommand cmd = new SqlCommand();
                sb.Remove(0, sb.Length);
                sb.Append("Create table #tempstockwip(FromProfitCenterCode nvarchar(50),PartCode nvarchar(50),");
                sb.Append("IssueQty float,ToProfitCenterCode nvarchar(50),ReceivedQty float,stockType Int)");
                sb.Append(" insert Into #tempstockwip(FromProfitCenterCode,PartCode,IssueQty,ToProfitCenterCode,ReceivedQty,stockType)");
                sb.Append(" select FromProfitCenterCode,PartCode,IssueQty,ToProfitCenterCode,ReceivedQty,stockType ");
                sb.Append(" from stockwip where fromprofitcentercode in ('" + strPCCode + "') and IssueQty>'0' and ");
                sb.Append(" len(ToProfitCenterCode)='6' and len(FromProfitCenterCode)='6'");
                sb.Append(" insert Into #tempstockwip(FromProfitCenterCode,PartCode,IssueQty,ToProfitCenterCode,ReceivedQty,stockType)");
                sb.Append(" select FromProfitCenterCode,PartCode,IssueQty,ToProfitCenterCode,ReceivedQty,stockType");
                sb.Append(" from stockwip where toprofitcentercode in ('" + strPCCode + "') and ReceivedQty>'0' and");
                sb.Append(" len(ToProfitCenterCode)='6' and len(FromProfitCenterCode)='6'");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                sb.Remove(0, sb.Length);
                sb.Append("Create table #TempPricelist(ProfitCenterCode nvarchar(50),PartCode nvarchar(50),PurRate Float,Rate Float,Pwt float,PSqFt float)");
                sb.Append("insert Into #TempPricelist(ProfitCenterCode,PartCode,PurRate,Rate,Pwt,PSqFt)");
                sb.Append("select ProfitCenterCode,PartCode,PurRate,Rate,Pwt,PSqFt");
                sb.Append(" from ProfitcenterPLdetails where ProfitcenterCode in ('" + RatePCCode + "')");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                sb.Remove(0, sb.Length);
                sb.Append("Create table #TempBOMdetails(BOMCode nvarchar(50),PartCode nvarchar(50),Qty Float,SuppRate Float,MOB nvarchar(50),KitCode nvarchar(50),StockQty float, TotalQty float, QAP float, Amount float,PrcCostUnitKG Float )");
                sb.Append("insert Into #TempBOMdetails(BOMCode,PartCode,Qty,SuppRate,MOB,KitCode,StockQty,TotalQty,QAP,Amount,PrcCostUnitKG)");
                sb.Append("select BOMCode,PartCode,Qty,SuppRate,MOB,KitCode,'0' as StockQty,'0' as TotalQty,'0' AS QAP,'0' AS Amount,PrcCostUnitKG from BOMdetails where BOMCode='" + ProdDts[0].Trim() + "'");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                strSql = "";
                if (strPCCode.ToString().Trim() == "01.023" || strPCCode.ToString().Trim() == "03.040")
                {
                    strSql = "select BD.PartCode,partdesc+' [' +BD.partcode +'] ' as PartDesc,u.UName,Qty,BD.Mob," +
                         "P.Kit,p.conversionvalue,p.UomCode,'0' as StockQty,'0' as TotalQty, " +
                         "SuppRate as Rate,0.00 as  QAP, P.CategoryID " +
                         "from #TempBOMdetails BD Inner Join Part P On Bd.Partcode=P.partcode " +
                         "Inner join UOM U On P.UOMCode=U.Uid " +
                         "where bd.BOMCode='" + ProdDts[0].Trim() + "' and kitcode='" + strPrdPartCode.Trim() + "' " +
                         "and Kit='1' and BD.PartCode LIKE '004%' " +
                         " Union all " +
                         "select BD2.PartCode,partdesc+' [' +BD2.partcode +'] ' as PartDesc,u.UName,Bd2.Qty,BD2.Mob," +
                         "P.Kit,p.conversionvalue,p.UomCode,'0' as StockQty,'0' as TotalQty, " +
                         "Bd2.SuppRate as Rate,0.00 as  QAP,P.CategoryID " +
                         "from #TempBOMdetails BD Inner JOin #TempBOMdetails Bd1 On Bd.BomCode=Bd1.BomCode and Bd.partcode=Bd1.KitCode " +
                         "Inner JOin #TempBOMdetails Bd2 On Bd1.BomCode=Bd2.BomCode and Bd1.partcode=Bd2.KitCode " +
                         "Inner Join Part P On Bd2.Partcode=P.partcode  " +
                         "Inner join UOM U On P.UOMCode=U.Uid " +
                         "where bd.BOMCode='" + ProdDts[0].Trim() + "' and Bd.kitcode='" + strPrdPartCode.Trim() + "' " +
                         "and BD1.PartCode LIKE '0125%' and Substring(BD1.PartCode,12,2) in ('12') " +
                         "ORDER BY PartDesc ";
                }
                else
                {

                    strSql = "select BD1.PartCode,partdesc + ' [' + BD1.partcode + '] ' as PartDesc,'' as SerialNo,u.UName,Bd1.Qty,BD1.Mob, " +
                            " P.Kit,p.conversionvalue,p.UomCode,'0' as StockQty,'0' as TotalQty, " +
                            "Bd1.SuppRate as Rate,Bd1.QAP,Bd1.Amount,P.CategoryID " +
                            "from #TempBOMdetails BD Inner JOin #TempBOMdetails Bd1 On Bd.BomCode = Bd1.BomCode and Bd.Partcode = Bd1.Kitcode " +
                            "Inner Join Part P On Bd1.Partcode = P.partcode " +
                            "Inner join UOM U On P.UOMCode = U.Uid where bd.BOMCode = '" + ProdDts[0].Trim() + "' and Bd.kitcode = '" + strPrdPartCode.Trim() + "' " +
                            "and BD1.kitCode LIKE '0124%' ORDER BY PartDesc ";

                    //strSql = "select BD.PartCode,partdesc+' [' +BD.partcode +'] ' as PartDesc,'' as SerialNo,u.UName,Qty,BD.Mob,P.Kit,p.conversionvalue,p.UomCode,'0' as StockQty,'0' as TotalQty," +
                    //         "SuppRate as Rate, QAP, Amount, P.CategoryID " +
                    //         " from #TempBOMdetails BD Inner Join Part P On Bd.Partcode=P.partcode Inner join UOM U On P.UOMCode=U.Uid " +
                    //         " where bd.KitCode='" + strPrdPartCode.Trim() + "' and bd.BOMCode='" + ProdDts[0].Trim() + "' and substring(BD.PartCode,1,8) Not in ('00002200','00002230','00003030','00410101')" +
                    //         " ORDER BY P.PartDesc";
                }

                cmd = new SqlCommand(strSql, con);
                adapter.SelectCommand = cmd;
                adapter.Fill(dSet, "TempBOMdetails");
                cmd.Dispose();
                double gridAmt = 0;
                if (dSet.Tables["TempBOMdetails"].Rows.Count > 0)
                {
                    for (int i = 0; i < dSet.Tables["TempBOMdetails"].Rows.Count; i++)
                    {

                        dSet.Tables["TempBOMdetails"].Rows[i]["TotalQty"] = 1 * double.Parse(dSet.Tables["TempBOMdetails"].Rows[i]["Qty"].ToString().Trim());
                        dSet.Tables["TempBOMdetails"].Rows[i]["Rate"] = Math.Round(getRate4Process(dSet.Tables["TempBOMdetails"].Rows[i]["PartCode"].ToString().Trim(), strPCCode.Trim(), double.Parse(dSet.Tables["TempBOMdetails"].Rows[i]["conversionvalue"].ToString().Trim()), dSet.Tables["TempBOMdetails"].Rows[i]["UomCode"].ToString().Trim(), dSet.Tables["TempBOMdetails"].Rows[i]["MOB"].ToString().Trim(), strPrdPartCode.Trim(), "N", "", ProdDts[0].Trim()), 2);
                        dSet.Tables["TempBOMdetails"].Rows[i]["StockQty"] = Math.Round(getWipStock(dSet.Tables["TempBOMdetails"].Rows[i]["PartCode"].ToString().Trim(), strPCCode.Trim()), 2);
                        dSet.Tables["TempBOMdetails"].Rows[i]["QAP"] = Math.Round((double.Parse(dSet.Tables["TempBOMdetails"].Rows[i]["StockQty"].ToString().Trim()) - double.Parse(dSet.Tables["TempBOMdetails"].Rows[i]["TotalQty"].ToString().Trim())), 2);
                        dSet.Tables["TempBOMdetails"].Rows[i]["Amount"] = Math.Round((double.Parse(dSet.Tables["TempBOMdetails"].Rows[i]["Rate"].ToString().Trim()) * double.Parse(dSet.Tables["TempBOMdetails"].Rows[i]["TotalQty"].ToString().Trim())), 2);
                        gridAmt = gridAmt + double.Parse(dSet.Tables["TempBOMdetails"].Rows[i]["Amount"].ToString().Trim());

                    }

                }

            }
            return dSet.Tables["TempBOMdetails"];
        }

        protected double getWipStock(string PartCode, string strPCCode)
        {
            DataSet RecQty = new DataSet();
            DataSet IssueQty = new DataSet();
            double tempRecQty = 0;
            double tempIssueQty = 0;
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand();
            strSql = "";
            strSql = "SELECT Sum(ReceivedQty) AS RecQty FROM #tempstockwip WHERE ToProfitCenterCode='" + strPCCode.Trim() + "' AND PartCode='" + PartCode.Trim() + "' ";
            cmd = new SqlCommand(strSql, con);
            adapter.SelectCommand = cmd;
            adapter.Fill(RecQty, "tempstockwip");
            cmd.Dispose();

            strSql = "";
            strSql = "SELECT Sum(IssueQty) AS IssueQty FROM #tempstockwip WHERE FromProfitCenterCode='" + strPCCode.Trim() + "' AND PartCode='" + PartCode.Trim() + "' ";
            cmd = new SqlCommand(strSql, con);
            adapter.SelectCommand = cmd;
            adapter.Fill(IssueQty, "tempstockwip");
            cmd.Dispose();

            if (RecQty.Tables["tempstockwip"].Rows[0]["RecQty"].ToString().Trim() == "")
            {
                tempRecQty = 0;
            }
            else
            {
                tempRecQty = double.Parse(RecQty.Tables["tempstockwip"].Rows[0]["RecQty"].ToString().Trim());
            }

            if (IssueQty.Tables["tempstockwip"].Rows[0]["IssueQty"].ToString().Trim() == "")
            {
                tempIssueQty = 0;
            }
            else
            {
                tempIssueQty = double.Parse(IssueQty.Tables["tempstockwip"].Rows[0]["IssueQty"].ToString().Trim());
            }

            return tempRecQty - tempIssueQty;

        }

        public double getRate4Process(String strPartCode, String PCCode, Double ConvValue, String struom, String strMOB, String strKIT, String RateType, String strRateFor, String strBOmCode)
        {
            string strOrignalPCCode = string.Empty;
            strOrignalPCCode = PCCode;
            string strFeild = "Rate";
            double prate = 0;
            string strsql = string.Empty;
            DataSet ds1 = new DataSet();

            if (PCCode == "01.005" || PCCode == "03.038") // Canopy Assembly
            {
                if (strPartCode.Trim().Substring(0, 3) == "005") //GoemKit
                {
                    PCCode = "01.064";
                }
                else if (strPartCode.Trim().Substring(0, 5) == "00712" || strPartCode.Trim().Substring(0, 11) == "00002060211" || strPartCode.Trim().Substring(0, 3) == "012") //Foam
                {
                    PCCode = "03.008";
                }
                else if (PCCode == "01.005" && strPartCode.Trim().Substring(0, 3) == "004" && strPartCode.Trim().Substring(11, 1) == "0" || strPartCode.Trim().Substring(11, 1) == "1" && strPartCode.Trim().Substring(10, 1) == "1") //canopy assly U1
                {
                    PCCode = "01.005";
                }
                else if (PCCode == "03.038" && strPartCode.Trim().Substring(0, 3) == "004" && strPartCode.Trim().Substring(11, 1) == "0" || strPartCode.Trim().Substring(11, 1) == "1" && strPartCode.Trim().Substring(10, 1) == "1") //canopy assly U4
                {
                    PCCode = "03.038";
                }
                else if (PCCode == "01.005" && strPartCode.Trim().Substring(0, 3) == "004" && strPartCode.Trim().Substring(10, 1) == "4" || strPartCode.Trim().Substring(10, 1) == "5") //Canopy assly U1
                {
                    PCCode = "01.007";
                }
                else if (PCCode == "03.038" && strPartCode.Trim().Substring(0, 3) == "004" && strPartCode.Trim().Substring(10, 1) == "4" || strPartCode.Trim().Substring(10, 1) == "5") //PowderCoating U4
                {
                    PCCode = "03.001";
                }
                else if (PCCode == "01.005" && strPartCode.Trim().Substring(0, 3) == "004")  //PowderCoating U1
                {
                    PCCode = "01.007";
                }
            }
            else if (PCCode == "03.040" || PCCode == "01.004" || PCCode == "05.001" || PCCode == "04.001" || PCCode == "14.001" || PCCode == "20.005") // DG Assembly
            {
                if (strPartCode.Trim().Substring(0, 3) == "005") //GoemKit
                {
                    PCCode = "01.064";
                }
                else if (strPartCode.Trim().Substring(0, 2) == "40") // Canopy
                {
                    PCCode = "01.005";
                }
                else if ((strPartCode.Trim().Substring(0, 3) == "004") && (strPartCode.Trim().Substring(11, 1) == "5" || strPartCode.Trim().Substring(11, 1) == "6"))// CP stand
                {
                    PCCode = "01.005','01.007','01.023";
                }
                else
                {
                    PCCode = "01.004','01.023','01.005','01.007','20.005','03.040";
                }
            }

            strsql = "";
            if (strMOB.Trim() == "B")
            {
                //Part Related To BOM
                if (strPartCode.Trim().Substring(0, 3) == "006")
                {
                    strsql = "Select isnull(Rate,0) as Rate,CostingCode From Supplierpricelistchanged Where PartCode='" + strPartCode + "' and " +
                   " ChangeDateTime=(Select max(ChangeDateTime) From supplierpricelistchanged SPC Inner Join PurchaseCosting PC On SPC.CostingCode=PC.PCCode  " +
                   "Where SPC.PartCode='" + strPartCode + "' and (Rateselected='M' or Rateselected='T') and PC.Active='1' and PC.Discard='1' and ChangeDateTime<=getdate())";
                }
                else
                {
                    strsql = "Select top 1 isnull(SuppRate+PrcCostUnitKG,0) as Rate from #TempBOMdetails where partcode='" + strPartCode + "'";
                }
            }
            else if (strMOB.Trim() == "M" || strMOB.Trim() == "O") //MFg Parts And strBOMcode <> ""
            {
                strsql = "";
                strsql = "select isnull(" + strFeild + ",0) as Rate from #TempPricelist where ProfitCenterCode in ('" + PCCode + "') and partcode='" + strPartCode + "'";
            }

            ds1.Clear();
            SqlDataAdapter dAd1 = new SqlDataAdapter(strsql, con);
            dAd1.Fill(ds1, "TempBOMRate");
            if (ds1.Tables["TempBOMRate"].Rows.Count > 0)
            {
                prate = (double)ds1.Tables["TempBOMRate"].Rows[0]["Rate"];
            }
            ds1.Dispose();
            dAd1.Dispose();

            return prate;
        }

        public string GetmaxPrc(string tablename, string fieldname, string Yr, string CompCode, SqlConnection con, SqlTransaction tran)
        {

            string PrcCode = "";
            string Max = "0";
            SqlCommand cmd = new SqlCommand("select max(substring(" + fieldname + ",13,7)) as MX from " + tablename.Trim() + " where yr='" + Yr.Trim() + "' and CompanyCode='" + CompCode.Trim() + "'", con, tran);
            cmd.CommandTimeout = 0;
            cmd.Transaction = tran;
            if ((cmd.ExecuteScalar().ToString() == System.DBNull.Value.ToString()))
            {
                Max = (CompCode + "000001");
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 9))
            {
                Max = (CompCode + ("00000" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 99))
            {
                Max = (CompCode + ("0000" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 999))
            {
                Max = (CompCode + ("000" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 9999))
            {
                Max = (CompCode + ("00" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else if ((Convert.ToInt32(cmd.ExecuteScalar()) < 99999))
            {
                Max = (CompCode + ("0" + (Convert.ToInt32(cmd.ExecuteScalar()) + 1)));
            }
            else
            {
                Max = (CompCode + (Convert.ToInt32(cmd.ExecuteScalar()) + 1));
            }
            PrcCode = "PSH" + "/" + Yr + "/" + Max;

            cmd.Dispose();
            return PrcCode;
        }

        public string JIRSubmit(string TRCode, string SerialNo, string UserID, string PCCode)
        {
            string[] strTRDts = Regex.Split(TRCode, ",");
            string[] strSRDts = Regex.Split(SerialNo, ",");
            for (int i = 0; i < strTRDts.Length; i++)
            {
                string chkAlreadySave = ComCon.getName("SELECT k.JIRCode FROM KSpanJIR k inner join KSpanJIRDetails kd on k.JIRCode=kd.JIRCode WHERE kd.TRCode='" + strTRDts[i].Trim() + "' and kd.SerialNo='" + strSRDts[i].Trim() + "' and k.active='1'", "tblKSpanJIRDetails", "JIRCode");
                if (chkAlreadySave.Trim() != "0")
                {
                    return "Joint Inspection for Vehicle No " + strTRDts[i].Trim() + " alredy Completed!";
                }
            }

            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                string JIRCode = ComCon.GetMaxNo("KSpanJIR", "JIR", PCCode.Substring(0, 2).Trim(), con, tran);
                sb.Remove(0, sb.Length);
                sb.Append("Insert into KSpanJIR(JIRCode,Yr,MaxSrNo,PCCode,CompanyCode) ");
                sb.Append("values('" + JIRCode.Trim() + "','" + JIRCode.Substring(4, 5).Trim() + "','" + JIRCode.Substring(10, 8).Trim() + "','" + PCCode.Trim() + "','" + PCCode.Substring(0, 2).Trim() + "') ");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                string DICode = ComCon.GetMaxNo("DispatchInstruction", "DIN", PCCode.Substring(0, 2).Trim(), con, tran);
                sb.Remove(0, sb.Length);
                sb.Append("Insert into DispatchInstruction(DINo,Yr,MaxSrNo,MOFCode,QtytoDispatch,ConsigneeCode,IndentorCode,BranchCode,CompanyCode,Remark,AuthRemark2,Auth2) ");
                sb.Append("values('" + DICode.Trim() + "','" + DICode.Substring(4, 5).Trim() + "','" + DICode.Substring(10, 8).Trim() + "',");
                sb.Append("'MOF/20-21/20000001','" + (strTRDts.Length + 1) + "','03.02.01.37.03.0082','04.02.01.37.03.0015',");
                sb.Append("'" + PCCode.Trim() + "','" + PCCode.Substring(0, 2).Trim() + "','Auto from ERP','Auto from ERP','1') ");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                int SrNo = 0;
                for (int i = 0; i < strTRDts.Length; i++)
                {
                    SrNo += 1;
                    sb.Remove(0, sb.Length);
                    sb.Append("Update TestReport set QAStatus='C' where TRCode='" + strTRDts[i].Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("Insert into KSpanJIRDetails(JIRCode,SrNo,TRCode,SerialNo) ");
                    sb.Append("values('" + JIRCode.Trim() + "','" + SrNo + "','" + strTRDts[i].Trim() + "','" + strSRDts[i].Trim() + "') ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("Insert into DispatchInstructionDetails(DINo,SrNo,RDGCode,PPWCode,RDGQty) ");
                    sb.Append("values('" + DICode.Trim() + "','" + SrNo + "','" + strTRDts[i].Trim() + "','" + strSRDts[i].Trim() + "','1') ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                // *********************User Acivity ***************************
                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@EmpID", UserID.Trim());
                cmd.Parameters.AddWithValue("@TransactionType", "S");
                cmd.Parameters.AddWithValue("@TransactionFrom", "KSpanJIR");
                cmd.Parameters.AddWithValue("@TransactionNo", JIRCode.Trim());
                cmd.Parameters.AddWithValue("@CompanyCode", PCCode.Substring(0, 2).Trim());
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                tran.Commit();
                return JIRCode.Trim();
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream("C:/Error/ApiError.txt", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter m_streamWriter = new StreamWriter(fs);
                m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                m_streamWriter.WriteLine();
                m_streamWriter.WriteLine("***************** " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:tt") + "*****************");
                m_streamWriter.WriteLine("StackTrace " + ex.StackTrace.ToString());
                m_streamWriter.WriteLine("Message " + ex.Message.ToString());
                m_streamWriter.Flush();
                m_streamWriter.Close();

                tran.Rollback();
                return ("StackTrace " + ex.StackTrace.ToString() + " Message " + ex.Message.ToString());
            }

            finally
            {
                con.Close();
            }
        }

        public DataTable GetKSpanMTTRReport(string FromDt, string ToDt, string VehicleNo, string StageName, string Type)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("KSpanJobCardMTTR_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@FromDt", SqlDbType.Char).Value = FromDt;
            dAd.SelectCommand.Parameters.Add("@ToDt", SqlDbType.Char).Value = ToDt;
            dAd.SelectCommand.Parameters.Add("@SerialNo", SqlDbType.Char).Value = VehicleNo;
            dAd.SelectCommand.Parameters.Add("@StageName", SqlDbType.Char).Value = StageName;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetKSpanMTTRAttachments(string VehicleNo, string StageName, string Type)
        {
            DataSet ds = new DataSet();
            ds = ComCon.procDS("exec [KSpanJobCardMTTR_sp] '','','" + VehicleNo.Trim() + "','','APPS'", "tbl_Master");
            return ds.Tables["tbl_Master"];
        }

    }
}