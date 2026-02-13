using KalaERPApi.Models.Request;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace KalaERPApi.Service
{
    public class JobCard_WHCon
    {

        #region
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"].ToString());
        StringBuilder sb = new StringBuilder();
        CommonCon ComCon = new CommonCon();
        public SqlTransaction tran = null;
        SqlCommand cmd = null;
        #endregion
       

        public DataTable GetWHPlan(string strJobCardType, string strcompID)
        {
            SqlDataAdapter dAd = new SqlDataAdapter("GetJobCard_WH_PlanDts", con);
            //SqlDataAdapter dAd = new SqlDataAdapter("GetJobCardDGDts_fs", con);
            dAd.SelectCommand.CommandType = CommandType.StoredProcedure;
            dAd.SelectCommand.Parameters.Add("@Type", SqlDbType.Char).Value = strJobCardType;
            dAd.SelectCommand.Parameters.Add("@CompId", SqlDbType.Char).Value = strcompID;
            dAd.SelectCommand.CommandTimeout = 0;
            DataSet dSet = new DataSet();
            dAd.Fill(dSet);
            return dSet.Tables[0];
        }

        public string Submit(JobCard_CpyRequest job_Cpyreq)
        {
            
            DataSet dsDetailsSub;
            string StrDisplayMsg = "";
            string StrDispCode_CPYPlan = "";
            String StrDispCode_MaterialReq_CNC_ALL_msg = "";
            String StrDispCode_MaterialReq_FAB_ALL_msg = "";
            String StrDispCode_MaterialReq_POC_ALL_msg = "";
            string StrDispCode_MaterialReq_CNC = "";

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
                if (con.State == ConnectionState.Open) { con.Close(); } else { con.Open(); };
                tran = con.BeginTransaction();
                //Save Cpy Plan
                StrDispCode_CPYPlan = ComCon.GetMaxNo("WiringHarnessPlan", "WHP", job_Cpyreq.CompCode.Trim(), con, tran);


                //For Mst Save
                #region
                cmd = new SqlCommand("InsertWiringHarnessPlan", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CPCode", StrDispCode_CPYPlan.Trim());
                cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                cmd.Parameters.AddWithValue("@MaxSrNo", StrDispCode_CPYPlan.Substring(10, 8).Trim());
                cmd.Parameters.AddWithValue("@Yr", StrDispCode_CPYPlan.Substring(4, 5).Trim());
                cmd.Parameters.AddWithValue("@FromDt", DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                cmd.Parameters.AddWithValue("@ToDt", DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                cmd.Parameters.AddWithValue("@PlanPCCode", "01.091"); //"01.041"
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

                    //     0               1                       2                   3                       4                   5                       6                       7                       8                       9                       10
                    //item.KVA + "@#@" + item.Model + "@#@" + item.Partcode + "@#@" + item.FNorm + "@#@" + item.TotStk + "@#@" + item.WIPStk + "@#@" + item.PenPlanQty + "@#@" + item.PReq + "@#@" + item.PlanQty + "@#@" + item.BatchQty + "@#@" + item.Bomcode

                    //For Dts Save
                    #region
                    cmd = new SqlCommand("InsertWiringHarnessPlanDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CPCode", StrDispCode_CPYPlan.Trim());
                    cmd.Parameters.AddWithValue("@Dt", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));
                    cmd.Parameters.AddWithValue("@SrNo", SrNo);
                    cmd.Parameters.AddWithValue("@PartCode", Dts[2].ToString().Trim());
                    cmd.Parameters.AddWithValue("@BomCode", Dts[10].ToString().Trim());

                    string PartCodeWOP = "";
                    PartCodeWOP = ComCon.getTranName("SELECT Partcode FROM BOMdetails where BOMCode='" + Dts[10].ToString().Trim() + "' " +
                    "AND KitCode='" + Dts[2].ToString().Trim() + "' AND Partcode like '005%' ", "tblBP", "Partcode", con, tran);

                    cmd.Parameters.AddWithValue("@PartCodeWOP", PartCodeWOP.Trim());
                    cmd.Parameters.AddWithValue("@Qty", int.Parse(Dts[8].ToString().Trim()));
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    #endregion
                    //For Dts Save         

                    //Auto CNC Req --START --******************************
                    #region

                    string StrDispCode_CNCReq = "";

                    String StrCNC_PCCode = "0";
                    if (job_Cpyreq.PCCode.Trim() == "01.041")
                    {
                        StrCNC_PCCode = "01.091"; //Wiring Harness
                    }
                    else if (job_Cpyreq.PCCode.Trim() == "03.064")
                    {
                        StrCNC_PCCode = "03.061"; ////CNC
                    }

                    //StrDispCode_CNCReq = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", job_Cpyreq.CompCode.Trim(), con, tran);


                    //if (string.IsNullOrEmpty(StrDispCode_MaterialReq_CNC_ALL_msg.Trim()))
                    //{
                    //    StrDispCode_MaterialReq_CNC_ALL_msg = StrDispCode_CNCReq.Trim();
                    //}
                    //else
                    //{
                    //    StrDispCode_MaterialReq_CNC_ALL_msg += ", " + StrDispCode_CNCReq.Trim();
                    //}


                    //sb.Remove(0, sb.Length);
                    //sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                    //    " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode) ");
                    //sb.Append("values('" + StrDispCode_CNCReq.Trim() + "','" + StrDispCode_CNCReq.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                    //sb.Append("'" + StrDispCode_CNCReq.Substring(4, 5).Trim() + "','" + StrCNC_PCCode.Trim() + "','23.001','" + Dts[2].ToString().Trim() + "','" + job_Cpyreq.CompCode.Trim() + "','" + Dts[8].ToString().Trim() + "','P','WIP',");
                    //sb.Append("'Auto Req For : " + Dts[0].ToString().Trim() + " Kva " + Dts[1].ToString().Trim() + " ','1','1','1','" + StrDispCode_CPYPlan.Trim() + "')");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                    ////Details Req STRAT ********************


                    //DataSet dsReqDts_CNCReq = ComCon.procTranDS("exec InternalReqLogisticsDetailsKitWH '" + Dts[2].ToString().Trim() + "'", "tbl_ReqDts_CNCReq", con, tran);
                    //if (dsReqDts_CNCReq != null && dsReqDts_CNCReq.Tables["tbl_ReqDts_CNCReq"].Rows.Count > 0)
                    //{
                    //    int SrNoReq_CNCReq = 0;

                    //    for (int cntd = 0; cntd < dsReqDts_CNCReq.Tables["tbl_ReqDts_CNCReq"].Rows.Count; cntd++)
                    //    {
                    //        SrNoReq_CNCReq = SrNoReq_CNCReq + 1;
                    //        cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                    //        cmd.CommandType = CommandType.StoredProcedure;
                    //        cmd.Parameters.AddWithValue("@REQCode", StrDispCode_CNCReq.Trim());
                    //        cmd.Parameters.AddWithValue("@SrNo", SrNoReq_CNCReq);
                    //        cmd.Parameters.AddWithValue("@PartCode", dsReqDts_CNCReq.Tables["tbl_ReqDts_CNCReq"].Rows[cntd]["Partcode"].ToString().Trim());
                    //        cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts_CNCReq.Tables["tbl_ReqDts_CNCReq"].Rows[cntd]["RaiseReqQty"].ToString().Trim()) * double.Parse(Dts[8].ToString().Trim()));
                    //        cmd.Parameters.AddWithValue("@REQStatus", "P");
                    //        cmd.Transaction = tran;
                    //        cmd.ExecuteNonQuery();
                    //        cmd.Dispose();

                    //       // GetReqDetailsSub(StrDispCode_CNCReq.Trim(), dsReqDts_CNCReq.Tables["tbl_ReqDts_CNCReq"].Rows[cntd]["Partcode"].ToString().Trim(), 0, double.Parse(Dts[8].ToString().Trim()));
                    //    }
                    //}

                    //Details Req  END ***************************

                    //}

                    #endregion
                    //Auto CNC Req --END



                    ////Auto FAB Req --START *****************************
                    //#region

                    //string StrDispCode_FABReq = "";

                    //String StrFAB_PCCode = "0";
                    //if (job_Cpyreq.PCCode.Trim() == "01.041")
                    //{
                    //    StrFAB_PCCode = "01.008"; //FAB
                    //}
                    //else if (job_Cpyreq.PCCode.Trim() == "03.064")
                    //{
                    //    StrFAB_PCCode = "03.002"; ////FAB
                    //}

                    //StrDispCode_FABReq = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", job_Cpyreq.CompCode.Trim(), con, tran);


                    //if (string.IsNullOrEmpty(StrDispCode_MaterialReq_FAB_ALL_msg.Trim()))
                    //{
                    //    StrDispCode_MaterialReq_FAB_ALL_msg = StrDispCode_FABReq.Trim();
                    //}
                    //else
                    //{
                    //    StrDispCode_MaterialReq_FAB_ALL_msg += ", " + StrDispCode_FABReq.Trim();
                    //}


                    //sb.Remove(0, sb.Length);
                    //sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                    //    " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode) ");
                    //sb.Append("values('" + StrDispCode_FABReq.Trim() + "','" + StrDispCode_FABReq.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                    //sb.Append("'" + StrDispCode_FABReq.Substring(4, 5).Trim() + "','" + StrFAB_PCCode.Trim() + "','23.001','" + Dts[2].ToString().Trim() + "','" + job_Cpyreq.CompCode.Trim() + "','" + Dts[8].ToString().Trim() + "','P','WIP',");
                    //sb.Append("'Auto Req For : " + Dts[0].ToString().Trim() + " Kva " + Dts[1].ToString().Trim() + " ','1','1','1','" + StrDispCode_CPYPlan.Trim() + "')");
                    //cmd = new SqlCommand(sb.ToString(), con);
                    //cmd.Transaction = tran;
                    //cmd.ExecuteNonQuery();
                    //cmd.Dispose();

                    ////Details Req STRAT ********************


                    //DataSet dsReqDts_FABReq = ComCon.procTranDS("exec InternalReqLogisticsKit '" + Dts[2].ToString().Trim() + "',1 ", "tbl_ReqDts_FABReq", con, tran);
                    //if (dsReqDts_FABReq != null && dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows.Count > 0)
                    //{
                    //    int SrNoReq_FABReq = 0;

                    //    for (int cnt_FAB = 0; cnt_FAB < dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows.Count; cnt_FAB++)
                    //    {
                    //        SrNoReq_FABReq = SrNoReq_FABReq + 1;
                    //        cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                    //        cmd.CommandType = CommandType.StoredProcedure;
                    //        cmd.Parameters.AddWithValue("@REQCode", StrDispCode_FABReq.Trim());
                    //        cmd.Parameters.AddWithValue("@SrNo", SrNoReq_FABReq);
                    //        cmd.Parameters.AddWithValue("@PartCode", dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows[cnt_FAB]["Partcode"].ToString().Trim());
                    //        cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows[cnt_FAB]["RaiseReqQty"].ToString().Trim()) * double.Parse(Dts[8].ToString().Trim()));
                    //        cmd.Parameters.AddWithValue("@REQStatus", "P");
                    //        cmd.Transaction = tran;
                    //        cmd.ExecuteNonQuery();
                    //        cmd.Dispose();

                    //        GetReqDetailsSub(StrDispCode_FABReq.Trim(), dsReqDts_FABReq.Tables["tbl_ReqDts_FABReq"].Rows[cnt_FAB]["Partcode"].ToString().Trim(), 1, double.Parse(Dts[8].ToString().Trim()));
                    //    }
                    //}

                    ////Details Req  END ***************************

                    ////}

                    //#endregion
                    ////Auto FAB Req --END


                    ////Auto U1 Powder C to U- FBB Req --START *****************************
                    //#region

                    //if (job_Cpyreq.CompCode.Trim() == "03")
                    //{

                    //    string StrDispCode_PCReq = "";

                    //    StrDispCode_PCReq = ComCon.GetMaxNo("MaterialRequisitionWithOutPlan", "REQ", "01", con, tran);

                    //    if (string.IsNullOrEmpty(StrDispCode_MaterialReq_POC_ALL_msg.Trim()))
                    //    {
                    //        StrDispCode_MaterialReq_POC_ALL_msg = StrDispCode_PCReq.Trim();
                    //    }
                    //    else
                    //    {
                    //        StrDispCode_MaterialReq_POC_ALL_msg += ", " + StrDispCode_PCReq.Trim();
                    //    }

                    //    sb.Remove(0, sb.Length);
                    //    sb.Append("insert into MaterialRequisitionWithOutPlan(REQCode,MaxSrNo,Dt," +
                    //        " Yr,ProfitCenterCode,ToProfitCenterCode,ClassCode,CompanyCode,ActNo,REQStatus,ReqType,Remark,Discard,Active,Auth,SourceCode) ");
                    //    sb.Append("values('" + StrDispCode_PCReq.Trim() + "','" + StrDispCode_PCReq.Substring(10, 8).ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "',");
                    //    sb.Append("'" + StrDispCode_PCReq.Substring(4, 5).Trim() + "','01.007','03.002','" + Dts[2].ToString().Trim() + "','01','" + Dts[8].ToString().Trim() + "','P','WIP',");
                    //    sb.Append("'Auto Req For : " + Dts[0].ToString().Trim() + " Kva " + Dts[1].ToString().Trim() + "','1','1','1','" + StrDispCode_CPYPlan.Trim() + "')");
                    //    cmd = new SqlCommand(sb.ToString(), con);
                    //    cmd.Transaction = tran;
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Dispose();

                    //    //Details Req STRAT ********************


                    //    DataSet dsReqDts_PCReq = ComCon.procTranDS("Select CPCode,CpyPartCode,SrNo,PartCode,CPQty as RaiseReqQty,Rate  from CanopyPlanDtsSub  where CPCode='" + StrDispCode_CPYPlan.Trim() + "' and CpyPartCode ='" + Dts[2].ToString().Trim() + "' " +
                    //        " Union ALL Select CPCode,CpyPartCode,SrNo,PartCode,CPQty as RaiseReqQty ,Rate from CanopyPlanDtsSubBelowStdRate  where CPCode='" + StrDispCode_CPYPlan.Trim() + "' and CpyPartCode ='" + Dts[2].ToString().Trim() + "' Order by Srno ", "tbl_ReqDts_PCReq", con, tran);
                    //    if (dsReqDts_PCReq != null && dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows.Count > 0)
                    //    {
                    //        int SrNoReq_PCReq = 0;

                    //        for (int cnt_PC = 0; cnt_PC < dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows.Count; cnt_PC++)
                    //        {
                    //            SrNoReq_PCReq = SrNoReq_PCReq + 1;
                    //            cmd = new SqlCommand("insertMaterialRequisitionWithOutPlanDetails", con);
                    //            cmd.CommandType = CommandType.StoredProcedure;
                    //            cmd.Parameters.AddWithValue("@REQCode", StrDispCode_PCReq.Trim());
                    //            cmd.Parameters.AddWithValue("@SrNo", SrNoReq_PCReq);
                    //            cmd.Parameters.AddWithValue("@PartCode", dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows[cnt_PC]["Partcode"].ToString().Trim());
                    //            cmd.Parameters.AddWithValue("@Qty", double.Parse(dsReqDts_PCReq.Tables["tbl_ReqDts_PCReq"].Rows[cnt_PC]["RaiseReqQty"].ToString().Trim()));
                    //            cmd.Parameters.AddWithValue("@REQStatus", "P");
                    //            cmd.Transaction = tran;
                    //            cmd.ExecuteNonQuery();
                    //            cmd.Dispose();


                    //        }
                    //    }

                    //    //Details Req  END ***************************
                    //}

                    //#endregion
                    ////Auto FAB Req --END

                } // Main For Loop END Bracket 


                //****************User Acivity****************
                cmd = new SqlCommand("InsertLoginTransactionDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransactionDtTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@EmpID", job_Cpyreq.EmpCode.Trim());
                cmd.Parameters.AddWithValue("@TransactionType", "S");
                cmd.Parameters.AddWithValue("@TransactionFrom", "WHPlan(Manual)");
                cmd.Parameters.AddWithValue("@TransactionNo", StrDispCode_CPYPlan.Trim());
                cmd.Parameters.AddWithValue("@CompanyCode", job_Cpyreq.CompCode.Trim());
                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                tran.Commit();

                //StrDisplayMsg = "";
                //
                if (!string.IsNullOrEmpty(StrDispCode_CPYPlan.Trim()))
                {
                    StrDisplayMsg = "Saved Successfully With Wiring Harness Plan: " + StrDispCode_CPYPlan.Trim() + "";
                }

                if (!string.IsNullOrEmpty(StrDispCode_MaterialReq_CNC_ALL_msg.Trim()))
                {
                    StrDisplayMsg += " & WH Req: " + StrDispCode_MaterialReq_CNC_ALL_msg.Trim() + "";
                }

                //if (!string.IsNullOrEmpty(StrDispCode_MaterialReq_FAB_ALL_msg.Trim()))
                //{
                //    StrDisplayMsg += " & Fab Req: " + StrDispCode_MaterialReq_FAB_ALL_msg.Trim() + "";
                //}

                //if (!string.IsNullOrEmpty(StrDispCode_MaterialReq_POC_ALL_msg.Trim()))
                //{
                //    StrDisplayMsg += " & PC_Unit_1 to Fab_U4 : " + StrDispCode_MaterialReq_POC_ALL_msg.Trim() + "";
                //}

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

        protected void GetReqDetailsSub(string ReqCode, string strKitCode, int strPCwise, double KitQty)
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