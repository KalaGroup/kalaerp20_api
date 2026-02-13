using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using TestReportRequest = KalaERPApi.Models.Request.Production.DG.Trans.TestReportRequest;

namespace KalaERPApi.Service.Production.DG.Trans
{
    public class TestReportCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        StringBuilder sb = new StringBuilder(); 
        public SqlTransaction tran = null;
        CommonCon ComCon = new CommonCon();

        public object SqlDbTypeChar { get; private set; }

        public DataTable GetScanDts(string strSrNo, string StrDGSrNo, string strCat, string StrPCCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetTRScanDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@strSrNo", SqlDbType.Char).Value = strSrNo;
            dAd.SelectCommand.Parameters.Add("@DGSrNo", SqlDbType.Char).Value = StrDGSrNo;
            dAd.SelectCommand.Parameters.Add("@strCat", SqlDbType.Char).Value = strCat;
            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = StrPCCode;

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

        public DataTable Get6M()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("Get6M", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;


            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string Submit(TestReportRequest TestReportReq)
        {
            string StrTRCode = "";
            string StrPDIRCode = "";
            string strTimefield = "";
            string TRQAStatus = "";
            string ChkDupTR = "";
            if (TestReportReq.TRTime == "TRStart")
            {
                ChkDupTR = ComCon.getTranName("Select isnull(Count(ProcessCode),0) as TRCnt from TestReport Where ProcessCode='" + TestReportReq.PFBCode.Trim() + "' and Active='1' ", "TblChkDupTR", "TRCnt", con, tran);
                if (int.Parse(ChkDupTR) > 0)
                {
                    return StrTRCode = "TRStart For This Process Already Done ";
                }
            }
            try
            {               
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                TRQAStatus = ComCon.getTranName("Select QAStatus from TestReport Where TRCode='" + TestReportReq.TRCode.Trim() + "' ", "TblTestReport", "QAStatus", con, tran);

                if (TestReportReq.TRTime == "TRStart")
                {
                    strTimefield = "TRStartTime";
                }
                else if (TestReportReq.TRTime == "TREnd")
                {
                    strTimefield = "TREndTime";
                }
                else if (TestReportReq.TRTime == "DGStart")
                {
                    strTimefield = "DGStartTime";
                }
                else if (TestReportReq.TRTime == "DGEnd")
                {
                    strTimefield = "DGEndTime";
                }


                if (strTimefield == "TRStartTime")
                {
                    StrTRCode = ComCon.GetMaxNo("TestReport", "TRC", TestReportReq.PFBCode.Trim().Substring(10, 2).Trim(), con, tran);

                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into TestReport(TRCode,Dt,Yr,MaxSrNo,ProcessCode,MachineNo,Remark,");
                    sb.Append("RevTRCode,BalQty,EngineModel,HP,KW,Speed,Alternator,RatedKVA,RatedVolt,RatedAMPS,");
                    sb.Append("Ph,PF, Frequency,AMBTemp,RY,YB,BR,VoltageRegulation,RoomTempreture,RPMRegulation,LLOP,HWT,HCT,OSD,DieselRate,PerUnitQty,PDIRStatus,CompanyCode," + strTimefield + ")");
                    sb.Append(" values('" + StrTRCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ComCon.yearEnd(con, tran) + "','" + (StrTRCode.Substring(10, 8)) + "', ");
                    sb.Append("'" + TestReportReq.PFBCode.Trim() + "','" + TestReportReq.DGSrNo.Trim() + "','" + TestReportReq.Remark.Trim() + "',");
                    sb.Append("'0','0','0','0','0','0','0','0','0','0',");
                    // Commented by KB on 04/01/2023
                    //sb.Append("'1','2','3','4','5','6','7','8','9','10','11','12','13','14','" + TestReportReq.DieselRate + "','" + TestReportReq.DieselQty + "','C','" + TestReportReq.PFBCode.Trim().Substring(10, 2).Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')");
                    sb.Append("'1','2','3','4','5','6','7','8','9','10','11','12','13','14','" + TestReportReq.DieselRate + "','" + TestReportReq.DieselQty + "','P','" + TestReportReq.PFBCode.Trim().Substring(10, 2).Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')");

                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    // Commented by KB on 04/01/2023
                    //StrPDIRCode = ComCon.GetMaxNo("PDIR", "PDI", TestReportReq.PFBCode.Trim().Substring(10, 2).Trim(), con, tran);

                    //sb.Remove(0, sb.Length);
                    //sb.Append("insert into PDIR(PDICode,Dt,Yr,MaxSrNo,DICode,TRCode,CLEng,CLAlt,CLBat,CLExhFan,CLCanopy,CLFuelTank,CLCP,CLGA,CLDOC,Remark,");
                    //sb.Append("DISPStatus,PSStatus,CompanyCode)");
                    //sb.Append(" values('" + StrPDIRCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ComCon.yearEnd(con, tran) + "','" + (StrTRCode.Substring(10, 8)) + "', ");
                    //sb.Append("'0','" + StrTRCode.Trim() + "','CHK/09-10/01000010','CHK/09-10/01000009','CHK/09-10/01000002','NIL','CHK/09-10/01000004','NIL','CHK/13-14/01000006',");
                    //sb.Append("'CHK/13-14/01000001','CHK/11-12/01000002','Nil','P','P','" + TestReportReq.PFBCode.Trim().Substring(10, 2).Trim() + "')");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                    // Comment END by KB on 04/01/2023

                    int recCount = ComCon.CountChars(TestReportReq.TRDts, ",");
                    string[] strPrcDts = Regex.Split(TestReportReq.TRDts, ",");

                    int SrNo = 0;
                    for (int cSub = 0; cSub <= recCount; cSub++)
                    {
                        SrNo += 1;
                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");

                        sb.Remove(0, sb.Length);
                        sb.Append("insert into TestReportSerialNoDetails(TRCode,SrNo,PartCode,SerialNo,SerialStatus,Qty,GiirCode)");
                        sb.Append("values('" + StrTRCode.Trim() + "','" + SrNo + "',");
                        sb.Append("'" + Dts[0].Trim() + "',");
                        sb.Append("'" + Dts[1].Trim() + "','D',1,'" + TestReportReq.PFBCode.Trim() + "')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    sb.Remove(0, sb.Length);
                    sb.Append("Update ProcessFeedbackDetailssub set TRSerialStatus='D' where  PFBCode='" + TestReportReq.PFBCode.Trim() + "'  ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("Update ProcessFeedback set TRStatus='D' where  PFBCode='" + TestReportReq.PFBCode.Trim() + "'  ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand();
                    if (strTimefield == "DGEndTime")
                    {
                        int recCount1 = ComCon.CountChars(TestReportReq.TRPrcChkDts, ",");
                        string[] strPrcChkDts = Regex.Split(TestReportReq.TRPrcChkDts, ",");
                        int SrNo = 0;

                        for (int cSub1 = 0; cSub1 <= recCount1; cSub1++)
                        {
                            SrNo += 1;
                            string[] PrcChkDts = Regex.Split(strPrcChkDts[cSub1].ToString().Trim(), "-->");
                            string GetDGStartTime = "";

                            GetDGStartTime = ComCon.getTranName("Select convert(nvarchar(20),DGStartTime,120) as DGStartTime from TestReport Where TRCode='" + TestReportReq.TRCode.Trim() + "' ", "TblTestReport", "DGStartTime", con, tran);
                            sb.Remove(0, sb.Length);
                            sb.Append("insert into PrcChkDetails(TransCode,Dt,MainSerialNo,QA6M,PrcName,ChkPointId,PrcChkPoints,PrcStatus,DGStartTime)");
                            sb.Append("values('" + TestReportReq.TRCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',");
                            sb.Append("'" + TestReportReq.DGSrNo.Trim() + "','" + TestReportReq.QA6M + "','DG TestReport', ");
                            sb.Append("'" + PrcChkDts[0].Trim() + "','" + PrcChkDts[1].Trim() + "',");
                            sb.Append("'" + TestReportReq.QAStatus + "',");
                            sb.Append("'" + GetDGStartTime.Trim() + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }

                        sb.Remove(0, sb.Length);
                        sb.Append("Update TestReport set ");
                        if (TestReportReq.QAStatus == "Rework")
                        {
                            sb.Append("" + strTimefield + "= NULL,DGStartTime=NULL,QAStatus='P',");
                        }
                        else
                        {
                            sb.Append("" + strTimefield + "='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',QAStatus='D',");
                        }
                        sb.Append(" PerUnitQty='" + TestReportReq.DieselQty + "' where  ProcessCode='" + TestReportReq.PFBCode.Trim() + "' and MachineNo='" + TestReportReq.DGSrNo.Trim() + "'  ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    else if (strTimefield == "DGStartTime" && TRQAStatus.Trim() == "Rework")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update TestReport set " + strTimefield + "='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',QAStatus='P' where  ProcessCode='" + TestReportReq.PFBCode.Trim() + "' and MachineNo='" + TestReportReq.DGSrNo.Trim() + "'  ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    else
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update TestReport set " + strTimefield + "='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where  ProcessCode='" + TestReportReq.PFBCode.Trim() + "' and MachineNo='" + TestReportReq.DGSrNo.Trim() + "'  ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                tran.Commit();
                if (strTimefield == "TRStartTime")
                {
                    return StrTRCode = "TestReport Code=" + StrTRCode + " and PDIR Code= " + StrPDIRCode;
                }
                else
                {
                    return StrTRCode = "TestReport =" + TestReportReq.TRCode.Trim() + " Updated With " + strTimefield + " Timings";
                }
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
    }
}