using KalaERPApi.Models.Production.CP.Plan;
using KalaERPApi.Models.Request;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace KalaERPApi.Service.Production.CP.Plan
{
    public class ControlPanel_Con
    {
        #region
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        CommonCon ComCon = new CommonCon();
        public SqlTransaction tran = null;

        SqlCommand cmd = null;
        #endregion


        public DataTable GetCpPlan(string strJobCardType, string strcompID)
        {
           
                SqlDataAdapter dAd = new SqlDataAdapter("GetJobCard_Cp_PlanDts", con);
                //SqlDataAdapter dAd = new SqlDataAdapter("GetJobCardDGDts_fs", con);
                dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
                dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = strJobCardType;
                dAd.SelectCommand.Parameters.Add("@CompId", SqlDbType.Char).Value = strcompID;
                dAd.SelectCommand.CommandTimeout = 0;
                DataSet dSet = new DataSet();
                dAd.Fill(dSet);
                return dSet.Tables[0];
           
        }

        public string SubmitCP(ControlPanel_Request job_Cpyreq)
        {

          
            string StrDisplayMsg = "";
            string StrDispCode_CPYPlan = "";
            String StrDispCode_MaterialReq_CNC_ALL_msg = "";
            String StrDispCode_MaterialReq_WH_ALL_msg = "";
            String StrDispCode_MaterialReq_FAB_ALL_msg = "";
            String StrDispCode_MaterialReq_POC_ALL_msg = "";

            // String StrCompCode = "";
            string[] strPlanDts;
            string[] Dts;
            //StrCompCode = job_Cpyreq.PCCode.Trim().Substring(0, 2).Trim();
            int SrNo;
            string ParentPart = "";

            if (string.IsNullOrEmpty(job_Cpyreq.JobCard_CpyDts))
            {
                StrDisplayMsg = "Please Check Record !";
                return StrDisplayMsg;
            }


            try
            {
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); }
                ;
                tran = con.BeginTransaction();
                //Save Cpy Plan
                StrDispCode_CPYPlan = ComCon.GetMaxNo("CanopyPlan", "CPY", job_Cpyreq.CompCode.Trim(), con, tran);

                string CurrentMnth = ComCon.getName("select Mon=case MONTH(GETDATE()) " +
                                                        "when 1 then '01' when 2 then '02' when 3 then '03' " +
                                                        "when 4 then '04' when 5 then '05' when 6 then '06' " +
                                                        "when 7 then '07' when 8 then '08' when 9 then '09' " +
                                                        "when 10 then '10' when 11 then '11' when  12 then '12' end", "tblM", "Mon");

                //For Mst Save
                #region
                cmd = new SqlCommand("InsertCanopyPlan", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CPCode", StrDispCode_CPYPlan.Trim());
                cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                cmd.Parameters.AddWithValue("@MaxSrNo", StrDispCode_CPYPlan.Substring(10, 8).Trim());
                cmd.Parameters.AddWithValue("@Yr", StrDispCode_CPYPlan.Substring(4, 5).Trim());
                cmd.Parameters.AddWithValue("@FromDt", DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                cmd.Parameters.AddWithValue("@ToDt", DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                cmd.Parameters.AddWithValue("@PlanPCCode", job_Cpyreq.PCCode.Trim()); //"01.041"
                cmd.Parameters.AddWithValue("@CompanyCode", job_Cpyreq.CompCode.Trim());
                cmd.Parameters.AddWithValue("@PlanType", "G");
                cmd.Parameters.AddWithValue("@AutoFlg", "Yes"); //ASK FS
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                #endregion


                //For CPY Plan Details  Save --Start
                #region

                strPlanDts = Regex.Split(job_Cpyreq.JobCard_CpyDts, "@@#@@");
                SrNo = 0;


                foreach (string StrSub in strPlanDts) //For Loop Start
                { // Main For Loop Start Bracket 

                    SrNo += 1;
                    Dts = Regex.Split(StrSub.ToString().Trim(), "@#@");

                    // Updated field mapping with new columns:
                    //     0           1           2           3           4           5           6               7           8           9           10          11          12          13
                    // item.KVA + "@#@" + item.Model + "@#@" + item.Partcode + "@#@" + item.FNorm + "@#@" + item.TotStk + "@#@" + item.WIPStk + "@#@" + item.PenPlanQty + "@#@" + item.PReq + "@#@" + item.PlanQty + "@#@" + item.BatchQty + "@#@" + item.Bomcode + "@#@" + item.PlanCode + "@#@" + item.PlanDate + "@#@" + item.DayPlanQty

                    //For Dts Save
                    #region
                    cmd = new SqlCommand("InsertCanopyPlanDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CPCode", StrDispCode_CPYPlan.Trim());
                    cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                    cmd.Parameters.AddWithValue("@SrNo", SrNo);

                    cmd.Parameters.AddWithValue("@PartCode", Dts[3].ToString().Trim());
                    cmd.Parameters.AddWithValue("@BomCode", Dts[11].ToString().Trim());

                    string PartCodeWOP = "";
                    //PartCodeWOP = ComCon.getTranName("SELECT Partcode FROM BOMdetails where BOMCode='" + Dts[10].ToString().Trim() + "' " +
                    //"AND KitCode='" + Dts[2].ToString().Trim() + "' AND Partcode like '004%' ", "tblBP", "Partcode", con, tran);

                    PartCodeWOP = ComCon.getTranName("SELECT Partcode FROM BOMdetails where BOMCode='" + Dts[11].ToString().Trim() + "' " +
                                "AND KitCode='" + Dts[3].ToString().Trim() + "' AND Substring(Partcode,12,1)IN ('6') AND Partcode like '004%' ", "tblBP", "Partcode", con, tran);


                    cmd.Parameters.AddWithValue("@PartCodeWOP", PartCodeWOP.Trim());
                    cmd.Parameters.AddWithValue("@Qty", int.Parse(Dts[9].ToString().Trim()));

                    // New Monthly Plan Parameters
                    cmd.Parameters.AddWithValue("@PlanCode", Dts.Length > 11 ? Dts[12].ToString().Trim() : "");

                    // Handle PlanDate - convert string to DateTime
                    DateTime planDate = DateTime.MinValue;
                    if (Dts.Length > 12 && !string.IsNullOrEmpty(Dts[13].ToString().Trim()))
                    {
                        if (DateTime.TryParse(Dts[13].ToString().Trim(), out planDate))
                        {
                            cmd.Parameters.AddWithValue("@PlanDate", planDate.ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@PlanDate", DBNull.Value);
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PlanDate", DBNull.Value);
                    }

                    // Handle DayPlanQty
                    int dayPlanQty = 0;
                    if (Dts.Length > 13 && int.TryParse(Dts[14].ToString().Trim(), out dayPlanQty))
                    {
                        cmd.Parameters.AddWithValue("@DayPlanQty", dayPlanQty);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@DayPlanQty", 0);
                    }

                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    #endregion

                    double kva = 0;

                    //All Requisition Start
                    #region
                    //Auto WH Req --START --******************************
                    #region

                    string StrDispCode_WHReq = "";

                    String StrWH_PCCode = "01.023";
                    //if (job_Cpyreq.PCCode.Trim() == "01.041")
                    //{
                    //    StrWH_PCCode = "01.005"; //CNC
                    //}
                    //else if (job_Cpyreq.PCCode.Trim() == "03.064")
                    //{
                    //    StrWH_PCCode = "03.040"; ////CNC
                    //}
                    //else if (job_Cpyreq.PCCode.Trim() == "28.012")
                    //{
                    //    StrWH_PCCode = "28.017"; ////CNC
                    //}


                    StrDispCode_WHReq = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", job_Cpyreq.CompCode.Trim(), con, tran);


                    if (string.IsNullOrEmpty(StrDispCode_MaterialReq_WH_ALL_msg.Trim()))
                    {
                        StrDispCode_MaterialReq_WH_ALL_msg = StrDispCode_WHReq.Trim();
                    }
                    else
                    {
                        StrDispCode_MaterialReq_WH_ALL_msg += ", " + StrDispCode_WHReq.Trim();
                    }


                    sb.Remove(0, sb.Length);
                    sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                        " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                    sb.Append("values('" + StrDispCode_WHReq.Trim() + "','" + StrDispCode_WHReq.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                    sb.Append("'" + StrDispCode_WHReq.Substring(4, 5).Trim() + "','" + StrWH_PCCode.Trim() + "','01.091','" + Dts[3].ToString().Trim() + "','" + job_Cpyreq.CompCode.Trim() + "','" + Dts[9].ToString().Trim() + "','P','WIP',");
                    sb.Append("'Auto Req For : " + Dts[0].ToString().Trim() + " Kva " + Dts[1].ToString().Trim() + " ','1','1','1','" + StrDispCode_CPYPlan.Trim() + "','0' )");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Details Req STRAT ********************

                    DataSet dsReqDts_WHReq = ComCon.procTranDS("exec InternalReqLogisticsdetailsWHKIT_CP '" + Dts[3].ToString().Trim() + "'", "tbl_ReqDts_WHReq", con, tran);
                    if (dsReqDts_WHReq != null && dsReqDts_WHReq.Tables["tbl_ReqDts_WHReq"].Rows.Count > 0)
                    {
                        int SrNoReq_WHReq = 0;

                        for (int cntd = 0; cntd < dsReqDts_WHReq.Tables["tbl_ReqDts_WHReq"].Rows.Count; cntd++)
                        {
                            SrNoReq_WHReq = SrNoReq_WHReq + 1;
                            cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@REQCode", StrDispCode_WHReq.Trim());
                            cmd.Parameters.AddWithValue("@SrNo", SrNoReq_WHReq);
                            cmd.Parameters.AddWithValue("@PartCode", dsReqDts_WHReq.Tables["tbl_ReqDts_WHReq"].Rows[cntd]["Partcode"].ToString().Trim());
                            cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts_WHReq.Tables["tbl_ReqDts_WHReq"].Rows[cntd]["TotQty"].ToString().Trim()) * double.Parse(Dts[9].ToString().Trim()));
                            cmd.Parameters.AddWithValue("@REQStatus", "P");
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }
                    }

                    #endregion
                    //Auto WH Req --END


                   
                    //Auto Logistics Req --START --******************************
                    #region

                    string StrDispCode_Log = "";

                    String StrLog_PCCode = "01.023";
                    //if (job_Cpyreq.PCCode.Trim() == "01.041")
                    //{
                    //    StrWH_PCCode = "01.005"; //CNC
                    //}
                    //else if (job_Cpyreq.PCCode.Trim() == "03.064")
                    //{
                    //    StrWH_PCCode = "03.040"; ////CNC
                    //}
                    //else if (job_Cpyreq.PCCode.Trim() == "28.012")
                    //{
                    //    StrWH_PCCode = "28.017"; ////CNC
                    //}

                    StrDispCode_Log = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", job_Cpyreq.CompCode.Trim(), con, tran);

                    if (string.IsNullOrEmpty(StrDispCode_MaterialReq_WH_ALL_msg.Trim()))
                    {
                        StrDispCode_MaterialReq_WH_ALL_msg = StrDispCode_Log.Trim();
                    }
                    else
                    {
                        StrDispCode_MaterialReq_WH_ALL_msg += ", " + StrDispCode_Log.Trim();
                    }


                    sb.Remove(0, sb.Length);
                    sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                        " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode,RequisitionFor) ");
                    sb.Append("values('" + StrDispCode_Log.Trim() + "','" + StrDispCode_Log.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                    sb.Append("'" + StrDispCode_Log.Substring(4, 5).Trim() + "','" + StrLog_PCCode.Trim() + "','23.001','" + Dts[3].ToString().Trim() + "','" + job_Cpyreq.CompCode.Trim() + "','" + Dts[9].ToString().Trim() + "','P','WIP',");
                    sb.Append("'Auto Req For : " + Dts[0].ToString().Trim() + " Kva " + Dts[1].ToString().Trim() + " ','1','1','1','" + StrDispCode_CPYPlan.Trim() + "','0' )");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    //Details Req STRAT ********************

                    DataSet dsReqDts_LogReq = ComCon.procTranDS("exec InternalReqLogisticsdetailsCPKit '" + Dts[3].ToString().Trim() + "'", "tbl_ReqDts_LogReq", con, tran);
                    if (dsReqDts_LogReq != null && dsReqDts_LogReq.Tables["tbl_ReqDts_LogReq"].Rows.Count > 0)
                    {
                        int SrNoReq_LogReq = 0;

                        for (int cntd = 0; cntd < dsReqDts_LogReq.Tables["tbl_ReqDts_LogReq"].Rows.Count; cntd++)
                        {
                            SrNoReq_LogReq = SrNoReq_LogReq + 1;
                            cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@REQCode", StrDispCode_Log.Trim());
                            cmd.Parameters.AddWithValue("@SrNo", SrNoReq_LogReq);
                            cmd.Parameters.AddWithValue("@PartCode", dsReqDts_LogReq.Tables["tbl_ReqDts_LogReq"].Rows[cntd]["Partcode"].ToString().Trim());
                            cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts_LogReq.Tables["tbl_ReqDts_LogReq"].Rows[cntd]["RaiseReqQty"].ToString().Trim()) * double.Parse(Dts[9].ToString().Trim()));
                            cmd.Parameters.AddWithValue("@REQStatus", "P");
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();

                        }
                    }

                    #endregion
                    //Auto WH Req --END





                    #endregion
                    //All Requisition End

                } // Main For Loop END Bracket 


                //****************User Acivity****************
                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@EmpID", job_Cpyreq.EmpCode.Trim());
                cmd.Parameters.AddWithValue("@TransactionType", "S");
                cmd.Parameters.AddWithValue("@TransactionFrom", "CpyPlan(Manual)");
                cmd.Parameters.AddWithValue("@TransactionNo", StrDispCode_CPYPlan.Trim());
                cmd.Parameters.AddWithValue("@CompanyCode", job_Cpyreq.CompCode.Trim());
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                tran.Commit();
               // tran.Rollback();

                //StrDisplayMsg = "";
                //
                if (!string.IsNullOrEmpty(StrDispCode_CPYPlan.Trim()))
                {
                    StrDisplayMsg = "Saved Successfully With Canopy Plan: " + StrDispCode_CPYPlan.Trim() + "";
                }

                if (!string.IsNullOrEmpty(StrDispCode_MaterialReq_WH_ALL_msg.Trim()))
                {
                    StrDisplayMsg += " & Material Req: " + StrDispCode_MaterialReq_WH_ALL_msg.Trim() + "";
                }

               

                return StrDisplayMsg;

                #endregion
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

        protected void GetReqDetailsSubCP(string ReqCode, string strKitCode, int strPCwise, double KitQty)
        {
            DataSet dsReqDtsSub = ComCon.procTranDS("exec InternalReqLogisticsdetails '" + strKitCode + "'," + strPCwise + "", "tbl_RaiseReqDtsSub", con, tran);
            if (dsReqDtsSub != null && dsReqDtsSub.Tables["tbl_RaiseReqDtsSub"].Rows.Count > 0)
            {
                int SrNok = 0;
                for (int k = 0; k < dsReqDtsSub.Tables["tbl_RaiseReqDtsSub"].Rows.Count; k++)
                {
                    if (Convert.ToDouble(dsReqDtsSub.Tables["tbl_RaiseReqDtsSub"].Rows[k]["RaiseReqQty"].ToString().Trim()) > 0)
                    {
                        SrNok += 1;
                    }
                    SqlCommand cmd = new SqlCommand();
                    sb.Remove(0, sb.Length);
                    sb.Append("insert into MaterialReqDetailsSub(REQCode,SrNo,RKitCode,PartCode,Qty,REQStatus) ");
                    sb.Append("values('" + ReqCode.Trim() + "','" + SrNok + "','" + strKitCode + "',");
                    sb.Append("'" + dsReqDtsSub.Tables["tbl_RaiseReqDtsSub"].Rows[k]["PartCode"].ToString().Trim() + "',");
                    sb.Append("'" + double.Parse(dsReqDtsSub.Tables["tbl_RaiseReqDtsSub"].Rows[k]["RaiseReqQty"].ToString().Trim()) * KitQty + "','P')");
                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
        }
    }
}