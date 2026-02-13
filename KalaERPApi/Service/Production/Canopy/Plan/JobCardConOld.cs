using KalaERPApi.Models.Request;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace KalaERPApi.Service
{
    public class JobCardCon
    {
        #region
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        StringBuilder sb = new StringBuilder();
        public SqlTransaction tran = null;
        DataSet dsDetailsSub = new DataSet();
        DataSet dsChkBat = new DataSet();
        DataSet dsDetailsCP = new DataSet();
        DataSet dsDetailsSubP = new DataSet();
        DataSet dsDetailsSubPV = new DataSet();
        DataSet dsDetailsCPSRNo = new DataSet();
        DataSet dsDetailsSubV = new DataSet();
        #endregion

        public object SqlDbTypeChar { get; private set; }

        public DataTable GetJOBCard1Report(string Type, string JobCardNo, string P_FromDt, string P_ToDt)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getJobCard1Rpt_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = JobCardNo;
            dAd.SelectCommand.Parameters.Add("@FromDt", SqlDbType.Char).Value = P_FromDt;
            dAd.SelectCommand.Parameters.Add("@ToDt", SqlDbType.Char).Value = P_ToDt;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetJOBCard2Report(string Type, string JobCardNo, string P_FromDt, string P_ToDt)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getJobCard2Rpt_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = JobCardNo;
            dAd.SelectCommand.Parameters.Add("@FromDt", SqlDbType.Char).Value = P_FromDt;
            dAd.SelectCommand.Parameters.Add("@ToDt", SqlDbType.Char).Value = P_ToDt;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetJabCardMttrChart(string StageType, string FromDt, string ToDt)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getJobCardMttrChart_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = StageType;
            dAd.SelectCommand.Parameters.Add("@FromDt", SqlDbType.Char).Value = FromDt;
            dAd.SelectCommand.Parameters.Add("@ToDt", SqlDbType.Char).Value = ToDt;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet, "tbl_JobCardChart");
            return dSet.Tables[0];
        }

        public DataTable GetJabCardMttrRpt(string Type, string JobCardNo, string P_FromDt, string P_ToDt)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getJobCardMttrRpt_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = JobCardNo;
            dAd.SelectCommand.Parameters.Add("@FromDt", SqlDbType.Char).Value = P_FromDt;
            dAd.SelectCommand.Parameters.Add("@ToDt", SqlDbType.Char).Value = P_ToDt;

            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetJabCardMttrRptNew(string Type, string CompanyCode, string JobCardNo, string P_FromDt, string P_ToDt)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getJobCardMttrRptNew_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = JobCardNo;
            dAd.SelectCommand.Parameters.Add("@FromDt", SqlDbType.Char).Value = P_FromDt;
            dAd.SelectCommand.Parameters.Add("@ToDt", SqlDbType.Char).Value = P_ToDt;
            dAd.SelectCommand.Parameters.Add("@CompanyCode", SqlDbType.Char).Value = CompanyCode;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetJabCardMttrRptNewQACkeckList(string StageType, string JobCardNo, string SerialNo)
        {
            string strSql = "select cd.PrcStatus,c.CheckPointDesc,cd.PrcChkPoints as Remark from Processcheckpoints c " +
                " inner join PrcChkDetails cd on cd.ChkPointId = c.id inner join [6M] m on m.id = cd.QA6M " +
                " where cd.PrcName ='" + StageType.Trim() + "' and cd.TransCode ='" + JobCardNo.Trim() + "' and cd.MainSerialNo = '" + SerialNo.Trim() + "'";
            SqlDataAdapter dAd = new SqlDataAdapter(strSql, con);
            dAd.SelectCommand.CommandType = CommandType.Text;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetDG(string strJobCardType,string strcompID)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetJobCardDGDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@JobCardType", SqlDbType.Char).Value = strJobCardType;
            dAd.SelectCommand.Parameters.Add("@CompCode", SqlDbType.Char).Value = strcompID;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetCP()
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetJobCard2CP", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            //dAd.SelectCommand.Parameters.Add("@JobCardType", SqlDbType.Char).Value = strJobCardType;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string GetCPStk(string StrKVA, string ph, string PanelType)
        {
            string CPStk = "";
            CommonCon ComCon = new CommonCon();
            dsDetailsSub = ComCon.procDS("exec GetCPStk  '" + StrKVA.Trim() + "'," + ph.Trim() + ",'" + PanelType.Trim() + "'", "tbl_JoBCardSerialNo");
            if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows.Count > 0)
            {
                for (int k = 0; k < dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows.Count; k++)
                {
                    if (k == 0)
                    {
                        CPStk = dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Stk"].ToString().Trim();

                    }
                    else
                    {
                        CPStk = CPStk + "-->" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Stk"].ToString().Trim();

                    }
                }

            }
            else
            {
                CPStk = "0-->0";
            }
            if (CPStk.Length < 5)
            {
                CPStk = CPStk + "-->0";
            }
            return CPStk;

        }

        public Boolean checkBOMForBat(string productcode)
        {
            Boolean BOMForBat = false;
            CommonCon ComCon = new CommonCon();
            dsChkBat = ComCon.procDS("Select  isNull(Count(Partcode),0)  as BatPart From BomDetails where kitcode='" + productcode + "' and Partcode like '010%'", "tbl_JoBCardBat");
            if (dsChkBat != null && dsChkBat.Tables["tbl_JoBCardBat"].Rows.Count == 1)
            {
                if (int.Parse(dsChkBat.Tables["tbl_JoBCardBat"].Rows[0]["BatPart"].ToString().Trim()) > 0)

                {
                    BOMForBat = true;
                }
                else
                {
                    BOMForBat = false;
                }
            }
            else
            {
                BOMForBat = false;

            }
            return BOMForBat;

        }

        public string Submit(JobCardRequest jobreq)
        {
            int JPEng = 0, JPAlt = 0, JPBat = 0, JPCPY = 0;
            int strEngQty = 0, strAltQty = 0, strCpyQty = 0, strBatQty = 0;
            //int Jpriority = 0;
            string JobCardNo = "", strKVA = "";
            int strBatCnt = 0;

            CommonCon ComCon = new CommonCon();
            int recCountV = ComCon.CountChars(jobreq.JobCardDts, ",");
            string[] strJCDDtsV = Regex.Split(jobreq.JobCardDts, ",");

            int SrNoV = 0;
            for (int cSubV = 0; cSubV <= recCountV; cSubV++)
            {
                SrNoV += 1;
                string[] DtsV = Regex.Split(strJCDDtsV[cSubV].ToString().Trim(), "-->");
                dsDetailsSubV = ComCon.procDS("exec GetJobCardSrNo 'DGWOP','" + DtsV[1] + "'," + DtsV[2].Trim() + ", '" + jobreq.PCCode.Trim().Substring(0, 2) + "'", "tbl_JoBCardSerialNoV");
                if (dsDetailsSubV != null && dsDetailsSubV.Tables["tbl_JoBCardSerialNoV"].Rows.Count > 0)
                {
                    strEngQty = 0; strAltQty = 0; strCpyQty = 0; strBatQty = 0;

                    for (int v = 0; v < dsDetailsSubV.Tables["tbl_JoBCardSerialNoV"].Rows.Count; v++)
                    {
                        if (dsDetailsSubV.Tables["tbl_JoBCardSerialNoV"].Rows[v]["PartCode"].ToString().Trim().Substring(0, 3) == "001")
                        {
                            strEngQty = strEngQty + 1;
                        }
                        else if (dsDetailsSubV.Tables["tbl_JoBCardSerialNoV"].Rows[v]["PartCode"].ToString().Trim().Substring(0, 3) == "002")
                        {
                            strAltQty = strAltQty + 1;
                        }
                        else if (dsDetailsSubV.Tables["tbl_JoBCardSerialNoV"].Rows[v]["PartCode"].ToString().Trim().Substring(0, 3) == "010")
                        {
                            strBatQty = strBatQty + 1;
                        }
                        else if (dsDetailsSubV.Tables["tbl_JoBCardSerialNoV"].Rows[v]["PartCode"].ToString().Trim().Substring(0, 2) == "40")
                        {
                            strCpyQty = strCpyQty + 1;
                        }
                    }

                    if (int.Parse(DtsV[2].Trim()) > strEngQty)
                    {
                        JobCardNo = "Engine SrNo Not available For DG " + ComCon.getName("select partdesc From part where Partcode='" + DtsV[1].Trim() + "'", "TblPart", "Partdesc");
                        return JobCardNo;
                    }
                    else if (int.Parse(DtsV[2].Trim()) > strAltQty)
                    {
                        JobCardNo = "Alternator SrNo Not available For DG " + ComCon.getName("select partdesc From part where Partcode='" + DtsV[1].Trim() + "'", "TblPart", "Partdesc");
                        return JobCardNo;
                    }
                    else if (int.Parse(DtsV[2].Trim()) > strBatQty)
                    {
                        if (checkBOMForBat(DtsV[1].Trim()) == true)
                        {
                            JobCardNo = "Battery SrNo Not available For DG " + ComCon.getName("select partdesc From part where Partcode='" + DtsV[1].Trim() + "'", "TblPart", "Partdesc");
                            return JobCardNo;
                        }
                    }
                    else if (int.Parse(DtsV[2].Trim()) > strCpyQty)
                    {
                        JobCardNo = "Canopy SrNo Not available For DG  " + ComCon.getName("select partdesc From part where Partcode='" + DtsV[1].Trim() + "'", "TblPart", "Partdesc");
                        return JobCardNo;
                    }
                }
            }

            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                JobCardNo = ComCon.GetMaxNo("JobCard", "JCD", jobreq.PCCode.Trim().Substring(0, 2), con, tran);

                SqlCommand cmd = new SqlCommand();
                sb.Remove(0, sb.Length);
                sb.Append("INSERT INTO JobCard(JobCode,Dt,Yr,MaxSrNo,PCCode,Remark,CompanyCode,Active,Auth)");
                sb.Append(" VALUES('" + JobCardNo.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:MM") + "','" + ComCon.yearEnd(con, tran) + "','" + (JobCardNo.Substring(10, 8)) + "',");
                sb.Append("'" + jobreq.PCCode.Trim() + "','" + jobreq.Remark.Trim() + "','" + jobreq.PCCode.Trim().Substring(0, 2) + "','1','1')");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                int recCount = ComCon.CountChars(jobreq.JobCardDts, ",");
                string[] strJCDDts = Regex.Split(jobreq.JobCardDts, ",");
                int SrNo = 0;
                for (int cSub = 0; cSub <= recCount; cSub++)
                {
                    SrNo += 1;
                    string[] Dts = Regex.Split(strJCDDts[cSub].ToString().Trim(), "-->");

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO JobCardDetails(JobCode,SrNo,BOMCode,PartCode,Qty,Stage1Status,Stage2Status,Stage3Status)");
                    sb.Append(" VALUES('" + JobCardNo.Trim() + "','" + SrNo + "','" + Dts[0].Trim() + "','" + Dts[1].Trim() + "'," + Dts[2].Trim() + ",'P','P','P')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    dsDetailsSub = ComCon.procTranDS("exec GetJobCardSrNo 'DGWOP','" + Dts[1] + "'," + Dts[2].Trim() + ", '" + jobreq.PCCode.Trim().Substring(0, 2) + "'", "tbl_JoBCardSerialNo", con, tran);
                    if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows.Count > 0)
                    {
                        strEngQty = 0; strAltQty = 0; strCpyQty = 0; strBatQty = 0;
                        int SrNok = 0;
                        JPEng = 0; JPAlt = 0; JPBat = 0; JPCPY = 0;
                        //Jpriority = 0;
                        strKVA = "";
                        strBatCnt = 0;
                        for (int k = 0; k < dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows.Count; k++)
                        {
                            strKVA = ComCon.getTranName("SELECT KVA FROM Part WHERE PartCode='" + Dts[1].Trim() + "' and Active='1'", "tblPart", "KVA", con, tran);
                            //for Priority
                            #region
                            if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "001")
                            {
                                JPEng = JPEng + 1;
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "002")
                            {
                                JPAlt = JPAlt + 1;
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "401")
                            {
                                JPCPY = JPCPY + 1;
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "010" && double.Parse(strKVA.Trim()) <= 160)
                            {
                                JPBat = JPBat + 1;
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "010" && double.Parse(strKVA.Trim()) >= 180)
                            {
                                if (strBatCnt == 0)
                                {
                                    JPBat = JPBat + 1;
                                    strBatCnt = 1;
                                }
                                else if (strBatCnt == 1)
                                {
                                    JPBat = JPBat + 0;
                                    strBatCnt = 0;
                                }
                            }
                            #endregion
                            //for Priority
                            #region
                            SrNok += 1;
                            sb.Remove(0, sb.Length);
                            sb.Append("insert into JobCardDetailsSub(JobCode,SrNo,PartCode,SrNoPartCode,SerialNo,JPriority,TransferCode,Transferstatus,stage1Status,stage2Status) ");
                            sb.Append("values('" + JobCardNo.Trim() + "','" + SrNok + "','" + Dts[1].Trim() + "',");
                            sb.Append("'" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim() + "',");
                            sb.Append("'" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SerialNo"].ToString().Trim() + "',");
                            //Jpri
                            if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "001")
                            {
                                sb.Append("" + JPEng + ",");
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "002")
                            {
                                sb.Append("" + JPAlt + ",");
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "401")
                            {
                                sb.Append("" + JPCPY + ",");
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "010")
                            {
                                sb.Append("" + JPBat + ",");
                            }
                            //Jpri

                            //TrfCode,Trfstatus
                            if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Gcode"].ToString().Trim().Substring(0, 3) == "MTF")
                            {
                                sb.Append("'" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Gcode"].ToString().Trim() + "','D',");
                            }
                            else
                            {
                                sb.Append("'" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Gcode"].ToString().Trim() + "','P',");
                            }
                            //TrfCode,Trfstatus

                            if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "401" ||
                                dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "010")
                            {
                                sb.Append("'D','D')");
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "001" ||
                                dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "002")
                            {
                                sb.Append("'P','P')");
                            }

                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "001"
                                || dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "002"
                                || dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "010")
                            {
                                if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Gcode"].ToString().Trim().Substring(0, 3) == "GIR")
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update GIIRDetailsSub set JobCardStatus='J' where  GiirCode='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Gcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    sb.Append("and PartCode='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim() + "'");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Gcode"].ToString().Trim().Substring(0, 3) == "MTF")
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update MTFDetailsSub  set JobCardStatus='J'  where  MTFCode='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Gcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    sb.Append("and PartCode='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim() + "'");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update GIIRDetailsSub set JobCardStatus='J' where  SerialNo='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    sb.Append("and PartCode='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim() + "'");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "401")
                            {
                                if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Gcode"].ToString().Trim().Substring(0, 3) == "PSH")
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update ProcessFeedbackDetailsSub set JobCardStatus='J' where  PfBCode='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Gcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    sb.Append("and PartCode='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim() + "'");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Gcode"].ToString().Trim().Substring(0, 3) == "MTF")
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update ProcessFeedbackDetailsSub set JobCardStatus='J' where TRFCode='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Gcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    sb.Append("and PartCode='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim() + "'");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    sb.Remove(0, sb.Length);
                                    sb.Append("Update MTFDetailsSub set JobCardStatus='J' where MTFCode='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Gcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SerialNo"].ToString().Trim() + "' ");
                                    sb.Append("and PartCode='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim() + "'");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                            }

                            //checked  SrNo To JobCardqty
                            #region
                            if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "001")
                            {
                                strEngQty = strEngQty + 1;
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "002")
                            {
                                strAltQty = strAltQty + 1;
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 3) == "010")
                            {
                                strBatQty = strBatQty + 1;
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["PartCode"].ToString().Trim().Substring(0, 2) == "40")
                            {
                                strCpyQty = strCpyQty + 1;
                            }
                            #endregion
                            //checked  SrNo To JobCardqty
                            #endregion
                        }
                        //checked  SrNo To JobCardqty
                        #region
                        if (int.Parse(Dts[2].Trim()) > strEngQty)
                        {
                            JobCardNo = "Engine SrNo Not available For DG " + ComCon.getName("select partdesc From part where Partcode='" + Dts[1].Trim() + "'", "TblPart", "Partdesc");
                            return JobCardNo;
                        }
                        else if (int.Parse(Dts[2].Trim()) > strAltQty)
                        {
                            JobCardNo = "Alternator SrNo Not available For DG " + ComCon.getName("select partdesc From part where Partcode='" + Dts[1].Trim() + "'", "TblPart", "Partdesc");
                            return JobCardNo;
                        }
                        else if (int.Parse(Dts[2].Trim()) > strBatQty)
                        {
                            if (checkTranBOMForBat(con, tran, Dts[1].Trim()) == true)
                            {
                                JobCardNo = "Battery SrNo Not available For DG " + ComCon.getName("select partdesc From part where Partcode='" + Dts[1].Trim() + "'", "TblPart", "Partdesc");
                                return JobCardNo;
                            }
                        }
                        else if (int.Parse(Dts[2].Trim()) > strCpyQty)
                        {
                            JobCardNo = "Canopy SrNo Not available For DG  " + ComCon.getName("select partdesc From part where Partcode='" + Dts[1].Trim() + "'", "TblPart", "Partdesc");
                            return JobCardNo;
                        }
                        #endregion
                        //checked  SrNo To JobCardqty
                    }
                }
                tran.Commit();

                return JobCardNo;
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

        public Boolean checkTranBOMForBat(SqlConnection con, SqlTransaction tran, string productcode)
        {
            Boolean BOMForBat = false;
            CommonCon ComCon = new CommonCon();
            dsChkBat = ComCon.procTranDS("Select  isNull(Count(Partcode),0)  as BatPart From BomDetails where kitcode='" + productcode + "' and Partcode like '010%'", "tbl_JoBCardBat", con, tran);
            if (dsChkBat != null && dsChkBat.Tables["tbl_JoBCardBat"].Rows.Count == 1)
            {
                if (int.Parse(dsChkBat.Tables["tbl_JoBCardBat"].Rows[0]["BatPart"].ToString().Trim()) > 0)

                {
                    BOMForBat = true;
                }
                else
                {
                    BOMForBat = false;
                }
            }
            else
            {
                BOMForBat = false;

            }
            return BOMForBat;

        }

        public string Submit2(JobCard2Request jobreq2)
        {
            int strEngQty = 0, strAltQty = 0, strCpyQty = 0, strBatQty = 0, strCPQty = 0;

            string JobCardNo = "";
            string panelTypeId = "0";
            CommonCon ComCon = new CommonCon();

            #region
            int recCountV = ComCon.CountChars(jobreq2.JobCard2Dts, ",");
            string[] strJCDDtsV = Regex.Split(jobreq2.JobCard2Dts, ",");
            int SrNoV = 0;

            for (int cSubV = 0; cSubV <= recCountV; cSubV++)
            {
                SrNoV += 1;
                string[] DtsV = Regex.Split(strJCDDtsV[cSubV].ToString().Trim(), "-->");
                if (DtsV[3].Trim() == "0")
                {
                    JobCardNo = "Panel Not Selected For DG " + ComCon.getName("select partdesc From part where Partcode='" + DtsV[1].Trim() + "'", "TblPart", "Partdesc");
                    return JobCardNo;
                }
                else
                {
                    dsDetailsSubPV = ComCon.procTranDS("exec GetJobCardSrNo 'DGWIP','" + DtsV[1] + "'," + DtsV[2].Trim() + ", '" + jobreq2.PCCode.Trim().Substring(0, 2) + "'", "tbl_JoBCardSerialNoV", con, tran);
                    if (dsDetailsSubPV != null && dsDetailsSubPV.Tables["tbl_JoBCardSerialNoV"].Rows.Count > 0)
                    {
                        for (int jV = 0; jV < dsDetailsSubPV.Tables["tbl_JoBCardSerialNoV"].Rows.Count; jV++)
                        {

                        }
                    }
                    else
                    {
                        JobCardNo = "JobCard1(Without Panel) Not available For DG " + ComCon.getName("select partdesc From part where Partcode='" + DtsV[1].Trim() + "'", "TblPart", "Partdesc");
                        return JobCardNo;
                    }
                }
            }

            #endregion
            //Check Srno Availability
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                JobCardNo = ComCon.GetMaxNo("JobCard2", "JCP", jobreq2.PCCode.Trim().Substring(0, 2), con, tran);

                SqlCommand cmd = new SqlCommand();
                sb.Remove(0, sb.Length);
                sb.Append("INSERT INTO JobCard2(JobCode,Dt,Yr,MaxSrNo,PCCode,Remark,CompanyCode,Active,Auth)");
                sb.Append(" VALUES('" + JobCardNo.Trim() + "',GetDate(),'" + ComCon.yearEnd(con, tran) + "','" + (JobCardNo.Substring(10, 8)) + "',");
                sb.Append("'" + jobreq2.PCCode.Trim() + "','" + jobreq2.Remark.Trim() + "','" + jobreq2.PCCode.Trim().Substring(0, 2) + "','1','1')");
                cmd = new SqlCommand(sb.ToString(), con);
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                int recCount = ComCon.CountChars(jobreq2.JobCard2Dts, ",");
                string[] strJCDDts = Regex.Split(jobreq2.JobCard2Dts, ",");
                int SrNo = 0;
                string strModel = "", strPhase = "";

                for (int cSub = 0; cSub <= recCount; cSub++)
                {
                    SrNo += 1;
                    string[] Dts = Regex.Split(strJCDDts[cSub].ToString().Trim(), "-->");
                    panelTypeId = "0";
                    if (Dts[3].Trim() == "SPL")
                    {
                        panelTypeId = "150";
                    }
                    else
                    {
                        strModel = ""; strPhase = "";
                        strModel = ComCon.getTranName("select Left(Model,5) as Model From Part where Partcode='" + Dts[1].Trim() + "'", "Part", "Model", con, tran);
                        strPhase = ComCon.getTranName("select Phase From Part where Partcode = '" + Dts[1].Trim() + "'", "Part", "Phase", con, tran);

                        /*
                        if (Convert.ToDouble(strPhase) == 1 && strModel == "HA294")
                        {
                            panelTypeId = "238";
                        }
                        else if (Convert.ToDouble(strPhase) == 3 && strModel == "HA294")
                        {
                            panelTypeId = "239";
                        }
                        else
                        */

                        if ((ComCon.getTranName("select Right(Model, 6) as DGType From Part where Partcode = '" + Dts[1].Trim() + "'", "Part", "DGType", con, tran)) == "iGreen")
                        {
                            panelTypeId = ComCon.getTranName("select PanelTypeId From PanelTypeKit where PanelTypeName='" + Dts[3].Trim() + "' and DGKVA=(select KVA From Part where Partcode='" + Dts[1].Trim() + "') and DGPhase=(select Phase From Part where Partcode='" + Dts[1].Trim() + "')  and DGType='iGreen' ", "PanelTypeKit", "PanelTypeId", con, tran);
                        }
                        else
                        {
                            panelTypeId = ComCon.getTranName("select PanelTypeId From PanelTypeKit where PanelTypeName='" + Dts[3].Trim() + "' and DGKVA=(select KVA From Part where Partcode='" + Dts[1].Trim() + "') and DGPhase=(select Phase From Part where Partcode='" + Dts[1].Trim() + "')  and DGType='KG' ", "PanelTypeKit", "PanelTypeId", con, tran);
                        }
                    }

                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO JobCard2Details(JobCode,SrNo,BOMCode,PartCode,Qty,PanelType)");
                    sb.Append(" VALUES('" + JobCardNo.Trim() + "','" + SrNo + "','" + Dts[0].Trim() + "','" + Dts[1].Trim() + "'," + Dts[2].Trim() + "," + panelTypeId + ")");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    dsDetailsSub = ComCon.procTranDS("exec GetJobCardSrNo 'DGWIP','" + Dts[1] + "'," + Dts[2].Trim() + ", '" + jobreq2.PCCode.Trim().Substring(0, 2) + "'", "tbl_JoBCardSerialNo", con, tran);

                    if (dsDetailsSub != null && dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows.Count > 0)
                    {
                        strEngQty = 0; strAltQty = 0; strCpyQty = 0; strBatQty = 0;

                        int SrNok = 0;
                        //int JPEng = 0; JPAlt = 0; JPBat = 0; JPCPY = 0;
                        for (int k = 0; k < dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows.Count; k++)
                        {
                            #region
                            SrNok += 1;
                            sb.Remove(0, sb.Length);
                            sb.Append("insert into JobCard2DetailsSub(JobCode,SrNo,PartCode,PanelType,TransCode,SrNoPartCode,SerialNo,Stage3Status,JobCard1,J2Priority) ");
                            sb.Append("values('" + JobCardNo.Trim() + "','" + SrNok + "','" + Dts[1].Trim() + "','" + panelTypeId + "',");
                            sb.Append("'" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["JobCode"].ToString().Trim() + "',");
                            sb.Append("'" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SrNoPartcode"].ToString().Trim() + "',");
                            sb.Append("'" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SerialNo"].ToString().Trim() + "',");
                            sb.Append("'" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["Stage3Status"].ToString().Trim() + "',");
                            if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 5) == "00002")
                            {
                                sb.Append("'" + ComCon.getTranName("select Jds.JobCode from Giirdetailssub Gs inner Join JobCardDetailsSub Jds on gs.SerialNo = jds.SerialNo and gs.PartCode = jds.SrNoPartCode where gs.Partcode = '" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["EPartcode"].ToString().Trim() + "'  and KRMNo = '" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SerialNo"].ToString().Trim() + "'  ", "JobCard", "JobCode", con, tran) + "',");
                                sb.Append("'" + ComCon.getTranName("select Jds.JPriority from Giirdetailssub Gs inner Join JobCardDetailsSub Jds on gs.SerialNo = jds.SerialNo and gs.PartCode = jds.SrNoPartCode where gs.Partcode = '" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["EPartcode"].ToString().Trim() + "'  and KRMNo = '" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SerialNo"].ToString().Trim() + "'  ", "JobCard", "JPriority", con, tran) + "')");
                            }
                            else
                            {
                                sb.Append("'" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["JobCode"].ToString().Trim() + "',");
                                sb.Append("'" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["JPriority"].ToString().Trim() + "')");
                            }

                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            #region

                            sb.Remove(0, sb.Length);
                            sb.Append("Update JobCardDetailsSub set JobCard2Status='J' where  SerialNo='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SerialNo"].ToString().Trim() + "' and JobCode='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["JobCode"].ToString().Trim() + "' ");
                            sb.Append("and SrNoPartCode='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SrNoPartcode"].ToString().Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "001")
                            {
                                sb.Remove(0, sb.Length);
                                sb.Append("Update JobCardDetails set JobCard2Qty=JobCard2Qty+1 where JobCode='" + dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["JobCode"].ToString().Trim() + "' ");
                                sb.Append(" and PartCode='" + Dts[1] + "'");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }

                            //checked  SrNo To JobCardqty
                            #region
                            if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "001")
                            {
                                strEngQty = strEngQty + 1;
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "002")
                            {
                                strAltQty = strAltQty + 1;
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 3) == "010")
                            {
                                strBatQty = strBatQty + 1;
                            }
                            else if (dsDetailsSub.Tables["tbl_JoBCardSerialNo"].Rows[k]["SrNoPartcode"].ToString().Trim().Substring(0, 2) == "40")
                            {
                                strCpyQty = strCpyQty + 1;
                            }
                            //#endregion
                            ////checked  SrNo To JobCardqty
                            #endregion
                            #endregion
                        }

                        //checked SrNo To JobCardqty

                        if (int.Parse(Dts[2].Trim()) > strEngQty)
                        {
                            JobCardNo = "Engine SrNo Not available For DG " + ComCon.getName("select partdesc From part where Partcode='" + Dts[1].Trim() + "'", "TblPart", "Partdesc");
                            return JobCardNo;
                        }
                        else if (int.Parse(Dts[2].Trim()) > strAltQty)
                        {
                            JobCardNo = "Alternator SrNo Not available For DG " + ComCon.getName("select partdesc From part where Partcode='" + Dts[1].Trim() + "'", "TblPart", "Partdesc");
                            return JobCardNo;
                        }
                        else if (int.Parse(Dts[2].Trim()) > strBatQty)
                        {
                            if (checkTranBOMForBat(con, tran, Dts[1].Trim()) == true)
                            {
                                JobCardNo = "Battery SrNo Not available For DG " + ComCon.getName("select partdesc From part where Partcode='" + Dts[1].Trim() + "'", "TblPart", "Partdesc");
                                return JobCardNo;
                            }
                        }
                        else if (int.Parse(Dts[2].Trim()) > strCpyQty)
                        {
                            JobCardNo = "Canopy SrNo Not available For DG  " + ComCon.getName("select partdesc From part where Partcode='" + Dts[1].Trim() + "'", "TblPart", "Partdesc");
                            return JobCardNo;
                        }
                        #endregion
                        //checked  SrNo To JobCardqty

                    }

                    //CP SrNo
                    #region
                    //For Job pri
                    if (double.Parse(panelTypeId) != 0 && double.Parse(panelTypeId) != 150)
                    {
                        dsDetailsSubP = ComCon.procTranDS("Select J2Priority,JobCard1 From jobcard2detailssub where JobCode='" + JobCardNo.Trim() + "'  and SrNoPartcode Like '001%' ", "tbl_JoBCardEngPri", con, tran);

                        if (dsDetailsSubP != null && dsDetailsSubP.Tables["tbl_JoBCardEngPri"].Rows.Count > 0)
                        {
                            for (int j = 0; j < dsDetailsSubP.Tables["tbl_JoBCardEngPri"].Rows.Count; j++)
                            {
                                if (double.Parse(panelTypeId) != 0)
                                {
                                    dsDetailsCP = ComCon.procTranDS("exec GetCPPartcode " + panelTypeId + ",'0'", "tbl_JoBCardCPPart", con, tran);
                                }

                                if (dsDetailsCP != null && dsDetailsCP.Tables["tbl_JoBCardCPPart"].Rows.Count > 0)
                                {
                                    strCPQty = 0;

                                    for (int p = 0; p < dsDetailsCP.Tables["tbl_JoBCardCPPart"].Rows.Count; p++)
                                    {
                                        dsDetailsCPSRNo = ComCon.procTranDS("exec GetCPPartcode 1,'" + dsDetailsCP.Tables["tbl_JoBCardCPPart"].Rows[p]["PanelTypePartcode"].ToString().Trim() + "'", "tbl_JoBCardSerialNoCP", con, tran);

                                        if (dsDetailsCPSRNo != null && dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows.Count > 0)
                                        {
                                            int SrNoP = 0;

                                            for (int p1 = 0; p1 < dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows.Count; p1++)
                                            {
                                                //,stage3status
                                                SrNoP += 1;
                                                sb.Remove(0, sb.Length);
                                                sb.Append("insert into JobCard2DetailsSub(JobCode,SrNo,PartCode,PanelType,JobCard1,J2Priority,TransCode,Stage3Status,SrNoPartCode,SerialNo) ");
                                                sb.Append("values('" + JobCardNo.Trim() + "','" + SrNoP + "','" + Dts[1].Trim() + "','" + panelTypeId + "',");
                                                sb.Append("'" + dsDetailsSubP.Tables["tbl_JoBCardEngPri"].Rows[j]["JobCard1"].ToString().Trim() + "',");
                                                sb.Append("'" + dsDetailsSubP.Tables["tbl_JoBCardEngPri"].Rows[j]["J2Priority"].ToString().Trim() + "',");
                                                sb.Append("'" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["GCode"].ToString().Trim() + "',");
                                                //if (dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["GCode"].ToString().Trim().Substring(0, 3) == "MTF")
                                                //{
                                                //    sb.Append("'D',");
                                                //}
                                                //else
                                                //{
                                                //    sb.Append("'P',");
                                                //}
                                                sb.Append("'" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["TRFStatus"].ToString().Trim() + "',");
                                                sb.Append("'" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["Partcode"].ToString().Trim() + "',");
                                                sb.Append("'" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["SerialNo"].ToString().Trim() + "')");
                                                cmd = new SqlCommand(sb.ToString(), con);
                                                cmd.Transaction = tran;
                                                cmd.ExecuteNonQuery();
                                                cmd.Dispose();

                                                if (dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["Gcode"].ToString().Trim().Substring(0, 3) == "PSH")
                                                {
                                                    sb.Remove(0, sb.Length);
                                                    sb.Append("Update ProcessFeedbackDetailsSub set JobCardStatus='J' where  PfBCode='" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["Gcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["SerialNo"].ToString().Trim() + "' ");
                                                    sb.Append("and PartCode='" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["PartCode"].ToString().Trim() + "'");
                                                    cmd = new SqlCommand(sb.ToString(), con);
                                                    cmd.Transaction = tran;
                                                    cmd.ExecuteNonQuery();
                                                    cmd.Dispose();
                                                }
                                                else if (dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["Gcode"].ToString().Trim().Substring(0, 3) == "MTF")
                                                {
                                                    sb.Remove(0, sb.Length);
                                                    sb.Append("Update MTFDetailsSub set JobCardStatus='J' where  MTFCode='" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["Gcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["SerialNo"].ToString().Trim() + "' ");
                                                    sb.Append("and PartCode='" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["PartCode"].ToString().Trim() + "'");
                                                    cmd = new SqlCommand(sb.ToString(), con);
                                                    cmd.Transaction = tran;
                                                    cmd.ExecuteNonQuery();
                                                    cmd.Dispose();

                                                    sb.Remove(0, sb.Length);
                                                    sb.Append("Update ProcessFeedbackDetailsSub set JobCardStatus='J' where  TRFCode='" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["Gcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["SerialNo"].ToString().Trim() + "' ");
                                                    sb.Append("and PartCode='" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["PartCode"].ToString().Trim() + "'");
                                                    cmd = new SqlCommand(sb.ToString(), con);
                                                    cmd.Transaction = tran;
                                                    cmd.ExecuteNonQuery();
                                                    cmd.Dispose();

                                                }
                                                else if (dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["Gcode"].ToString().Trim().Substring(0, 3) == "GIR")
                                                {
                                                    sb.Remove(0, sb.Length);
                                                    sb.Append("Update giirDetailsSub set JobCardStatus='J',TRFStatus='D' where  giircode='" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["Gcode"].ToString().Trim() + "' and SerialNo='" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["SerialNo"].ToString().Trim() + "' ");
                                                    sb.Append("and PartCode='" + dsDetailsCPSRNo.Tables["tbl_JoBCardSerialNoCP"].Rows[p1]["PartCode"].ToString().Trim() + "'");
                                                    cmd = new SqlCommand(sb.ToString(), con);
                                                    cmd.Transaction = tran;
                                                    cmd.ExecuteNonQuery();
                                                    cmd.Dispose();
                                                }
                                                strCPQty = strCPQty + 1;
                                            }
                                        }
                                        else
                                        {
                                            JobCardNo = "CP SrNo Not available For DG " + ComCon.getName("select partdesc From part where Partcode='" + Dts[1].Trim() + "'", "TblPart", "Partdesc") + " and CP Type " + Dts[3].Trim();
                                            return JobCardNo;
                                        }

                                    }
                                }
                            }
                            //CP Priority
                        }
                    }
                    #endregion
                    //CP SrNo

                    //Update in Jobcard

                    //Update in Jobcard
                }
                tran.Commit();
                //tran.Rollback();
                return JobCardNo;
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

        public DataTable GetTRAllocate(string MofCode, string Partcode, int DiQty)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetTRAllocationDtls", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@MofCode", SqlDbType.Char).Value = MofCode;
            dAd.SelectCommand.Parameters.Add("@Partcode", SqlDbType.Char).Value = Partcode;
            dAd.SelectCommand.Parameters.Add("@DiQty", SqlDbType.Int).Value = DiQty;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetDGAllocationDts(string Type, string P_FromDt, string P_ToDt)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("DGAllocationDts", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            //dAd.SelectCommand.Parameters.Add("@Code", SqlDbType.Char).Value = JobCardNo;
            dAd.SelectCommand.Parameters.Add("@FromDt", SqlDbType.Char).Value = P_FromDt;
            dAd.SelectCommand.Parameters.Add("@ToDt", SqlDbType.Char).Value = P_ToDt;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string SubmitDiTRAllocate(DITRAllocationRequest DiTrAlloReq)
        {
            string DiTRAllocate = "";
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                CommonCon ComCon = new CommonCon();
                int recCount = ComCon.CountChars(DiTrAlloReq.DiTRDts, ",");
                string[] strDiTrAllo = Regex.Split(DiTrAlloReq.DiTRDts, ",");
                int SrNo = 0;
                for (int cSub = 0; cSub <= recCount; cSub++)
                {
                    SrNo += 1;
                    string[] Dts = Regex.Split(strDiTrAllo[cSub].ToString().Trim(), "-->");
                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO DispatchInstructionDetails(DiNo,SrNo,RDGCode,AllocDate,RDGQty,PPWCode)");
                    sb.Append(" VALUES('" + Dts[0].Trim() + "','" + SrNo + "','" + Dts[1].Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:MM") + "','1','0')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    sb.Remove(0, sb.Length);
                    sb.Append("Update TestReport set DIStatus='C' where  TRCode='" + Dts[1].Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                tran.Commit();
                DiTRAllocate = "Allocation Done ";
                return DiTRAllocate;
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