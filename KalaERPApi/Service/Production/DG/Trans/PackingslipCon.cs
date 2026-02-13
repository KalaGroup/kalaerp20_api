using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.Http;
using System.Text.RegularExpressions;
using PackingslipRequest = KalaERPApi.Models.Request.Production.DG.Trans.PackingslipRequest;

namespace KalaERPApi.Service.Production.DG.Trans
{
    public class PackingslipCon
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        StringBuilder sb = new StringBuilder();
        DataSet dsDetailsSub = new DataSet();
        //string strSql = "";
        public SqlTransaction tran = null;

        public object SqlDbTypeChar { get; private set; }

        public DataTable GetScanDts(string strSrNo, string StrDGSrNo, string strCat, string strCPBatCnt, string StrPCCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetPSScanDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@strSrNo", SqlDbType.Char).Value = strSrNo;
            dAd.SelectCommand.Parameters.Add("@DGSrNo", SqlDbType.Char).Value = StrDGSrNo;
            dAd.SelectCommand.Parameters.Add("@strCat", SqlDbType.Char).Value = strCat;
            dAd.SelectCommand.Parameters.Add("@strCPBatCnt", SqlDbType.Char).Value = strCPBatCnt;
            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = StrPCCode;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }


        public DataTable GetMOFAddPartDts(string strMOFCode)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetMOFAddPartDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@MOFCode", SqlDbType.Char).Value = strMOFCode;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string Submit(PackingslipRequest PSReq)
        {
            string StrPSLCode = "";

            string StrPCCodeStkIssue = "";

            string StrPCCodeStkRecieved = "";

            int SrNo = 0;
            CommonCon ComCon = new CommonCon();
            if (PSReq.PSTime == "PSStartTime")
            {
                int recCountV = ComCon.CountChars(PSReq.MOFAddParts, ",");
                string[] strPrcDtsV = Regex.Split(PSReq.MOFAddParts, ",");               
                for (int cSubV = 0; cSubV <= recCountV; cSubV++)
                {                    
                    string[] DtsV = Regex.Split(strPrcDtsV[cSubV].ToString().Trim(), "-->");
                    if (double.Parse(DtsV[2].Trim()) - double.Parse(DtsV[1].Trim()) < 0)
                    {
                        StrPSLCode = "Insufficient Stock(Additional Part) For: " + DtsV[3].Trim();
                        return StrPSLCode;
                    }
                }
            }
            try
            {

                String PC_CompanyCode = "";

                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand();

                String StrTPSStatus = ComCon.getTranName("Select TPSStatus from TestReport where TRCode='" + PSReq.TRCode.Trim() + "' and active='1'", "TestReport", "TPSStatus", con, tran).ToString().Trim();


                if (PSReq.PSTime == "PSStartTime")
                {
                   
                    if (PSReq.TRCode.Trim().Substring(10, 2).Trim() == "01" && StrTPSStatus.Trim() == "P")
                    {
                        StrPCCodeStkIssue = "01.004";
                        StrPCCodeStkRecieved = "01.018";
                        PC_CompanyCode = PSReq.TRCode.Trim().Substring(10, 2).Trim();
                    }
                    else if (PSReq.TRCode.Trim().Substring(10, 2).Trim() == "28" && StrTPSStatus.Trim() == "P")
                    {
                        StrPCCodeStkIssue = "28.001";
                        StrPCCodeStkRecieved = "28.005";
                        PC_CompanyCode = PSReq.TRCode.Trim().Substring(10, 2).Trim();
                    }
                    else
                    {
                       
                        StrPCCodeStkIssue = "03.051";
                        StrPCCodeStkRecieved = "03.019";
                        PC_CompanyCode = "03";
                        //'" + StrPCCodeStkIssue.Trim() + "'
                        //    '" + StrPCCodeStkRecieved.Trim() + "'
                    }



                    //StrPSLCode = ComCon.GetMaxNo("Packingslip", "PSL", PSReq.TRCode.Trim().Substring(10, 2).Trim(), con, tran);

                    StrPSLCode = ComCon.GetMaxNo("Packingslip", "PSL", PC_CompanyCode.Trim(), con, tran);

                    sb.Remove(0, sb.Length);
                    sb.Append("insert into Packingslip(PSCode,Dt,Yr,MaxSrNo,PCCode,SOFCode,PSStartTime,PDICode,SrNo,Remark,PCCodeStkIssue,");
                    sb.Append("BatteryTerminals,Batterylead,ExhaustPipe,DCBulb,CanopyKey,FuelCapKey,Rate,RubberPad,FunnelPad,PrdManual,CompanyCode)");
                    sb.Append(" values('" + StrPSLCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ComCon.yearEnd(con, tran) + "','" + (StrPSLCode.Substring(10, 8)) + "', ");
                    sb.Append("'" + PSReq.TRCode.Trim() + "','" + PSReq.DiNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + PSReq.PDICode.Trim() + "','" + PSReq.DGSrNo.Trim() + "','" + PSReq.Remark.Trim() + "',");
                    sb.Append(" '" + StrPCCodeStkIssue.Trim() + "','" + PSReq.BatTer.Trim() + "','" + PSReq.BatLead.Trim() + "','" + PSReq.ExhPipe.Trim() + "','" + PSReq.DCBulb.Trim() + "',");
                   // sb.Append("'" + PSReq.CanopyKey.Trim() + "','" + PSReq.FuelCapKey.Trim() + "',' 1','" + PSReq.RubberPad.Trim() + "','" + PSReq.FunnelPad.Trim() + "','" + PSReq.PrdManual.Trim() + "','" + PSReq.TRCode.Trim().Substring(10, 2).Trim() + "')");
                    sb.Append("'" + PSReq.CanopyKey.Trim() + "','" + PSReq.FuelCapKey.Trim() + "',' 1','" + PSReq.RubberPad.Trim() + "','" + PSReq.FunnelPad.Trim() + "','" + PSReq.PrdManual.Trim() + "','" + PC_CompanyCode.Trim() + "')");

                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    if (PSReq.EngPartCode.Substring(0, 3) == "001")
                    {
                        SrNo += 1;
                        sb.Remove(0, sb.Length);
                        sb.Append("insert into PackingslipDetails(PSCode,SrNo,PartCode,SerialNo,Qty,Rate)");
                        sb.Append("values('" + StrPSLCode.Trim() + "','" + SrNo + "',");
                        sb.Append("'" + PSReq.EngPartCode.Trim() + "','" + PSReq.EngSrNo.Trim() + "','1','1')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    if (PSReq.AltPartcode.Substring(0, 3) == "002")
                    {
                        SrNo += 1;
                        sb.Remove(0, sb.Length);
                        sb.Append("insert into PackingslipDetails(PSCode,SrNo,PartCode,SerialNo,Qty,Rate)");
                        sb.Append("values('" + StrPSLCode.Trim() + "','" + SrNo + "',");
                        sb.Append("'" + PSReq.AltPartcode.Trim() + "','" + PSReq.AltSrno.Trim() + "','1','1')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    if (PSReq.CpyPartcode.Substring(0, 3) == "401")
                    {
                        SrNo += 1;
                        sb.Remove(0, sb.Length);
                        sb.Append("insert into PackingslipDetails(PSCode,SrNo,PartCode,SerialNo,Qty,Rate)");
                        sb.Append("values('" + StrPSLCode.Trim() + "','" + SrNo + "',");
                        sb.Append("'" + PSReq.CpyPartcode.Trim() + "','" + PSReq.CpySrno.Trim() + "','1','1')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    if (PSReq.BatPartcode.Trim().Length > 2)
                    {
                        if (PSReq.BatPartcode.Substring(0, 3) == "010")
                        {
                            SrNo += 1;
                            sb.Remove(0, sb.Length);
                            sb.Append("insert into PackingslipDetails(PSCode,SrNo,PartCode,SerialNo,Qty,Rate)");
                            sb.Append("values('" + StrPSLCode.Trim() + "','" + SrNo + "',");
                            sb.Append("'" + PSReq.BatPartcode.Trim() + "','" + PSReq.BatSrno.Trim() + "','1','1')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                    if (PSReq.Bat2Partcode.Trim().Length > 2)
                    {
                        if (double.Parse(PSReq.KVA) >= 180)
                        {
                            SrNo += 1;
                            if (PSReq.Bat2Partcode.Substring(0, 3) == "010")
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("insert into PackingslipDetails(PSCode,SrNo,PartCode,SerialNo,Qty,Rate)");
                                sb.Append("values('" + StrPSLCode.Trim() + "','" + SrNo + "',");
                                sb.Append("'" + PSReq.Bat2Partcode.Trim() + "','" + PSReq.Bat2Srno.Trim() + "','1','1')");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                        }
                    }
                    string[] CPTYpe = Regex.Split(PSReq.CPType.Trim(), "-->");
                    if (double.Parse(CPTYpe[1].Trim()) > 0)
                    {
                        SrNo += 1;
                        if (PSReq.CPPartcode.Substring(0, 3) == "003")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("insert into PackingslipDetails(PSCode,SrNo,PartCode,SerialNo,Qty,Rate)");
                            sb.Append("values('" + StrPSLCode.Trim() + "','" + SrNo + "',");
                            sb.Append("'" + PSReq.CPPartcode.Trim() + "','" + PSReq.CPSrno.Trim() + "','1','1')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //Update Jobcard Status CP1
                            #region
                            //Process
                            sb.Remove(0, sb.Length);
                            sb.Append("Update ProcessFeedbackDetailsSub set JobCardStatus='J' where  SerialNo='" + PSReq.CPSrno.Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //MTF
                            sb.Remove(0, sb.Length);
                            sb.Append("Update MTFDetailsSub set JobCardStatus='J' where  SerialNo='" + PSReq.CPSrno.Trim() + "' ");
                            sb.Append("and PartCode='" + PSReq.CPPartcode.Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //Giir
                            sb.Remove(0, sb.Length);
                            sb.Append("Update GiirDetailsSub set JobCardStatus='J' where  SerialNo='" + PSReq.CPSrno.Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            #endregion
                            //Update Jobcard Status CP1
                        }
                    }                    
                    if (double.Parse(CPTYpe[2].Trim()) > 1)
                    {
                        SrNo += 1;
                        if (PSReq.CP2Partcode.Substring(0, 3) == "003")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("insert into PackingslipDetails(PSCode,SrNo,PartCode,SerialNo,Qty,Rate)");
                            sb.Append("values('" + StrPSLCode.Trim() + "','" + SrNo + "',");
                            sb.Append("'" + PSReq.CP2Partcode.Trim() + "','" + PSReq.CP2Srno.Trim() + "','1','1')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //Update Jobcard Status CP2
                            #region
                            //Process
                            sb.Remove(0, sb.Length);
                            sb.Append("Update ProcessFeedbackDetailsSub set JobCardStatus='J' where SerialNo='" + PSReq.CP2Srno.Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //MTF
                            sb.Remove(0, sb.Length);
                            sb.Append("Update MTFDetailsSub set JobCardStatus='J' where  SerialNo='" + PSReq.CP2Srno.Trim() + "' ");
                            sb.Append("and PartCode='" + PSReq.CP2Partcode.Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            //Giir
                            sb.Remove(0, sb.Length);
                            sb.Append("Update GiirDetailsSub set JobCardStatus='J' where  SerialNo='" + PSReq.CP2Srno.Trim() + "' ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            #endregion
                            //Update Jobcard Status CP2
                        }
                    }
                    
                    if (PSReq.KRMPartcode.Trim() != "0")
                    {
                        SrNo += 1;
                        sb.Remove(0, sb.Length);
                        sb.Append("insert into PackingslipDetails(PSCode,SrNo,PartCode,SerialNo,Qty,Rate)");
                        sb.Append("values('" + StrPSLCode.Trim() + "','" + SrNo + "',");
                        sb.Append("'" + PSReq.KRMPartcode.Trim() + "','" + PSReq.KRMSrno.Trim() + "','1','1')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    //Mst
                    sb.Remove(0, sb.Length);
                    sb.Append("UPDATE DispatchInstructionDetails SET PSLDStatus='C' WHERE DINo='" + PSReq.DiNo.Trim() + "' and RDGCode='" + PSReq.TRCode.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    ////For U1 to U4 Stock Tranfer
                    //if (PC_CompanyCode.Trim() == "03" && StrTPSStatus.Trim() == "M")
                    //{
                    //    sb.Remove(0, sb.Length);
                    //    sb.Append("UPDATE TestReport SET TPSStatus='C' WHERE TRCode='" + PSReq.TRCode.Trim() + "'");
                    //    cmd = new SqlCommand(sb.ToString(), con);
                    //    cmd.Transaction = tran;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();
                    //}

                    //Details status
                        string cntDIStatus = ComCon.getTranName("Select count(PSLDStatus) as Cnt From DispatchInstructionDetails Where PSLDStatus<>'C' and DINo='" + PSReq.DiNo.Trim() + "'", "DispatchInstructionDetails", "Cnt", con, tran);
                    if (cntDIStatus == "0")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("UPDATE DispatchInstruction SET PSLStatus='C' WHERE DINo='" + PSReq.DiNo.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    int recCount = ComCon.CountChars(PSReq.MOFAddParts, ",");
                    string[] strPrcDts = Regex.Split(PSReq.MOFAddParts, ",");                    
                    for (int cSub = 0; cSub <= recCount; cSub++)
                    {
                        SrNo += 1;
                        string[] Dts = Regex.Split(strPrcDts[cSub].ToString().Trim(), "-->");
                        if (Dts[0].Trim() != "0")
                        {
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO StockWIP(FromProfitCenterCode,PartCode,IssueCode,IssueDate,IssueQty,ToProfitCenterCode,StockType)");
                            sb.Append(" values( '" + StrPCCodeStkIssue.Trim() + "','" + Dts[0].ToString().Trim() + "','" + StrPSLCode.Trim() + "',");
                            sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:MM") + "','" + double.Parse(Dts[1].ToString().Trim()) + "','" + StrPCCodeStkRecieved.Trim() + "',0)");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            sb.Append("insert into PackingslipDetails(PSCode,SrNo,PartCode,SerialNo,Qty,Rate)");
                            sb.Append("values('" + StrPSLCode.Trim() + "','" + SrNo + "','" + Dts[0].ToString().Trim() + "','-','1','1')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                }
             
                else if (PSReq.PSTime == "PSEndTime")
                {
                    sb.Remove(0, sb.Length);
                    sb.Append("Update PackingSlip set " + PSReq.PSTime + "='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where  PSCode='" + PSReq.PSCode.Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                tran.Commit();              
                return StrPSLCode = "Packingslip Code=" + StrPSLCode;
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