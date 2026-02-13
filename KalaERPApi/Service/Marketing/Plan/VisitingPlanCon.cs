using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using KalaERPApi.Models.Request.Marketing.Plan;
using System.IO;
using KalaERPApi.Models.Marketing.Plan;

namespace KalaERPApi.Service.Marketing.Plan
{
    public class VisitingPlanCon
    {
        #region
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        CommonCon ComCon = new CommonCon();
        public SqlTransaction tran = null;
        SqlCommand cmd = null;
        #endregion

        public DataTable GetVisitPlanLocation(string Type, string VPPlanNo, string P_PCCode, string P_EmpCode, string P_FromDt, string P_ToDt)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("[GetVisitPlanLocation_sp]", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.Parameters.Add("@VPlanNo", SqlDbType.Char).Value = VPPlanNo;
            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = P_PCCode;
            dAd.SelectCommand.Parameters.Add("@EmpCode", SqlDbType.Char).Value = P_EmpCode;
            dAd.SelectCommand.Parameters.Add("@FromDt", SqlDbType.Char).Value = P_FromDt;
            dAd.SelectCommand.Parameters.Add("@ToDt", SqlDbType.Char).Value = P_ToDt;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public DataTable GetAllVisitPlanRpt(string Type, string VPPlanNo, string P_PCCode, string P_EmpCode, string P_FromDt, string P_ToDt)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("getVisitPlanMktRpt_sp", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = Type;
            dAd.SelectCommand.Parameters.Add("@VPlanNo", SqlDbType.Char).Value = VPPlanNo;
            dAd.SelectCommand.Parameters.Add("@PCCode", SqlDbType.Char).Value = P_PCCode;
            dAd.SelectCommand.Parameters.Add("@EmpCode", SqlDbType.Char).Value = P_EmpCode;
            dAd.SelectCommand.Parameters.Add("@FromDt", SqlDbType.Char).Value = P_FromDt;
            dAd.SelectCommand.Parameters.Add("@ToDt", SqlDbType.Char).Value = P_ToDt;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string Submit(VisitingPlanRequest VisitPlanreq)
        {
            #region
            DataSet dsVPDetails;
            int recCount;
            string[] strPlanDts;
            string[] strFileDts;
            int SrNo;
            int SrAtaachNo;
            int cSub;
            string[] Dts;
            string StrDispCode = "";
            String StrOdGatePassCode = "";
            string StrDisplayMsg = "";
            string deletepath = "";
            String Reason = "";
            String ClientName = "";
            #endregion

            #region           
            //if (VisitPlanreq.StrType.Trim() == "AddNew" || VisitPlanreq.StrType.Trim() == "Update")
            if (VisitPlanreq.StrType.Trim() == "AddNew")
            {
                // fromdate and Today Date is same then = 0
                // fromdate  less than Today Date is same then = -1 
                // fromdate Grater than Today Date is same then = 1

                if (DateTime.Compare(Convert.ToDateTime(VisitPlanreq.FromDate.Trim()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) < 0)
                {
                    StrDisplayMsg = "From Date must be same as todays date .. !";
                    return StrDisplayMsg;
                }
                if (DateTime.Compare(Convert.ToDateTime(VisitPlanreq.ToDate.Trim()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) < 0)
                {
                    StrDisplayMsg = "To Date must be Same as todays date .. !";
                    return StrDisplayMsg;
                }

                if (DateTime.Compare(Convert.ToDateTime(VisitPlanreq.ToDate.Trim()), Convert.ToDateTime(VisitPlanreq.FromDate.Trim())) < 0)
                {
                    StrDisplayMsg = "To date must be greater than or equal to from date.. !";
                    return StrDisplayMsg;
                }

                //if (DateTime.Compare(Convert.ToDateTime(VisitPlanreq.FromDate.Trim()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) > 0)
                //{
                //    StrDisplayMsg = " From Date must be same as today date .. !";
                //    return StrDisplayMsg;
                //}

                //if (DateTime.Compare(Convert.ToDateTime(VisitPlanreq.ToDate.Trim()), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) > 0)
                //{
                //    StrDisplayMsg = " To Date must be same as today date .. !";
                //    return StrDisplayMsg;
                //}                               


                //Code to check OD Filled for Selected Date
                int cntEmp = int.Parse(ComCon.getName("Select COUNT(EmpCode)AS CntEmp From OutDoorDuty Where (Fromdate between '" + VisitPlanreq.FromDate.Trim() + " 00:00:00" + "' AND '" + VisitPlanreq.ToDate.Trim() + " 23:59:59" + "' or Todate between '" + VisitPlanreq.FromDate.Trim() + " 00:00:00" + "'  AND '" + VisitPlanreq.ToDate.Trim() + " 23:59:59" + "' ) AND Active='1' AND EmpCode='" + VisitPlanreq.EmpCode.Trim() + "'", "OutDoorDuty", "CntEmp"));
                if (cntEmp > 0)
                {
                    StrDisplayMsg = "OutDoorDuty already filled for Selected Date, Please Uncheck OutDoor Duty and select other Option !";
                    return StrDisplayMsg;
                }
            }
            #endregion

            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                //Save Visit Plan
                if (VisitPlanreq.StrType.Trim() == "AddNew")
                {
                    #region
                    StrDispCode = ComCon.GetMaxNo("VisitPlanMkt", "VPM", VisitPlanreq.CompCode.Trim(), con, tran);

                    cmd = new SqlCommand("InsertVisitPlanMKT", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@VPCode", StrDispCode.Trim());
                    cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Yr", StrDispCode.Substring(4, 5));
                    cmd.Parameters.AddWithValue("@MaxSrNo", StrDispCode.Substring(10, 8));
                    cmd.Parameters.AddWithValue("@PCCode", VisitPlanreq.PCCode.Trim());
                    cmd.Parameters.AddWithValue("@EmpCode", VisitPlanreq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@FromDate", VisitPlanreq.FromDate.Trim() + " 09:30:00");
                    cmd.Parameters.AddWithValue("@ToDate", VisitPlanreq.ToDate.Trim() + " 18:00:00");
                    cmd.Parameters.AddWithValue("@Remark", VisitPlanreq.Remark.Trim());
                    cmd.Parameters.AddWithValue("@ODType", VisitPlanreq.ODType.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", VisitPlanreq.CompCode.Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Details                    
                    strPlanDts = Regex.Split(VisitPlanreq.PlanDts, "@#@");
                    SrNo = 0;
                    foreach (string StrSub in strPlanDts)
                    {
                        SrNo += 1;
                        Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                        cmd = new SqlCommand("InsertVisitPlanMKTDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@VPCode", StrDispCode.Trim());
                        cmd.Parameters.AddWithValue("@SrNo", SrNo);

                        if (SrNo == 1)
                        {
                            ClientName = Dts[1].ToString().Trim();
                            Reason = Dts[2].ToString().Trim();
                        }

                        cmd.Parameters.AddWithValue("@ClientName", Dts[1].ToString().Trim());
                        cmd.Parameters.AddWithValue("@Reason", Dts[2].ToString().Trim());
                        cmd.Parameters.AddWithValue("@SourceFrom", Dts[3].ToString().Trim());
                        cmd.Parameters.AddWithValue("@DestinationTo", Dts[4].ToString().Trim());

                        cmd.Parameters.AddWithValue("@Feedback", "");

                        cmd.Parameters.AddWithValue("@TotalKm", double.Parse(Dts[5].ToString().Trim()));
                        cmd.Parameters.AddWithValue("@RateKm", double.Parse(Dts[6].ToString().Trim()));
                        cmd.Parameters.AddWithValue("@TravExps", double.Parse(Dts[7].ToString().Trim()));
                        cmd.Parameters.AddWithValue("@LunchDinner", double.Parse(Dts[8].ToString().Trim()));
                        cmd.Parameters.AddWithValue("@Lodge", double.Parse(Dts[9].ToString().Trim()));
                        cmd.Parameters.AddWithValue("@OtherExps", double.Parse(Dts[10].ToString().Trim()));

                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    //****************User Acivity****************
                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", VisitPlanreq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "VistiPlanMkt");
                    cmd.Parameters.AddWithValue("@TransactionNo", StrDispCode.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", VisitPlanreq.CompCode.Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Gate Pass Or OD Save Code
                    if (VisitPlanreq.ODType.Trim() == "OD")
                    {
                        //OD CODE 
                        #region
                        StrOdGatePassCode = ComCon.GetMaxNo("OutDoorDuty", "OTD", VisitPlanreq.CompCode.Trim(), con, tran);
                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO OutDoorDuty ");
                        sb.Append("(ODCode,Dt,Yr,MaxSrNo,PCCode,EmpCode,ClientName,FromDate,ToDate,TotalDays,Reason,CompanyCode,VPCode,Active)");
                        sb.Append(" VALUES('" + StrOdGatePassCode.Trim() + "', '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                        sb.Append("'" + ComCon.yearEnd(con, tran) + "', '" + (StrOdGatePassCode.Substring(10, 8)) + "',");
                        sb.Append("'" + VisitPlanreq.PCCode.Trim() + "', '" + VisitPlanreq.EmpCode.Trim() + "',");
                        sb.Append("'" + ClientName.Trim() + "',");
                        sb.Append("'" + VisitPlanreq.FromDate.Trim() + " 09:30:00','" + VisitPlanreq.ToDate.Trim() + " 18:00:00','1',");
                        sb.Append("'" + Reason.Trim() + "',");
                        sb.Append("'" + VisitPlanreq.CompCode.Trim() + "','" + StrDispCode.Trim() + "','1')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //Details   
                        strPlanDts = null;
                        strPlanDts = Regex.Split(VisitPlanreq.PlanDts, "@#@");
                        SrNo = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNo += 1;
                            Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO OutDoorDutyDetails(ODCode,SrNo,ClientName,Reason,SourceFrom,DestinationTo,TotalKm,RateKm,TravExps,LunchDinner,Lodge,OtherExps)");
                            sb.Append(" VALUES('" + StrOdGatePassCode.Trim() + "','" + SrNo + "',");
                            sb.Append("'" + Dts[1].ToString().Trim() + "',");
                            sb.Append("'" + Dts[2].ToString().Trim() + "',");
                            sb.Append("'" + Dts[3].ToString().Trim() + "',");
                            sb.Append("'" + Dts[4].ToString().Trim() + "',");
                            sb.Append("'" + double.Parse(Dts[5].ToString().Trim()) + "',");
                            if (double.Parse(Dts[5].ToString().Trim()) > 0)
                            {
                                sb.Append("'" + double.Parse(Dts[6].ToString().Trim()) + "',");
                            }
                            else
                            {
                                sb.Append(" '0',");
                            }
                            sb.Append("'" + double.Parse(Dts[7].ToString().Trim()) + "',");
                            sb.Append("'" + double.Parse(Dts[8].ToString().Trim()) + "',");
                            sb.Append("'" + double.Parse(Dts[9].ToString().Trim()) + "',");
                            sb.Append("'" + double.Parse(Dts[10].ToString().Trim()) + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }

                        //****************User Acivity****************
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", VisitPlanreq.EmpCode.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "OutDoorDuty");
                        cmd.Parameters.AddWithValue("@TransactionNo", StrOdGatePassCode.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", VisitPlanreq.CompCode.Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        #endregion
                        StrDisplayMsg = "";
                        StrDisplayMsg = "Visting Plan Saved Successfully with : " + StrDispCode.Trim() + " and OutDoorDuty Code : " + StrOdGatePassCode.Trim();
                    }
                    else if (VisitPlanreq.ODType.Trim() == "GP")
                    {
                        //GatePass
                        #region
                        StrOdGatePassCode = ComCon.GetMaxNo("GatePass", "GPN", VisitPlanreq.CompCode.Trim(), con, tran);

                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO GatePass(GPNNo,Dt,Yr,MaxSrNo,PCCode,EmpCode,Reason,NatureofWork,GatePassType,Representing,");
                        sb.Append("VehicleNo,VisitorInDate,InTime,OutTime,IITime,IOTime,IIRemark,IORemark,HODRemark,SecurityRemark,IISecurityRemark," +
                                  "InRemark,OutRemark,WCStatus,WCCode,IOStatus,ReturnBack,CompanyCode,VPCode,Active,Auth,Status) ");
                        sb.Append(" VALUES('" + StrOdGatePassCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                        sb.Append("'" + ComCon.yearEnd(con, tran) + "','" + (StrOdGatePassCode.Substring(10, 8)) + "',");
                        sb.Append("'" + VisitPlanreq.PCCode.Trim() + "','" + VisitPlanreq.EmpCode.Trim() + "', 'Official', ");
                        sb.Append("'" + Reason.Trim() + "','S','','NIL','" + DBNull.Value.ToString() + "',NULL, NULL, NULL, NULL,");
                        sb.Append("'NIL', 'NIL', 'NIL', 'NIL', 'NIL','NIL', 'NIL', 'N','', 'P','N', ");
                        sb.Append("'" + VisitPlanreq.CompCode.Trim() + "','" + StrDispCode.Trim() + "','1', '0', '0')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //Details   
                        strPlanDts = null;
                        strPlanDts = Regex.Split(VisitPlanreq.PlanDts, "@#@");
                        SrNo = 0;
                        foreach (String StrSub in strPlanDts)
                        {
                            SrNo += 1;
                            Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            sb.Remove(0, sb.Length);
                            sb.Append("INSERT INTO GatePassDetails(GPNNo,SrNo,ClientName,Reason,SourceFrom,DestinationTo,TotalKm,RateKm,TravExps,LunchDinner,Lodge,OtherExps)");
                            sb.Append(" VALUES('" + StrOdGatePassCode.Trim() + "','" + SrNo + "',");
                            sb.Append("'" + Dts[1].ToString().Trim() + "',");
                            sb.Append("'" + Dts[2].ToString().Trim() + "',");
                            sb.Append("'" + Dts[3].ToString().Trim() + "',");
                            sb.Append("'" + Dts[4].ToString().Trim() + "',");
                            sb.Append("'" + double.Parse(Dts[5].ToString().Trim()) + "',");
                            if (double.Parse(Dts[5].ToString().Trim()) > 0)
                            {
                                sb.Append("'" + double.Parse(Dts[6].ToString().Trim()) + "',");
                            }
                            else
                            {
                                sb.Append(" '0',");
                            }
                            sb.Append("'" + double.Parse(Dts[7].ToString().Trim()) + "',");
                            sb.Append("'" + double.Parse(Dts[8].ToString().Trim()) + "',");
                            sb.Append("'" + double.Parse(Dts[9].ToString().Trim()) + "',");
                            sb.Append("'" + double.Parse(Dts[10].ToString().Trim()) + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }

                        //****************User Acivity****************
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", VisitPlanreq.EmpCode.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "S");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "GatePass");
                        cmd.Parameters.AddWithValue("@TransactionNo", StrOdGatePassCode.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", VisitPlanreq.CompCode.Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        #endregion
                        StrDisplayMsg = "";
                        StrDisplayMsg = "Visting Plan Saved Successfully with : " + StrDispCode.Trim() + " and GatePass : " + StrOdGatePassCode.Trim();
                    }
                    else if (VisitPlanreq.ODType.Trim() == "N")
                    {
                        StrDisplayMsg = "";
                        StrDisplayMsg = "Visting Plan Saved Successfully with : " + StrDispCode.Trim();
                    }
                    #endregion
                }
                //Update Visit Plan by MKT With in One Day Only
                else if (VisitPlanreq.StrType.Trim() == "Update")
                {
                    #region
                    StrDispCode = VisitPlanreq.VPCode.Trim();

                    //Check
                    // && (ComCon.getName("SELECT Count(VPCode) as Auth1  FROM VisitPlanMktDetails WHERE VPCode='" + VisitPlanreq.VPCode.Trim() + "' and AuthExpHod='1'  ", "VisitPlanMkt", "Auth1").ToString().Trim() == "0")

                    //lock to Update plan with in today date.
                    //if (
                    //    (ComCon.getName("SELECT Count(VPCode) as Auth1  FROM VisitPlanMkt WHERE VPCode='" + VisitPlanreq.VPCode.Trim() + "' AND Active='1' and (convert(char(10), ToDate, 126) = convert(char(10), GETDATE(), 126)) ", "VisitPlanMkt", "Auth1").ToString().Trim() == "1")
                    //   )
                    //{

                    if (
                       // Lock to check Expense Done Or not
                       (ComCon.getName("Select Count(ExpCode) as Auth1 from VisitPlanMktDetails WHERE VPCode='" + VisitPlanreq.VPCode.Trim() + "' and ExpCode like 'ERW%'", "VisitPlanMkt", "Auth1").ToString().Trim() == "0")
                       &&
                    //// lock to Update plan with in today date.
                    // (ComCon.getName("SELECT Count(VPCode) as Auth1  FROM VisitPlanMkt WHERE VPCode='" + VisitPlanreq.VPCode.Trim() + "' AND Active='1' and (convert(char(10), Getdate(), 126))  between (convert(char(10), FromDate, 126))  and (convert(char(10), ToDate, 126) ) ", "VisitPlanMkt", "Auth1").ToString().Trim() == "1")
                    // )
                    //New condition for Plan Updatation from Plan Date to Next 72 Hrs)
                    (Convert.ToDecimal(ComCon.getName("Select CAST(ABS(DATEDIFF(minute, FORMAT(ToDate,'yyyy-MM-dd 00:00:00'), GETDATE()) / 60) AS VARCHAR(8)) + '.' + right('0' + CAST(ABS(DATEDIFF(minute,  FORMAT(ToDate,'yyyy-MM-dd 00:00:00'),GETDATE()) % 60) AS VARCHAR(2)),2) as Diff  from VisitPlanMkt WHERE VPCode='" + VisitPlanreq.VPCode.Trim() + "' AND Active='1' ", "VisitPlanMkt", "Diff").ToString().Trim()) < 72)
                    )
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update VisitPlanMkt SET Remark='" + VisitPlanreq.Remark.Trim() + "' WHERE VPCode ='" + StrDispCode.Trim() + "' and active='1' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //Details              
                        strPlanDts = Regex.Split(VisitPlanreq.PlanDts, "@#@");
                        if (strPlanDts.Length > 0)
                        {
                            //// Delete Deatils 
                            //sb.Remove(0, sb.Length);
                            //sb.Append("delete from VisitPlanMktDetails WHERE VPCode='" + StrDispCode.Trim() + "'");
                            //cmd = new SqlCommand(sb.ToString(), con);
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();
                            foreach (string StrSub in strPlanDts)
                            {
                                Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                                SrNo = int.Parse(Dts[0].ToString().Trim());

                                //Add New Row in Existing Visting Plan
                                if (Dts[11].ToString().Trim() == "Yes")
                                {
                                    cmd = new SqlCommand("InsertVisitPlanMKTDetails", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@VPCode", StrDispCode.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", Dts[0].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@ClientName", Dts[1].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@Reason", Dts[2].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@SourceFrom", Dts[3].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@DestinationTo", Dts[4].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@FeedBack", "");
                                    cmd.Parameters.AddWithValue("@TotalKm", double.Parse(Dts[5].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@RateKm", double.Parse(Dts[6].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@TravExps", double.Parse(Dts[7].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@LunchDinner", double.Parse(Dts[8].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@Lodge", double.Parse(Dts[9].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@OtherExps", double.Parse(Dts[10].ToString().Trim()));
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                //Update Row in Existing Visting Plan
                                else
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append(" Update VisitPlanMktDetails SET ClientName='" + Dts[1].ToString().Trim() + "' ,");
                                    sb.Append(" Reason ='" + Dts[2].ToString().Trim() + "' ,");
                                    sb.Append(" SourceFrom ='" + Dts[3].ToString().Trim() + "' ,");
                                    sb.Append(" DestinationTo ='" + Dts[4].ToString().Trim() + "' ,");
                                    sb.Append(" FeedBack ='' ,");
                                    sb.Append(" TotalKm ='" + double.Parse(Dts[5].ToString().Trim()) + "' ,");
                                    sb.Append(" RateKm ='" + double.Parse(Dts[6].ToString().Trim()) + "' ,");
                                    sb.Append(" TravExps ='" + double.Parse(Dts[7].ToString().Trim()) + "' ,");
                                    sb.Append(" LunchDinner ='" + double.Parse(Dts[8].ToString().Trim()) + "' ,");
                                    sb.Append(" Lodge ='" + double.Parse(Dts[9].ToString().Trim()) + "' ,");
                                    sb.Append(" OtherExps ='" + double.Parse(Dts[10].ToString().Trim()) + "'  ");
                                    sb.Append(" WHERE VPCode='" + StrDispCode.Trim() + "' and  SrNo = '" + Dts[0].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                            }
                        }

                        //Gate Pass Or OD Save Code
                        if (VisitPlanreq.ODType.Trim() == "OD")
                        {
                            //OD CODE 
                            #region
                            StrOdGatePassCode = VisitPlanreq.VP_OD_GPNo.Trim();
                            sb.Remove(0, sb.Length);
                            sb.Append(" Delete From  OutDoorDutyDetails where ODCode = '" + VisitPlanreq.VP_OD_GPNo.Trim() + "'  ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            dsVPDetails = null;
                            sb.Append(" Select SrNo,ClientName,Reason,SourceFrom,DestinationTo,TotalKm,RateKm,TravExps,LunchDinner,Lodge,OtherExps from VisitPlanMktDetails where VpCode= '" + VisitPlanreq.VPCode.Trim() + "' Order by SrNo");
                            dsVPDetails = ComCon.procTranDS(sb.ToString(), "VPD", con, tran);
                            if (dsVPDetails.Tables["VPD"].Rows.Count > 0)
                            {
                                for (int D = 0; D < dsVPDetails.Tables["VPD"].Rows.Count; D++)
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("INSERT INTO OutDoorDutyDetails(ODCode,SrNo,ClientName,Reason,SourceFrom,DestinationTo,TotalKm,RateKm,TravExps,LunchDinner,Lodge,OtherExps)");
                                    sb.Append(" VALUES('" + VisitPlanreq.VP_OD_GPNo.Trim() + "','" + dsVPDetails.Tables["VPD"].Rows[D]["SrNo"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["ClientName"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["Reason"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["SourceFrom"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["DestinationTo"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["TotalKm"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["RateKm"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["TravExps"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["LunchDinner"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["Lodge"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["OtherExps"].ToString().Trim() + "')");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }

                                //****************User Acivity****************
                                //****************User Acivity****************
                                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                                cmd.Parameters.AddWithValue("@EmpID", VisitPlanreq.EmpCode.Trim());
                                cmd.Parameters.AddWithValue("@TransactionType", "U");
                                cmd.Parameters.AddWithValue("@TransactionFrom", "OutDoorDuty");
                                cmd.Parameters.AddWithValue("@TransactionNo", StrOdGatePassCode.Trim());
                                cmd.Parameters.AddWithValue("@CompanyCode", VisitPlanreq.CompCode.Trim());
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }

                            ////Details   
                            //strPlanDts = null;
                            //strPlanDts = Regex.Split(VisitPlanreq.PlanDts, "@#@");
                            //SrNo = 0;
                            //foreach (String StrSub in strPlanDts)
                            //{
                            //    SrNo += 1;
                            //    Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            //    sb.Remove(0, sb.Length);
                            //    sb.Append("INSERT INTO OutDoorDutyDetails(ODCode,SrNo,ClientName,Reason,SourceFrom,DestinationTo,TotalKm,RateKm,TravExps,LunchDinner,Lodge,OtherExps)");
                            //    sb.Append(" VALUES('" + StrOdGatePassCode.Trim() + "','" + SrNo + "',");
                            //    sb.Append("'" + Dts[1].ToString().Trim() + "',");
                            //    sb.Append("'" + Dts[2].ToString().Trim() + "',");
                            //    sb.Append("'" + Dts[3].ToString().Trim() + "',");
                            //    sb.Append("'" + Dts[4].ToString().Trim() + "',");
                            //    sb.Append("'" + double.Parse(Dts[5].ToString().Trim()) + "',");
                            //    if (double.Parse(Dts[5].ToString().Trim()) > 0)
                            //    {
                            //        sb.Append("'" + double.Parse(Dts[6].ToString().Trim()) + "',");
                            //    }
                            //    else
                            //    {
                            //        sb.Append(" '0',");
                            //    }
                            //    sb.Append("'" + double.Parse(Dts[7].ToString().Trim()) + "',");
                            //    sb.Append("'" + double.Parse(Dts[8].ToString().Trim()) + "',");
                            //    sb.Append("'" + double.Parse(Dts[9].ToString().Trim()) + "',");
                            //    sb.Append("'" + double.Parse(Dts[10].ToString().Trim()) + "')");
                            //    cmd = new SqlCommand(sb.ToString(), con);
                            //    cmd.Transaction = tran;
                            //    cmd.ExecuteNonQuery();
                            //    cmd.Dispose();
                            //}



                            #endregion
                            //StrDisplayMsg = "";
                            //StrDisplayMsg = "Visting Plan Saved Successfully with : " + StrDispCode.Trim() + " and OutDoorDuty Code : " + StrOdGatePassCode.Trim();
                        }
                        else if (VisitPlanreq.ODType.Trim() == "GP")
                        {
                            //GatePass
                            #region
                            StrOdGatePassCode = VisitPlanreq.VP_OD_GPNo.Trim();

                            sb.Remove(0, sb.Length);
                            sb.Append(" Delete  From  GatePassDetails where GPNNo = '" + VisitPlanreq.VP_OD_GPNo.Trim() + "'  ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            ////Details   
                            //strPlanDts = null;
                            //strPlanDts = Regex.Split(VisitPlanreq.PlanDts, "@#@");
                            //SrNo = 0;
                            //foreach (String StrSub in strPlanDts)
                            //{
                            //    SrNo += 1;
                            //    Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            //    sb.Remove(0, sb.Length);
                            //    sb.Append("INSERT INTO GatePassDetails(GPNNo,SrNo,ClientName,Reason,SourceFrom,DestinationTo,TotalKm,RateKm,TravExps,LunchDinner,Lodge,OtherExps)");
                            //    sb.Append(" VALUES('" + StrOdGatePassCode.Trim() + "','" + SrNo + "',");
                            //    sb.Append("'" + Dts[1].ToString().Trim() + "',");
                            //    sb.Append("'" + Dts[2].ToString().Trim() + "',");
                            //    sb.Append("'" + Dts[3].ToString().Trim() + "',");
                            //    sb.Append("'" + Dts[4].ToString().Trim() + "',");
                            //    sb.Append("'" + double.Parse(Dts[5].ToString().Trim()) + "',");
                            //    if (double.Parse(Dts[5].ToString().Trim()) > 0)
                            //    {
                            //        sb.Append("'" + double.Parse(Dts[6].ToString().Trim()) + "',");
                            //    }
                            //    else
                            //    {
                            //        sb.Append(" '0',");
                            //    }
                            //    sb.Append("'" + double.Parse(Dts[7].ToString().Trim()) + "',");
                            //    sb.Append("'" + double.Parse(Dts[8].ToString().Trim()) + "',");
                            //    sb.Append("'" + double.Parse(Dts[9].ToString().Trim()) + "',");
                            //    sb.Append("'" + double.Parse(Dts[10].ToString().Trim()) + "')");
                            //    cmd = new SqlCommand(sb.ToString(), con);
                            //    cmd.Transaction = tran;
                            //    cmd.ExecuteNonQuery();
                            //    cmd.Dispose();
                            //}
                            sb.Remove(0, sb.Length);
                            dsVPDetails = null;
                            sb.Append("Select SrNo,ClientName,Reason,SourceFrom,DestinationTo,TotalKm,RateKm,TravExps,LunchDinner,Lodge,OtherExps from VisitPlanMktDetails where VpCode= '" + VisitPlanreq.VPCode.Trim() + "' Order by SrNo ");
                            dsVPDetails = ComCon.procTranDS(sb.ToString(), "VPD", con, tran);
                            if (dsVPDetails.Tables["VPD"].Rows.Count > 0)
                            {
                                for (int D = 0; D < dsVPDetails.Tables["VPD"].Rows.Count; D++)
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("INSERT INTO GatePassDetails(GPNNo,SrNo,ClientName,Reason,SourceFrom,DestinationTo,TotalKm,RateKm,TravExps,LunchDinner,Lodge,OtherExps)");
                                    sb.Append(" VALUES('" + VisitPlanreq.VP_OD_GPNo.Trim() + "','" + dsVPDetails.Tables["VPD"].Rows[D]["SrNo"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["ClientName"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["Reason"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["SourceFrom"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["DestinationTo"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["TotalKm"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["RateKm"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["TravExps"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["LunchDinner"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["Lodge"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["OtherExps"].ToString().Trim() + "')");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }

                                //****************User Acivity****************
                                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                                cmd.Parameters.AddWithValue("@EmpID", VisitPlanreq.EmpCode.Trim());
                                cmd.Parameters.AddWithValue("@TransactionType", "U");
                                cmd.Parameters.AddWithValue("@TransactionFrom", "GatePass");
                                cmd.Parameters.AddWithValue("@TransactionNo", StrOdGatePassCode.Trim());
                                cmd.Parameters.AddWithValue("@CompanyCode", VisitPlanreq.CompCode.Trim());
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                            #endregion
                        }

                        //else if (VisitPlanreq.ODType.Trim() == "N")
                        //{
                        //    StrDisplayMsg = "";
                        //    StrDisplayMsg = "Visting Plan Saved Successfully with : " + StrDispCode.Trim();
                        //}

                        //****************User Acivity****************
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", VisitPlanreq.EmpCode.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "U");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "VistiPlanMkt");
                        cmd.Parameters.AddWithValue("@TransactionNo", StrDispCode.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", VisitPlanreq.CompCode.Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        StrDisplayMsg = "";
                        StrDisplayMsg = "Visting Plan Updated Successfully with : " + StrDispCode.Trim();
                        /*
                        deletepath = "C:/TempERPFile/TempVisitPlanMkt" + "/" + VisitPlanreq.EmpCode.Trim();
                        if (Directory.Exists(deletepath))
                        {
                            foreach (string strFile in Directory.GetFiles(deletepath, "*.*"))
                            {
                                File.Delete(strFile);
                            }
                            Directory.Delete(deletepath);
                        }
                        */
                    }
                    else
                    {
                        StrDisplayMsg = "";
                        //StrDisplayMsg = "Visting Plan " + StrDispCode.Trim() + " HOD Level Authorized, You Cannot Update";
                        StrDisplayMsg = "You Cannot Update Visting Plan :- " + StrDispCode.Trim() + "   ";
                    }
                    #endregion
                }
                //AuthHOD Visit Plan
                else if (VisitPlanreq.StrType.Trim() == "AuthHod")
                {
                    #region
                    StrDispCode = VisitPlanreq.VPCode.Trim();

                    sb.Remove(0, sb.Length);
                    sb.Append("Update VisitPlanMkt SET Auth1='1',Auth2='1',HODRemark='" + VisitPlanreq.HODRemark.Trim() + "' WHERE VPCode='" + StrDispCode.Trim() + "' AND Active='1' AND Auth1='0' and Auth2='0' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //sb.Remove(0, sb.Length);
                    //sb.Append("Update GatePass SET Auth='1',HODRemark='" + VisitPlanreq.HODRemark.Trim() + "' WHERE VPCode='" + StrDispCode.Trim() + "' AND Active='1' AND Auth='0' ");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                    //sb.Remove(0, sb.Length);
                    //sb.Append("Update OutDoorDuty SET Auth1='1',HODRemark='" + VisitPlanreq.HODRemark.Trim() + "' WHERE VPCode='" + StrDispCode.Trim() + "' AND Active='1' AND Auth1='0' ");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                    //Gate Pass Or OD Save Code
                    if (VisitPlanreq.ODType.Trim() == "OD")
                    {
                        //OD CODE 
                        #region
                        StrOdGatePassCode = VisitPlanreq.VP_OD_GPNo.Trim();

                        sb.Remove(0, sb.Length);
                        sb.Append("Update OutDoorDuty SET Auth1='1',HODRemark='" + VisitPlanreq.HODRemark.Trim() + "' WHERE ODCode='" + StrOdGatePassCode.Trim() + "' AND Active='1' AND Auth1='0' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //****************Auth Acivity**************** 
                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO AuthorizationDetails");
                        sb.Append("(AuthDateTime,EmpID,AuthForm,TransactionNo,CompanyCode)");
                        sb.Append(" VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "' ,'" + VisitPlanreq.EmpCode.Trim() + "','OutDoorDuty HOD Authorize','" + StrOdGatePassCode.Trim() + "','" + VisitPlanreq.CompCode.Trim() + "')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        #endregion
                    }
                    else if (VisitPlanreq.ODType.Trim() == "GP")
                    {
                        //GatePass
                        #region
                        StrOdGatePassCode = VisitPlanreq.VP_OD_GPNo.Trim();

                        sb.Remove(0, sb.Length);
                        sb.Append("Update GatePass SET Auth='1',HODRemark='" + VisitPlanreq.HODRemark.Trim() + "' WHERE GPNNo='" + StrOdGatePassCode.Trim() + "' AND Active='1' AND Auth='0' ");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //****************Auth Acivity**************** 
                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO AuthorizationDetails");
                        sb.Append("(AuthDateTime,EmpID,AuthForm,TransactionNo,CompanyCode)");
                        sb.Append(" VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "' ,'" + VisitPlanreq.EmpCode.Trim() + "','GatePass HOD Authorize','" + StrOdGatePassCode.Trim() + "','" + VisitPlanreq.CompCode.Trim() + "')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                    }
                    #endregion


                    //****************User Acivity**************** 
                    sb.Remove(0, sb.Length);
                    sb.Append("INSERT INTO AuthorizationDetails");
                    sb.Append("(AuthDateTime,EmpID,AuthForm,TransactionNo,CompanyCode)");
                    sb.Append(" VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "' ,'" + VisitPlanreq.EmpCode.Trim() + "','VistiPlanMkt HOD Authorize','" + StrDispCode.Trim() + "','" + VisitPlanreq.CompCode.Trim() + "')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    StrDisplayMsg = "";
                    StrDisplayMsg = "Visting Plan HOD Level Auth Successfully for : " + StrDispCode.Trim();
                    #endregion
                }

                //Update Single Visit Plan row for Expense when HOD and BOD leve Auth 
                else if (VisitPlanreq.StrType.Trim() == "UpdateExpHOD" || VisitPlanreq.StrType.Trim() == "UpdateExpBOD")
                {
                    #region
                    StrDispCode = VisitPlanreq.VPCode.Trim();
                    String AllowEXPUpdate = "";
                    if (VisitPlanreq.StrType.Trim() == "UpdateExpHOD")
                    {
                        if (ComCon.getName("Select Count(VPCode) as AuthExpHOD from VisitPlanMktDetails where VPCode='" + VisitPlanreq.VPCode.Trim() + "' and  ExpCode='" + VisitPlanreq.ExpCode.Trim() + "'  and AuthExpHod='1'  ", "VisitPlanMkt", "AuthExpHOD").ToString().Trim() == "1")
                        {
                            AllowEXPUpdate = "NO";
                        }
                        else
                        {
                            AllowEXPUpdate = "YES";
                        }
                    }
                    else if (VisitPlanreq.StrType.Trim() == "UpdateExpBOD")
                    {
                        if (ComCon.getName("Select Count(VPCode) as AuthExpBOD from VisitPlanMktDetails where VPCode='" + VisitPlanreq.VPCode.Trim() + "' and ExpCode='" + VisitPlanreq.ExpCode.Trim() + "' and AuthExpHod='1' and AuthExpBod='1'", "VisitPlanMkt", "AuthExpBOD").ToString().Trim() == "1")
                        {
                            AllowEXPUpdate = "NO";
                        }
                        else
                        {
                            AllowEXPUpdate = "YES";
                        }
                    }

                    //Common for HOD nad BOD Level to Update Visit plan and ERW Details 
                    if (AllowEXPUpdate == "YES")
                    {
                        //Details              
                        strPlanDts = Regex.Split(VisitPlanreq.PlanDts, "@#@");
                        if (strPlanDts.Length > 0)
                        {
                            foreach (string StrSub in strPlanDts)
                            {
                                Dts = Regex.Split(StrSub.ToString().Trim(), "-->");

                                SrNo = int.Parse(Dts[0].ToString().Trim());
                                //Add New Row in Existing Visting Plan
                                if (Dts[11].ToString().Trim() == "Yes")
                                {
                                    cmd = new SqlCommand("InsertVisitPlanMKTDetails", con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@VPCode", StrDispCode.Trim());
                                    cmd.Parameters.AddWithValue("@SrNo", Dts[0].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@ClientName", Dts[1].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@Reason", Dts[2].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@SourceFrom", Dts[3].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@DestinationTo", Dts[4].ToString().Trim());
                                    cmd.Parameters.AddWithValue("@FeedBack", "");
                                    cmd.Parameters.AddWithValue("@TotalKm", double.Parse(Dts[5].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@RateKm", double.Parse(Dts[6].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@TravExps", double.Parse(Dts[7].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@LunchDinner", double.Parse(Dts[8].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@Lodge", double.Parse(Dts[9].ToString().Trim()));
                                    cmd.Parameters.AddWithValue("@OtherExps", double.Parse(Dts[10].ToString().Trim()));
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();

                                    //Update ExpCode No for new adde Row
                                    sb.Remove(0, sb.Length);
                                    sb.Append(" Update VisitPlanMktDetails SET ExpCode='" + VisitPlanreq.ExpCode.Trim() + "'  WHERE VPCode='" + StrDispCode.Trim() + "' and  SrNo = '" + Dts[0].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                //Update Row in Existing Visting Plan
                                else
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append(" Update VisitPlanMktDetails SET ClientName='" + Dts[1].ToString().Trim() + "' ,");
                                    sb.Append(" Reason ='" + Dts[2].ToString().Trim() + "' ,");
                                    sb.Append(" SourceFrom ='" + Dts[3].ToString().Trim() + "' ,");
                                    sb.Append(" DestinationTo ='" + Dts[4].ToString().Trim() + "' ,");
                                    sb.Append(" FeedBack ='' ,");
                                    sb.Append(" TotalKm ='" + double.Parse(Dts[5].ToString().Trim()) + "' ,");
                                    sb.Append(" RateKm ='" + double.Parse(Dts[6].ToString().Trim()) + "' ,");
                                    sb.Append(" TravExps ='" + double.Parse(Dts[7].ToString().Trim()) + "' ,");
                                    sb.Append(" LunchDinner ='" + double.Parse(Dts[8].ToString().Trim()) + "' ,");
                                    sb.Append(" Lodge ='" + double.Parse(Dts[9].ToString().Trim()) + "' ,");
                                    sb.Append(" OtherExps ='" + double.Parse(Dts[10].ToString().Trim()) + "'  ");
                                    sb.Append(" WHERE VPCode='" + StrDispCode.Trim() + "' and  SrNo = '" + Dts[0].ToString().Trim() + "' ");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                            }
                        }
                        //Gate Pass Or OD Save Code
                        if (VisitPlanreq.ODType.Trim() == "OD")
                        {
                            //OD CODE 
                            #region
                            StrOdGatePassCode = VisitPlanreq.VP_OD_GPNo.Trim();

                            sb.Remove(0, sb.Length);
                            sb.Append(" Delete  From  OutDoorDutyDetails where ODCode = '" + VisitPlanreq.VP_OD_GPNo.Trim() + "'  ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            dsVPDetails = null;
                            sb.Append(" Select SrNo,ClientName,Reason,SourceFrom,DestinationTo,TotalKm,RateKm,TravExps,LunchDinner,Lodge,OtherExps from VisitPlanMktDetails where VpCode= '" + VisitPlanreq.VPCode.Trim() + "' Order by SrNo");
                            dsVPDetails = ComCon.procTranDS(sb.ToString(), "VPD", con, tran);
                            if (dsVPDetails.Tables["VPD"].Rows.Count > 0)
                            {
                                for (int D = 0; D < dsVPDetails.Tables["VPD"].Rows.Count; D++)
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("INSERT INTO OutDoorDutyDetails(ODCode,SrNo,ClientName,Reason,SourceFrom,DestinationTo,TotalKm,RateKm,TravExps,LunchDinner,Lodge,OtherExps)");
                                    sb.Append(" VALUES('" + VisitPlanreq.VP_OD_GPNo.Trim() + "','" + dsVPDetails.Tables["VPD"].Rows[D]["SrNo"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["ClientName"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["Reason"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["SourceFrom"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["DestinationTo"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["TotalKm"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["RateKm"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["TravExps"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["LunchDinner"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["Lodge"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["OtherExps"].ToString().Trim() + "')");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }

                                //****************User Acivity****************
                                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                                cmd.Parameters.AddWithValue("@EmpID", VisitPlanreq.EmpCode.Trim());
                                cmd.Parameters.AddWithValue("@TransactionType", "U");
                                cmd.Parameters.AddWithValue("@TransactionFrom", "OutDoorDuty");
                                cmd.Parameters.AddWithValue("@TransactionNo", StrOdGatePassCode.Trim());
                                cmd.Parameters.AddWithValue("@CompanyCode", VisitPlanreq.CompCode.Trim());
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }

                            //Details   
                            //strPlanDts = null;
                            //strPlanDts = Regex.Split(VisitPlanreq.PlanDts, "@#@");
                            //SrNo = 0;
                            //foreach (String StrSub in strPlanDts)
                            //{
                            //SrNo += 1;
                            //Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            //sb.Remove(0, sb.Length);
                            //sb.Append("INSERT INTO OutDoorDutyDetails(ODCode,SrNo,ClientName,Reason,SourceFrom,DestinationTo,TotalKm,RateKm,TravExps,LunchDinner,Lodge,OtherExps)");
                            //sb.Append(" VALUES('" + StrOdGatePassCode.Trim() + "','" + SrNo + "',");
                            //sb.Append("'" + Dts[1].ToString().Trim() + "',");
                            //sb.Append("'" + Dts[2].ToString().Trim() + "',");
                            //sb.Append("'" + Dts[3].ToString().Trim() + "',");
                            //sb.Append("'" + Dts[4].ToString().Trim() + "',");
                            //sb.Append("'" + double.Parse(Dts[5].ToString().Trim()) + "',");
                            //if (double.Parse(Dts[5].ToString().Trim()) > 0)
                            //{
                            //    sb.Append("'" + double.Parse(Dts[6].ToString().Trim()) + "',");
                            //}
                            //else
                            //{
                            //    sb.Append(" '0',");
                            //}
                            //sb.Append("'" + double.Parse(Dts[7].ToString().Trim()) + "',");
                            //sb.Append("'" + double.Parse(Dts[8].ToString().Trim()) + "',");
                            //sb.Append("'" + double.Parse(Dts[9].ToString().Trim()) + "',");
                            //sb.Append("'" + double.Parse(Dts[10].ToString().Trim()) + "')");
                            //cmd = new SqlCommand(sb.ToString(), con);
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();

                            #endregion
                        }
                        else if (VisitPlanreq.ODType.Trim() == "GP")
                        {
                            //GatePass
                            #region
                            StrOdGatePassCode = VisitPlanreq.VP_OD_GPNo.Trim();

                            sb.Remove(0, sb.Length);
                            sb.Append(" Delete  From  GatePassDetails where GPNNo = '" + VisitPlanreq.VP_OD_GPNo.Trim() + "'  ");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            dsVPDetails = null;
                            sb.Append("Select SrNo,ClientName,Reason,SourceFrom,DestinationTo,TotalKm,RateKm,TravExps,LunchDinner,Lodge,OtherExps from VisitPlanMktDetails where VpCode= '" + VisitPlanreq.VPCode.Trim() + "' Order by SrNo ");
                            dsVPDetails = ComCon.procTranDS(sb.ToString(), "VPD", con, tran);
                            if (dsVPDetails.Tables["VPD"].Rows.Count > 0)
                            {
                                for (int D = 0; D < dsVPDetails.Tables["VPD"].Rows.Count; D++)
                                {
                                    sb.Remove(0, sb.Length);
                                    sb.Append("INSERT INTO GatePassDetails(GPNNo,SrNo,ClientName,Reason,SourceFrom,DestinationTo,TotalKm,RateKm,TravExps,LunchDinner,Lodge,OtherExps)");
                                    sb.Append(" VALUES('" + VisitPlanreq.VP_OD_GPNo.Trim() + "','" + dsVPDetails.Tables["VPD"].Rows[D]["SrNo"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["ClientName"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["Reason"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["SourceFrom"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["DestinationTo"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["TotalKm"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["RateKm"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["TravExps"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["LunchDinner"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["Lodge"].ToString().Trim() + "',");
                                    sb.Append("'" + dsVPDetails.Tables["VPD"].Rows[D]["OtherExps"].ToString().Trim() + "')");
                                    cmd = new SqlCommand(sb.ToString(), con);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                //****************User Acivity****************
                                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                                cmd.Parameters.AddWithValue("@EmpID", VisitPlanreq.EmpCode.Trim());
                                cmd.Parameters.AddWithValue("@TransactionType", "U");
                                cmd.Parameters.AddWithValue("@TransactionFrom", "GatePass");
                                cmd.Parameters.AddWithValue("@TransactionNo", StrOdGatePassCode.Trim());
                                cmd.Parameters.AddWithValue("@CompanyCode", VisitPlanreq.CompCode.Trim());
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }

                            ////Details   
                            //strPlanDts = null;
                            //strPlanDts = Regex.Split(VisitPlanreq.PlanDts, "@#@");
                            //SrNo = 0;
                            //foreach (String StrSub in strPlanDts)
                            //{
                            //    SrNo += 1;
                            //    Dts = Regex.Split(StrSub.ToString().Trim(), "-->");
                            //    sb.Remove(0, sb.Length);
                            //    sb.Append("INSERT INTO GatePassDetails(GPNNo,SrNo,ClientName,Reason,SourceFrom,DestinationTo,TotalKm,RateKm,TravExps,LunchDinner,Lodge,OtherExps)");
                            //    sb.Append(" VALUES('" + StrOdGatePassCode.Trim() + "','" + SrNo + "',");
                            //    sb.Append("'" + Dts[1].ToString().Trim() + "',");
                            //    sb.Append("'" + Dts[2].ToString().Trim() + "',");
                            //    sb.Append("'" + Dts[3].ToString().Trim() + "',");
                            //    sb.Append("'" + Dts[4].ToString().Trim() + "',");
                            //    sb.Append("'" + double.Parse(Dts[5].ToString().Trim()) + "',");
                            //    if (double.Parse(Dts[5].ToString().Trim()) > 0)
                            //    {
                            //        sb.Append("'" + double.Parse(Dts[6].ToString().Trim()) + "',");
                            //    }
                            //    else
                            //    {
                            //        sb.Append(" '0',");
                            //    }
                            //    sb.Append("'" + double.Parse(Dts[7].ToString().Trim()) + "',");
                            //    sb.Append("'" + double.Parse(Dts[8].ToString().Trim()) + "',");
                            //    sb.Append("'" + double.Parse(Dts[9].ToString().Trim()) + "',");
                            //    sb.Append("'" + double.Parse(Dts[10].ToString().Trim()) + "')");
                            //    cmd = new SqlCommand(sb.ToString(), con);
                            //    cmd.Transaction = tran;
                            //    cmd.ExecuteNonQuery();
                            //    cmd.Dispose();
                            //}

                            #endregion
                        }
                        //****************User Acivity****************
                        cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@EmpID", VisitPlanreq.EmpCode.Trim());
                        cmd.Parameters.AddWithValue("@TransactionType", "U");
                        cmd.Parameters.AddWithValue("@TransactionFrom", "VistiPlanMkt");
                        cmd.Parameters.AddWithValue("@TransactionNo", StrDispCode.Trim());
                        cmd.Parameters.AddWithValue("@CompanyCode", VisitPlanreq.CompCode.Trim());
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        StrDisplayMsg = "";
                        StrDisplayMsg = "Visting Plan Expense Updated Successfully with : " + StrDispCode.Trim();
                    }
                    else
                    {
                        StrDisplayMsg = "";
                        //StrDisplayMsg = "Visting Plan " + StrDispCode.Trim() + " HOD Level Authorized, You Cannot Update";
                        StrDisplayMsg = "You Cannot Update Visit Plan Expense for :- " + StrDispCode.Trim() + " , Because it Auth level Authorized .";
                    }
                    #endregion
                }

                tran.Commit();
                return StrDisplayMsg;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ("StackTrace " + ex.StackTrace.ToString() + ", Message " + ex.Message.ToString());
            }
            finally
            {
                con.Close();
            }
        }
        public string VPExpenseSubmit(VisitingPlanExpRequest VisitPlanExpreq)
        {
            #region
            string StrDisplayMsg = "";
            string StrErwDispCode = "";
            string[] strDts;
            string[] Dts;
            int SrNo;
            double Travelling = 0;
            double MatPurchase = 0;
            double LabourCharges = 0;
            double Lodge = 0;
            double BF_Lunch_Dinner = 0;
            double Diesel_Petrol = 0;
            double Other = 0;
            double ReqAmt = 0;
            double TotReqAmt = 0;
            #endregion
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                //Save Visit Plan
                if (VisitPlanExpreq.SaveType.Trim() == "S")
                {
                    //Action Taken ERW
                    #region
                    if (!string.IsNullOrEmpty(VisitPlanExpreq.VP_Pln_Exp_Dtls.ToString().Trim()))
                    {                       
                        //Main ERW
                        #region

                        StrErwDispCode = ComCon.GetMaxNo("ExpenceRequisitionWithPlan", "ERW", VisitPlanExpreq.ExpCompanyCode.Trim(), con, tran);

                        cmd = new SqlCommand("InsertExpenseRequisition", con);
                        cmd.CommandType = CommandType.StoredProcedure;                      
                        cmd.Parameters.AddWithValue("@REQCode", StrErwDispCode.Trim());
                        cmd.Parameters.AddWithValue("@MaxSrNo", (StrErwDispCode.Substring(10, 8)));
                        cmd.Parameters.AddWithValue("@Dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Yr", (StrErwDispCode.Substring(4, 5)));
                        cmd.Parameters.AddWithValue("@PCCode", VisitPlanExpreq.PCCode.Trim());
                        cmd.Parameters.AddWithValue("@ExpType", VisitPlanExpreq.ExpSupEmpType.Trim());
                        cmd.Parameters.AddWithValue("@SupEmpcode", VisitPlanExpreq.ExpSupEmpcode.Trim());
                        cmd.Parameters.AddWithValue("@EmpSupCode", "0");
                        cmd.Parameters.AddWithValue("@BalAmount", "0");                     
                        if (VisitPlanExpreq.ExpTypeAdv.Trim() == "Normal")
                        {
                            cmd.Parameters.AddWithValue("@Advance", "0");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Advance", "1");
                        }
                        cmd.Parameters.AddWithValue("@PathA", "NA");
                        string ERWRemark = "Expenses against Visit Plan ";
                        cmd.Parameters.AddWithValue("@Remark", ERWRemark.Trim());
                        cmd.Parameters.AddWithValue("@AuthRemark", "NIL");
                        cmd.Parameters.AddWithValue("@CompanyCode", VisitPlanExpreq.ExpCompanyCode.Trim());
                        cmd.Parameters.AddWithValue("@SRVNo", "0");                       
                        cmd.Parameters.AddWithValue("@ACTNo", "VPM");
                        cmd.Parameters.AddWithValue("@EngTransAmt", 0);
                        cmd.Parameters.AddWithValue("@Auth", "0");
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        #endregion

                        //Action Taken ERW Details 
                        strDts = null;
                        strDts = Regex.Split(VisitPlanExpreq.VP_Pln_Exp_Dtls, "@#@");
                        SrNo = 0;
                        string ERWDt = "";
                        foreach (String StrSub in strDts)
                        {
                            SrNo += 1;
                            Dts = Regex.Split(StrSub.ToString().Trim(), "-->");

                            Travelling = (Travelling + Convert.ToDouble(Dts[6].ToString().Trim()));                         
                            BF_Lunch_Dinner = (BF_Lunch_Dinner + Convert.ToDouble(Dts[7].ToString().Trim()));
                            Lodge = (Lodge + Convert.ToDouble(Dts[8].ToString().Trim()));                          
                            Other = (Other + Convert.ToDouble(Dts[9].ToString().Trim()));

                            //Update ExpCode in  VisitPlanMktDetails
                            sb.Remove(0, sb.Length);
                            sb.Append("Update VisitPlanMktDetails SET ExpCode='" + StrErwDispCode.Trim() + "' Where VPCode='" + Dts[0].ToString().Trim() + "' and SrNo= '" + Dts[3].ToString().Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            //END  To VisitPlanMktDetails

                            if (ERWDt == "")
                            {
                                ERWDt = ComCon.getName("select max(ToDate) as LastToDate from VisitPlanMkt where VPCode='" + Dts[0].ToString().Trim() + "'", "VisitPlanMkt", "LastToDate").ToString().Trim();
                            }
                        }

                        sb.Remove(0, sb.Length);
                        sb.Append("Update ExpenceRequisitionWithPlan set Dt='" + ERWDt + "' where ReqCode='" + StrErwDispCode.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        TotReqAmt = 0;
                        SrNo = 0;
                        for (int i = 1; i <= 7; i++)
                        {
                            #region
                            cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@REQCode", StrErwDispCode.Trim());
                            ReqAmt = 0;
                            if (i == 1 && Travelling > 0) //Travelling
                            {
                                SrNo += 1;                                
                                ReqAmt = Travelling;
                                cmd.Parameters.AddWithValue("@EXPCode", "0138");
                            }
                            if (i == 2 && MatPurchase > 0) //MatPurchase
                            {
                                SrNo += 1;                               
                                ReqAmt = MatPurchase;
                                cmd.Parameters.AddWithValue("@EXPCode", "0128");
                            }
                            if (i == 3 && LabourCharges > 0) //LabourCharges
                            {
                                SrNo += 1;                              
                                ReqAmt = LabourCharges;
                                cmd.Parameters.AddWithValue("@EXPCode", "0009");
                            }
                            if (i == 4 && Lodge > 0) //Lodge
                            {
                                SrNo += 1;
                                ReqAmt = Lodge;
                                cmd.Parameters.AddWithValue("@EXPCode", "0127");
                            }
                            if (i == 5 && BF_Lunch_Dinner > 0) //BF_Lunch_Dinner
                            {
                                SrNo += 1;                                
                                ReqAmt = BF_Lunch_Dinner;
                                cmd.Parameters.AddWithValue("@EXPCode", "0126");
                            }
                            if (i == 6 && Diesel_Petrol > 0) //Diesel_Petrol
                            {
                                SrNo += 1;                                
                                ReqAmt = Diesel_Petrol;
                                cmd.Parameters.AddWithValue("@EXPCode", "0004");
                            }
                            if (i == 7 && Other > 0) //Other
                            {
                                SrNo += 1;
                                ReqAmt = Other;                                
                                cmd.Parameters.AddWithValue("@EXPCode", "0113");
                            }

                            if (ReqAmt > 0)
                            {
                                TotReqAmt += ReqAmt;
                                cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                cmd.Parameters.AddWithValue("@Qty", 1);
                                cmd.Parameters.AddWithValue("@Rate", ReqAmt);
                                cmd.Parameters.AddWithValue("@Amount", ReqAmt);
                                cmd.Parameters.AddWithValue("@ExpReqVATPer", 0);
                                cmd.Parameters.AddWithValue("@ExpReqCSTPer", 0);
                                cmd.Parameters.AddWithValue("@ExpReqServieTPer", 0);
                                cmd.Parameters.AddWithValue("@ExpReqSBCessPer", 0);
                                cmd.Parameters.AddWithValue("@ExpReqTDSPer", 0);
                                cmd.Parameters.AddWithValue("@ExpBillNo", 0);
                                cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                            #endregion
                        }
                    }

                    //****************User Acivity****************
                    cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;                
                    cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmpID", VisitPlanExpreq.EmpCode.Trim());
                    cmd.Parameters.AddWithValue("@TransactionType", "S");
                    cmd.Parameters.AddWithValue("@TransactionFrom", "ExpenseRequisition");
                    cmd.Parameters.AddWithValue("@TransactionNo", StrErwDispCode.Trim());
                    cmd.Parameters.AddWithValue("@CompanyCode", VisitPlanExpreq.ExpCompanyCode.Trim());
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    #endregion

                    StrDisplayMsg = "Expense Save Successfully With Code : " + StrErwDispCode.Trim();
                }
                //Auth HOD Expense
                else if (VisitPlanExpreq.SaveType.Trim() == "AuthExpHod")
                {
                    StrErwDispCode = VisitPlanExpreq.ReqCode.Trim();
                    if (ComCon.getName("Select Count(Expcode) as AuthExpHOD from VisitPlanMktDetails where ExpCode= '" + StrErwDispCode.Trim() + "'  and AuthExpHod='1' ", "VisitPlanMkt", "AuthExpHOD").ToString().Trim() == "0")
                    {
                        // Delete Exp Details
                        sb.Remove(0, sb.Length);
                        sb.Append("Delete From ExpenceRequisitionWithPlanDetails WHERE REQCode='" + StrErwDispCode.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //Action Taken ERW
                        #region
                        if (!string.IsNullOrEmpty(VisitPlanExpreq.VP_Pln_Exp_Dtls.ToString().Trim()))
                        {
                            //  StrErwDispCode = "";
                            //Action Taken ERW Details 
                            strDts = null;
                            strDts = Regex.Split(VisitPlanExpreq.VP_Pln_Exp_Dtls, "@#@");
                            SrNo = 0;
                            foreach (String StrSub in strDts)
                            {
                                SrNo += 1;
                                Dts = Regex.Split(StrSub.ToString().Trim(), "-->");

                                Travelling = (Travelling + Convert.ToDouble(Dts[6].ToString().Trim()));
                                // MatPurchase = (MatPurchase+Convert.ToDouble(Dts[1].ToString().Trim()));
                                //  LabourCharges = (LabourCharges +Convert.ToDouble(Dts[1].ToString().Trim()));
                                BF_Lunch_Dinner = (BF_Lunch_Dinner + Convert.ToDouble(Dts[7].ToString().Trim()));
                                Lodge = (Lodge + Convert.ToDouble(Dts[8].ToString().Trim()));
                                // Diesel_Petrol = (Diesel_Petrol+Convert.ToDouble(Dts[1].ToString().Trim()));
                                Other = (Other + Convert.ToDouble(Dts[9].ToString().Trim()));

                                // Update ExpCode in  VisitPlanMktDetails
                                sb.Remove(0, sb.Length);
                                sb.Append("Update VisitPlanMktDetails SET AuthExpHod='1', AuthExpHODRemark ='" + Dts[11].ToString().Trim() + "'   Where VPCode = '" + Dts[0].ToString().Trim() + "'  and SrNo= '" + Dts[3].ToString().Trim() + "' and ExpCode = '" + StrErwDispCode.Trim() + "'    ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                                // END To VisitPlanMktDetails
                            }

                            TotReqAmt = 0;
                            SrNo = 0;
                            for (int i = 1; i <= 7; i++)
                            {
                                #region
                                cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@REQCode", StrErwDispCode.Trim());

                                ReqAmt = 0;

                                if (i == 1 && Travelling > 0) //Travelling
                                {
                                    SrNo += 1;
                                    // ReqAmt = Convert.ToDouble(clsCommonFunctions.getTranName("select isnull(sum(Travelling),0) as ReqAmt from DailyExpenseDetails where DEXCode='" + lblDispID.Text.Trim() + "' and Travelling>0", "DailyExpenseDetails", "ReqAmt", con, tran));
                                    ReqAmt = Travelling;
                                    cmd.Parameters.AddWithValue("@EXPCode", "0082");
                                }
                                if (i == 2 && MatPurchase > 0) //MatPurchase
                                {
                                    SrNo += 1;
                                    //ReqAmt = Convert.ToDouble(clsCommonFunctions.getTranName("select isnull(sum(MatPurchase),0) as ReqAmt from DailyExpenseDetails where DEXCode='" + lblDispID.Text.Trim() + "' and MatPurchase>0", "DailyExpenseDetails", "ReqAmt", con, tran));
                                    ReqAmt = MatPurchase;
                                    cmd.Parameters.AddWithValue("@EXPCode", "0128");
                                }
                                if (i == 3 && LabourCharges > 0) //LabourCharges
                                {
                                    SrNo += 1;
                                    //  ReqAmt = Convert.ToDouble(clsCommonFunctions.getTranName("select isnull(sum(LabourCharges),0) as ReqAmt from DailyExpenseDetails where DEXCode='" + lblDispID.Text.Trim() + "' and LabourCharges>0", "DailyExpenseDetails", "ReqAmt", con, tran));
                                    ReqAmt = LabourCharges;
                                    cmd.Parameters.AddWithValue("@EXPCode", "0009");
                                }
                                if (i == 4 && Lodge > 0) //Lodge
                                {
                                    SrNo += 1;
                                    //  ReqAmt = Convert.ToDouble(clsCommonFunctions.getTranName("select isnull(sum(Lodge),0) as ReqAmt from DailyExpenseDetails where DEXCode='" + lblDispID.Text.Trim() + "' and Lodge>0", "DailyExpenseDetails", "ReqAmt", con, tran));
                                    ReqAmt = Lodge;
                                    cmd.Parameters.AddWithValue("@EXPCode", "0127");
                                }
                                if (i == 5 && BF_Lunch_Dinner > 0) //BF_Lunch_Dinner
                                {
                                    SrNo += 1;
                                    //  ReqAmt = Convert.ToDouble(clsCommonFunctions.getTranName("select isnull(sum(BF_Lunch_Dinner),0) as ReqAmt from DailyExpenseDetails where DEXCode='" + lblDispID.Text.Trim() + "' and BF_Lunch_Dinner>0", "DailyExpenseDetails", "ReqAmt", con, tran));
                                    ReqAmt = BF_Lunch_Dinner;
                                    cmd.Parameters.AddWithValue("@EXPCode", "0126");
                                }
                                if (i == 6 && Diesel_Petrol > 0) //Diesel_Petrol
                                {
                                    SrNo += 1;
                                    //  ReqAmt = Convert.ToDouble(clsCommonFunctions.getTranName("select isnull(sum(Diesel_Petrol),0) as ReqAmt from DailyExpenseDetails where DEXCode='" + lblDispID.Text.Trim() + "' and Diesel_Petrol>0", "DailyExpenseDetails", "ReqAmt", con, tran));
                                    ReqAmt = Diesel_Petrol;
                                    cmd.Parameters.AddWithValue("@EXPCode", "0004");
                                }
                                if (i == 7 && Other > 0) //Other
                                {
                                    SrNo += 1;
                                    ReqAmt = Other;
                                    //ReqAmt = Convert.ToDouble(clsCommonFunctions.getTranName("select isnull(sum(Other),0) as ReqAmt from DailyExpenseDetails where DEXCode='" + lblDispID.Text.Trim() + "' and Other>0", "DailyExpenseDetails", "ReqAmt", con, tran));
                                    cmd.Parameters.AddWithValue("@EXPCode", "0113");
                                }

                                if (ReqAmt > 0)
                                {
                                    TotReqAmt += ReqAmt;
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@Qty", 1);
                                    cmd.Parameters.AddWithValue("@Rate", ReqAmt);
                                    cmd.Parameters.AddWithValue("@Amount", ReqAmt);
                                    cmd.Parameters.AddWithValue("@ExpReqVATPer", 0);
                                    cmd.Parameters.AddWithValue("@ExpReqCSTPer", 0);
                                    cmd.Parameters.AddWithValue("@ExpReqServieTPer", 0);
                                    cmd.Parameters.AddWithValue("@ExpReqSBCessPer", 0);
                                    cmd.Parameters.AddWithValue("@ExpReqTDSPer", 0);
                                    cmd.Parameters.AddWithValue("@ExpBillNo", 0);
                                    cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                #endregion
                            }

                            //sb.Remove(0, sb.Length);
                            //sb.Append("Delete From EmpFundTransaction WHERE TransCode='" + StrErwDispCode.Trim() + "'");
                            //cmd = new SqlCommand(sb.ToString(), con);
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();

                            //sb.Remove(0, sb.Length);
                            //sb.Append("insert into EmpFundTransaction(EmpCode,DocType,TransCode,IssueCode,IssueDt,IssAmount,ReceivedCode,ReceivedDt,RecAmount) ");
                            //sb.Append("Values('" + ddlEmployeeName.SelectedValue.Trim() + "','ERW','" + lblERWNo.Text.Trim() + "','0',NULL,'0.00',");
                            //sb.Append("'" + lblERWNo.Text.Trim() + "','" + DateTime.Now + "','" + TotReqAmt + "')");
                            //cmd = new SqlCommand(sb.ToString(), con);
                            //cmd.Transaction = tran;
                            //cmd.ExecuteNonQuery();
                            //cmd.Dispose();
                        }

                        ////****************User Acivity****************
                        //cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        //cmd.CommandType = CommandType.StoredProcedure;
                        ////cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                        //cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        //cmd.Parameters.AddWithValue("@EmpID", VisitPlanExpreq.EmpCode.Trim());
                        //cmd.Parameters.AddWithValue("@TransactionType", "U");
                        //cmd.Parameters.AddWithValue("@TransactionFrom", "ExpenseRequisition");
                        //cmd.Parameters.AddWithValue("@TransactionNo", StrErwDispCode.Trim());
                        //cmd.Parameters.AddWithValue("@CompanyCode", VisitPlanExpreq.ExpCompanyCode.Trim());
                        //cmd.Transaction = tran;
                        //cmd.ExecuteNonQuery();
                        //cmd.Dispose();

                        // *********************User Acivity *************************** 
                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO AuthorizationDetails");
                        sb.Append("(AuthDateTime,EmpID,AuthForm,TransactionNo,CompanyCode)");
                        sb.Append(" VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "','" + VisitPlanExpreq.AuthExpByEcode.ToString().Trim() + "','VisitplanExpenseHOD','" + StrErwDispCode.Trim() + "','" + VisitPlanExpreq.ExpCompanyCode.Trim() + "')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        StrDisplayMsg = "Hod level Auth Successfully With Code : " + StrErwDispCode.Trim();
                        #endregion
                    }
                    else
                    {
                        StrDisplayMsg = "You can not authorized Expense beacause  Hod level allready Auth - for selectd  :- " + StrErwDispCode.Trim().Trim() + "      ";
                    }
                }
                //Auth BOD Expense
                else if (VisitPlanExpreq.SaveType.Trim() == "AuthExpBod")
                {
                    String StrAuthBODRemark = "";
                    StrErwDispCode = VisitPlanExpreq.ReqCode.Trim();
                    if (ComCon.getName("Select Count(Expcode) as AuthExpBOD from VisitPlanMktDetails where ExpCode= '" + StrErwDispCode.Trim() + "'  and AuthExpBod='1'     ", "VisitPlanMkt", "AuthExpBOD").ToString().Trim() == "0")
                    {

                        // Delete Exp Details
                        sb.Remove(0, sb.Length);
                        sb.Append("Delete From ExpenceRequisitionWithPlanDetails WHERE REQCode='" + StrErwDispCode.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        //Action Taken ERW
                        #region
                        if (!string.IsNullOrEmpty(VisitPlanExpreq.VP_Pln_Exp_Dtls.ToString().Trim()))
                        {
                            //  StrErwDispCode = "";
                            //Action Taken ERW Details 
                            strDts = null;
                            strDts = Regex.Split(VisitPlanExpreq.VP_Pln_Exp_Dtls, "@#@");
                            SrNo = 0;
                            foreach (String StrSub in strDts)
                            {
                                SrNo += 1;
                                Dts = Regex.Split(StrSub.ToString().Trim(), "-->");

                                Travelling = (Travelling + Convert.ToDouble(Dts[6].ToString().Trim()));
                                // MatPurchase = (MatPurchase+Convert.ToDouble(Dts[1].ToString().Trim()));
                                //  LabourCharges = (LabourCharges +Convert.ToDouble(Dts[1].ToString().Trim()));
                                BF_Lunch_Dinner = (BF_Lunch_Dinner + Convert.ToDouble(Dts[7].ToString().Trim()));
                                Lodge = (Lodge + Convert.ToDouble(Dts[8].ToString().Trim()));
                                // Diesel_Petrol = (Diesel_Petrol+Convert.ToDouble(Dts[1].ToString().Trim()));
                                Other = (Other + Convert.ToDouble(Dts[9].ToString().Trim()));

                                // Update ExpCode in  VisitPlanMktDetails
                                sb.Remove(0, sb.Length);
                                sb.Append("Update VisitPlanMktDetails SET  AuthExpBod='1', AuthExpBODRemark ='" + Dts[11].ToString().Trim() + "'   Where VPCode = '" + Dts[0].ToString().Trim() + "'  and SrNo= '" + Dts[3].ToString().Trim() + "' and ExpCode = '" + StrErwDispCode.Trim() + "'    ");
                                cmd = new SqlCommand(sb.ToString(), con);
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                                // END To VisitPlanMktDetails

                                StrAuthBODRemark = Dts[11].ToString().Trim();
                            }

                            TotReqAmt = 0;
                            SrNo = 0;
                            for (int i = 1; i <= 7; i++)
                            {
                                #region
                                cmd = new SqlCommand("InsertExpenseRequisitionDetails", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@REQCode", StrErwDispCode.Trim());

                                ReqAmt = 0;

                                if (i == 1 && Travelling > 0) //Travelling
                                {
                                    SrNo += 1;
                                    // ReqAmt = Convert.ToDouble(clsCommonFunctions.getTranName("select isnull(sum(Travelling),0) as ReqAmt from DailyExpenseDetails where DEXCode='" + lblDispID.Text.Trim() + "' and Travelling>0", "DailyExpenseDetails", "ReqAmt", con, tran));
                                    ReqAmt = Travelling;
                                    cmd.Parameters.AddWithValue("@EXPCode", "0082");
                                }
                                if (i == 2 && MatPurchase > 0) //MatPurchase
                                {
                                    SrNo += 1;
                                    //ReqAmt = Convert.ToDouble(clsCommonFunctions.getTranName("select isnull(sum(MatPurchase),0) as ReqAmt from DailyExpenseDetails where DEXCode='" + lblDispID.Text.Trim() + "' and MatPurchase>0", "DailyExpenseDetails", "ReqAmt", con, tran));
                                    ReqAmt = MatPurchase;
                                    cmd.Parameters.AddWithValue("@EXPCode", "0128");
                                }
                                if (i == 3 && LabourCharges > 0) //LabourCharges
                                {
                                    SrNo += 1;
                                    //  ReqAmt = Convert.ToDouble(clsCommonFunctions.getTranName("select isnull(sum(LabourCharges),0) as ReqAmt from DailyExpenseDetails where DEXCode='" + lblDispID.Text.Trim() + "' and LabourCharges>0", "DailyExpenseDetails", "ReqAmt", con, tran));
                                    ReqAmt = LabourCharges;
                                    cmd.Parameters.AddWithValue("@EXPCode", "0009");
                                }
                                if (i == 4 && Lodge > 0) //Lodge
                                {
                                    SrNo += 1;
                                    //  ReqAmt = Convert.ToDouble(clsCommonFunctions.getTranName("select isnull(sum(Lodge),0) as ReqAmt from DailyExpenseDetails where DEXCode='" + lblDispID.Text.Trim() + "' and Lodge>0", "DailyExpenseDetails", "ReqAmt", con, tran));
                                    ReqAmt = Lodge;
                                    cmd.Parameters.AddWithValue("@EXPCode", "0127");
                                }
                                if (i == 5 && BF_Lunch_Dinner > 0) //BF_Lunch_Dinner
                                {
                                    SrNo += 1;
                                    //  ReqAmt = Convert.ToDouble(clsCommonFunctions.getTranName("select isnull(sum(BF_Lunch_Dinner),0) as ReqAmt from DailyExpenseDetails where DEXCode='" + lblDispID.Text.Trim() + "' and BF_Lunch_Dinner>0", "DailyExpenseDetails", "ReqAmt", con, tran));
                                    ReqAmt = BF_Lunch_Dinner;
                                    cmd.Parameters.AddWithValue("@EXPCode", "0126");
                                }
                                if (i == 6 && Diesel_Petrol > 0) //Diesel_Petrol
                                {
                                    SrNo += 1;
                                    //  ReqAmt = Convert.ToDouble(clsCommonFunctions.getTranName("select isnull(sum(Diesel_Petrol),0) as ReqAmt from DailyExpenseDetails where DEXCode='" + lblDispID.Text.Trim() + "' and Diesel_Petrol>0", "DailyExpenseDetails", "ReqAmt", con, tran));
                                    ReqAmt = Diesel_Petrol;
                                    cmd.Parameters.AddWithValue("@EXPCode", "0004");
                                }
                                if (i == 7 && Other > 0) //Other
                                {
                                    SrNo += 1;
                                    ReqAmt = Other;
                                    //ReqAmt = Convert.ToDouble(clsCommonFunctions.getTranName("select isnull(sum(Other),0) as ReqAmt from DailyExpenseDetails where DEXCode='" + lblDispID.Text.Trim() + "' and Other>0", "DailyExpenseDetails", "ReqAmt", con, tran));
                                    cmd.Parameters.AddWithValue("@EXPCode", "0113");
                                }

                                if (ReqAmt > 0)
                                {
                                    TotReqAmt += ReqAmt;
                                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                                    cmd.Parameters.AddWithValue("@Qty", 1);
                                    cmd.Parameters.AddWithValue("@Rate", ReqAmt);
                                    cmd.Parameters.AddWithValue("@Amount", ReqAmt);
                                    cmd.Parameters.AddWithValue("@ExpReqVATPer", 0);
                                    cmd.Parameters.AddWithValue("@ExpReqCSTPer", 0);
                                    cmd.Parameters.AddWithValue("@ExpReqServieTPer", 0);
                                    cmd.Parameters.AddWithValue("@ExpReqSBCessPer", 0);
                                    cmd.Parameters.AddWithValue("@ExpReqTDSPer", 0);
                                    cmd.Parameters.AddWithValue("@ExpBillNo", 0);
                                    cmd.Parameters.AddWithValue("@ExpBillDt", DBNull.Value);
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                                #endregion
                            }

                            sb.Remove(0, sb.Length);
                            sb.Append("Delete From EmpFundTransaction WHERE TransCode='" + StrErwDispCode.Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            sb.Append("insert into EmpFundTransaction(EmpCode,DocType,TransCode,IssueCode,IssueDt,IssAmount,ReceivedCode,ReceivedDt,RecAmount) ");
                            sb.Append("Values('" + VisitPlanExpreq.EmpCode.Trim() + "','ERW','" + StrErwDispCode.Trim() + "','0',NULL,'0.00',");
                            sb.Append("'" + StrErwDispCode.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "','" + TotReqAmt + "')");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                            sb.Remove(0, sb.Length);
                            sb.Append("Update ExpenceRequisitionWithPlan set Auth='1',AuthRemark='" + StrAuthBODRemark.ToString().Trim() + "',AuthBy='" + VisitPlanExpreq.AuthExpByEcode.ToString().Trim() + "',AuthDtTime='" + DateTime.Now + "' where ");
                            sb.Append("ReqCode='" + StrErwDispCode.Trim() + "'");
                            cmd = new SqlCommand(sb.ToString(), con);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }

                        ////****************User Acivity****************
                        //cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                        //cmd.CommandType = CommandType.StoredProcedure;
                        ////cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                        //cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                        //cmd.Parameters.AddWithValue("@EmpID", VisitPlanExpreq.EmpCode.Trim());
                        //cmd.Parameters.AddWithValue("@TransactionType", "U");
                        //cmd.Parameters.AddWithValue("@TransactionFrom", "ExpenseRequisition");
                        //cmd.Parameters.AddWithValue("@TransactionNo", StrErwDispCode.Trim());
                        //cmd.Parameters.AddWithValue("@CompanyCode", VisitPlanExpreq.ExpCompanyCode.Trim());
                        //cmd.Transaction = tran;
                        //cmd.ExecuteNonQuery();
                        //cmd.Dispose();

                        // *********************Auth by  Acivity *************************** 
                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO AuthorizationDetails");
                        sb.Append("(AuthDateTime,EmpID,AuthForm,TransactionNo,CompanyCode)");
                        sb.Append(" VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "','" + VisitPlanExpreq.AuthExpByEcode.ToString().Trim() + "','VisitplanExpenseBOD','" + StrErwDispCode.Trim() + "','" + VisitPlanExpreq.ExpCompanyCode.Trim() + "')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();


                        // *********************Expense log *************************** 
                        sb.Remove(0, sb.Length);
                        sb.Append("INSERT INTO AuthorizationDetails");
                        sb.Append("(AuthDateTime,EmpID,AuthForm,TransactionNo,CompanyCode)");
                        sb.Append(" VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "','" + VisitPlanExpreq.AuthExpByEcode.ToString().Trim() + "','ExpenseRequisition Authorize','" + StrErwDispCode.Trim() + "','" + VisitPlanExpreq.ExpCompanyCode.Trim() + "')");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        StrDisplayMsg = "Bod level Auth Successfully With Code : " + StrErwDispCode.Trim();
                        #endregion
                    }
                    else
                    {
                        StrDisplayMsg = "You can not authorized Expense beacause BOD level allready Auth - for selectd  :- " + StrErwDispCode.Trim().Trim() + "      ";
                    }
                }

                tran.Commit();
                // StrDisplayMsg = "Bod level Auth Successfully With Code : " + StrErwDispCode.Trim();
                return StrDisplayMsg;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ("StackTrace " + ex.StackTrace.ToString() + ", Message " + ex.Message.ToString());
            }
            finally
            {
                con.Close();
            }
        }
        public string SaveLocation(ILocation location)
        {
            String StrDisplayMsg = "";
            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();

                if (location.savetype.Trim() == "Start")
                {
                    sb.Remove(0, sb.Length);
                    sb.Append("Update VisitPlanMkt set StartTime=getdate(),StartLatitude='" + location.latitude.Trim() + "',StartLongitude='" + location.longitude.Trim() + "' ");
                    sb.Append("where VPCode='" + location.vpcode.Trim() + "'");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    StrDisplayMsg = location.savetype.Trim() + " Location Updated for : " + location.vpcode.Trim();
                }
                /*
                else if (location.savetype.Trim() == "Current")
                {
                    sb.Remove(0, sb.Length);
                    sb.Append("Update VisitPlanMKTDetails SET CurrentTime=getdate(),CurrentLatitude='" + location.latitude.Trim() + "',CurrentLongitude='" + location.longitude.Trim() + "', ");
                    sb.Append("Feedback='" + location.feedback.Trim() + "' WHERE VPCode ='" + location.vpcode.Trim() + "' and SrNo='" + location.srno.Trim() + "' and ClientName='" + location.customer.Trim() + "' ");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                                       
                    StrDisplayMsg = location.savetype.Trim() + " Location Updated for : " + location.vpcode.Trim();
                }
                */
                else if (location.savetype.Trim() == "End")
                {
                    string compl_all_visit = ComCon.getTranName("select count(SrNo) as Cnt from VisitPlanMKTDetails where VPCode='" + location.vpcode.Trim() + "' and CurrentTime is Null", "VisitPlanMKTDetails", "Cnt", con, tran);
                    if (compl_all_visit == "0")
                    {
                        sb.Remove(0, sb.Length);
                        sb.Append("Update VisitPlanMkt set EndTime=getdate(),EndLatitude='" + location.latitude.Trim() + "',EndLongitude='" + location.longitude.Trim() + "' ");
                        sb.Append("where VPCode='" + location.vpcode.Trim() + "'");
                        cmd = new SqlCommand(sb.ToString(), con);
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        StrDisplayMsg = location.savetype.Trim() + " Location Updated for : " + location.vpcode.Trim();
                    }
                    else
                    {
                        StrDisplayMsg = "Complete All the Visits before End Day for : " + location.vpcode.Trim();
                    }
                }

                tran.Commit();
                return StrDisplayMsg;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ("Error: StackTrace" + ex.StackTrace.ToString() + ", Message" + ex.Message.ToString());
            }
            finally
            {
                con.Close();
            }

        }

        /*
        public string ReverseGeoLoc(string longitude, string latitude)
        {   
            string url = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false", latitude, longitude);
            XElement xml = XElement.Load(url);                       
            if (xml.Element("status").Value == "OK")
            {
                return string.Format("<strong>Origem</strong>: {0}",xml.Element("result").Element("formatted_address").Value);                              
            }
            else
            {
                return String.Concat("Ocorreu o seguinte erro: ", xml.Element("status").Value);
            }           
           
            try
            {  
                XmlDocument doc = new XmlDocument();
                string place = String.Format("http://maps.googleapis.com/maps/api/geocode/xml?latlng=" + longitude + "," + latitude + "&sensor=false,&key=");
                doc.Load(place);
                XmlNode element = doc.SelectSingleNode("//GeocodeResponse/status");
                if (element.InnerText == "ZERO_RESULTS")
                {
                    return ("No data available for the specified location");
                }
                else
                {
                    element = doc.SelectSingleNode("//GeocodeResponse/result/formatted_address");
                    return element.InnerText;
                }  
            }
            catch (Exception ex)
            {                
                return ("Error: StackTrace" + ex.StackTrace.ToString() + ", Message" + ex.Message.ToString());
            } 
        }
        */
    }
}